import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

import { RequestBrowseService } from '../../services/request-browse.service';
import { LightboxService } from '../../../../shared/services/lightbox.service';
import { ServiceRequestSummary, ServiceRequestDetails } from '../../../customer/models/service-request.model';

@Component({
  selector: 'app-request-details-modal',
  imports: [CurrencyPipe, DatePipe, RouterLink, TranslatePipe],
  templateUrl: './request-details-modal.component.html',
})
export class RequestDetailsModalComponent implements OnInit {
  private readonly activeModal = inject(NgbActiveModal);
  private readonly service = inject(RequestBrowseService);
  readonly lightbox = inject(LightboxService);

  // Set by the opener via componentInstance. The summary gives instant data + canOffer;
  // the full record (with image + full description) is fetched on open.
  summary!: ServiceRequestSummary;

  loading = signal(true);
  details = signal<ServiceRequestDetails | null>(null);

  ngOnInit() {
    this.service.getById(this.summary.id).subscribe({
      next: (d) => {
        this.details.set(d);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  /** Google Maps link for the request's coordinates (opens in a new tab). */
  get mapUrl(): string {
    const lat = this.details()?.latitude ?? this.summary.latitude;
    const lng = this.details()?.longitude ?? this.summary.longitude;
    return `https://www.google.com/maps/search/?api=1&query=${lat},${lng}`;
  }

  /** Close, signalling the opener to launch the Make Offer flow. */
  makeOffer() {
    this.activeModal.close('offer');
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
