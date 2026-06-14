import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable, Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { JobService } from '../../services/job.service';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { MyBookingSummary, BookingStatus, DisputeType } from '../../../customer/models/booking.model';
import { QuoteJobModalComponent } from '../../components/quote-job-modal/quote-job-modal.component';
import { CompleteJobModalComponent } from '../../components/complete-job-modal/complete-job-modal.component';
import { RaiseDisputeModalComponent } from '../../../customer/components/raise-dispute-modal/raise-dispute-modal.component';
import { LeaveReviewModalComponent } from '../../../customer/components/leave-review-modal/leave-review-modal.component';
import { googleMapsUrl } from '../../../../core/utils/maps.util';
import { NotificationServiceService } from '../../../../shared/services/notification-service.service';

interface Tab {
  value: 'All' | BookingStatus;
}

@Component({
  selector: 'app-technician-jobs',
  imports: [CurrencyPipe, DatePipe, TranslatePipe],
  templateUrl: './technician-jobs.component.html',
})
export class TechnicianJobsComponent implements OnInit {
  private readonly service = inject(JobService);
  private readonly modal = inject(NgbModal);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);
  private readonly notificationService = inject(NotificationServiceService);

  readonly tabs: Tab[] = [
    { value: 'All' },
    { value: 'Pending' },
    { value: 'Quoted' },
    { value: 'Confirmed' },
    { value: 'Completed' },
    { value: 'Cancelled' },
  ];
  readonly pageSize = 10;

  loading = signal(false);
  result = signal<PagedResult<MyBookingSummary> | null>(null);
  activeTab = signal<'All' | BookingStatus>('All');
  searchTerm = signal('');
  page = signal(1);
  busyId = signal<number | null>(null);

  protected readonly mapsUrl = googleMapsUrl;

  private readonly search$ = new Subject<string>();

  constructor() {
    this.search$
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => {
        this.page.set(1);
        this.load();
      });

    // Refresh the list when a realtime notification arrives (e.g. new booking request, quote accepted).
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

  selectTab(value: 'All' | BookingStatus) {
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

  // Pending: first quote.  Quoted: tech can edit the existing quote (same modal, prefilled).
  quote(job: MyBookingSummary) {
    const ref = this.modal.open(QuoteJobModalComponent);
    ref.componentInstance.job = job;
    ref.result.then(
      () => {
        this.toastr.success(this.translate.instant('technician.jobs.quoted'));
        this.load();
      },
      () => {},
    );
  }

  complete(job: MyBookingSummary) {
    const ref = this.modal.open(CompleteJobModalComponent);
    ref.componentInstance.job = job;
    ref.result.then(
      () =>
        this.run(job.id, this.service.complete(job.id), this.translate.instant('technician.jobs.completed')),
      () => {},
    );
  }

  // Technician disputes the customer — type list is the customer-facing subset that makes sense
  // from the tech's side (non-payment, scam/abuse, or anything else).
  raiseDispute(job: MyBookingSummary) {
    const ref = this.modal.open(RaiseDisputeModalComponent);
    const types: DisputeType[] = ['PaymentIssue', 'Scam', 'Other'];
    ref.componentInstance.types = types;
    ref.componentInstance.disputeType.set(types[0]);
    ref.result.then(
      (res: { disputeType: DisputeType; reason: string }) =>
        this.run(
          job.id,
          this.service.raiseDispute(job.id, res.disputeType, res.reason),
          this.translate.instant('dispute.raised'),
        ),
      () => {},
    );
  }

  // A dispute only makes sense once there's a committed job (mirrors the backend rule).
  canDispute(status: BookingStatus): boolean {
    return status === 'Confirmed' || status === 'Completed';
  }

  // Tech can review the customer only after the customer has confirmed the completed work.
  // Hidden once a review exists (HasReview is per-current-user from /api/bookings/mine).
  canReview(b: MyBookingSummary): boolean {
    return b.status === 'Completed' && b.clientConfirmed && !b.hasReview;
  }

  leaveReview(job: MyBookingSummary) {
    const ref = this.modal.open(LeaveReviewModalComponent);
    ref.result.then(
      (res: { rating: number; comment: string }) =>
        this.run(
          job.id,
          this.service.createReview(job.id, res.rating, res.comment),
          this.translate.instant('review.submitted'),
        ),
      () => {},
    );
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

  badgeClass(status: BookingStatus): string {
    switch (status) {
      case 'Confirmed': return 'text-bg-primary';
      case 'Completed': return 'text-bg-success';
      case 'Cancelled': return 'text-bg-danger';
      case 'Disputed': return 'text-bg-warning';
      case 'Quoted': return 'text-bg-info';
      case 'Pending': return 'text-bg-secondary';
    }
  }
}
