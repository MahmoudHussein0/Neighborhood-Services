import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { PagedResult } from '../../../core/models/paged-result.model';
import { RecurringBooking, RecurringBookingStatus, CreateRecurringBooking, UpdateRecurringBooking } from '../models/recurring-booking.model';

export interface GetMyRecurringParams {
  status?: RecurringBookingStatus;
  search?: string;
  page?: number;
  pageSize?: number;
}

@Injectable({
  providedIn: 'root',
})
export class RecurringBookingService {
  private readonly api = inject(ApiService);

  /** GET /api/recurringbookings/mine — paged + optional filter/search */
  getMine(params: GetMyRecurringParams = {}): Observable<PagedResult<RecurringBooking>> {
    const query = new URLSearchParams();
    if (params.status) query.set('status', params.status);
    if (params.search?.trim()) query.set('search', params.search.trim());
    query.set('page', String(params.page ?? 1));
    query.set('pageSize', String(params.pageSize ?? 10));

    return this.api.get<PagedResult<RecurringBooking>>(`/recurringbookings/mine?${query.toString()}`);
  }

  /** POST /api/recurringbookings — customer creates a recurring arrangement */
  create(body: CreateRecurringBooking): Observable<{ id: number }> {
    return this.api.post<{ id: number }>('/recurringbookings', body);
  }

  /** GET /api/recurringbookings/{id} — single arrangement */
  getById(id: number): Observable<RecurringBooking> {
    return this.api.get<RecurringBooking>(`/recurringbookings/${id}`);
  }

  /** PUT /api/recurringbookings/{id} — edit schedule/address (resets to Awaiting Price) */
  update(id: number, body: UpdateRecurringBooking): Observable<void> {
    return this.api.put<void>(`/recurringbookings/${id}`, body);
  }

  /** POST /api/recurringbookings/{id}/approve — customer approves the proposed price (→ Active) */
  approve(id: number): Observable<void> {
    return this.api.post<void>(`/recurringbookings/${id}/approve`, {});
  }

  /** POST /api/recurringbookings/{id}/reject-price — customer rejects price (→ PendingApproval) */
  rejectPrice(id: number): Observable<void> {
    return this.api.post<void>(`/recurringbookings/${id}/reject-price`, {});
  }

  /** POST /api/recurringbookings/{id}/pause */
  pause(id: number): Observable<void> {
    return this.api.post<void>(`/recurringbookings/${id}/pause`, {});
  }

  /** POST /api/recurringbookings/{id}/resume */
  resume(id: number): Observable<void> {
    return this.api.post<void>(`/recurringbookings/${id}/resume`, {});
  }

  /** POST /api/recurringbookings/{id}/cancel */
  cancel(id: number, cancellationReason?: string): Observable<boolean> {
    return this.api.post<boolean>(`/recurringbookings/${id}/cancel`, {
      recurringBookingId: id,
      cancellationReason: cancellationReason ?? null,
    });
  }
}
