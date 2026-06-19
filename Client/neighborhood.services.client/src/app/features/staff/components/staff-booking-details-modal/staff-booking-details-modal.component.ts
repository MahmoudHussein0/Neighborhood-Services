import { Component, Input } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { StaffBooking, StaffBookingStatus } from '../../services/staff-booking.service';

/**
 * Read-only details view for a single booking on the staff oversight page. Renders the
 * StaffBooking already loaded in the table row — no extra fetch needed.
 */
@Component({
  selector: 'app-staff-booking-details-modal',
  imports: [DatePipe, CurrencyPipe, TranslatePipe],
  template: `
    <div class="modal-header">
      <h5 class="modal-title">
        {{ 'staffBookings.detailsTitle' | translate }} #{{ booking.id }}
      </h5>
      <button type="button" class="btn-close" (click)="activeModal.dismiss()"
              [attr.aria-label]="'common.close' | translate"></button>
    </div>

    <div class="modal-body">
      <div class="d-flex align-items-center gap-2 mb-3">
        <span class="badge rounded-pill" [class]="badgeClass(booking.status)">
          {{ ('staffBookings.tabs.' + booking.status) | translate }}
        </span>
        <span class="badge text-bg-light border">
          {{ ('bookings.bookingType.' + booking.bookingType) | translate }}
        </span>
      </div>

      <dl class="row mb-0 small">
        <dt class="col-5 text-muted">{{ 'staffBookings.customer' | translate }}</dt>
        <dd class="col-7">{{ booking.customerName }}</dd>

        <dt class="col-5 text-muted">{{ 'staffBookings.technician' | translate }}</dt>
        <dd class="col-7">{{ booking.technicianName || '—' }}</dd>

        <dt class="col-5 text-muted">{{ 'staffBookings.address' | translate }}</dt>
        <dd class="col-7">{{ booking.address || '—' }}</dd>

        <dt class="col-5 text-muted">{{ 'staffBookings.scheduled' | translate }}</dt>
        <dd class="col-7">{{ booking.scheduledAt | date: 'medium' }}</dd>

        <dt class="col-5 text-muted">{{ 'staffBookings.createdAt' | translate }}</dt>
        <dd class="col-7">{{ booking.createdAt | date: 'medium' }}</dd>

        <dt class="col-5 text-muted">{{ 'staffBookings.estimatedPrice' | translate }}</dt>
        <dd class="col-7">{{ booking.estimatedPrice | currency: 'EGP ' }}</dd>

        <dt class="col-5 text-muted">{{ 'staffBookings.finalPrice' | translate }}</dt>
        <dd class="col-7">
          {{ booking.finalPrice > 0 ? (booking.finalPrice | currency: 'EGP ') : '—' }}
        </dd>
      </dl>
    </div>

    <div class="modal-footer">
      <button type="button" class="btn btn-secondary" (click)="activeModal.close()">
        {{ 'common.close' | translate }}
      </button>
    </div>
  `,
})
export class StaffBookingDetailsModalComponent {
  @Input({ required: true }) booking!: StaffBooking;

  constructor(public readonly activeModal: NgbActiveModal) {}

  badgeClass(status: StaffBookingStatus): string {
    switch (status) {
      case 'Pending': return 'text-bg-warning';
      case 'Quoted': return 'text-bg-info';
      case 'Confirmed': return 'text-bg-primary';
      case 'Completed': return 'text-bg-success';
      case 'Cancelled': return 'text-bg-danger';
      case 'Disputed': return 'text-bg-secondary';
    }
  }
}
