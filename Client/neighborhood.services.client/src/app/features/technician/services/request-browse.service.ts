import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { ServiceRequestSummary, ServiceRequestDetails } from '../../customer/models/service-request.model';
import { PagedResult } from '../../../core/models/paged-result.model';

@Injectable({
  providedIn: 'root',
})
export class RequestBrowseService {
  private readonly api = inject(ApiService);

  /**
   * GET /api/servicerequests/open — open requests near the technician (paged).
   * radiusInMeters <= 0 means "All" (no distance filter).
   */
  getOpen(
    latitude: number,
    longitude: number,
    radiusInMeters: number,
    onlyMyCategories: boolean,
    page: number,
    pageSize: number,
  ): Observable<PagedResult<ServiceRequestSummary>> {
    const query = new URLSearchParams({
      latitude: String(latitude),
      longitude: String(longitude),
      radiusInMeters: String(radiusInMeters),
      onlyMyCategories: String(onlyMyCategories),
      page: String(page),
      pageSize: String(pageSize),
    });
    return this.api.get<PagedResult<ServiceRequestSummary>>(`/servicerequests/open?${query.toString()}`);
  }

  /** GET /api/servicerequests/{id} — full details (full description + image) for one request. */
  getById(id: number): Observable<ServiceRequestDetails> {
    return this.api.get<ServiceRequestDetails>(`/servicerequests/${id}`);
  }
}
