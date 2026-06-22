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
    return this.api.get<TechnicianSummary[]>('/technicians');
  }

  /** GET /api/technicians/available — currently available technicians */
  getAvailable(): Observable<TechnicianSummary[]> {
    return this.api.get<TechnicianSummary[]>('/technicians/available');
  }

  /** GET /api/technicians/browse — customer-facing cards (name/photo/location + categories) */
  getForBrowse(): Observable<TechnicianCard[]> {
    return this.api.get<TechnicianCard[]>('/technicians/browse');
  }

  /** GET /api/technitianavailability/{technicianId} — the technician's working days + hours */
  getAvailability(technicianId: number): Observable<TechnicianAvailabilitySlot[]> {
    return this.api.get<TechnicianAvailabilitySlot[]>(`/technitianavailability/${technicianId}`);
  }

  /**
   * GET /api/technicians/{id}/available-slots — bookable start-times for a day (working hours
   * minus confirmed bookings minus past). Returns wall-clock ISO strings e.g. "2026-06-22T09:00:00".
   */
  getAvailableSlots(technicianId: number, date: string, slotMinutes = 30): Observable<string[]> {
    return this.api.get<string[]>(`/technicians/${technicianId}/available-slots?date=${date}&slotMinutes=${slotMinutes}`);
  }
}
