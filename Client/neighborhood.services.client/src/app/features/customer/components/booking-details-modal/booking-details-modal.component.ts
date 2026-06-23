import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';
import { BookingService } from '../../services/booking.service';
import { BookingDetails, BookingImage } from '../../models/booking.model';
import { googleMapsUrl } from '../../../../core/utils/maps.util';
import { LightboxService } from '../../../../shared/services/lightbox.service';

@Component({
  selector: 'app-booking-details-modal',
  imports: [DatePipe, CurrencyPipe, TranslatePipe],
  templateUrl: './booking-details-modal.component.html',
})
export class BookingDetailsModalComponent implements OnInit {
  /** Set by the caller via componentInstance before the modal opens */
  bookingId!: number;

  private readonly activeModal = inject(NgbActiveModal);
  private readonly bookingService = inject(BookingService);
  protected readonly lightbox = inject(LightboxService);

  loading = signal(true);
  details = signal<BookingDetails | null>(null);
  images = signal<BookingImage[]>([]);

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

  close() {
    this.activeModal.close();
  }
}
