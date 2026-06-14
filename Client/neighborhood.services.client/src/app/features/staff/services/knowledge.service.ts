import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';

/**
 * RAG knowledge index maintenance (Staff only).
 * Rebuild is deliberate/off-boot — re-embeds only changed catalog docs into the vector store.
 */
@Injectable({ providedIn: 'root' })
export class KnowledgeService {
  private readonly api = inject(ApiService);

  /** POST /api/knowledge/reindex — rebuild the vector index from the catalog. */
  reindex(): Observable<{ message?: string }> {
    return this.api.post<{ message?: string }>('/knowledge/reindex', {});
  }
}
