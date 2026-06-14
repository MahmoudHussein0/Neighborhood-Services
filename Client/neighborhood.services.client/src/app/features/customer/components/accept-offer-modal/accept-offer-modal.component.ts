import { Component, inject, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

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

  private readonly activeModal = inject(NgbActiveModal);

  // Closes with the trimmed promo code (or null when blank). Dismiss rejects the promise.
  confirm() {
    const code = this.promoCode().trim();
    this.activeModal.close(code ? code : null);
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
