import { Injectable, inject } from '@angular/core';
import { Observable, catchError, of } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { ApiService } from '../../../core/services/api.service';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  BookingStatus,
  BookingImageType,
  BookingImage,
  MyBookingSummary,
  TechnicianPricingRange,
  AiAnalysis,
  DisputeType,
} from '../../customer/models/booking.model';

export interface GetMyJobsParams {
  status?: BookingStatus;
  search?: string;
  page?: number;
  pageSize?: number;
}

@Injectable({
  providedIn: 'root',
})
export class JobService {
  private readonly api = inject(ApiService);

  /** GET /api/bookings/mine — the technician's assigned bookings (paged + filter/search). */
  getMyJobs(params: GetMyJobsParams = {}): Observable<PagedResult<MyBookingSummary>> {
    const query = new URLSearchParams();
    if (params.status) query.set('status', params.status);
    if (params.search?.trim()) query.set('search', params.search.trim());
    query.set('page', String(params.page ?? 1));
    query.set('pageSize', String(params.pageSize ?? 10));

    return this.api.get<PagedResult<MyBookingSummary>>(`/api/bookings/mine?${query.toString()}`);
  }

  /** POST /api/bookings/{id}/quote — technician sets FinalPrice + DurationMinutes (Pending -> Quoted). */
  quote(id: number, finalPrice: number, durationMinutes: number): Observable<void> {
    return this.api.post<void>(`/api/bookings/${id}/quote`, { bookingId: id, finalPrice, durationMinutes });
  }

  /** POST /api/bookings/{id}/complete — technician marks the job done. */
  complete(id: number): Observable<void> {
    return this.api.post<void>(`/api/bookings/${id}/complete`, {});
  }

  /** POST /api/bookings/{id}/dispute — technician raises a dispute (flips booking to Disputed + creates the dispute record). */
  raiseDispute(id: number, disputeType: DisputeType, reason: string): Observable<void> {
    return this.api.post<void>(`/api/bookings/${id}/dispute`, { disputeType, reason });
  }

  /** POST /api/reviews — leave a review on a completed+confirmed booking. Backend derives reviewer/reviewee from auth + booking. */
  createReview(bookingId: number, rating: number, comment: string): Observable<void> {
    return this.api.post<void>('/api/reviews', { bookingId, rating, comment });
  }

  /** POST /api/bookings/{id}/images — upload a Before/After photo (URL already hosted on Cloudinary). */
  uploadImage(id: number, imageUrl: string, type: BookingImageType): Observable<{ id: number }> {
    return this.api.post<{ id: number }>(`/api/bookings/${id}/images`, { bookingId: id, imageUrl, type });
  }

  /** GET /api/bookings/{id}/images — Before/After photos for this booking. */
  getImages(id: number): Observable<BookingImage[]> {
    return this.api.get<BookingImage[]>(`/api/bookings/${id}/images`);
  }

  /** POST /api/aianalysis — optional AI triage of the problem from the customer's before-photo. */
  analyze(bookingId: number, problemTypeId: number, description: string, imageUrl: string): Observable<AiAnalysis> {
    return this.api.post<AiAnalysis>('/api/aianalysis', { bookingId, problemTypeId, description, imageUrl });
  }

  /** GET /api/bookings/tech-pricing-range — tech's own range for one problem type (null if not configured). */
  getMyPricingRange(technicianId: number, problemTypeId: number): Observable<TechnicianPricingRange | null> {
    return this.api
      .get<TechnicianPricingRange>(
        `/api/bookings/tech-pricing-range?technicianId=${technicianId}&problemTypeId=${problemTypeId}`,
      )
      .pipe(
        catchError((err: HttpErrorResponse) => (err.status === 404 ? of(null) : (() => { throw err; })())),
      );
  }
}
