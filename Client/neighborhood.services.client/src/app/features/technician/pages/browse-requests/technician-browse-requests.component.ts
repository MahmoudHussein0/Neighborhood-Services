import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { RequestBrowseService } from '../../services/request-browse.service';
import { ServiceRequestSummary } from '../../../customer/models/service-request.model';
import { PagedResult } from '../../../../core/models/paged-result.model';
import { MakeOfferModalComponent } from '../../components/make-offer-modal/make-offer-modal.component';
import { RequestDetailsModalComponent } from '../../components/request-details-modal/request-details-modal.component';

@Component({
  selector: 'app-technician-browse-requests',
  imports: [CurrencyPipe, DatePipe, RouterLink, TranslatePipe],
  templateUrl: './technician-browse-requests.component.html',
  styleUrls: ['../../../../shared/styles/ns-card.css', './technician-browse-requests.component.css'],
})
export class TechnicianBrowseRequestsComponent implements OnInit {
  private readonly service = inject(RequestBrowseService);
  private readonly modal = inject(NgbModal);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  readonly radiusOptions = [5000, 10000, 20000, 50000];
  // 0 = "All" — no distance filter (backend skips IsWithinDistance).
  readonly allRadius = 0;
  readonly pageSize = 5;

  loading = signal(false);
  result = signal<PagedResult<ServiceRequestSummary> | null>(null);
  radius = signal(10000);
  // 'mine' = only the tech's own categories; 'all' = whole market (can still only offer in own categories).
  scope = signal<'mine' | 'all'>('mine');
  page = signal(1);
  private lat: number | null = null;
  private lng: number | null = null;
  locationError = signal(false);

  ngOnInit() {
    this.locateAndLoad();
  }

  locateAndLoad() {
    if (!navigator.geolocation) {
      this.locationError.set(true);
      return;
    }
    this.loading.set(true);
    this.locationError.set(false);
    navigator.geolocation.getCurrentPosition(
      (pos) => {
        this.lat = pos.coords.latitude;
        this.lng = pos.coords.longitude;
        this.load();
      },
      () => {
        this.loading.set(false);
        this.locationError.set(true);
      },
    );
  }

  load() {
    // "All" needs no coordinates; distance filters require a known location.
    if (this.radius() !== this.allRadius && (this.lat == null || this.lng == null)) return;
    this.loading.set(true);
    this.service.getOpen(this.lat ?? 0, this.lng ?? 0, this.radius(), this.scope() === 'mine', this.page(), this.pageSize).subscribe({
      next: (r) => {
        this.result.set(r);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  setRadius(r: number) {
    this.radius.set(r);
    this.page.set(1);
    if (r === this.allRadius) {
      // All requests — works even if location was denied.
      this.locationError.set(false);
      this.load();
    } else if (this.lat != null && this.lng != null) {
      this.load();
    } else {
      this.locateAndLoad();
    }
  }

  openDetails(req: ServiceRequestSummary) {
    const ref = this.modal.open(RequestDetailsModalComponent, { size: 'lg' });
    ref.componentInstance.summary = req;
    ref.result.then(
      (action: string) => {
        // The details modal resolves with 'offer' when the tech taps Make Offer there.
        if (action === 'offer') this.makeOffer(req);
      },
      () => {},
    );
  }

  setScope(scope: 'mine' | 'all') {
    if (this.scope() === scope) return;
    this.scope.set(scope);
    this.page.set(1);
    this.load();
  }

  goToPage(p: number) {
    this.page.set(p);
    this.load();
  }

  /** Google Maps link for the request's coordinates (opens in a new tab). */
  mapUrl(req: ServiceRequestSummary): string {
    return `https://www.google.com/maps/search/?api=1&query=${req.latitude},${req.longitude}`;
  }

  makeOffer(req: ServiceRequestSummary) {
    // Backstop the template guard: only offer on requests within the tech's categories.
    if (req.canOffer === false) return;
    const ref = this.modal.open(MakeOfferModalComponent, { size: 'lg' });
    ref.componentInstance.serviceRequestId = req.id;
    ref.componentInstance.requestDescription = req.description;
    ref.componentInstance.requestAddress = req.address;
    ref.result.then(
      () => {
        this.toastr.success(this.translate.instant('technician.browse.offerSent'));
        this.load();
      },
      () => {},
    );
  }
}
