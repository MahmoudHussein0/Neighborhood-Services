// Mirrors the backend enums (serialized as strings via JsonStringEnumConverter)
export type BookingType = 'Direct' | 'Bidding' | 'Recurring';
export type BookingStatus = 'Pending' | 'Confirmed' | 'Completed' | 'Cancelled' | 'Disputed';

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
}

// Mirrors BookingSummaryDto (GET /api/bookings/mine)
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
}
