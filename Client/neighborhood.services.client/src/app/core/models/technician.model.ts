// Mirrors TechnicianSummaryDTO (GET /api/technicians) — owned by another teammate, consume only.
// Note: no display name yet (it lives on the User entity, not exposed in this DTO).
export interface TechnicianSummary {
  id: number;
  applicationUserId: string;
  nationalId: string;
  rating: number;
  maxTravelDistance: number;
  verificationStatus: string;
  isAvailable: boolean;
  isActive: boolean;
}
