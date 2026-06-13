import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from '../../../core/services/api.service';
import { DisputeDto } from '../models/staff-disput.model';

@Injectable({
  providedIn: 'root'
})
export class DisputeService {

  private readonly api = inject(ApiService);
  // ApiService already prepends `${environment.apiUrl}/api`, so paths are relative.
  private readonly path = '/disputes';

  getDisputes(): Observable<DisputeDto[]> {
    return this.api.get<DisputeDto[]>(this.path);
  }

  getDispute(id: number): Observable<DisputeDto> {
    return this.api.get<DisputeDto>(`${this.path}/${id}`);
  }

  getByStatus(status: string): Observable<DisputeDto[]> {
    return this.api.get<DisputeDto[]>(`${this.path}/status/${status}`);
  }

  getByType(type: string): Observable<DisputeDto[]> {
    return this.api.get<DisputeDto[]>(`${this.path}/type/${type}`);
  }

  updateDispute(id: number, body: any): Observable<DisputeDto> {
    return this.api.put<DisputeDto>(`${this.path}/${id}`, body);
  }

  deleteDispute(id: number): Observable<void> {
    return this.api.delete<void>(`${this.path}/${id}`);
  }

  getById(id: number): Observable<DisputeDto> {
    return this.api.get<DisputeDto>(`${this.path}/${id}`);
  }

  // ── Dispute resolution actions (used by the details modal) ──────────────────

  /** PATCH /api/users/{id}/deactivate — ban a customer or technician. */
  banUser(userId: string): Observable<void> {
    return this.api.patch<void>(`/users/${userId}/deactivate`, {});
  }

  /** POST /api/escrows/{escrowId}/refund — return the held funds to the customer. */
  refundEscrow(escrowId: number): Observable<any> {
    return this.api.post<any>(`/escrows/${escrowId}/refund`, {});
  }

  /** POST /api/escrows/{escrowId}/release — release the held funds to the technician. */
  releaseEscrow(escrowId: number): Observable<any> {
    return this.api.post<any>(`/escrows/${escrowId}/release`, {});
  }
}
