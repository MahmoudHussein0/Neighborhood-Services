export type OfferStatus = 'Pending' | 'Accepted' | 'Rejected' | 'Expired' | 'Withdrawn';

// Mirrors OfferDto (GET /api/offers/mine)
export interface Offer {
  id: number;
  serviceRequestId: number;
  technicianId: number;
  // The customer who posted the service request — lets the Offers page link to their public profile.
  customerId: number;
  customerName: string;
  // A brief of the request the offer is on, so the tech can see what they bid on.
  serviceRequestDescription: string;
  serviceRequestAddress: string;
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
