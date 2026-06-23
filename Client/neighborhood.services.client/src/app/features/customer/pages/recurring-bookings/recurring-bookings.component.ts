import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable, Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { RecurringBookingService } from '../../services/recurring-booking.service';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { RecurringBooking, RecurringBookingStatus } from '../../models/recurring-booking.model';
import { CancelBookingModalComponent } from '../../components/cancel-booking-modal/cancel-booking-modal.component';
import { EditRecurringBookingModalComponent } from '../../components/edit-recurring-booking-modal/edit-recurring-booking-modal.component';
import { RecurringBookingDetailsModalComponent } from '../../components/recurring-booking-details-modal/recurring-booking-details-modal.component';
import { nextOccurrence } from '../../utils/recurrence.util';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { NotificationServiceService } from '../../../../shared/services/notification-service.service';
import { LightboxService } from '../../../../shared/services/lightbox.service';

interface Tab {
  value: 'All' | RecurringBookingStatus;
  label: string;
}

@Component({
  selector: 'app-recurring-bookings',
  imports: [CurrencyPipe, DatePipe, TranslatePipe],
  templateUrl: './recurring-bookings.component.html',
  styleUrl: './recurring-bookings.component.css',
})
export class RecurringBookingsComponent implements OnInit {
  private readonly service = inject(RecurringBookingService);
  private readonly modal = inject(NgbModal);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);
  private readonly confirm = inject(ConfirmService);
  private readonly notificationService = inject(NotificationServiceService);
  protected readonly lightbox = inject(LightboxService);

  readonly tabs: Tab[] = [
    { value: 'All', label: 'All' },
    { value: 'PendingApproval', label: 'Awaiting Price' },
    { value: 'PendingCustomerApproval', label: 'Awaiting You' },
    { value: 'Active', label: 'Active' },
    { value: 'Paused', label: 'Paused' },
    { value: 'Cancelled', label: 'Cancelled' },
  ];
  readonly pageSize = 5;

  loading = signal(false);
  result = signal<PagedResult<RecurringBooking> | null>(null);
  activeTab = signal<'All' | RecurringBookingStatus>('All');
  searchTerm = signal('');
  page = signal(1);
  busyId = signal<number | null>(null);

  private readonly search$ = new Subject<string>();

  constructor() {
    this.search$
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => {
        this.page.set(1);
        this.load();
      });

    // Refresh the list when a realtime notification arrives (e.g. technician set the price).
    this.notificationService.notificationReceived$
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.load());
  }

  ngOnInit() {
    this.load();
  }

  /** "New" → go pick a technician first; the recurring is set up from a technician card. */
  create() {
    this.router.navigate(['/customer/find-technician']);
  }

  load() {
    this.loading.set(true);
    this.service
      .getMine({
        status: this.activeTab() === 'All' ? undefined : (this.activeTab() as RecurringBookingStatus),
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

  selectTab(value: 'All' | RecurringBookingStatus) {
    this.activeTab.set(value);
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

  // --- details + edit ---

  details(rb: RecurringBooking) {
    const ref = this.modal.open(RecurringBookingDetailsModalComponent);
    ref.componentInstance.booking = rb;
  }

  edit(rb: RecurringBooking) {
    const ref = this.modal.open(EditRecurringBookingModalComponent, { size: 'lg' });
    ref.componentInstance.booking = rb;
    ref.result.then(
      () => {
        this.toastr.success(this.translate.instant('recurring.updated'));
        this.load();
      },
      () => {}
    );
  }

  // --- lifecycle actions ---

  approve(rb: RecurringBooking) {
    const price = `EGP ${rb.agreedPrice ?? 0}`;
    this.confirm
      .confirm({
        messageKey: 'recurring.approvePrompt',
        messageParams: { price },
        confirmKey: 'recurring.approve',
        variant: 'success',
      })
      .then((ok) => {
        if (!ok) return;
        this.run(rb.id, this.service.approve(rb.id), this.translate.instant('recurring.approved'));
      });
  }

  rejectPrice(rb: RecurringBooking) {
    this.confirm
      .confirm({
        messageKey: 'recurring.rejectPrompt',
        confirmKey: 'recurring.rejectPrice',
        variant: 'danger',
      })
      .then((ok) => {
        if (!ok) return;
        this.run(rb.id, this.service.rejectPrice(rb.id), this.translate.instant('recurring.rejected'));
      });
  }

  pause(rb: RecurringBooking) {
    this.run(rb.id, this.service.pause(rb.id), this.translate.instant('recurring.paused'));
  }

  resume(rb: RecurringBooking) {
    this.run(rb.id, this.service.resume(rb.id), this.translate.instant('recurring.resumed'));
  }

  cancel(rb: RecurringBooking) {
    const ref = this.modal.open(CancelBookingModalComponent);
    ref.result.then(
      (reason: string) => this.run(rb.id, this.service.cancel(rb.id, reason), this.translate.instant('recurring.cancelled')),
      () => {}
    );
  }

  /** Runs an action observable with a busy guard + toast + reload. */
  private run(id: number, action: Observable<unknown>, successMsg: string) {
    this.busyId.set(id);
    action.subscribe({
      next: () => {
        this.busyId.set(null);
        this.toastr.success(successMsg);
        this.load();
      },
      error: () => this.busyId.set(null),
    });
  }

  // --- UI helpers ---

  /** Next projected visit datetime for the card one-liner (null if none / paused / cancelled). */
  nextVisit(rb: RecurringBooking): Date | null {
    return nextOccurrence(rb);
  }

  scheduleText(rb: RecurringBooking): string {
    const time = rb.timeOfDay?.slice(0, 5) ?? '';
    switch (rb.pattern) {
      case 'Daily':
        return this.translate.instant('recurring.schedule.Daily', { time });
      case 'Weekly':
        return this.translate.instant('recurring.schedule.Weekly', {
          day: this.translate.instant('recurring.days.' + rb.dayOfWeek),
          time,
        });
      case 'Monthly':
        return this.translate.instant('recurring.schedule.Monthly', { day: rb.dayOfMonth, time });
    }
  }

  badgeClass(status: RecurringBookingStatus): string {
    switch (status) {
      case 'Active': return 'text-bg-success';
      case 'Paused': return 'text-bg-warning';
      case 'Cancelled': return 'text-bg-danger';
      case 'PendingCustomerApproval': return 'text-bg-primary';
      case 'PendingApproval': return 'text-bg-secondary';
    }
  }
}
