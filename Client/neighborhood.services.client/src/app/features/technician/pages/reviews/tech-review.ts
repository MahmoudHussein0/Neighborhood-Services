import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subscription } from 'rxjs';

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
// 1) API_BASE
//    Set the API domain if it's different from the current origin.
//    Leave empty if served from the same domain.
//
// 2) ENDPOINTS.reviewsByReviewee(id)
//    Uses: GET /api/Reviews/reviewee/{revieweeId}
//    This is the endpoint you already have.
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

export type ViewStatus = 'loading' | 'success' | 'error' | 'empty';

interface GaugeTick {
  v: number;
  x1: number;
  y1: number;
  x2: number;
  y2: number;
}

const API_BASE = ''; // e.g. 'https://api.example.com'

const ENDPOINTS = {
  reviewsByReviewee: (id: string | number) => `${API_BASE}/api/Reviews/reviewee/${id}`,
  userById: (id: string | number) => `${API_BASE}/api/Users/${id}`,
  bookingById: (id: string | number) => `${API_BASE}/api/Bookings/${id}`,
};

// Values considered "approved" - adjust based on your status enum
const APPROVED_VALUES: Array<string | number> = ['Approved', 'approved', 2, '2'];

// ----------------------------------------------------------------
// Demo data (used only when no revieweeId is provided, or on error)
// ----------------------------------------------------------------
const DEMO_REVIEWS: NormalizedReview[] = [
  {
    id: 'demo-1',
    rating: 5,
    comment: 'شغل نضيف ومحترم جدًا، وصل في الميعاد وحل المشكلة بسرعة. تعامل راقي.',
    createdAt: '2026-05-28T10:30:00Z',
    reviewerName: 'أحمد فتحي',
    bookingDescription: 'تصليح تسريب مواسير المطبخ',
    reviewerLoading: false,
    bookingLoading: false,
  },
  {
    id: 'demo-2',
    rating: 4,
    comment: 'خدمة كويسة، بس استغرق وقت أطول من المتوقع شوية.',
    createdAt: '2026-05-15T14:00:00Z',
    reviewerName: 'منى عبد الرازق',
    bookingDescription: 'صيانة تكييف سبليت 1.5 حصان',
    reviewerLoading: false,
    bookingLoading: false,
  },
  {
    id: 'demo-3',
    rating: 5,
    comment: 'أكثر من رائع، شرح لي المشكلة بالتفصيل واقترح حلول وقائية كمان.',
    createdAt: '2026-04-30T09:15:00Z',
    reviewerName: 'كريم سامي',
    bookingDescription: 'تركيب سخان مياه كهربائي 50 لتر',
    reviewerLoading: false,
    bookingLoading: false,
  },
];

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
  selector: 'app-approved-reviews',
  templateUrl: './tech-review.html',
  styleUrls: ['./tech-review.css'],
})
export class ApprovedReviewsComponent implements OnInit, OnDestroy {
  // The technician's id (the "reviewee")
  @Input() revieweeId: string | number | null = null;

  reviews: NormalizedReview[] = [];
  status: ViewStatus = 'loading';
  isDemo = false;

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
    this.load();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((s) => s.unsubscribe());
  }

  // ----------------------------------------------------------------
  // Data loading
  // ----------------------------------------------------------------
  private load(): void {
    this.status = 'loading';

    if (this.revieweeId === null || this.revieweeId === undefined) {
      this.reviews = DEMO_REVIEWS;
      this.isDemo = true;
      this.status = 'success';
      return;
    }

    const sub = this.http
      .get<RawReview[]>(ENDPOINTS.reviewsByReviewee(this.revieweeId))
      .subscribe({
        next: (data) => this.handleReviews(data ?? []),
        error: () => {
          // Fall back to demo data so the UI still has something to show
          this.reviews = DEMO_REVIEWS;
          this.isDemo = true;
          this.status = 'error';
        },
      });

    this.subscriptions.push(sub);
  }

  private handleReviews(data: RawReview[]): void {
    // Frontend-side filter: only keep reviews approved by the admin
    const approved = data.filter((r) => APPROVED_VALUES.includes(r.status));

    if (approved.length === 0) {
      this.reviews = [];
      this.isDemo = false;
      this.status = 'empty';
      return;
    }

    this.reviews = approved.map((r) => this.normalizeReview(r));
    this.isDemo = false;
    this.status = 'success';

    // Fetch any missing reviewer names / booking descriptions in the background
    this.reviews.forEach((item, index) => {
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
    if (!this.reviews[index]) return;
    this.reviews = this.reviews.map((rv, i) => (i === index ? { ...rv, ...patch } : rv));
  }

  // ----------------------------------------------------------------
  // Derived values used by the template
  // ----------------------------------------------------------------
  get averageRating(): number {
    if (this.reviews.length === 0) return 0;
    const sum = this.reviews.reduce((acc, r) => acc + (Number(r.rating) || 0), 0);
    return sum / this.reviews.length;
  }

  get averageRatingLabel(): string {
    return this.averageRating.toFixed(1);
  }

  get gaugeProgressEnd(): { x: number; y: number } {
    const value = Math.min(Math.max(this.averageRating, 0), GAUGE_MAX);
    return pointAt(angleFor(value), GAUGE_R);
  }

  get gaugeNeedleEnd(): { x: number; y: number } {
    const value = Math.min(Math.max(this.averageRating, 0), GAUGE_MAX);
    return pointAt(angleFor(value), GAUGE_R - 14);
  }

  get gaugeProgressPath(): string {
    const end = this.gaugeProgressEnd;
    return `M ${this.gaugeArcStart.x} ${this.gaugeArcStart.y} A ${GAUGE_R} ${GAUGE_R} 0 0 1 ${end.x} ${end.y}`;
  }

  get gaugeBackgroundPath(): string {
    return `M ${this.gaugeArcStart.x} ${this.gaugeArcStart.y} A ${GAUGE_R} ${GAUGE_R} 0 0 1 ${this.gaugeArcEnd.x} ${this.gaugeArcEnd.y}`;
  }

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