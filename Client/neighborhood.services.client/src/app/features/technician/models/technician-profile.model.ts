export interface TechnicianUserProfile {
  id: string;
  fullName: string;
  email: string;
  age: number;
  photo: string;
  applicationUserRole: string;
  isActive: boolean;
}

export interface TechnicianProfile {
  id: number;
  applicationUserId: string;
  nationalId: string;
  experience: string;
  rating: number;
  maxTravelDistance: number;
  verificationStatus: string;
  isAvailable: boolean;
  isDeleted: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface TechnicianPhoto {
  id: number;
  photoUrl: string;
  caption: string;
  createdAt: string;
  applicationUserId: string;
  technicianId: number;
}
