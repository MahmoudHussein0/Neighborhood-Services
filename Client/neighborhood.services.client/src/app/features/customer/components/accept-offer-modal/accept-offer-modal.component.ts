import { Component, inject, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { Subject, debounceTime, distinctUntilChanged, switchMap, of, catchError, firstValueFrom } from 'rxjs';

import { BookingService, PromoCodePreview } from '../../services/booking.service';

@Component({
  selector: 'app-accept-offer-modal',
  imports: [CurrencyPipe, FormsModule, TranslatePipe],
  templateUrl: './accept-offer-modal.component.html',
})
export class AcceptOfferModalComponent {
  /** Set by the caller via componentInstance */
  technicianName!: string;
  price!: number;
  duration!: number;

  // Optional promo code — discount is applied server-side when the offer is accepted.
  promoCode = signal('');

  /** Previewed total after an applicable promo (null = blank/invalid → show the original price). */
  discountedPrice = signal<number | null>(null);

  /** True when a non-blank code came back not-applicable (invalid, expired, or already used). */
  promoUnavailable = signal(false);

  private readonly activeModal = inject(NgbActiveModal);
  private readonly bookingService = inject(BookingService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);
  private readonly promoInput$ = new Subject<string>();

  constructor() {
    this.promoInput$
      .pipe(
        debounceTime(400),
        distinctUntilChanged(),
        switchMap((code) => {
          const trimmed = code.trim();
          if (!trimmed) return of(null);
          // Swallow errors so the stream stays alive; treat as "no preview available".
          return this.bookingService.getPromoPreview(trimmed).pipe(catchError(() => of(null)));
        }),
        takeUntilDestroyed(),
      )
      .subscribe((preview) => {
        if (preview && preview.isApplicable && preview.discountPercentage > 0) {
          // Same math as the backend: discount off the offer price, rounded to cents, clamped at 0.
          const discount = Math.round((this.price * preview.discountPercentage) / 100 * 100) / 100;
          this.discountedPrice.set(Math.max(0, this.price - discount));
          this.promoUnavailable.set(false);
        } else {
          this.discountedPrice.set(null);
          this.promoUnavailable.set(!!preview && !preview.isApplicable);
        }
      });
  }

  onPromoChange(value: string) {
    this.promoCode.set(value);
    this.promoInput$.next(value);
  }

  // Closes with the trimmed promo code (or null when blank). Dismiss rejects the promise.
  // A non-blank code is re-checked first; if it can't be used we warn, clear it, and stay open.
  async confirm() {
    const code = this.promoCode().trim();
    if (code) {
      let preview: PromoCodePreview | null = null;
      try {
        preview = await firstValueFrom(this.bookingService.getPromoPreview(code));
      } catch {
        preview = null;
      }
      if (!preview || !preview.isApplicable) {
        this.toastr.warning(this.translate.instant('common.promoUnavailable'));
        this.promoCode.set('');
        this.discountedPrice.set(null);
        this.promoUnavailable.set(false);
        return;
      }
    }
    this.activeModal.close(code ? code : null);
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
