import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { OfferService } from '../../services/offer.service';
import { CreateOffer } from '../../models/offer.model';

@Component({
  selector: 'app-make-offer-modal',
  imports: [ReactiveFormsModule, TranslatePipe],
  templateUrl: './make-offer-modal.component.html',
})
export class MakeOfferModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly activeModal = inject(NgbActiveModal);
  private readonly service = inject(OfferService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  submitting = signal(false);

  // Set by the opener via componentInstance.
  serviceRequestId = 0;
  requestDescription: string | null = null;
  requestAddress: string | null = null;

  readonly minDateTime = new Date(Date.now() - new Date().getTimezoneOffset() * 60000)
    .toISOString()
    .slice(0, 16);

  form = this.fb.group({
    price: this.fb.control<number | null>(null, [Validators.required, Validators.min(1)]),
    estimatedDuration: this.fb.control<number | null>(null, [Validators.required, Validators.min(15)]),
    scheduledAt: ['', Validators.required],
    message: [''],
  });

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    const payload: CreateOffer = {
      serviceRequestId: this.serviceRequestId,
      price: v.price!,
      estimatedDuration: v.estimatedDuration!,
      message: v.message ?? '',
      scheduledAt: `${v.scheduledAt}:00`,
    };

    this.submitting.set(true);
    this.service.create(payload).subscribe({
      next: (res) => {
        this.submitting.set(false);
        (res.warnings ?? []).forEach((w) => this.toastr.warning(w));
        this.activeModal.close(true);
      },
      error: () => this.submitting.set(false),
    });
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
