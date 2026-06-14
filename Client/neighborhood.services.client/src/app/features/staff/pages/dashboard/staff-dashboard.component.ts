import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, DecimalPipe, CurrencyPipe, DatePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { StaffBookingService, StaffBooking, StaffBookingStatus } from '../../services/staff-booking.service';
import { StaffUsersService } from '../../services/staff-users.service';
import { SupportTicketsService } from '../../services/support-ticket.service';
import { DisputeService } from '../../services/disputes.service';
import { ReviewsService } from '../../services/Reviews.service';
import { KnowledgeService } from '../../services/knowledge.service';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-staff-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, DecimalPipe, CurrencyPipe, DatePipe],
  template: `
    <div class="dash-root">

      <!-- Header -->
      <div class="dash-header mb-4">
        <div>
          <h2 class="dash-title">Staff Overview</h2>
          <p class="dash-subtitle">Real-time snapshot of platform activity</p>
        </div>
        <div class="header-actions">
          <button class="btn-refresh btn-reindex" (click)="reindexKnowledge()" [disabled]="reindexing()">
            <span class="refresh-icon" [class.spinning]="reindexing()">{{ reindexing() ? '↻' : '🧠' }}</span>
            {{ reindexing() ? 'Reindexing…' : 'Reindex Knowledge' }}
          </button>
          <button class="btn-refresh" (click)="loadAll()" [disabled]="loading()">
            <span class="refresh-icon" [class.spinning]="loading()">↻</span>
            Refresh
          </button>
        </div>
      </div>

      <!-- Skeleton -->
      @if (loading()) {
        <div class="skeleton-grid mb-4">
          @for (i of [1,2,3,4,5,6]; track i) {
            <div class="skeleton-card"></div>
          }
        </div>
        <div class="skeleton-grid cols-3">
          @for (i of [1,2,3]; track i) {
            <div class="skeleton-card tall"></div>
          }
        </div>
      }

      <!-- Error -->
      @else if (error()) {
        <div class="error-banner">
          <span>⚠️ فشل تحميل بيانات الـ Dashboard</span>
          <button class="btn-retry" (click)="loadAll()">إعادة المحاولة</button>
        </div>
      }

      @else {

        <!-- ── Stat Cards ── -->
        <div class="stats-grid mb-4">

          <a routerLink="/staff/bookings" class="stat-card" style="--c:#3b82f6">
            <div class="stat-icon">📋</div>
            <div class="stat-body">
              <span class="stat-value" style="color:#3b82f6">{{ totalBookings() }}</span>
              <span class="stat-label">Total Bookings</span>
              <span class="stat-sub">{{ completedBookings() }} completed</span>
            </div>
            <div class="stat-accent" style="background:#3b82f6"></div>
          </a>

          <a routerLink="/staff/users" class="stat-card" style="--c:#8b5cf6">
            <div class="stat-icon">👥</div>
            <div class="stat-body">
              <span class="stat-value" style="color:#8b5cf6">{{ totalUsers() }}</span>
              <span class="stat-label">Users</span>
              <span class="stat-sub">Registered accounts</span>
            </div>
            <div class="stat-accent" style="background:#8b5cf6"></div>
          </a>

          <a routerLink="/staff/support-tickets" class="stat-card" style="--c:#f59e0b">
            <div class="stat-icon">🎫</div>
            <div class="stat-body">
              <span class="stat-value" style="color:#f59e0b">{{ totalTickets() }}</span>
              <span class="stat-label">Support Tickets</span>
              <span class="stat-sub">All tickets</span>
            </div>
            <div class="stat-accent" style="background:#f59e0b"></div>
          </a>

          <a routerLink="/staff/disputes" class="stat-card" style="--c:#ef4444">
            <div class="stat-icon">⚖️</div>
            <div class="stat-body">
              <span class="stat-value" style="color:#ef4444">{{ openDisputes() }}</span>
              <span class="stat-label">Open Disputes</span>
              <span class="stat-sub">Need resolution</span>
            </div>
            <div class="stat-accent" style="background:#ef4444"></div>
          </a>

          <a routerLink="/staff/reviews" class="stat-card" style="--c:#10b981">
            <div class="stat-icon">⭐</div>
            <div class="stat-body">
              <span class="stat-value" style="color:#10b981">{{ totalReviews() }}</span>
              <span class="stat-label">Reviews</span>
              <span class="stat-sub">{{ flaggedReviews() }} flagged</span>
            </div>
            <div class="stat-accent" style="background:#10b981"></div>
          </a>

          <a routerLink="/staff/bookings" class="stat-card" style="--c:#f43f5e">
            <div class="stat-icon">🚨</div>
            <div class="stat-body">
              <span class="stat-value" style="color:#f43f5e">{{ disputedBookings() }}</span>
              <span class="stat-label">Disputed Bookings</span>
              <span class="stat-sub">Active disputes</span>
            </div>
            <div class="stat-accent" style="background:#f43f5e"></div>
          </a>

        </div>

        <!-- ── Charts Row ── -->
        <div class="charts-row mb-4">

          <!-- Bookings by Status -->
          <div class="chart-card">
            <h3 class="chart-title">Bookings by Status</h3>
            <div class="bar-list">
              @for (bar of bookingBars(); track bar.status) {
                <div class="bar-item">
                  <div class="bar-meta">
                    <span class="bar-label">{{ bar.status }}</span>
                    <span class="bar-count">{{ bar.count }}</span>
                  </div>
                  <div class="bar-track">
                    <div
                      class="bar-fill"
                      [style.width.%]="bar.pct"
                      [style.background]="bar.color"
                    ></div>
                  </div>
                </div>
              }
            </div>
          </div>

          <!-- Open Issues Donut -->
          <div class="chart-card center-content">
            <h3 class="chart-title">Open Issues</h3>
            <div class="donut-wrap">
              <div class="donut" [style]="donutGradient()">
                <div class="donut-hole">
                  <span class="donut-num">{{ openDisputes() + totalTickets() }}</span>
                  <span class="donut-sub">Total</span>
                </div>
              </div>
              <div class="donut-legend">
                <div class="legend-item">
                  <span class="dot" style="background:#f59e0b"></span>
                  <span class="leg-label">Tickets</span>
                  <span class="leg-val">{{ totalTickets() }}</span>
                </div>
                <div class="legend-item">
                  <span class="dot" style="background:#ef4444"></span>
                  <span class="leg-label">Disputes</span>
                  <span class="leg-val">{{ openDisputes() }}</span>
                </div>
                <div class="legend-item">
                  <span class="dot" style="background:#f43f5e"></span>
                  <span class="leg-label">Flagged Reviews</span>
                  <span class="leg-val">{{ flaggedReviews() }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Reviews Rings -->
          <div class="chart-card">
            <h3 class="chart-title">Reviews Breakdown</h3>
            <div class="rings-wrap">
              @for (ring of reviewRings(); track ring.label) {
                <div class="ring-item">
                  <svg viewBox="0 0 36 36" class="ring-svg">
                    <circle cx="18" cy="18" r="15.9"
                      fill="none" stroke="#f1f5f9" stroke-width="3.5"/>
                    <circle cx="18" cy="18" r="15.9"
                      fill="none"
                      [attr.stroke]="ring.color"
                      stroke-width="3.5"
                      [attr.stroke-dasharray]="ring.pct + ' ' + (100 - ring.pct)"
                      stroke-dashoffset="25"
                      stroke-linecap="round"/>
                  </svg>
                  <div class="ring-meta">
                    <span class="ring-pct">{{ ring.pct | number:'1.0-0' }}%</span>
                    <span class="ring-label">{{ ring.label }}</span>
                    <span class="ring-count">{{ ring.count }}</span>
                  </div>
                </div>
              }
            </div>
          </div>

        </div>

        <!-- ── Recent Bookings Table ── -->
        <div class="table-card">
          <div class="table-head-row">
            <h3 class="chart-title mb-0">Recent Bookings</h3>
            <a routerLink="/staff/bookings" class="view-all">View all →</a>
          </div>
          <div class="table-scroll">
            <table class="dash-table">
              <thead>
                <tr>
                  <th>#</th>
                  <th>Customer</th>
                  <th>Technician</th>
                  <th>Type</th>
                  <th>Status</th>
                  <th>Scheduled</th>
                  <th>Price</th>
                </tr>
              </thead>
              <tbody>
                @for (b of recentBookings(); track b.id) {
                  <tr>
                    <td class="muted">{{ b.id }}</td>
                    <td>{{ b.customerName }}</td>
                    <td>{{ b.technicianName }}</td>
                    <td><span class="type-badge">{{ b.bookingType }}</span></td>
                    <td><span [class]="'sbadge sbadge-' + b.status.toLowerCase()">{{ b.status }}</span></td>
                    <td class="muted">{{ b.scheduledAt | date:'dd MMM, HH:mm' }}</td>
                    <td>{{ (b.finalPrice || b.estimatedPrice) | currency:'EGP':'symbol':'1.0-0' }}</td>
                  </tr>
                } @empty {
                  <tr><td colspan="7" class="empty-row">No bookings found</td></tr>
                }
              </tbody>
            </table>
          </div>
        </div>

      }
    </div>
  `,
  styles: [`
    :host { display: block; }

    .dash-root {
      padding: 1.5rem;
      font-family: 'Segoe UI', system-ui, sans-serif;
      color: #1e293b;
      min-height: 100vh;
      background: #f8fafc;
    }

    /* Header */
    .dash-header { display:flex; align-items:flex-start; justify-content:space-between; flex-wrap:wrap; gap:.75rem; }
    .dash-title  { font-size:1.6rem; font-weight:700; margin:0; color:#0f172a; }
    .dash-subtitle { font-size:.85rem; color:#64748b; margin:.2rem 0 0; }
    .btn-refresh {
      display:flex; align-items:center; gap:.4rem;
      padding:.45rem 1rem; border-radius:8px;
      border:1px solid #e2e8f0; background:#fff;
      font-size:.85rem; color:#475569; cursor:pointer;
      transition:background .15s;
    }
    .btn-refresh:hover  { background:#f1f5f9; }
    .btn-refresh:disabled { opacity:.5; cursor:not-allowed; }
    .header-actions { display:flex; align-items:center; gap:.6rem; flex-wrap:wrap; }
    .btn-reindex { border-color:#c7d7fe; color:#1d4ed8; }
    .btn-reindex:hover { background:#eff6ff; }
    .refresh-icon { font-size:1.1rem; display:inline-block; }
    .refresh-icon.spinning { animation: spin .7s linear infinite; }
    @keyframes spin { to { transform:rotate(360deg); } }

    /* Skeleton */
    .skeleton-grid {
      display:grid;
      grid-template-columns:repeat(auto-fill, minmax(175px,1fr));
      gap:1rem;
    }
    .skeleton-grid.cols-3 { grid-template-columns:repeat(3,1fr); }
    .skeleton-card {
      height:110px; border-radius:14px;
      background:linear-gradient(90deg,#f0f0f0 25%,#e0e0e0 50%,#f0f0f0 75%);
      background-size:200% 100%;
      animation:shimmer 1.4s infinite;
    }
    .skeleton-card.tall { height:220px; }
    @keyframes shimmer { to { background-position:-200% 0; } }

    /* Error */
    .error-banner {
      padding:1rem 1.25rem; border-radius:10px;
      background:#fef2f2; border:1px solid #fecaca;
      color:#dc2626; display:flex; align-items:center; gap:1rem;
    }
    .btn-retry {
      padding:.3rem .8rem; border-radius:6px;
      border:1px solid #dc2626; background:transparent;
      color:#dc2626; cursor:pointer; font-size:.85rem;
    }

    /* Stat Cards */
    .stats-grid {
      display:grid;
      grid-template-columns:repeat(auto-fill, minmax(175px,1fr));
      gap:1rem;
    }
    .stat-card {
      position:relative; overflow:hidden;
      background:#fff; border-radius:14px;
      border:1px solid #e2e8f0;
      padding:1.2rem 1rem 1rem;
      display:flex; gap:.75rem; align-items:flex-start;
      text-decoration:none; color:inherit;
      transition:box-shadow .2s, transform .2s;
    }
    .stat-card:hover { box-shadow:0 6px 20px rgba(0,0,0,.08); transform:translateY(-2px); }
    .stat-accent { position:absolute; bottom:0; left:0; right:0; height:3px; }
    .stat-icon   { font-size:1.5rem; line-height:1; }
    .stat-body   { display:flex; flex-direction:column; gap:.1rem; }
    .stat-value  { font-size:1.65rem; font-weight:700; line-height:1; }
    .stat-label  { font-size:.78rem; color:#64748b; font-weight:500; margin-top:.2rem; }
    .stat-sub    { font-size:.7rem; color:#94a3b8; }

    /* Charts Row */
    .charts-row {
      display:grid;
      grid-template-columns:1fr 1fr 1fr;
      gap:1rem;
    }
    @media(max-width:900px) { .charts-row { grid-template-columns:1fr; } }

    .chart-card {
      background:#fff; border-radius:14px;
      border:1px solid #e2e8f0; padding:1.25rem;
    }
    .chart-card.center-content { display:flex; flex-direction:column; }
    .chart-title { font-size:.95rem; font-weight:700; color:#0f172a; margin:0 0 1rem; }

    /* Bar Chart */
    .bar-list   { display:flex; flex-direction:column; gap:.7rem; }
    .bar-item   { display:flex; flex-direction:column; gap:.25rem; }
    .bar-meta   { display:flex; justify-content:space-between; align-items:center; }
    .bar-label  { font-size:.75rem; color:#475569; font-weight:500; }
    .bar-count  { font-size:.75rem; color:#94a3b8; font-weight:600; }
    .bar-track  { height:8px; border-radius:99px; background:#f1f5f9; overflow:hidden; }
    .bar-fill   { height:100%; border-radius:99px; transition:width .8s cubic-bezier(.4,0,.2,1); }

    /* Donut */
    .donut-wrap { display:flex; flex-direction:column; align-items:center; gap:1.25rem; flex:1; justify-content:center; }
    .donut {
      width:120px; height:120px; border-radius:50%;
      position:relative; flex-shrink:0;
    }
    .donut-hole {
      position:absolute; inset:20px; border-radius:50%;
      background:#fff; display:flex; flex-direction:column;
      align-items:center; justify-content:center;
    }
    .donut-num  { font-size:1.4rem; font-weight:700; color:#0f172a; line-height:1; }
    .donut-sub  { font-size:.65rem; color:#94a3b8; }
    .donut-legend { display:flex; flex-direction:column; gap:.5rem; width:100%; }
    .legend-item  { display:flex; align-items:center; gap:.5rem; }
    .dot          { width:9px; height:9px; border-radius:50%; flex-shrink:0; }
    .leg-label    { font-size:.78rem; color:#475569; flex:1; }
    .leg-val      { font-size:.78rem; font-weight:600; color:#0f172a; }

    /* Rings */
    .rings-wrap { display:flex; gap:.75rem; justify-content:space-around; flex-wrap:wrap; }
    .ring-item  { display:flex; flex-direction:column; align-items:center; gap:.4rem; }
    .ring-svg   { width:75px; height:75px; transform:rotate(-90deg); }
    .ring-meta  { display:flex; flex-direction:column; align-items:center; }
    .ring-pct   { font-size:1rem; font-weight:700; color:#0f172a; }
    .ring-label { font-size:.68rem; color:#64748b; text-align:center; }
    .ring-count { font-size:.68rem; color:#94a3b8; }

    /* Table */
    .table-card { background:#fff; border-radius:14px; border:1px solid #e2e8f0; overflow:hidden; }
    .table-head-row {
      display:flex; align-items:center; justify-content:space-between;
      padding:1rem 1.25rem; border-bottom:1px solid #f1f5f9;
    }
    .view-all   { font-size:.82rem; color:#3b82f6; text-decoration:none; font-weight:500; }
    .view-all:hover { text-decoration:underline; }
    .table-scroll { overflow-x:auto; }
    .dash-table { width:100%; border-collapse:collapse; font-size:.84rem; }
    .dash-table th {
      text-align:left; padding:.65rem 1rem;
      font-size:.7rem; text-transform:uppercase;
      letter-spacing:.05em; color:#94a3b8;
      background:#f8fafc; border-bottom:1px solid #e2e8f0;
    }
    .dash-table td {
      padding:.72rem 1rem; border-bottom:1px solid #f8fafc;
      color:#334155; vertical-align:middle;
    }
    .dash-table tr:last-child td { border-bottom:none; }
    .dash-table tr:hover td { background:#f8fafc; }
    .muted     { color:#94a3b8; font-size:.78rem; }
    .empty-row { text-align:center; color:#94a3b8; padding:2rem !important; }

    /* Badges */
    .type-badge {
      padding:.2rem .55rem; border-radius:6px;
      font-size:.7rem; font-weight:600;
      background:#f1f5f9; color:#475569;
    }
    .sbadge { padding:.25rem .6rem; border-radius:99px; font-size:.72rem; font-weight:600; }
    .sbadge-pending   { background:#fef3c7; color:#92400e; }
    .sbadge-quoted    { background:#dbeafe; color:#1e40af; }
    .sbadge-confirmed { background:#ede9fe; color:#5b21b6; }
    .sbadge-completed { background:#dcfce7; color:#166534; }
    .sbadge-cancelled { background:#fee2e2; color:#991b1b; }
    .sbadge-disputed  { background:#f1f5f9; color:#475569; }

    .mb-4 { margin-bottom:1.25rem; }
    .mb-0 { margin-bottom:0; }
  `]
})
export class StaffDashboardComponent implements OnInit {
  private bookingSvc = inject(StaffBookingService);
  private usersSvc   = inject(StaffUsersService);
  private ticketsSvc = inject(SupportTicketsService);
  private disputeSvc = inject(DisputeService);
  private reviewsSvc = inject(ReviewsService);
  private knowledgeSvc = inject(KnowledgeService);
  private confirm    = inject(ConfirmService);
  private toastr     = inject(ToastrService);

  loading = signal(true);
  error   = signal(false);
  reindexing = signal(false);

  // Raw signals
  private _bookings  = signal<StaffBooking[]>([]);
  private _totalBk   = signal(0);
  private _users     = signal(0);
  private _tickets   = signal<any[]>([]);
  private _disputes  = signal<any[]>([]);
  private _reviews   = signal<any[]>([]);
  private _flagged   = signal<any[]>([]);
  private _approved  = signal<any[]>([]);

  // ── Computed stats ──────────────────────────────────────────
  totalBookings    = computed(() => this._totalBk());
  completedBookings= computed(() => this._bookings().filter(b => b.status === 'Completed').length);
  disputedBookings = computed(() => this._bookings().filter(b => b.status === 'Disputed').length);
  totalUsers       = computed(() => this._users());
  totalTickets     = computed(() => this._tickets().length);
  openDisputes     = computed(() => this._disputes().length);
  totalReviews     = computed(() => this._reviews().length);
  flaggedReviews   = computed(() => this._flagged().length);

  recentBookings = computed(() =>
    [...this._bookings()]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 8)
  );

  bookingBars = computed(() => {
    const all   = this._bookings();
    const total = all.length || 1;
    const map: Record<StaffBookingStatus, string> = {
      Pending:   '#f59e0b',
      Quoted:    '#3b82f6',
      Confirmed: '#8b5cf6',
      Completed: '#10b981',
      Cancelled: '#ef4444',
      Disputed:  '#94a3b8',
    };
    return (['Pending','Quoted','Confirmed','Completed','Cancelled','Disputed'] as StaffBookingStatus[])
      .map(s => ({
        status: s,
        count:  all.filter(b => b.status === s).length,
        pct:    Math.round((all.filter(b => b.status === s).length / total) * 100),
        color:  map[s],
      }));
  });

  donutGradient = computed(() => {
    const t = this.totalTickets();
    const d = this.openDisputes();
    const f = this.flaggedReviews();
    const total = t + d + f || 1;
    const td = Math.round((t / total) * 360);
    const dd = Math.round((d / total) * 360);
    return `background: conic-gradient(
      #f59e0b 0deg ${td}deg,
      #ef4444 ${td}deg ${td + dd}deg,
      #f43f5e ${td + dd}deg 360deg
    )`;
  });

  reviewRings = computed(() => {
    const total    = this._reviews().length || 1;
    const approved = this._approved().length;
    const flagged  = this._flagged().length;
    const pending  = total - approved - flagged;
    return [
      { label: 'Approved', count: approved, pct: Math.round((approved / total) * 100), color: '#10b981' },
      { label: 'Flagged',  count: flagged,  pct: Math.round((flagged  / total) * 100), color: '#f43f5e' },
      { label: 'Pending',  count: Math.max(0, pending), pct: Math.round((Math.max(0,pending) / total) * 100), color: '#f59e0b' },
    ];
  });

  // ── Lifecycle ────────────────────────────────────────────────
  ngOnInit() { this.loadAll(); }

  loadAll() {
    this.loading.set(true);
    this.error.set(false);

    // Per-stream catchError so one failing widget doesn't blank out the whole dashboard
    // (forkJoin's default behaviour is to abort all the other inner streams on any error).
    forkJoin({
      bookings: this.bookingSvc.getBookings({ pageSize: 100 }).pipe(catchError(() => of(null))),
      users:    this.usersSvc.getUsers().pipe(catchError(() => of([] as any[]))),
      tickets:  this.ticketsSvc.getTickets({ pageSize: 100 }).pipe(catchError(() => of([] as any))),
      disputes: this.disputeSvc.getByStatus('Open').pipe(catchError(() => of([] as any[]))),
      reviews:  this.reviewsSvc.getAll().pipe(catchError(() => of([] as any[]))),
      flagged:  this.reviewsSvc.getFlagged().pipe(catchError(() => of([] as any[]))),
      approved: this.reviewsSvc.getByStatus('Approved' as any).pipe(catchError(() => of([] as any[]))),
    }).subscribe({
      next: ({ bookings, users, tickets, disputes, reviews, flagged, approved }) => {
        // Bookings — PagedResult (null if the call failed; treat as empty)
        const pr = bookings as PagedResult<StaffBooking> | null;
        this._bookings.set(pr?.items ?? []);
        this._totalBk.set(pr?.totalCount ?? pr?.items?.length ?? 0);

        // Users — array
        this._users.set(Array.isArray(users) ? users.length : 0);

        // Tickets — may be paged or array
        const tickArr = (tickets as any)?.items ?? (Array.isArray(tickets) ? tickets : []);
        this._tickets.set(tickArr);

        // Disputes — array
        this._disputes.set(Array.isArray(disputes) ? disputes : []);

        // Reviews
        this._reviews.set(Array.isArray(reviews) ? reviews : []);
        this._flagged.set(Array.isArray(flagged)  ? flagged  : []);
        this._approved.set(Array.isArray(approved) ? approved : []);

        this.loading.set(false);
      },
      error: () => {
        this.error.set(true);
        this.loading.set(false);
      },
    });
  }

  // Rebuild the RAG vector index from the catalog (deliberate, off-boot maintenance action).
  async reindexKnowledge() {
    if (this.reindexing()) return;

    const ok = await this.confirm.confirm({
      titleKey: 'reindex.confirmTitle',
      messageKey: 'reindex.confirmBody',
      confirmKey: 'reindex.confirmBtn',
      variant: 'primary',
    });
    if (!ok) return;

    this.reindexing.set(true);
    this.knowledgeSvc.reindex().subscribe({
      next: (res) => {
        this.reindexing.set(false);
        this.toastr.success(res?.message || 'Knowledge base reindexed.');
      },
      error: () => {
        this.reindexing.set(false);
        this.toastr.error('Reindex failed. Please try again.');
      },
    });
  }
}