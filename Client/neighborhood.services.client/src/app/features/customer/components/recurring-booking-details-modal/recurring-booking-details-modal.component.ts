import { Component, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

import { RecurringBooking, RecurringBookingStatus } from '../../models/recurring-booking.model';
import { BookingSummary, BookingStatus } from '../../models/booking.model';
import { BookingService } from '../../services/booking.service';
import { LightboxService } from '../../../../shared/services/lightbox.service';
import { nextOccurrences } from '../../utils/recurrence.util';

@Component({
  selector: 'app-recurring-booking-details-modal',
  imports: [CurrencyPipe, DatePipe, TranslatePipe],
  templateUrl: './recurring-booking-details-modal.component.html',
})
export class RecurringBookingDetailsModalComponent {
  private readonly activeModal = inject(NgbActiveModal);
  private readonly bookingService = inject(BookingService);
  protected readonly lightbox = inject(LightboxService);

  private _booking!: RecurringBooking;

  upcoming = signal<Date[]>([]);
  recent = signal<BookingSummary[]>([]);
  loadingVisits = signal(false);

  // Set by the opener via componentInstance (the row from the list — already a full DTO).
  set booking(rb: RecurringBooking) {
    this._booking = rb;
    this.upcoming.set(nextOccurrences(rb, 5));   // projected future (the job only materializes ~7 days out)
    this.loadRecent(rb.id);                      // actual past visits with real status
  }
  get booking(): RecurringBooking {
    return this._booking;
  }

  private loadRecent(recurringBookingId: number) {
    this.loadingVisits.set(true);
    this.bookingService.getByRecurring(recurringBookingId).subscribe({
      next: (visits) => {
        const now = Date.now();
        this.recent.set(
          visits
            .filter((v) => new Date(v.scheduledAt).getTime() < now)
            .sort((a, b) => new Date(b.scheduledAt).getTime() - new Date(a.scheduledAt).getTime())
            .slice(0, 5),
        );
        this.loadingVisits.set(false);
      },
      error: () => this.loadingVisits.set(false),
    });
  }

  badgeClass(status: RecurringBookingStatus): string {
    switch (status) {
      case 'Active': return 'text-bg-success';
      case 'Paused': return 'text-bg-warning';
      case 'Cancelled': return 'text-bg-danger';
      case 'PendingCustomerApproval': return 'text-bg-primary';
      case 'PendingApproval': return 'text-bg-secondary';
    }
  }

  visitBadgeClass(status: BookingStatus): string {
    switch (status) {
      case 'Completed': return 'text-bg-success';
      case 'Confirmed': return 'text-bg-primary';
      case 'Cancelled': return 'text-bg-danger';
      case 'Disputed': return 'text-bg-warning';
      case 'Quoted': return 'text-bg-info';
      case 'Pending': return 'text-bg-secondary';
    }
  }

  close() {
    this.activeModal.close();
  }
}
