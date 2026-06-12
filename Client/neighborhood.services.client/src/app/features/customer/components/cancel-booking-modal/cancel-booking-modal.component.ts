import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-cancel-booking-modal',
  imports: [FormsModule, TranslatePipe],
  templateUrl: './cancel-booking-modal.component.html',
})
export class CancelBookingModalComponent {
  private readonly activeModal = inject(NgbActiveModal);

  reason = signal('');

  submit() {
    const text = this.reason().trim();
    if (!text) return;
    // Returns the reason to the caller via the modal result
    this.activeModal.close(text);
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
