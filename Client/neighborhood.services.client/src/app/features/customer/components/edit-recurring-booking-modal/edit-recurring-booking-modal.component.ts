import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

import { RecurringBookingService } from '../../services/recurring-booking.service';
import { RecurringBooking, RecurringPattern, UpdateRecurringBooking } from '../../models/recurring-booking.model';

@Component({
  selector: 'app-edit-recurring-booking-modal',
  imports: [ReactiveFormsModule, TranslatePipe],
  templateUrl: './edit-recurring-booking-modal.component.html',
})
export class EditRecurringBookingModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly activeModal = inject(NgbActiveModal);
  private readonly service = inject(RecurringBookingService);

  readonly daysOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  readonly patterns: RecurringPattern[] = ['Daily', 'Weekly', 'Monthly'];

  submitting = signal(false);

  form = this.fb.group({
    pattern: this.fb.control<RecurringPattern | null>(null, Validators.required),
    dayOfWeek: this.fb.control<string | null>(null),
    dayOfMonth: this.fb.control<number | null>(null),
    timeOfDay: ['', Validators.required],
    durationMinutes: this.fb.control<number | null>(null, [Validators.required, Validators.min(15)]),
    startDate: ['', Validators.required],
    endDate: [''],
    address: ['', Validators.required],
  });

  private bookingId = 0;

  // Set by the opener via componentInstance — prefills the form from the existing arrangement.
  set booking(rb: RecurringBooking) {
    this.bookingId = rb.id;
    this.form.patchValue({
      pattern: rb.pattern,
      dayOfWeek: rb.dayOfWeek ?? null,
      dayOfMonth: rb.dayOfMonth ?? null,
      timeOfDay: rb.timeOfDay?.slice(0, 5) ?? '',
      durationMinutes: rb.durationMinutes,
      startDate: rb.startDate,
      endDate: rb.endDate ?? '',
      address: rb.address,
    });
    this.applyPatternValidators(rb.pattern);
  }

  onPatternChange() {
    this.applyPatternValidators(this.form.controls.pattern.value);
  }

  private applyPatternValidators(pattern: RecurringPattern | null) {
    const dow = this.form.controls.dayOfWeek;
    const dom = this.form.controls.dayOfMonth;
    dow.clearValidators();
    dom.clearValidators();
    if (pattern === 'Weekly') dow.setValidators(Validators.required);
    if (pattern === 'Monthly') dom.setValidators([Validators.required, Validators.min(1), Validators.max(31)]);
    dow.updateValueAndValidity();
    dom.updateValueAndValidity();
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.getRawValue();
    const payload: UpdateRecurringBooking = {
      address: v.address!,
      pattern: v.pattern!,
      dayOfWeek: v.pattern === 'Weekly' ? v.dayOfWeek! : null,
      dayOfMonth: v.pattern === 'Monthly' ? v.dayOfMonth! : null,
      timeOfDay: `${v.timeOfDay}:00`,
      durationMinutes: v.durationMinutes!,
      startDate: v.startDate!,
      endDate: v.endDate ? v.endDate : null,
    };

    this.submitting.set(true);
    this.service.update(this.bookingId, payload).subscribe({
      next: () => {
        this.submitting.set(false);
        this.activeModal.close(true);
      },
      error: () => this.submitting.set(false),
    });
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
