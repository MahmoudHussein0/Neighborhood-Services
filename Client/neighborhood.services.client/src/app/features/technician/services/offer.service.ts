import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { PagedResult } from '../../../core/models/paged-result.model';
import { Offer, OfferStatus, CreateOffer } from '../models/offer.model';

export interface GetMyOffersParams {
  status?: OfferStatus;
  page?: number;
  pageSize?: number;
}

@Injectable({
  providedIn: 'root',
})
export class OfferService {
  private readonly api = inject(ApiService);

  /** GET /api/offers/mine — the technician's own offers (paged + optional status filter). */
  getMyOffers(params: GetMyOffersParams = {}): Observable<PagedResult<Offer>> {
    const query = new URLSearchParams();
    if (params.status) query.set('status', params.status);
    query.set('page', String(params.page ?? 1));
    query.set('pageSize', String(params.pageSize ?? 10));

    return this.api.get<PagedResult<Offer>>(`/offers/mine?${query.toString()}`);
  }

  /** POST /api/offers — submit an offer on a service request. */
  create(body: CreateOffer): Observable<{ offerId: number; warnings: string[] }> {
    return this.api.post<{ offerId: number; warnings: string[] }>('/offers', body);
  }

  /** POST /api/offers/{id}/withdraw — withdraw the technician's own offer. */
  withdraw(id: number): Observable<void> {
    return this.api.post<void>(`/offers/${id}/withdraw`, {});
  }
}
