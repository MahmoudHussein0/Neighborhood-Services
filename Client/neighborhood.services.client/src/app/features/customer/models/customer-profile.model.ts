export type CustomerAddressLabel = 'Home' | 'Work' | 'Other';

export interface CustomerProfile {
  id: string;
  fullName: string;
  email: string;
  age: number;
  photo: string;
  applicationUserRole: string;
  isActive: boolean;
}

export interface CustomerAddress {
  id: number;
  label: CustomerAddressLabel;
  address: string;
  latitude: number;
  longitude: number;
  isDefault: boolean;
  isDeleted: boolean;
  createdAt: string;
  applicationUserId: string;
  customerId: number;
}

export interface CustomerAddressRequest {
  label: CustomerAddressLabel;
  address: string;
  latitude: number;
  longitude: number;
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

export interface CustomerRecord {
  id: number;
  applicationUserId: string;
  isDeleted: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}
