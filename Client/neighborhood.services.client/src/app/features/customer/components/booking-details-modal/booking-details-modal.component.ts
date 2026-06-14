import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { BookingService } from '../../services/booking.service';
import { BookingDetails, BookingImage } from '../../models/booking.model';
import { googleMapsUrl } from '../../../../core/utils/maps.util';

@Component({
  selector: 'app-booking-details-modal',
  imports: [DatePipe, CurrencyPipe, FormsModule, TranslatePipe],
  templateUrl: './booking-details-modal.component.html',
})
export class BookingDetailsModalComponent implements OnInit {
  /** Set by the caller via componentInstance before the modal opens */
  bookingId!: number;

  private readonly activeModal = inject(NgbActiveModal);
  private readonly bookingService = inject(BookingService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  loading = signal(true);
  details = signal<BookingDetails | null>(null);
  images = signal<BookingImage[]>([]);

  promoCode = signal('');
  applyingPromo = signal(false);
  promoApplied = signal(false);

  protected readonly mapsUrl = googleMapsUrl;

  ngOnInit() {
    this.loadDetails();

    this.bookingService.getImages(this.bookingId).subscribe({
      next: (images) => this.images.set(images ?? []),
    });
  }

  private loadDetails() {
    this.bookingService.getById(this.bookingId).subscribe({
      next: (d) => {
        this.details.set(d);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  /** Promo can be applied while the booking is still active (not finished/cancelled/disputed). */
  canApplyPromo(status: BookingDetails['status']): boolean {
    return status === 'Pending' || status === 'Quoted' || status === 'Confirmed';
  }

  applyPromo() {
    const code = this.promoCode().trim();
    if (!code) {
      this.toastr.warning(this.translate.instant('bookings.detailsModal.promoEmpty'));
      return;
    }

    this.applyingPromo.set(true);
    this.bookingService.applyPromoCode(this.bookingId, code).subscribe({
      next: () => {
        this.promoApplied.set(true);
        this.promoCode.set('');
        this.toastr.success(this.translate.instant('bookings.detailsModal.promoSuccess'));
        // Backend updated FinalPrice — re-fetch so the invoice reflects the new total.
        this.loadDetails();
        this.applyingPromo.set(false);
      },
      error: () => this.applyingPromo.set(false), // interceptor surfaces the reason
    });
  }

  close() {
    // Signal the parent to refresh if a discount changed the total.
    this.activeModal.close(this.promoApplied());
  }
}
