import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { OfferService } from '../../services/offer.service';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { Offer, OfferStatus } from '../../models/offer.model';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { NotificationServiceService } from '../../../../shared/services/notification-service.service';

interface Tab {
  value: 'All' | OfferStatus;
}

@Component({
  selector: 'app-technician-offers',
  imports: [CurrencyPipe, DatePipe, TranslatePipe, RouterLink],
  templateUrl: './technician-offers.component.html',
  styleUrl: '../../../../shared/styles/ns-card.css',
})
export class TechnicianOffersComponent implements OnInit {
  private readonly service = inject(OfferService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);
  private readonly confirm = inject(ConfirmService);
  private readonly notificationService = inject(NotificationServiceService);

  readonly tabs: Tab[] = [
    { value: 'All' },
    { value: 'Pending' },
    { value: 'Accepted' },
    { value: 'Rejected' },
    { value: 'Withdrawn' },
    { value: 'Expired' },
  ];
  readonly pageSize = 5;

  loading = signal(false);
  result = signal<PagedResult<Offer> | null>(null);
  activeTab = signal<'All' | OfferStatus>('All');
  page = signal(1);
  busyId = signal<number | null>(null);

  constructor() {
    // Refresh the list when a realtime notification arrives (e.g. offer accepted/rejected).
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
      .getMyOffers({
        status: this.activeTab() === 'All' ? undefined : (this.activeTab() as OfferStatus),
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

  selectTab(value: 'All' | OfferStatus) {
    this.activeTab.set(value);
    this.page.set(1);
    this.load();
  }

  goToPage(p: number) {
    this.page.set(p);
    this.load();
  }

  withdraw(offer: Offer) {
    this.confirm
      .confirm({
        messageKey: 'technician.offers.withdrawPrompt',
        confirmKey: 'technician.offers.withdraw',
        variant: 'danger',
      })
      .then((ok) => {
        if (!ok) return;
        this.busyId.set(offer.id);
        this.service.withdraw(offer.id).subscribe({
          next: () => {
            this.busyId.set(null);
            this.toastr.success(this.translate.instant('technician.offers.withdrawn'));
            this.load();
          },
          error: () => this.busyId.set(null),
        });
      });
  }

  badgeClass(status: OfferStatus): string {
    switch (status) {
      case 'Accepted': return 'text-bg-success';
      case 'Pending': return 'text-bg-secondary';
      case 'Rejected': return 'text-bg-danger';
      case 'Withdrawn': return 'text-bg-dark';
      case 'Expired': return 'text-bg-warning';
    }
  }
}
