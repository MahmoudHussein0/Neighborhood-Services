// Mirrors TechnicianCardDTO (GET /api/technicians/browse).
// Customer-facing card data: joins the technician with its user (name/photo/location)
// and its categories. Owned API-side by us (additive browse endpoint).

export interface TechnicianCardCategory {
  id: number;
  nameEn: string;
  nameAr: string;
  icon: string;
}

export interface TechnicianCard {
  id: number;
  applicationUserId: string;
  fullName: string;
  photo: string;
  rating: number;
  maxTravelDistance: number;
  verificationStatus: 'Pending' | 'Approved' | 'Rejected';
  isAvailable: boolean;
  latitude: number | null;
  longitude: number | null;
  categories: TechnicianCardCategory[];
}
