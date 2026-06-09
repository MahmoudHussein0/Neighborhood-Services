import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { NgbModal, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { BookingService } from '../../services/booking.service';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { BookingSummary, BookingStatus } from '../../models/booking.model';
import { BookingDetailsModalComponent } from '../../components/booking-details-modal/booking-details-modal.component';
import { CancelBookingModalComponent } from '../../components/cancel-booking-modal/cancel-booking-modal.component';

type StatusTab = 'All' | BookingStatus;

@Component({
  selector: 'app-bookings',
  imports: [DatePipe, CurrencyPipe, NgbDropdownModule, TranslatePipe],
  templateUrl: './bookings.component.html',
  styleUrl: './bookings.component.css',
})
export class BookingsComponent implements OnInit {
  private readonly bookingService = inject(BookingService);
  private readonly modal = inject(NgbModal);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  readonly tabs: StatusTab[] = ['All', 'Pending', 'Confirmed', 'Completed', 'Cancelled', 'Disputed'];
  readonly pageSize = 10;

  loading = signal(false);
  result = signal<PagedResult<BookingSummary> | null>(null);
  activeTab = signal<StatusTab>('All');
  searchTerm = signal('');
  page = signal(1);

  private readonly search$ = new Subject<string>();

  constructor() {
    // Debounce typing so we don't fire a request on every keystroke
    this.search$
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => {
        this.page.set(1);
        this.load();
      });
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
  }

  confirm(b: BookingSummary) {
    if (!confirm(this.translate.instant('bookings.confirmPrompt'))) return;
    this.bookingService.confirm(b.id).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('bookings.confirmed'));
        this.load();
      },
    });
  }

  cancel(b: BookingSummary) {
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

  // --- UI helpers ---

  badgeClass(status: BookingStatus): string {
    switch (status) {
      case 'Pending': return 'text-bg-warning';
      case 'Confirmed': return 'text-bg-primary';
      case 'Completed': return 'text-bg-success';
      case 'Cancelled': return 'text-bg-danger';
      case 'Disputed': return 'text-bg-secondary';
    }
  }

  canCancel(status: BookingStatus): boolean {
    return status === 'Pending' || status === 'Confirmed';
  }

  canConfirm(status: BookingStatus): boolean {
    return status === 'Completed';
  }

  hasActions(status: BookingStatus): boolean {
    return this.canCancel(status) || this.canConfirm(status);
  }
}
