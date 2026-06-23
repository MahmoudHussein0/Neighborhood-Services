import { TechnicianCard } from '../../../core/models/technician-card.model';

// Mirrors the backend enums (serialized as strings via JsonStringEnumConverter)
export type BookingType = 'Direct' | 'Bidding' | 'Recurring';
// "Quoted" = Direct flow only: tech has set FinalPrice + DurationMinutes, customer must accept / reject.
export type BookingStatus = 'Pending' | 'Quoted' | 'Confirmed' | 'Completed' | 'Cancelled' | 'Disputed';

// Mirrors DisputeType (serialized by name). Reason a customer/technician can dispute a booking.
export type DisputeType = 'PaymentIssue' | 'TechnicianBehavior' | 'PoorService' | 'Scam' | 'Other';

// Mirrors SeverityLevel (serialized by name).
export type SeverityLevel = 'Low' | 'Medium' | 'High';

// Mirrors AiAnalysisDto (POST /api/aianalysis) — optional AI triage of a problem from its photo.
export interface AiAnalysis {
  detectedProblem: string;
  confidenceScore: number;
  severityLevel: SeverityLevel;
  estimatedMinPrice: number;
  estimatedMaxPrice: number;
  generatedAt: string;
}

// Body for POST /api/bookings (Direct booking). Customer is resolved from the token server-side.
export interface CreateBooking {
  technicianId: number;
  problemTypeId: number;
  description: string;
  address: string;
  latitude: number;
  longitude: number;
  region?: string | null;
  scheduledAt: string;   // "yyyy-MM-ddTHH:mm:ss" (must be in the future)
  promoCodeId?: number | null;
  beforeImageUrl?: string | null;  // optional photo of the problem, shown to the tech before quoting
}

// ---- Smart Match (POST /api/bookings/match) ----

// What the customer picks in the "Smart Match" modal.
export interface SmartMatchCriteria {
  categoryId: number;
  problemTypeId?: number | null;
  latitude?: number | null;
  longitude?: number | null;
  description?: string | null;   // optional free text — makes the match smarter
  topN?: number;
}

// One ranked suggestion. `technician` is a full browse card → drops into Book Now / Recurring.
export interface TechnicianMatch {
  rank: number;
  score: number;
  reason: string;
  technician: TechnicianCard;
}

export interface TechnicianMatchResult {
  rankedByAi: boolean;            // true = LLM ranked; false = rule-based fallback
  matches: TechnicianMatch[];
}

// Mirrors BookingSummaryDto (used by GET /api/bookings/recurring/{id} and admin lists).
export interface BookingSummary {
  id: number;
  bookingType: BookingType;
  description: string;
  address: string;
  scheduledAt: string;    // ISO date string from the API
  estimatedPrice: number;
  status: BookingStatus;
  createdAt: string;      // ISO date string
}

// Mirrors MyBookingSummaryDto — only returned by GET /api/bookings/mine.
// Includes the quoted FinalPrice + DurationMinutes (set by the technician on the
// Pending -> Quoted transition) and the (TechnicianId, ProblemTypeId) so the UI
// can fetch the tech's pricing range for that problem type.
export interface MyBookingSummary {
  id: number;
  bookingType: BookingType;
  description: string;
  address: string;
  scheduledAt: string;
  estimatedPrice: number;
  finalPrice: number;
  durationMinutes?: number | null;
  status: BookingStatus;
  clientConfirmed: boolean;
  createdAt: string;
  // Both parties — each side shows the other (customer page → technician, jobs page → customer).
  technicianId: number;
  technicianName: string;
  customerId: number;
  customerName: string;
  problemTypeId: number;
  latitude: number;
  longitude: number;
  // True if the current user already left a review on this booking — drives the
  // "Leave a review" vs "Reviewed ✓" state on the bookings/jobs pages.
  hasReview: boolean;
}

// Mirrors TechnicianPricingRangeDto (GET /api/bookings/tech-pricing-range)
export interface TechnicianPricingRange {
  technicianId: number;
  problemTypeId: number;
  minPrice: number;
  maxPrice: number;
}

// Mirrors BookingDetailsDto (GET /api/bookings/{id})
export interface BookingDetails {
  id: number;
  bookingType: BookingType;
  description: string;
  address: string;
  scheduledAt: string;
  estimatedPrice: number;
  finalPrice: number;
  status: BookingStatus;
  clientConfirmed: boolean;
  cancellationReason?: string | null;
  createdAt: string;
  customerId: number;
  technicianId: number;
  problemTypeId: number;
  offerId?: number | null;
  serviceRequestId?: number | null;
  latitude: number;
  longitude: number;
}

// Mirrors BookingImageDto (GET /api/bookings/{id}/images)
export type BookingImageType = 'Before' | 'After';

export interface BookingImage {
  id: number;
  bookingId: number;
  imageUrl: string;
  type: BookingImageType;
  uploadedBy: string;
  uploadedAt: string;
}
