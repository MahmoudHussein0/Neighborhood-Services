export type OfferStatus = 'Pending' | 'Accepted' | 'Rejected' | 'Expired' | 'Withdrawn';

// Mirrors OfferDto (GET /api/offers/mine)
export interface Offer {
  id: number;
  serviceRequestId: number;
  technicianId: number;
  price: number;
  estimatedDuration: number;
  message: string;
  scheduledAt: string;   // ISO date string
  status: OfferStatus;
  createdAt: string;
}

// Body for POST /api/offers
export interface CreateOffer {
  serviceRequestId: number;
  price: number;
  estimatedDuration: number;
  message: string;
  scheduledAt: string;   // ISO date string
}
