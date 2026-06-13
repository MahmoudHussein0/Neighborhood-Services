import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { ServiceRequestSummary } from '../../customer/models/service-request.model';

@Injectable({
  providedIn: 'root',
})
export class RequestBrowseService {
  private readonly api = inject(ApiService);

  /** GET /api/servicerequests/open — open requests near the technician (geo radius). */
  getOpen(latitude: number, longitude: number, radiusInMeters: number): Observable<ServiceRequestSummary[]> {
    const query = new URLSearchParams({
      latitude: String(latitude),
      longitude: String(longitude),
      radiusInMeters: String(radiusInMeters),
    });
    return this.api.get<ServiceRequestSummary[]>(`/servicerequests/open?${query.toString()}`);
  }
}
