export interface StaffUserSummary {
  id: string;
  fullName: string;
  email: string;
  photo: string;
  isActive: boolean;
  applicationUserRole: string;
}

export interface StaffUserDetails extends StaffUserSummary {
  age: number;
}
