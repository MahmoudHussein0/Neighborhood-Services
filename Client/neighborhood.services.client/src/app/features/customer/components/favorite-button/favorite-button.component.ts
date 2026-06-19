import { Component, Input, OnInit, computed, inject, signal } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { FavoritesStore } from '../../services/favorites.store';

/**
 * Reusable heart toggle a customer can drop onto any card/header that shows a
 * technician. Reads/writes the shared FavoritesStore so all instances stay in sync.
 */
@Component({
  selector: 'app-favorite-button',
  imports: [TranslatePipe],
  template: `
    <button
      type="button"
      class="btn btn-sm"
      [class.btn-danger]="isFav()"
      [class.btn-outline-danger]="!isFav()"
      [disabled]="busy()"
      [title]="(isFav() ? 'favorites.remove' : 'favorites.add') | translate"
      [attr.aria-label]="(isFav() ? 'favorites.remove' : 'favorites.add') | translate"
      (click)="toggle($event)">
      <i class="bi" [class.bi-heart-fill]="isFav()" [class.bi-heart]="!isFav()"></i>
    </button>
  `,
})
export class FavoriteButtonComponent implements OnInit {
  @Input({ required: true }) technicianId!: number;

  private readonly store = inject(FavoritesStore);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  busy = signal(false);
  isFav = computed(() => this.store.isFavorite(this.technicianId));

  ngOnInit(): void {
    this.store.ensureLoaded();
  }

  toggle(event: Event): void {
    // Stop the click from triggering a parent link/card navigation.
    event.stopPropagation();
    event.preventDefault();
    if (this.busy()) return;

    this.busy.set(true);
    this.store.toggle(this.technicianId).subscribe({
      next: (nowFav) => {
        this.busy.set(false);
        this.toastr.success(this.translate.instant(nowFav ? 'favorites.added' : 'favorites.removed'));
      },
      error: () => {
        this.busy.set(false);
        this.toastr.error(this.translate.instant('favorites.error'));
      },
    });
  }
}
