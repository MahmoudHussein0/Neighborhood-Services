import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';

import { StaffBookingService, StaffBooking, StaffBookingStatus, BookingSort } from '../../services/staff-booking.service';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { CancelBookingModalComponent } from '../../../customer/components/cancel-booking-modal/cancel-booking-modal.component';
import { StaffBookingDetailsModalComponent } from '../../components/staff-booking-details-modal/staff-booking-details-modal.component';

type StatusTab = 'All' | StaffBookingStatus;

@Component({
  selector: 'app-staff-bookings',
  imports: [DatePipe, CurrencyPipe, TranslatePipe],
  templateUrl: './staff-bookings.component.html',
})
export class StaffBookingsComponent implements OnInit {
  private readonly service = inject(StaffBookingService);
  private readonly modal = inject(NgbModal);
  private readonly toastr = inject(ToastrService);

  readonly tabs: StatusTab[] = ['All', 'Pending', 'Quoted', 'Confirmed', 'Completed', 'Cancelled', 'Disputed'];
  readonly pageSize = 5;

  loading = signal(false);
  result = signal<PagedResult<StaffBooking> | null>(null);
  activeTab = signal<StatusTab>('All');
  searchTerm = signal('');
  sort = signal<BookingSort>('NewestCreated');
  page = signal(1);

  readonly sortOptions: BookingSort[] = ['NewestCreated', 'OldestCreated', 'SoonestScheduled', 'LatestScheduled'];
  busyId = signal<number | null>(null);

  private readonly search$ = new Subject<string>();

  constructor() {
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

  load() {
    this.loading.set(true);
    this.service
      .getBookings({
        status: this.activeTab() === 'All' ? undefined : (this.activeTab() as StaffBookingStatus),
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

  onSearchInput(value: string) {
    this.searchTerm.set(value);
    this.search$.next(value);
  }

  openDetails(b: StaffBooking) {
    const ref = this.modal.open(StaffBookingDetailsModalComponent, { size: 'md' });
    ref.componentInstance.booking = b;
  }

  // Staff can cancel only active bookings (not finished, cancelled, or under dispute).
  canCancel(status: StaffBookingStatus): boolean {
    return status === 'Pending' || status === 'Quoted' || status === 'Confirmed';
  }

  cancel(b: StaffBooking) {
    const ref = this.modal.open(CancelBookingModalComponent);
    ref.result.then(
      (reason: string) => {
        this.busyId.set(b.id);
        this.service.cancel(b.id, reason).subscribe({
          next: () => {
            this.busyId.set(null);
            this.toastr.success('Booking cancelled.');
            this.load();
          },
          error: () => this.busyId.set(null),
        });
      },
      () => {}, // dismissed
    );
  }

  badgeClass(status: StaffBookingStatus): string {
    switch (status) {
      case 'Pending': return 'text-bg-warning';
      case 'Quoted': return 'text-bg-info';
      case 'Confirmed': return 'text-bg-primary';
      case 'Completed': return 'text-bg-success';
      case 'Cancelled': return 'text-bg-danger';
      case 'Disputed': return 'text-bg-secondary';
    }
  }
}
