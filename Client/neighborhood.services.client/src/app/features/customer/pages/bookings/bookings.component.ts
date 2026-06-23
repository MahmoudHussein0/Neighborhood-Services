import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, debounceTime, distinctUntilChanged, switchMap, map, of, catchError, firstValueFrom } from 'rxjs';
import { NgbModal, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { BookingService, PromoCodePreview, BookingSort } from '../../services/booking.service';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { MyBookingSummary, BookingStatus, DisputeType } from '../../models/booking.model';
import { BookingDetailsModalComponent } from '../../components/booking-details-modal/booking-details-modal.component';
import { CancelBookingModalComponent } from '../../components/cancel-booking-modal/cancel-booking-modal.component';
import { RaiseDisputeModalComponent } from '../../components/raise-dispute-modal/raise-dispute-modal.component';
import { LeaveReviewModalComponent } from '../../components/leave-review-modal/leave-review-modal.component';
import { googleMapsUrl } from '../../../../core/utils/maps.util';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { NotificationServiceService } from '../../../../shared/services/notification-service.service';
import { environment } from '../../../../environments/environment';

type StatusTab = 'All' | BookingStatus;

@Component({
  selector: 'app-bookings',
  imports: [DatePipe, CurrencyPipe, FormsModule, NgbDropdownModule, TranslatePipe, RouterLink],
  templateUrl: './bookings.component.html',
  styleUrl: './bookings.component.css',
})
export class BookingsComponent implements OnInit {
  private readonly bookingService = inject(BookingService);
  private readonly modal = inject(NgbModal);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);
  private readonly confirmDialog = inject(ConfirmService);
  private readonly notificationService = inject(NotificationServiceService);

  readonly tabs: StatusTab[] = ['All', 'Pending', 'Quoted', 'Confirmed', 'Completed', 'Cancelled', 'Disputed'];
  readonly pageSize = 5;

  loading = signal(false);
  result = signal<PagedResult<MyBookingSummary> | null>(null);
  activeTab = signal<StatusTab>('All');
  searchTerm = signal('');
  sort = signal<BookingSort>('NewestCreated');
  page = signal(1);
  busyId = signal<number | null>(null);

  readonly sortOptions: BookingSort[] = ['NewestCreated', 'OldestCreated', 'SoonestScheduled', 'LatestScheduled'];

  // Optional promo code entered inline on a Quoted booking, keyed by booking id.
  quotePromo = signal<Record<number, string>>({});
  // Live preview of the discounted total per booking once an applicable promo is typed.
  quoteDiscountedPrice = signal<Record<number, number>>({});
  // Per booking: a non-blank code came back not-applicable (invalid/expired/already used).
  quotePromoUnavailable = signal<Record<number, boolean>>({});

  protected readonly mapsUrl = googleMapsUrl;

  private readonly search$ = new Subject<string>();
  // Emits { booking id, typed code, base price } so we can debounce promo previews per booking.
  private readonly quotePromo$ = new Subject<{ id: number; code: string; base: number }>();

  constructor() {
    // Debounce typing so we don't fire a request on every keystroke
    this.search$
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => {
        this.page.set(1);
        this.load();
      });

    // Live promo preview: debounce typing, look the code up for the current user, and
    // show the discounted total (or a "can't be used" hint) without consuming the code.
    this.quotePromo$
      .pipe(
        debounceTime(400),
        distinctUntilChanged((a, b) => a.id === b.id && a.code === b.code),
        switchMap(({ id, code, base }) => {
          const trimmed = code.trim();
          if (!trimmed) return of({ id, base, preview: null });
          return this.bookingService.getPromoPreview(trimmed).pipe(
            map((preview) => ({ id, base, preview })),
            catchError(() => of({ id, base, preview: null })),
          );
        }),
        takeUntilDestroyed(),
      )
      .subscribe(({ id, base, preview }) => {
        if (preview && preview.isApplicable && preview.discountPercentage > 0) {
          const discount = Math.round((base * preview.discountPercentage) / 100 * 100) / 100;
          this.quoteDiscountedPrice.update((m) => ({ ...m, [id]: Math.max(0, base - discount) }));
          this.quotePromoUnavailable.update((m) => ({ ...m, [id]: false }));
        } else {
          this.quoteDiscountedPrice.update((m) => { const next = { ...m }; delete next[id]; return next; });
          this.quotePromoUnavailable.update((m) => ({ ...m, [id]: !!preview && !preview.isApplicable }));
        }
      });

    // Refresh the list when a realtime notification arrives (e.g. quote received, booking completed).
    this.notificationService.notificationReceived$
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.load());
  }

  ngOnInit() {
    this.load();
  }

  onSearchInput(value: string) {
    this.searchTerm.set(value);
    this.search$.next(value);
  }

  load() {
    this.loading.set(true);
    this.bookingService
      .getMyBookings({
        status: this.activeTab() === 'All' ? undefined : (this.activeTab() as BookingStatus),
        search: this.searchTerm(),
        sort: this.sort(),
        page: this.page(),
        pageSize: this.pageSize,
      })
      .subscribe({
        next: (r) => {
          this.result.set(r);
          this.loading.set(false);
        },
        error: () => this.loading.set(false),
      });
  }

  selectTab(tab: StatusTab) {
    this.activeTab.set(tab);
    this.page.set(1);
    this.load();
  }

  changeSort(value: string) {
    this.sort.set(value as BookingSort);
    this.page.set(1);
    this.load();
  }

  goToPage(p: number) {
    this.page.set(p);
    this.load();
  }

  openDetails(id: number) {
    const ref = this.modal.open(BookingDetailsModalComponent, { size: 'lg' });
    ref.componentInstance.bookingId = id;
    // The modal closes with `true` when a promo code changed the total — refresh the list.
    ref.result.then(
      (changed: boolean) => { if (changed) this.load(); },
      () => {}, // dismissed
    );
  }

  confirm(b: MyBookingSummary) {
    if (this.busyId() === b.id || b.clientConfirmed) return;
    this.confirmDialog
      .confirm({
        messageKey: 'bookings.confirmPrompt',
        confirmKey: 'bookings.confirmCompleted',
        variant: 'success',
      })
      .then((ok) => {
        if (!ok) return;
        this.busyId.set(b.id);
        this.bookingService.confirm(b.id).subscribe({
          next: () => {
            this.busyId.set(null);
            this.toastr.success(this.translate.instant('bookings.confirmed'));
            this.load();
          },
          error: () => this.busyId.set(null),
        });
      });
  }

  onQuotePromoChange(b: MyBookingSummary, value: string) {
    this.quotePromo.update((m) => ({ ...m, [b.id]: value }));
    const base = b.finalPrice > 0 ? b.finalPrice : b.estimatedPrice;
    this.quotePromo$.next({ id: b.id, code: value, base });
  }

  private clearQuotePromo(id: number) {
    this.quotePromo.update((m) => ({ ...m, [id]: '' }));
    this.quoteDiscountedPrice.update((m) => { const next = { ...m }; delete next[id]; return next; });
    this.quotePromoUnavailable.update((m) => ({ ...m, [id]: false }));
  }

  async acceptQuote(b: MyBookingSummary) {
    const promo = (this.quotePromo()[b.id] ?? '').trim();
    let price = b.finalPrice;

    // Re-check the code authoritatively at accept time (covers a click before the live
    // preview resolves). A bad code → tell the user, clear it, and stop so they can re-accept.
    if (promo) {
      let preview: PromoCodePreview | null = null;
      try {
        preview = await firstValueFrom(this.bookingService.getPromoPreview(promo));
      } catch {
        preview = null;
      }
      if (!preview || !preview.isApplicable) {
        this.toastr.warning(this.translate.instant('common.promoUnavailable'));
        this.clearQuotePromo(b.id);
        return;
      }
      const base = b.finalPrice > 0 ? b.finalPrice : b.estimatedPrice;
      const discount = Math.round((base * preview.discountPercentage) / 100 * 100) / 100;
      price = Math.max(0, base - discount);
    }

    this.confirmDialog
      .confirm({
        messageKey: 'bookings.acceptQuotePrompt',
        messageParams: { price },
        confirmKey: 'bookings.acceptQuote',
        variant: 'success',
      })
      .then((ok) => {
        if (!ok) return;
        this.busyId.set(b.id);
        this.bookingService.acceptQuote(b.id, promo || null).subscribe({
          next: () => {
            this.busyId.set(null);
            this.toastr.success(this.translate.instant('bookings.quoteAccepted'));
            this.load();
          },
          error: () => this.busyId.set(null),
        });
      });
  }

  rejectQuote(b: MyBookingSummary) {
    this.confirmDialog
      .confirm({
        messageKey: 'bookings.rejectQuotePrompt',
        confirmKey: 'bookings.rejectQuote',
        variant: 'danger',
      })
      .then((ok) => {
        if (!ok) return;
        this.busyId.set(b.id);
        this.bookingService.rejectQuote(b.id).subscribe({
          next: () => {
            this.busyId.set(null);
            this.toastr.success(this.translate.instant('bookings.quoteRejected'));
            this.load();
          },
          error: () => this.busyId.set(null),
        });
      });
  }

  cancel(b: MyBookingSummary) {
    const ref = this.modal.open(CancelBookingModalComponent);
    ref.result.then(
      (reason: string) => {
        this.bookingService.cancel(b.id, reason).subscribe({
          next: () => {
            this.toastr.success(this.translate.instant('bookings.cancelled'));
            this.load();
          },
        });
      },
      () => {} // dismissed — do nothing
    );
  }

  raiseDispute(b: MyBookingSummary) {
    const ref = this.modal.open(RaiseDisputeModalComponent);
    ref.result.then(
      (res: { disputeType: DisputeType; reason: string }) => {
        this.busyId.set(b.id);
        this.bookingService.raiseDispute(b.id, res.disputeType, res.reason).subscribe({
          next: () => {
            this.busyId.set(null);
            this.toastr.success(this.translate.instant('dispute.raised'));
            this.load();
          },
          error: () => this.busyId.set(null),
        });
      },
      () => {} // dismissed — do nothing
    );
  }

  // --- UI helpers ---

  badgeClass(status: BookingStatus): string {
    switch (status) {
      case 'Pending': return 'text-bg-warning';
      case 'Quoted': return 'text-bg-info';
      case 'Confirmed': return 'text-bg-primary';
      case 'Completed': return 'text-bg-success';
      case 'Cancelled': return 'text-bg-danger';
      case 'Disputed': return 'text-bg-secondary';
    }
  }

  canCancel(status: BookingStatus): boolean {
    return status === 'Pending' || status === 'Quoted' || status === 'Confirmed';
  }

  // Only when the tech has marked it Completed AND the customer hasn't confirmed yet.
  canConfirm(b: MyBookingSummary): boolean {
    return b.status === 'Completed' && !b.clientConfirmed;
  }

  canRespondToQuote(status: BookingStatus): boolean {
    return status === 'Quoted';
  }

  // A dispute only makes sense once there's a committed job.
  canDispute(status: BookingStatus): boolean {
    return status === 'Confirmed' || status === 'Completed';
  }

  // Reviews unlock only after the customer has confirmed the completed work and the
  // escrow released — proof the cycle is fully done. Hidden once a review exists.
  canReview(b: MyBookingSummary): boolean {
    return b.status === 'Completed' && b.clientConfirmed && !b.hasReview;
  }

  leaveReview(b: MyBookingSummary) {
    const ref = this.modal.open(LeaveReviewModalComponent);
    ref.result.then(
      (res: { rating: number; comment: string }) => {
        this.busyId.set(b.id);
        this.bookingService.createReview(b.id, res.rating, res.comment).subscribe({
          next: () => {
            this.busyId.set(null);
            this.toastr.success(this.translate.instant('review.submitted'));
            this.load();
          },
          error: () => this.busyId.set(null),
        });
      },
      () => {} // dismissed — do nothing
    );
  }

  viewInvoice(bookingId: number): void {
    window.open(`${environment.apiUrl}/api/invoices/booking/${bookingId}/pdf/view`, '_blank');
  }

  downloadInvoice(bookingId: number): void {
    window.open(`${environment.apiUrl}/api/invoices/booking/${bookingId}/pdf`, '_blank');
  }

  hasActions(b: MyBookingSummary): boolean {
    return this.canCancel(b.status) || this.canConfirm(b) || this.canRespondToQuote(b.status) || this.canDispute(b.status) || this.canReview(b);
  }

  // Returns the index (0–5) of the active progress step so the timeline can highlight it.
  // -1 means Cancelled/Disputed — no timeline shown.
  bookingStep(b: MyBookingSummary): number {
    switch (b.status) {
      case 'Pending':   return 0;
      case 'Quoted':    return 1;
      case 'Confirmed': return 2;
      case 'Completed':
        if (!b.clientConfirmed) return 3;
        if (!b.hasReview)       return 4;
        return 5;
      default: return -1;
    }
  }
}
