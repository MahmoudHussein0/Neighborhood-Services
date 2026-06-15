import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { NgbModal, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { BookingService } from '../../services/booking.service';
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
  imports: [DatePipe, CurrencyPipe, FormsModule, NgbDropdownModule, TranslatePipe],
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
  readonly pageSize = 10;

  loading = signal(false);
  result = signal<PagedResult<MyBookingSummary> | null>(null);
  activeTab = signal<StatusTab>('All');
  searchTerm = signal('');
  page = signal(1);
  busyId = signal<number | null>(null);

  // Optional promo code entered inline on a Quoted booking, keyed by booking id.
  quotePromo = signal<Record<number, string>>({});

  protected readonly mapsUrl = googleMapsUrl;

  private readonly search$ = new Subject<string>();

  constructor() {
    // Debounce typing so we don't fire a request on every keystroke
    this.search$
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => {
        this.page.set(1);
        this.load();
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

  setQuotePromo(id: number, value: string) {
    this.quotePromo.update((m) => ({ ...m, [id]: value }));
  }

  acceptQuote(b: MyBookingSummary) {
    const promo = (this.quotePromo()[b.id] ?? '').trim();
    this.confirmDialog
      .confirm({
        messageKey: 'bookings.acceptQuotePrompt',
        messageParams: { price: b.finalPrice },
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
}
