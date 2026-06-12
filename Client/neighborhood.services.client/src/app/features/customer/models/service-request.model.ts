export type ServiceRequestStatus = 'Open' | 'Closed' | 'Expired' | 'PendingReview' | 'Flagged';
export type OfferStatus = 'Pending' | 'Accepted' | 'Rejected' | 'Expired' | 'Withdrawn';

// Mirrors ServiceRequestSummaryDto (GET /api/servicerequests/mine)
export interface ServiceRequestSummary {
  id: number;
  description: string;
  address: string;
  budget: number;
  status: ServiceRequestStatus;
  scheduledAt: string;
  customerId: number;
  problemTypeId: number;
  createdAt: string;
  expiresAt: string;
  offerCount: number;
  latitude: number;
  longitude: number;
  categoryId: number;
}

// Mirrors ServiceRequestDetailsDto (GET /api/servicerequests/{id})
export interface ServiceRequestDetails {
  id: number;
  description: string;
  address: string;
  image?: string | null;
  budget: number;
  status: ServiceRequestStatus;
  scheduledAt: string;
  categoryId: number;
  problemTypeId: number;
  customerId: number;
  latitude: number;
  longitude: number;
  createdAt: string;
  expiresAt: string;
  offerCount: number;
}

// Mirrors OfferSummaryDto (offers received on a request)
export interface OfferSummary {
  id: number;
  price: number;
  estimatedDuration: number;
  message: string;
  status: OfferStatus;
  technicianId: number;
  technicianName: string;
  technicianRating: number;
  createdAt: string;
}

// Mirrors ServiceRequestWithOffersDto (GET /api/servicerequests/{id}/with-offers)
export interface ServiceRequestWithOffers extends ServiceRequestDetails {
  offers: OfferSummary[];
}

// Body for POST /api/servicerequests
export interface CreateServiceRequest {
  categoryId: number;
  problemTypeId: number;
  description: string;
  address: string;
  budget: number;
  image?: string | null;
  scheduledAt: string;
  latitude: number;
  longitude: number;
}
