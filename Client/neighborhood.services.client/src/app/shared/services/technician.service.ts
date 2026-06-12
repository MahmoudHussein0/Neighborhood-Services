import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../core/services/api.service';
import { TechnicianSummary } from '../../core/models/technician.model';
import { TechnicianCard } from '../../core/models/technician-card.model';
import { TechnicianAvailabilitySlot } from '../../core/models/technician-availability.model';

/**
 * Read-only access to technicians (owned by another teammate — consume only).
 */
@Injectable({
  providedIn: 'root',
})
export class TechnicianService {
  private readonly api = inject(ApiService);

  /** GET /api/technicians — all technicians */
  getAll(): Observable<TechnicianSummary[]> {
    return this.api.get<TechnicianSummary[]>('/api/technicians');
  }

  /** GET /api/technicians/available — currently available technicians */
  getAvailable(): Observable<TechnicianSummary[]> {
    return this.api.get<TechnicianSummary[]>('/api/technicians/available');
  }

  /** GET /api/technicians/browse — customer-facing cards (name/photo/location + categories) */
  getForBrowse(): Observable<TechnicianCard[]> {
    return this.api.get<TechnicianCard[]>('/api/technicians/browse');
  }

  /** GET /api/technitianavailability/{technicianId} — the technician's working days + hours */
  getAvailability(technicianId: number): Observable<TechnicianAvailabilitySlot[]> {
    return this.api.get<TechnicianAvailabilitySlot[]>(`/api/technitianavailability/${technicianId}`);
  }
}
