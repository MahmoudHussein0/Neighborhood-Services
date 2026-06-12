import { Component, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { UploadService } from '../../../../shared/services/upload.service';
import { JobService } from '../../services/job.service';
import { MyBookingSummary } from '../../../customer/models/booking.model';
import { googleMapsUrl } from '../../../../core/utils/maps.util';

@Component({
  selector: 'app-complete-job-modal',
  imports: [CurrencyPipe, DatePipe, TranslatePipe],
  templateUrl: './complete-job-modal.component.html',
})
export class CompleteJobModalComponent {
  private readonly activeModal = inject(NgbActiveModal);
  private readonly uploadService = inject(UploadService);
  private readonly jobService = inject(JobService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  // Set by the opener via componentInstance.
  job: MyBookingSummary | null = null;

  protected readonly mapsUrl = googleMapsUrl;

  photoUrl = signal<string | null>(null);
  uploading = signal(false);
  submitting = signal(false);
  touched = signal(false);

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.uploading.set(true);
    this.uploadService.upload(file).subscribe({
      next: (url) => {
        this.photoUrl.set(url);
        this.uploading.set(false);
      },
      error: () => {
        this.uploading.set(false);
        this.toastr.error(this.translate.instant('technician.jobs.photoFailed'));
      },
    });
  }

  confirm() {
    this.touched.set(true);
    const url = this.photoUrl();
    if (!url || !this.job) return;

    this.submitting.set(true);
    this.jobService.uploadImage(this.job.id, url, 'After').subscribe({
      next: () => this.activeModal.close(true),
      error: () => this.submitting.set(false),
    });
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
