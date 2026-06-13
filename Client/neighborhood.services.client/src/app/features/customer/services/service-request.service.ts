import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { PagedResult } from '../../../core/models/paged-result.model';
import {
  ServiceRequestSummary,
  ServiceRequestDetails,
  ServiceRequestWithOffers,
  ServiceRequestStatus,
  CreateServiceRequest,
} from '../models/service-request.model';

export interface GetMyRequestsParams {
  status?: ServiceRequestStatus;
  search?: string;
  page?: number;
  pageSize?: number;
}

@Injectable({
  providedIn: 'root',
})
export class ServiceRequestService {
  private readonly api = inject(ApiService);

  /** GET /api/servicerequests/mine — current customer's requests, paged + optional filter/search */
  getMine(params: GetMyRequestsParams = {}): Observable<PagedResult<ServiceRequestSummary>> {
    const query = new URLSearchParams();
    if (params.status) query.set('status', params.status);
    if (params.search?.trim()) query.set('search', params.search.trim());
    query.set('page', String(params.page ?? 1));
    query.set('pageSize', String(params.pageSize ?? 10));

    return this.api.get<PagedResult<ServiceRequestSummary>>(`/servicerequests/mine?${query.toString()}`);
  }

  /** GET /api/servicerequests/{id} */
  getById(id: number): Observable<ServiceRequestDetails> {
    return this.api.get<ServiceRequestDetails>(`/servicerequests/${id}`);
  }

  /** GET /api/servicerequests/{id}/with-offers — request + the offers received */
  getWithOffers(id: number): Observable<ServiceRequestWithOffers> {
    return this.api.get<ServiceRequestWithOffers>(`/servicerequests/${id}/with-offers`);
  }

  /** POST /api/servicerequests — post a new request */
  create(body: CreateServiceRequest): Observable<{ id: number }> {
    return this.api.post<{ id: number }>('/servicerequests', body);
  }
}
