import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

import { DisputeType } from '../../models/booking.model';

@Component({
  selector: 'app-raise-dispute-modal',
  imports: [FormsModule, TranslatePipe],
  templateUrl: './raise-dispute-modal.component.html',
})
export class RaiseDisputeModalComponent {
  private readonly activeModal = inject(NgbActiveModal);

  // Overridable by the opener (e.g. the technician passes a customer-facing subset).
  types: DisputeType[] = ['PoorService', 'TechnicianBehavior', 'PaymentIssue', 'Scam', 'Other'];
  disputeType = signal<DisputeType>('PoorService');
  reason = signal('');

  submit() {
    const text = this.reason().trim();
    if (!text) return;
    this.activeModal.close({ disputeType: this.disputeType(), reason: text });
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
