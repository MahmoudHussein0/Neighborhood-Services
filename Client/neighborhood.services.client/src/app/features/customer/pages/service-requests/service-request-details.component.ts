import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { ServiceRequestService } from '../../services/service-request.service';
import { OfferService } from '../../services/offer.service';
import { ServiceRequestWithOffers, OfferSummary } from '../../models/service-request.model';
import { AcceptOfferModalComponent } from '../../components/accept-offer-modal/accept-offer-modal.component';
import { FavoriteButtonComponent } from '../../components/favorite-button/favorite-button.component';

@Component({
  selector: 'app-service-request-details',
  imports: [DatePipe, CurrencyPipe, RouterLink, TranslatePipe, FavoriteButtonComponent],
  templateUrl: './service-request-details.component.html',
  styleUrl: './service-request-details.component.css',
})
export class ServiceRequestDetailsComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly service = inject(ServiceRequestService);
  private readonly offerService = inject(OfferService);
  private readonly modal = inject(NgbModal);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  loading = signal(true);
  data = signal<ServiceRequestWithOffers | null>(null);
  accepting = signal(false);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.load(id);
  }

  load(id: number) {
    this.loading.set(true);
    this.service.getWithOffers(id).subscribe({
      next: (d) => {
        this.data.set(d);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  canAccept(offer: OfferSummary): boolean {
    return this.data()?.status === 'Open' && offer.status === 'Pending';
  }

  accept(offer: OfferSummary) {
    const ref = this.modal.open(AcceptOfferModalComponent);
    ref.componentInstance.technicianName = offer.technicianName;
    ref.componentInstance.price = offer.price;
    ref.componentInstance.duration = offer.estimatedDuration;

    ref.result.then(
      (promoCode: string | null) => {
        this.accepting.set(true);
        this.offerService.accept(offer.id, promoCode).subscribe({
          next: () => {
            this.accepting.set(false);
            this.toastr.success(this.translate.instant('serviceRequests.details.accepted'));
            this.router.navigate(['/customer/bookings']);
          },
          error: () => this.accepting.set(false),
        });
      },
      () => {} // dismissed
    );
  }

  offerBadgeClass(status: OfferSummary['status']): string {
    switch (status) {
      case 'Pending': return 'text-bg-warning';
      case 'Accepted': return 'text-bg-success';
      case 'Rejected': return 'text-bg-danger';
      case 'Expired': return 'text-bg-secondary';
      case 'Withdrawn': return 'text-bg-secondary';
    }
  }
}
