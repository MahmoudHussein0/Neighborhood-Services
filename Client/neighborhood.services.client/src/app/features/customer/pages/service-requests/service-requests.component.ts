import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { ServiceRequestService } from '../../services/service-request.service';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { ServiceRequestSummary, ServiceRequestStatus } from '../../models/service-request.model';
import { CreateServiceRequestModalComponent } from '../../components/create-service-request-modal/create-service-request-modal.component';
import { NotificationServiceService } from '../../../../shared/services/notification-service.service';

type StatusTab = 'All' | ServiceRequestStatus;

@Component({
  selector: 'app-service-requests',
  imports: [DatePipe, CurrencyPipe, TranslatePipe],
  templateUrl: './service-requests.component.html',
  styleUrl: './service-requests.component.css',
})
export class ServiceRequestsComponent implements OnInit {
  private readonly service = inject(ServiceRequestService);
  private readonly modal = inject(NgbModal);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);
  private readonly notificationService = inject(NotificationServiceService);

  readonly tabs: StatusTab[] = ['All', 'Open', 'Closed', 'Expired'];
  readonly pageSize = 10;

  loading = signal(false);
  result = signal<PagedResult<ServiceRequestSummary> | null>(null);
  activeTab = signal<StatusTab>('All');
  searchTerm = signal('');
  page = signal(1);

  private readonly search$ = new Subject<string>();

  constructor() {
    this.search$
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(() => {
        this.page.set(1);
        this.load();
      });

    // Refresh the list when a realtime notification arrives (e.g. a new offer came in).
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
      .getMine({
        status: this.activeTab() === 'All' ? undefined : (this.activeTab() as ServiceRequestStatus),
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

  onSearchInput(value: string) {
    this.searchTerm.set(value);
    this.search$.next(value);
  }

  postRequest() {
    const ref = this.modal.open(CreateServiceRequestModalComponent, { size: 'lg' });
    ref.result.then(
      () => {
        this.toastr.success(this.translate.instant('serviceRequests.posted'));
        this.activeTab.set('All');
        this.page.set(1);
        this.load();
      },
      () => {} // dismissed — do nothing
    );
  }

  viewOffers(sr: ServiceRequestSummary) {
    this.router.navigate(['/customer/service-requests', sr.id]);
  }

  // --- UI helpers ---
  badgeClass(status: ServiceRequestStatus): string {
    switch (status) {
      case 'Open': return 'text-bg-primary';
      case 'Closed': return 'text-bg-secondary';
      case 'Expired': return 'text-bg-warning';
      case 'PendingReview': return 'text-bg-info';
      case 'Flagged': return 'text-bg-danger';
    }
  }
}
