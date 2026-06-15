import { Injectable, inject, signal } from '@angular/core';
import { Observable, map } from 'rxjs';
import { FavoritesService } from './favorites.service';
import { FavoriteItem } from '../models/favorite-item';


/**
 * Holds the signed-in customer's favorite technicians as a shared signal, so every
 * favorite button on a page reflects (and updates) the same state without each card
 * hitting the API on its own.
 */
@Injectable({ providedIn: 'root' })
export class FavoritesStore {
  private readonly api = inject(FavoritesService);

  private readonly items = signal<FavoriteItem[]>([]);
  private loading = false;

  /** Fetch the customer's favorites. Safe to call from every card — concurrent calls
   *  while a request is in flight are deduped to one. Unlike a load-once cache, this
   *  re-fetches on each fresh call (e.g. when a page with hearts is (re)opened), so the
   *  hearts stay in sync with changes made elsewhere such as the Favorites tab. */
  ensureLoaded(): void {
    if (this.loading) return;
    this.loading = true;
    this.api.getForCurrentUser().subscribe({
      next: (list) => {
        this.items.set(list ?? []);
        this.loading = false;
      },
      error: () => (this.loading = false), // allow a later card to retry
    });
  }

  isFavorite(technicianId: number): boolean {
    return this.items().some((f) => f.technicianId === technicianId);
  }


  /** Adds or removes the technician; emits the resulting state (true = now favorited). */
  toggle(technicianId: number): Observable<boolean> {
    const existing = this.items().find((f) => f.technicianId === technicianId);

    if (existing) {
      return this.api.Delete(existing.favoriteId).pipe(
        map(() => {
          this.items.update((l) => l.filter((f) => f.favoriteId !== existing.favoriteId));
          return false;
        }),
      );
    }

    return this.api.AddToMyFavorites(technicianId).pipe(
      map((res) => {
        // The PUT endpoint returns a single FavoriteDto (typed as an array upstream).
        const added = (Array.isArray(res) ? res[0] : res) as FavoriteItem;
        this.items.update((l) => [...l, added]);
        return true;
      }),
    );
  }
}
