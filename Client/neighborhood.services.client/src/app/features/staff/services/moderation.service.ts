import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { PagedResult } from '../../../core/models/paged-result.model';

// Mirrors FlaggedServiceRequestDto (GET /api/servicerequests/flagged)
export interface FlaggedServiceRequest {
  id: number;
  description: string;
  address: string;
  image?: string | null;
  budget: number;
  customerId: number;
  scheduledAt: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class ModerationService {
  private readonly api = inject(ApiService);

  /** GET /api/servicerequests/flagged — the moderation queue (staff only). */
  getFlagged(page = 1, pageSize = 10): Observable<PagedResult<FlaggedServiceRequest>> {
    return this.api.get<PagedResult<FlaggedServiceRequest>>(
      `/servicerequests/flagged?page=${page}&pageSize=${pageSize}`
    );
  }

  /** POST /api/servicerequests/{id}/approve — Flagged -> Open. */
  approve(id: number): Observable<void> {
    return this.api.post<void>(`/servicerequests/${id}/approve`, {});
  }

  /** POST /api/servicerequests/{id}/reject — Flagged -> Closed. */
  reject(id: number): Observable<void> {
    return this.api.post<void>(`/servicerequests/${id}/reject`, {});
  }
}
