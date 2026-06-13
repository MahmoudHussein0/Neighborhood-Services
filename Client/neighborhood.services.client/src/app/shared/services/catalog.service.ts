import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../core/services/api.service';
import { Category, CategoryDetails } from '../../core/models/catalog.model';

/**
 * Read-only access to the catalog (Categories + Problem Types).
 * These endpoints are owned by another teammate — consume only, don't modify.
 */
@Injectable({
  providedIn: 'root',
})
export class CatalogService {
  private readonly api = inject(ApiService);

  /** GET /api/categories — all categories */
  getCategories(lang = 'en'): Observable<Category[]> {
    return this.api.get<Category[]>(`/categories?lang=${lang}`);
  }

  /** GET /api/categories/{id} — a category with its problem types */
  getCategory(id: number, lang = 'en'): Observable<CategoryDetails> {
    return this.api.get<CategoryDetails>(`/categories/${id}?lang=${lang}`);
  }
}
