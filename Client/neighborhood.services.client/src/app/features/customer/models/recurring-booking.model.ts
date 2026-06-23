export type RecurringPattern = 'Daily' | 'Weekly' | 'Monthly';

export type RecurringBookingStatus =
  | 'PendingApproval'          // waiting for technician to set price
  | 'PendingCustomerApproval'  // technician set a price, waiting on the customer
  | 'Active'                   // both agreed, generating bookings
  | 'Paused'
  | 'Cancelled';

// Body for POST /api/recurringbookings
export interface CreateRecurringBooking {
  technicianId: number;
  problemTypeId: number;
  // What the job is + an optional reference photo — copied onto each generated booking.
  description: string;
  imageUrl?: string | null;
  address: string;
  latitude: number;
  longitude: number;
  pattern: RecurringPattern;
  dayOfWeek?: string | null;   // required when pattern = Weekly (e.g. "Monday")
  dayOfMonth?: number | null;  // required when pattern = Monthly (1–31)
  timeOfDay: string;           // "HH:mm:ss"
  durationMinutes: number;
  startDate: string;           // "yyyy-MM-dd"
  endDate?: string | null;
}

// Body for PUT /api/recurringbookings/{id} — schedule + address only
// (technician, problem type and location are NOT editable). Editing resets to PendingApproval.
export interface UpdateRecurringBooking {
  description: string;
  imageUrl?: string | null;
  address: string;
  pattern: RecurringPattern;
  dayOfWeek?: string | null;   // required when pattern = Weekly
  dayOfMonth?: number | null;  // required when pattern = Monthly
  timeOfDay: string;           // "HH:mm:ss"
  durationMinutes: number;
  startDate: string;           // "yyyy-MM-dd"
  endDate?: string | null;
}

// Mirrors RecurringBookingDto (GET /api/recurringbookings/mine)
export interface RecurringBooking {
  id: number;
  description: string;
  imageUrl?: string | null;
  address: string;
  pattern: RecurringPattern;
  dayOfWeek?: string | null;   // e.g. "Monday" (System.DayOfWeek serialized as string)
  dayOfMonth?: number | null;
  timeOfDay: string;           // "HH:mm:ss"
  durationMinutes: number;
  startDate: string;           // "yyyy-MM-dd"
  endDate?: string | null;
  status: RecurringBookingStatus;
  agreedPrice?: number | null;
  customerId: number;
  technicianId: number;
  problemTypeId: number;
  createdAt: string;
}
