export type ReviewStatus = 'Pending' | 'Approved' | 'Rejected' | 'Flagged';

export interface ReviewDto {
  id: number;
  bookingId: number;
  reviewerId: string;
  revieweeId: string;
  reviewerName?: string;
  revieweeName?: string;
  rating: number;
  comment: string;
  status: ReviewStatus;
  createdAt: string;
}

export interface CreateReviewRequest {
  bookingId: number;
  reviewerId: string;
  revieweeId: string;
  rating: number;
  comment: string;
}

export interface UpdateReviewRequest {
  rating?: number;
  comment?: string;
  status?: ReviewStatus;
}

// For display in the UI — enriched from API data
export interface ReviewDisplay extends ReviewDto {
  reviewerName?: string;
  revieweeName?: string;
  sourceType?: 'customer' | 'technician';
}

export interface ReviewFilters {
  search: string;
  status: ReviewStatus | '';
  rating: number | '';
  revieweeId: string;
}

export interface AnalyticsSummary {
  averageRating: number;
  totalReviews: number;
  positivePercent: number;
  flaggedCount: number;
}

export interface StaffRatingSummary {
  staffId: string;
  staffName: string;
  averageRating: number;
  totalReviews: number;
}