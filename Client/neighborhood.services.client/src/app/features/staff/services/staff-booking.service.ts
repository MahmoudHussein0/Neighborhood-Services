import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { PagedResult } from '../../../core/models/paged-result.model';

export type StaffBookingStatus = 'Pending' | 'Quoted' | 'Confirmed' | 'Completed' | 'Cancelled' | 'Disputed';

// Mirrors StaffBookingDto (GET /api/bookings/staff)
export interface StaffBooking {
  id: number;
  bookingType: 'Direct' | 'Bidding' | 'Recurring';
  status: StaffBookingStatus;
  customerName: string;
  technicianName: string;
  estimatedPrice: number;
  finalPrice: number;
  scheduledAt: string;
  createdAt: string;
  address: string;
}

export interface GetStaffBookingsParams {
  status?: StaffBookingStatus;
  search?: string;
  page?: number;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class StaffBookingService {
  private readonly api = inject(ApiService);

  /** GET /api/bookings/staff — all bookings, paged + filter/search (staff only). */
  getBookings(params: GetStaffBookingsParams = {}): Observable<PagedResult<StaffBooking>> {
    const q = new URLSearchParams();
    if (params.status) q.set('status', params.status);
    if (params.search?.trim()) q.set('search', params.search.trim());
    q.set('page', String(params.page ?? 1));
    q.set('pageSize', String(params.pageSize ?? 10));
    return this.api.get<PagedResult<StaffBooking>>(`/bookings/staff?${q.toString()}`);
  }

  /** POST /api/bookings/{id}/staff-cancel — admin cancel (no refund/reassign). */
  cancel(id: number, reason: string): Observable<void> {
    return this.api.post<void>(`/bookings/${id}/staff-cancel`, { cancellationReason: reason });
  }
}
