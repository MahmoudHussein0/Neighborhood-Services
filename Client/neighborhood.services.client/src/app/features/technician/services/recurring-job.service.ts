import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { PagedResult } from '../../../core/models/paged-result.model';
import { RecurringBooking, RecurringBookingStatus } from '../../customer/models/recurring-booking.model';

export interface GetMyRecurringJobsParams {
  status?: RecurringBookingStatus;
  search?: string;
  page?: number;
  pageSize?: number;
}

@Injectable({
  providedIn: 'root',
})
export class RecurringJobService {
  private readonly api = inject(ApiService);

  /** GET /api/recurringbookings/mine — the technician's assigned recurring arrangements. */
  getMyJobs(params: GetMyRecurringJobsParams = {}): Observable<PagedResult<RecurringBooking>> {
    const query = new URLSearchParams();
    if (params.status) query.set('status', params.status);
    if (params.search?.trim()) query.set('search', params.search.trim());
    query.set('page', String(params.page ?? 1));
    query.set('pageSize', String(params.pageSize ?? 10));

    return this.api.get<PagedResult<RecurringBooking>>(`/recurringbookings/mine?${query.toString()}`);
  }

  /** POST /api/recurringbookings/{id}/set-price — technician quotes a price (→ awaiting customer). */
  setPrice(id: number, price: number): Observable<boolean> {
    return this.api.post<boolean>(`/recurringbookings/${id}/set-price`, { recurringBookingId: id, price });
  }

  /** POST /api/recurringbookings/{id}/cancel */
  cancel(id: number, cancellationReason?: string): Observable<boolean> {
    return this.api.post<boolean>(`/recurringbookings/${id}/cancel`, {
      recurringBookingId: id,
      cancellationReason: cancellationReason ?? null,
    });
  }
}
