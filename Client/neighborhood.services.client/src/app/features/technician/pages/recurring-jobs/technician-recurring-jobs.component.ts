import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable, Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { RecurringJobService } from '../../services/recurring-job.service';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { RecurringBooking, RecurringBookingStatus } from '../../../customer/models/recurring-booking.model';
import { SetPriceModalComponent } from '../../components/set-price-modal/set-price-modal.component';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { NotificationServiceService } from '../../../../shared/services/notification-service.service';
import { LightboxService } from '../../../../shared/services/lightbox.service';
import { RecurringBookingDetailsModalComponent } from '../../../customer/components/recurring-booking-details-modal/recurring-booking-details-modal.component';

interface Tab {
  value: 'All' | RecurringBookingStatus;
}

@Component({
  selector: 'app-technician-recurring-jobs',
  imports: [CurrencyPipe, TranslatePipe],
  templateUrl: './technician-recurring-jobs.component.html',
  styleUrl: '../../../../shared/styles/ns-card.css',
})
export class TechnicianRecurringJobsComponent implements OnInit {
  private readonly service = inject(RecurringJobService);
  private readonly modal = inject(NgbModal);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);
  private readonly confirm = inject(ConfirmService);
  private readonly notificationService = inject(NotificationServiceService);
  protected readonly lightbox = inject(LightboxService);

  readonly tabs: Tab[] = [
    { value: 'All' },
    { value: 'PendingApproval' },
    { value: 'PendingCustomerApproval' },
    { value: 'Active' },
    { value: 'Paused' },
    { value: 'Cancelled' },
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

    // Refresh the list when a realtime notification arrives (e.g. customer approved the price).
    this.notificationService.notificationReceived$
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.load());
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading.set(true);
    this.service
      .getMyJobs({
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

  // --- actions ---

  details(rb: RecurringBooking) {
    const ref = this.modal.open(RecurringBookingDetailsModalComponent, { size: 'lg', scrollable: true });
    ref.componentInstance.booking = rb;
  }

  setPrice(rb: RecurringBooking) {
    const ref = this.modal.open(SetPriceModalComponent);
    ref.componentInstance.address = rb.address;
    ref.componentInstance.scheduleText = this.scheduleText(rb);
    ref.componentInstance.durationMinutes = rb.durationMinutes;
    ref.componentInstance.description = rb.description;
    ref.componentInstance.imageUrl = rb.imageUrl ?? null;
    ref.result.then(
      (price: number) =>
        this.run(rb.id, this.service.setPrice(rb.id, price), this.translate.instant('technician.recurring.priceSent')),
      () => {},
    );
  }

  cancel(rb: RecurringBooking) {
    this.confirm
      .confirm({
        messageKey: 'technician.recurring.cancelPrompt',
        confirmKey: 'technician.recurring.cancel',
        variant: 'danger',
      })
      .then((ok) => {
        if (!ok) return;
        this.run(rb.id, this.service.cancel(rb.id), this.translate.instant('technician.recurring.cancelled'));
      });
  }

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

  // --- UI helpers (shared formatting with the customer recurring i18n) ---

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
