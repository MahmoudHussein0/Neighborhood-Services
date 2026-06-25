export type ApplicationUserRole = 'Customer' | 'Technician' | 'Staff';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  photo?: string;
  password: string;
  age: number;
  applicationUserRole: ApplicationUserRole;
  latitude: number;
  longitude: number;
  nationalId?: string;
  experience?: string;
  maxTravelDistance?: number;
  categoryIds?: number[];
}

export interface RegisterFormValue {
  fullName: string;
  email: string;
  password: string;
  age: number;
  applicationUserRole: ApplicationUserRole;
  address: string;
  nationalId?: string;
  experience?: string;
  maxTravelDistance?: number;
}

export interface GeocodingResult {
  formattedAddress: string;
  latitude: number;
  longitude: number;
  // Structured place fields from reverse-geocoding (any may be null).
  city?: string | null;
  county?: string | null;
  state?: string | null;
}

export interface AuthResponse {
  userId: string;
  fullName: string;
  email: string;
  photo: string;
  role: string;
  expiresAt: string;
}

export interface RegisterResponse {
  id: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface ChangePasswordResponse {
  succeeded: boolean;
  message: string;
  errors: string[];
}

export type SafeAuthUser = AuthResponse;
