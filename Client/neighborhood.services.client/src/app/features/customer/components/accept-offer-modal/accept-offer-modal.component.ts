import { Component, inject } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-accept-offer-modal',
  imports: [CurrencyPipe, TranslatePipe],
  templateUrl: './accept-offer-modal.component.html',
})
export class AcceptOfferModalComponent {
  /** Set by the caller via componentInstance */
  technicianName!: string;
  price!: number;
  duration!: number;

  private readonly activeModal = inject(NgbActiveModal);

  confirm() {
    this.activeModal.close(true);
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
