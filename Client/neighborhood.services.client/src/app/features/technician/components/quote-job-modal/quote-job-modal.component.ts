import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { JobService } from '../../services/job.service';
import { MyBookingSummary, TechnicianPricingRange, BookingImage, AiAnalysis } from '../../../customer/models/booking.model';
import { googleMapsUrl } from '../../../../core/utils/maps.util';

@Component({
  selector: 'app-quote-job-modal',
  imports: [CurrencyPipe, DatePipe, ReactiveFormsModule, TranslatePipe],
  templateUrl: './quote-job-modal.component.html',
})
export class QuoteJobModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly activeModal = inject(NgbActiveModal);
  private readonly jobService = inject(JobService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  /** Set by the opener via componentInstance. */
  job!: MyBookingSummary;

  // Tech's own configured range for this problem type — required to quote.
  pricingRange = signal<TechnicianPricingRange | null>(null);
  pricingLoaded = signal(false);
  submitting = signal(false);

  // "Before" photos the customer attached so the tech can assess before quoting.
  beforePhotos = signal<BookingImage[]>([]);

  // Optional AI triage of the before-photo (on-demand, not automatic).
  analyzing = signal(false);
  analysis = signal<AiAnalysis | null>(null);

  protected readonly mapsUrl = googleMapsUrl;

  form = this.fb.group({
    finalPrice: this.fb.control<number | null>(null, [Validators.required, Validators.min(1)]),
    durationMinutes: this.fb.control<number | null>(null, [Validators.required, Validators.min(15)]),
  });

  ngOnInit() {
    // Prefill if the tech already quoted (Quoted -> editing the existing quote)
    if (this.job.finalPrice > 0) this.form.controls.finalPrice.setValue(this.job.finalPrice);
    if (this.job.durationMinutes != null) this.form.controls.durationMinutes.setValue(this.job.durationMinutes);

    this.jobService.getMyPricingRange(this.job.technicianId, this.job.problemTypeId).subscribe({
      next: (range) => {
        this.pricingRange.set(range);
        this.pricingLoaded.set(true);
      },
      error: () => this.pricingLoaded.set(true),
    });

    this.jobService.getImages(this.job.id).subscribe({
      next: (images) => this.beforePhotos.set((images ?? []).filter((i) => i.type === 'Before')),
    });
  }

  // Optional: ask the AI to read the customer's before-photo and suggest the problem + a range.
  detectFromPhoto() {
    const photo = this.beforePhotos()[0];
    if (!photo || this.analyzing()) return;

    this.analyzing.set(true);
    this.analysis.set(null);
    this.jobService.analyze(this.job.id, this.job.problemTypeId, this.job.description, photo.imageUrl).subscribe({
      next: (a) => {
        this.analysis.set(a);
        this.analyzing.set(false);
      },
      error: () => {
        this.analyzing.set(false);
        this.toastr.error(this.translate.instant('technician.jobs.quote.detectFailed'));
      },
    });
  }

  severityClass(level: string): string {
    switch (level) {
      case 'High': return 'text-bg-danger';
      case 'Medium': return 'text-bg-warning';
      default: return 'text-bg-success';
    }
  }

  /** Out-of-range warning — for UX only; the backend enforces the hard limit. */
  rangeWarning(): string | null {
    const range = this.pricingRange();
    const price = this.form.controls.finalPrice.value;
    if (!range || price == null) return null;
    if (price < range.minPrice || price > range.maxPrice) {
      return this.translate.instant('technician.jobs.quote.outOfRange', {
        min: range.minPrice,
        max: range.maxPrice,
      });
    }
    return null;
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    if (!this.pricingRange()) {
      this.toastr.error(this.translate.instant('technician.jobs.quote.noPricing'));
      return;
    }

    const v = this.form.getRawValue();
    this.submitting.set(true);
    this.jobService.quote(this.job.id, v.finalPrice!, v.durationMinutes!).subscribe({
      next: () => this.activeModal.close(true),
      error: () => this.submitting.set(false),
    });
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
