// Mirrors PublicProfileDto (GET /api/technicians/{id}/public-profile and /api/customers/{id}/public-profile)
export interface PublicProfile {
  role: 'Technician' | 'Customer';
  applicationUserId: string;
  fullName: string;
  photo: string;
  latitude?: number | null;
  longitude?: number | null;
  memberSince: string;
  averageRating: number;
  reviewCount: number;
  completedJobs: number;
  // Technician-only
  experience?: string | null;
  verificationStatus?: string | null;
  categories: PublicProfileCategory[];
  reviews: PublicReview[];
}

export interface PublicReview {
  id: number;
  rating: number;
  comment: string;
  createdAt: string;
  reviewerName: string;
  reviewerPhoto: string;
}

export interface PublicProfileCategory {
  id: number;
  nameEn: string;
  nameAr: string;
  icon: string;
}
