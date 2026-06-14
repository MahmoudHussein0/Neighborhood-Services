import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { RequestBrowseService } from '../../services/request-browse.service';
import { ServiceRequestSummary } from '../../../customer/models/service-request.model';
import { MakeOfferModalComponent } from '../../components/make-offer-modal/make-offer-modal.component';

@Component({
  selector: 'app-technician-browse-requests',
  imports: [CurrencyPipe, DatePipe, RouterLink, TranslatePipe],
  templateUrl: './technician-browse-requests.component.html',
})
export class TechnicianBrowseRequestsComponent implements OnInit {
  private readonly service = inject(RequestBrowseService);
  private readonly modal = inject(NgbModal);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  readonly radiusOptions = [5000, 10000, 20000, 50000];

  loading = signal(false);
  requests = signal<ServiceRequestSummary[]>([]);
  radius = signal(5000);
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
    if (this.lat == null || this.lng == null) return;
    this.loading.set(true);
    this.service.getOpen(this.lat, this.lng, this.radius()).subscribe({
      next: (r) => {
        this.requests.set(r ?? []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  setRadius(r: number) {
    this.radius.set(r);
    this.load();
  }

  makeOffer(req: ServiceRequestSummary) {
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
