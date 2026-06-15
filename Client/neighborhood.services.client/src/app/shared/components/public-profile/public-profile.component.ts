import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe, Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { PublicProfileService } from '../../services/public-profile.service';
import { PublicProfile } from '../../../core/models/public-profile.model';
import { environment } from '../../../environments/environment';
import { googleMapsUrl } from '../../../core/utils/maps.util';
import { FavoriteButtonComponent } from '../../../features/customer/components/favorite-button/favorite-button.component';

@Component({
  selector: 'app-public-profile',
  imports: [DatePipe, DecimalPipe, TranslatePipe, FavoriteButtonComponent],
  templateUrl: './public-profile.component.html',
  styleUrl: './public-profile.component.css',
})
export class PublicProfileComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(PublicProfileService);
  private readonly translate = inject(TranslateService);
  private readonly location = inject(Location);

  loading = signal(true);
  profile = signal<PublicProfile | null>(null);
  avatarError = signal(false);
  reviewPhotoErrors = signal<Set<number>>(new Set());
  // The route :id is the technician's id; only set when a customer is viewing a technician.
  technicianId = signal<number | null>(null);

  protected readonly mapsUrl = googleMapsUrl;

  onAvatarError(): void {
    this.avatarError.set(true);
  }

  onReviewPhotoError(reviewId: number): void {
    this.reviewPhotoErrors.update((set) => new Set(set).add(reviewId));
  }

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    const role = this.route.snapshot.data['role'] as 'technician' | 'customer';

    // A technician profile is only ever viewed from the customer layout → safe to favorite.
    if (role === 'technician') this.technicianId.set(id);

    const request$ =
      role === 'technician' ? this.service.getTechnician(id) : this.service.getCustomer(id);

    request$.subscribe({
      next: (p) => {
        this.profile.set(p);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  goBack(): void {
    this.location.back();
  }

  categoryName(c: { nameEn: string; nameAr: string }): string {
    return (this.translate.currentLang || 'en') === 'ar' ? c.nameAr : c.nameEn;
  }

  /** Resolve a stored photo path to a displayable URL (Cloudinary/abs URLs pass through). */
  photoSrc(photo: string | null | undefined): string {
    if (!photo) return '';
    if (photo.startsWith('http') || photo.startsWith('blob:') || photo.startsWith('data:')) return photo;
    return `${environment.apiUrl}${photo.startsWith('/') ? '' : '/'}${photo}`;
  }

  initials(name: string): string {
    return (name || '?')
      .split(' ')
      .map((p) => p.charAt(0))
      .slice(0, 2)
      .join('')
      .toUpperCase();
  }

  /** Full + (optional) half + empty star glyphs for a rating out of 5. */
  stars(rating: number): { full: number; half: boolean; empty: number } {
    const full = Math.floor(rating);
    const half = rating - full >= 0.5;
    return { full, half, empty: 5 - full - (half ? 1 : 0) };
  }

  range(n: number): number[] {
    return Array.from({ length: n }, (_, i) => i);
  }
}
