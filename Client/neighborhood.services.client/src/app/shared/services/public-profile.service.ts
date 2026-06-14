import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../core/services/api.service';
import { PublicProfile } from '../../core/models/public-profile.model';

/**
 * Public profiles — the customer-facing view of a technician, and the
 * technician-facing view of a customer (public details + approved reviews).
 */
@Injectable({ providedIn: 'root' })
export class PublicProfileService {
  private readonly api = inject(ApiService);

  /** GET /api/technicians/{technicianId}/public-profile */
  getTechnician(technicianId: number): Observable<PublicProfile> {
    return this.api.get<PublicProfile>(`/technicians/${technicianId}/public-profile`);
  }

  /** GET /api/customers/{customerId}/public-profile */
  getCustomer(customerId: number): Observable<PublicProfile> {
    return this.api.get<PublicProfile>(`/customers/${customerId}/public-profile`);
  }
}
