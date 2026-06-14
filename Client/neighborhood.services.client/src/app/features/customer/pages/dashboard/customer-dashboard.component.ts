import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { BookingService } from '../../services/booking.service';
import { ServiceRequestService } from '../../services/service-request.service';
import { RecurringBookingService } from '../../services/recurring-booking.service';
import { CustomerWalletService } from '../../services/customer-wallet.service';
import { MyBookingSummary, BookingStatus } from '../../models/booking.model';
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
  selector: 'app-customer-dashboard',
  imports: [RouterLink, DecimalPipe, CurrencyPipe, DatePipe],
  templateUrl: './customer-dashboard.component.html',
})
export class CustomerDashboardComponent implements OnInit {
  private readonly bookingSvc = inject(BookingService);
  private readonly requestSvc = inject(ServiceRequestService);
  private readonly recurringSvc = inject(RecurringBookingService);
  private readonly walletSvc = inject(CustomerWalletService);
  private readonly auth = inject(AuthService);

  loading = signal(true);
  error = signal(false);

  private bookings = signal<MyBookingSummary[]>([]);
  totalBookings = signal(0);
  openRequests = signal(0);
  activeRecurring = signal(0);
  walletBalance = signal(0);

  readonly firstName = computed(() => (this.auth.currentUser()?.fullName ?? '').split(' ')[0] || 'there');

  count(status: BookingStatus) {
    return this.bookings().filter((b) => b.status === status).length;
  }

  active = computed(() => this.count('Confirmed'));
  completed = computed(() => this.count('Completed'));
  pending = computed(() => this.count('Pending'));

  recent = computed(() =>
    [...this.bookings()]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 7),
  );

  bars = computed(() => {
    const all = this.bookings();
    const total = all.length || 1;
    return STATUS_ORDER.map((s) => {
      const count = all.filter((b) => b.status === s).length;
      return { status: s, count, pct: Math.round((count / total) * 100), color: STATUS_COLOR[s] };
    });
  });

  completionPct = computed(() => {
    const total = this.bookings().length;
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
      bookings: this.bookingSvc.getMyBookings({ pageSize: 100 }).pipe(catchError(() => of(null))),
      requests: this.requestSvc.getMine({ status: 'Open', pageSize: 1 }).pipe(catchError(() => of(null))),
      recurring: this.recurringSvc.getMine({ status: 'Active', pageSize: 1 }).pipe(catchError(() => of(null))),
      wallet: this.walletSvc.getMyWallet().pipe(catchError(() => of(null))),
    }).subscribe({
      next: ({ bookings, requests, recurring, wallet }) => {
        this.bookings.set(bookings?.items ?? []);
        this.totalBookings.set(bookings?.totalCount ?? 0);
        this.openRequests.set(requests?.totalCount ?? 0);
        this.activeRecurring.set(recurring?.totalCount ?? 0);
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
