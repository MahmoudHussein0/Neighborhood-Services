import { Component, Input, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { environment } from '../../../../environments/environment';

// ====================================================================
// "My Approved Reviews" component - Technician screen
// ====================================================================
//
// Scenario: a customer leaves a review for the technician after a
// booking is completed. Once an admin approves that review, it shows
// up here for the technician.
//
// --------------------------------------------------------------------
// Configuration points (check / adjust based on your backend):
// --------------------------------------------------------------------
//
// 1) environment.apiUrl
//    The base API domain, coming from your environment file.
//
// 2) ENDPOINTS.reviewsByReviewee(id)
//    Uses: GET /api/Reviews/reviewee/{revieweeId}
//
// 3) APPROVED_VALUES
//    Filtering happens on the frontend against `status`.
//    Currently supports the string "Approved" or a numeric enum value.
//    Add your enum's actual value if it's different (e.g. Approved = 1).
//
// 4) normalizeReview()
//    Tries to read the reviewer name and booking description from the
//    review object itself (common case, e.g. embedded navigation
//    properties). If they are missing, it fetches:
//      GET /api/Users/{reviewerId}    -> customer name
//      GET /api/Bookings/{bookingId}  -> booking description
//    Adjust ENDPOINTS.userById / ENDPOINTS.bookingById if your backend
//    uses different routes or field names.
//
// 5) Technician id (revieweeId)
//    If no [revieweeId] input is passed, the component tries to read
//    the id from the JWT stored after login (see getCurrentUserId()
//    and decodeJwt() below). Adjust TOKEN_STORAGE_KEY and the claim
//    names to match your backend/auth flow.
// ====================================================================

interface RawReview {
  id: string | number;
  rating: number;
  comment: string;
  status: string | number;
  createdAt?: string;
  createdDate?: string;
  date?: string;
  reviewerId?: string | number;
  bookingId?: string | number;
  reviewerName?: string;
  bookingDescription?: string;
  reviewer?: { fullName?: string; name?: string };
  booking?: { description?: string };
}

export interface NormalizedReview {
  id: string | number;
  rating: number;
  comment: string;
  createdAt: string;
  reviewerId?: string | number;
  bookingId?: string | number;
  reviewerName: string | null;
  bookingDescription: string | null;
  reviewerLoading: boolean;
  bookingLoading: boolean;
}

export type ViewStatus = 'loading' | 'success' | 'error' | 'empty' | 'no-user';

interface GaugeTick {
  v: number;
  x1: number;
  y1: number;
  x2: number;
  y2: number;
}

const API_BASE = environment.apiUrl;

const ENDPOINTS = {
  reviewsByReviewee: (id: string | number) => `${API_BASE}/api/Reviews/reviewee/${id}`,
  userById: (id: string | number) => `${API_BASE}/api/Users/${id}`,
  bookingById: (id: string | number) => `${API_BASE}/api/Bookings/${id}`,
};

// Values considered "approved" - adjust based on your status enum.
// Comparison below is case-insensitive for strings, and also checks
// common numeric enum values (adjust if your enum order is different).
const APPROVED_VALUES: Array<string | number> = ['approved', 1, '1', 2, '2'];

function isApproved(status: string | number): boolean {
  if (typeof status === 'string') {
    return APPROVED_VALUES.includes(status.toLowerCase());
  }
  return APPROVED_VALUES.includes(status);
}

// ----------------------------------------------------------------
// Read the technician's id from the auth user object stored after login
// ----------------------------------------------------------------

// ⚠️ this matches the key seen in localStorage: ns_auth_user
const AUTH_USER_STORAGE_KEY = 'ns_auth_user';

function getCurrentUserId(): string | number | null {
  const raw =
    localStorage.getItem(AUTH_USER_STORAGE_KEY) ?? sessionStorage.getItem(AUTH_USER_STORAGE_KEY);
  if (!raw) return null;

  try {
    const user = JSON.parse(raw);
    // ⚠️ adjust this if the reviewee id should come from a different field
    return user.userId ?? user.id ?? null;
  } catch {
    return null;
  }
}

// ----------------------------------------------------------------
// Gauge geometry helpers (half-circle rating gauge, 0 -> 5)
// ----------------------------------------------------------------
const GAUGE_MAX = 5;
const GAUGE_CX = 100;
const GAUGE_CY = 100;
const GAUGE_R = 78;

function angleFor(value: number): number {
  // 0 -> 180deg (left side of the arc), 5 -> 0deg (right side)
  return 180 - (value / GAUGE_MAX) * 180;
}

function pointAt(angleDeg: number, radius: number): { x: number; y: number } {
  const rad = (angleDeg * Math.PI) / 180;
  return {
    x: GAUGE_CX + radius * Math.cos(rad),
    y: GAUGE_CY - radius * Math.sin(rad),
  };
}

@Component({
  selector: 'app-tech-reviews',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tech-review.html',
  styleUrls: ['./tech-review.css'],
})
export class TechReviewsComponent implements OnInit, OnDestroy {
  // The technician's id (the "reviewee"). If not passed in, it's read
  // from the JWT token (see getCurrentUserId above).
  @Input() revieweeId: string | number | null = null;

  reviews = signal<NormalizedReview[]>([]);
  status = signal<ViewStatus>('loading');

  // Static gauge geometry (doesn't depend on the rating value)
  readonly gaugeArcStart = pointAt(180, GAUGE_R);
  readonly gaugeArcEnd = pointAt(0, GAUGE_R);
  readonly gaugeTicks: GaugeTick[] = [0, 1, 2, 3, 4, 5].map((v) => {
    const a = angleFor(v);
    const outer = pointAt(a, GAUGE_R);
    const inner = pointAt(a, GAUGE_R - 10);
    return { v, x1: inner.x, y1: inner.y, x2: outer.x, y2: outer.y };
  });

  private subscriptions: Subscription[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    if (this.revieweeId === null || this.revieweeId === undefined) {
      this.revieweeId = getCurrentUserId();
    }
    this.load();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((s) => s.unsubscribe());
  }

  // ----------------------------------------------------------------
  // Data loading
  // ----------------------------------------------------------------
  private load(): void {
    this.status.set('loading');

    if (this.revieweeId === null || this.revieweeId === undefined) {
      // No id from @Input and none found in the stored auth user either
      this.reviews.set([]);
      this.status.set('no-user');
      return;
    }

    const sub = this.http
      .get<RawReview[]>(ENDPOINTS.reviewsByReviewee(this.revieweeId))
      .subscribe({
        next: (data) => this.handleReviews(data ?? []),
        error: () => {
          this.reviews.set([]);
          this.status.set('error');
        },
      });

    this.subscriptions.push(sub);
  }

  private handleReviews(data: RawReview[]): void {
    // 🔍 temporary debug - check the real field names/status values coming
    // from the API, then remove this line once everything is confirmed.
    console.log('Raw reviews from API:', JSON.stringify(data, null, 2));

    // Frontend-side filter: only keep reviews approved by the admin
    const approved = data.filter((r) => isApproved(r.status));

    if (approved.length === 0) {
      this.reviews.set([]);
      this.status.set('empty');
      return;
    }

    const normalized = approved.map((r) => this.normalizeReview(r));
    this.reviews.set(normalized);
    this.status.set('success');

    // Fetch any missing reviewer names / booking descriptions in the background
    normalized.forEach((item, index) => {
      if (item.reviewerLoading && item.reviewerId !== undefined) {
        this.fetchReviewerName(item.reviewerId, index);
      }
      if (item.bookingLoading && item.bookingId !== undefined) {
        this.fetchBookingDescription(item.bookingId, index);
      }
    });
  }

  private normalizeReview(r: RawReview): NormalizedReview {
    const reviewerName = r.reviewerName ?? r.reviewer?.fullName ?? r.reviewer?.name ?? null;
    const bookingDescription = r.bookingDescription ?? r.booking?.description ?? null;

    return {
      id: r.id,
      rating: r.rating,
      comment: r.comment,
      createdAt: r.createdAt ?? r.createdDate ?? r.date ?? '',
      reviewerId: r.reviewerId,
      bookingId: r.bookingId,
      reviewerName,
      bookingDescription,
      reviewerLoading: !reviewerName && r.reviewerId !== undefined,
      bookingLoading: !bookingDescription && r.bookingId !== undefined,
    };
  }

  private fetchReviewerName(reviewerId: string | number, index: number): void {
    const sub = this.http.get<any>(ENDPOINTS.userById(reviewerId)).subscribe({
      next: (user) => {
        const name = user?.fullName ?? user?.name ?? user?.userName ?? 'عميل';
        this.updateReview(index, { reviewerName: name, reviewerLoading: false });
      },
      error: () => this.updateReview(index, { reviewerName: 'عميل', reviewerLoading: false }),
    });
    this.subscriptions.push(sub);
  }

  private fetchBookingDescription(bookingId: string | number, index: number): void {
    const sub = this.http.get<any>(ENDPOINTS.bookingById(bookingId)).subscribe({
      next: (booking) => {
        const desc = booking?.description ?? booking?.serviceDescription ?? 'بدون تفاصيل';
        this.updateReview(index, { bookingDescription: desc, bookingLoading: false });
      },
      error: () =>
        this.updateReview(index, { bookingDescription: 'بدون تفاصيل', bookingLoading: false }),
    });
    this.subscriptions.push(sub);
  }

  private updateReview(index: number, patch: Partial<NormalizedReview>): void {
    this.reviews.update((current) => {
      if (!current[index]) return current;
      return current.map((rv, i) => (i === index ? { ...rv, ...patch } : rv));
    });
  }

  // ----------------------------------------------------------------
  // Derived values used by the template
  // ----------------------------------------------------------------
  averageRating = computed(() => {
    const reviews = this.reviews();
    if (reviews.length === 0) return 0;
    const sum = reviews.reduce((acc, r) => acc + (Number(r.rating) || 0), 0);
    return sum / reviews.length;
  });

  averageRatingLabel = computed(() => this.averageRating().toFixed(1));

  gaugeProgressEnd = computed(() => {
    const value = Math.min(Math.max(this.averageRating(), 0), GAUGE_MAX);
    return pointAt(angleFor(value), GAUGE_R);
  });

  gaugeNeedleEnd = computed(() => {
    const value = Math.min(Math.max(this.averageRating(), 0), GAUGE_MAX);
    return pointAt(angleFor(value), GAUGE_R - 14);
  });

  gaugeProgressPath = computed(() => {
    const end = this.gaugeProgressEnd();
    return `M ${this.gaugeArcStart.x} ${this.gaugeArcStart.y} A ${GAUGE_R} ${GAUGE_R} 0 0 1 ${end.x} ${end.y}`;
  });

  gaugeBackgroundPath = `M ${this.gaugeArcStart.x} ${this.gaugeArcStart.y} A ${GAUGE_R} ${GAUGE_R} 0 0 1 ${this.gaugeArcEnd.x} ${this.gaugeArcEnd.y}`;

  // ----------------------------------------------------------------
  // Template helpers
  // ----------------------------------------------------------------
  stars(rating: number): boolean[] {
    const rounded = Math.round(Number(rating) || 0);
    return [1, 2, 3, 4, 5].map((i) => i <= rounded);
  }

  initial(name: string | null): string {
    return (name || '?').trim().charAt(0);
  }

  formatDate(dateString: string): string {
    if (!dateString) return '';
    try {
      return new Date(dateString).toLocaleDateString('ar-EG', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
      });
    } catch {
      return dateString;
    }
  }

  trackById(_index: number, item: NormalizedReview): string | number {
    return item.id;
  }
}