import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-set-price-modal',
  imports: [FormsModule, TranslatePipe],
  templateUrl: './set-price-modal.component.html',
})
export class SetPriceModalComponent {
  private readonly activeModal = inject(NgbActiveModal);

  address: string | null = null;
  scheduleText: string | null = null;
  durationMinutes: number | null = null;

  price = signal<number | null>(null);
  touched = signal(false);

  confirm() {
    this.touched.set(true);
    const p = this.price();
    if (p == null || p <= 0) return;
    this.activeModal.close(p);
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
