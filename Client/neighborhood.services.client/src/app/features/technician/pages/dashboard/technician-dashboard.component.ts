import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { JobService } from '../../services/job.service';
import { OfferService } from '../../services/offer.service';
import { TechnicianWalletService } from '../../services/technician-wallet.service';
import { MyBookingSummary, BookingStatus } from '../../../customer/models/booking.model';
import { AuthService } from '../../../auth/services/auth.service';

const STATUS_ORDER: BookingStatus[] = ['Pending', 'Quoted', 'Confirmed', 'Completed', 'Cancelled', 'Disputed'];
const STATUS_COLOR: Record<BookingStatus, string> = {
  Pending: '#f59e0b',
  Quoted: '#3b82f6',
  Confirmed: '#8b5cf6',
  Completed: '#10b981',
  Cancelled: '#ef4444',
  Disputed: '#94a3b8',
};

@Component({
  selector: 'app-technician-dashboard',
  imports: [RouterLink, CurrencyPipe, DatePipe, TranslatePipe],
  templateUrl: './technician-dashboard.component.html',
})
export class TechnicianDashboardComponent implements OnInit {
  private readonly jobSvc = inject(JobService);
  private readonly offerSvc = inject(OfferService);
  private readonly walletSvc = inject(TechnicianWalletService);
  private readonly auth = inject(AuthService);

  loading = signal(true);
  error = signal(false);

  private jobs = signal<MyBookingSummary[]>([]);
  totalJobs = signal(0);
  pendingOffers = signal(0);
  walletBalance = signal(0);

  readonly firstName = computed(() => (this.auth.currentUser()?.fullName ?? '').split(' ')[0] || 'there');

  count(status: BookingStatus) {
    return this.jobs().filter((b) => b.status === status).length;
  }

  active = computed(() => this.count('Confirmed'));
  completed = computed(() => this.count('Completed'));
  needsQuote = computed(() => this.count('Pending'));

  recent = computed(() =>
    [...this.jobs()]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 7),
  );

  bars = computed(() => {
    const all = this.jobs();
    const total = all.length || 1;
    return STATUS_ORDER.map((s) => {
      const count = all.filter((b) => b.status === s).length;
      return { status: s, count, pct: Math.round((count / total) * 100), color: STATUS_COLOR[s] };
    });
  });

  completionPct = computed(() => {
    const total = this.jobs().length;
    return total ? Math.round((this.completed() / total) * 100) : 0;
  });

  donutStyle = computed(() => {
    const pct = this.completionPct();
    return `background: conic-gradient(#10b981 0% ${pct}%, #eef1f6 ${pct}% 100%)`;
  });

  ngOnInit() {
    this.loadAll();
  }

  loadAll() {
    this.loading.set(true);
    this.error.set(false);

    forkJoin({
      jobs: this.jobSvc.getMyJobs({ pageSize: 100 }).pipe(catchError(() => of(null))),
      offers: this.offerSvc.getMyOffers({ status: 'Pending', pageSize: 1 }).pipe(catchError(() => of(null))),
      wallet: this.walletSvc.getMyWallet().pipe(catchError(() => of(null))),
    }).subscribe({
      next: ({ jobs, offers, wallet }) => {
        this.jobs.set(jobs?.items ?? []);
        this.totalJobs.set(jobs?.totalCount ?? 0);
        this.pendingOffers.set(offers?.totalCount ?? 0);
        this.walletBalance.set(wallet?.balance ?? 0);
        this.loading.set(false);
      },
      error: () => {
        this.error.set(true);
        this.loading.set(false);
      },
    });
  }
}
