import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { ServiceRequestService } from '../../services/service-request.service';
import { CatalogService } from '../../../../shared/services/catalog.service';
import { UploadService } from '../../../../shared/services/upload.service';
import { Category, ProblemType } from '../../../../core/models/catalog.model';
import { CreateServiceRequest } from '../../models/service-request.model';

@Component({
  selector: 'app-create-service-request-modal',
  imports: [ReactiveFormsModule, TranslatePipe],
  templateUrl: './create-service-request-modal.component.html',
})
export class CreateServiceRequestModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly activeModal = inject(NgbActiveModal);
  private readonly service = inject(ServiceRequestService);
  private readonly catalog = inject(CatalogService);
  private readonly uploadService = inject(UploadService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  categories = signal<Category[]>([]);
  problemTypes = signal<ProblemType[]>([]);
  submitting = signal(false);
  uploading = signal(false);

  // Location (captured via the browser)
  lat = signal<number | null>(null);
  lng = signal<number | null>(null);
  locating = signal(false);

  form = this.fb.group({
    categoryId: this.fb.control<number | null>(null, Validators.required),
    problemTypeId: this.fb.control<number | null>(null, Validators.required),
    description: ['', [Validators.required, Validators.maxLength(1000)]],
    address: ['', Validators.required],
    budget: this.fb.control<number | null>(null, [Validators.required, Validators.min(1)]),
    scheduledAt: ['', Validators.required],
    image: [''],
  });

  // Price hint for the currently selected problem type
  selectedProblemType = computed(() => {
    const id = this.form.controls.problemTypeId.value;
    return this.problemTypes().find((p) => p.id === id) ?? null;
  });

  ngOnInit() {
    this.catalog.getCategories().subscribe({
      next: (c) => this.categories.set(c),
    });
  }

  onCategoryChange() {
    const categoryId = this.form.controls.categoryId.value;
    this.form.controls.problemTypeId.setValue(null);
    this.problemTypes.set([]);
    if (categoryId == null) return;

    this.catalog.getCategory(categoryId).subscribe({
      next: (details) => this.problemTypes.set(details.problemTypes),
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.uploading.set(true);
    this.uploadService.upload(file).subscribe({
      next: (url) => {
        this.form.controls.image.setValue(url);
        this.uploading.set(false);
        this.toastr.success(this.translate.instant('serviceRequests.form.imageUploaded'));
      },
      error: () => {
        this.uploading.set(false);
        this.toastr.error(this.translate.instant('serviceRequests.form.imageFailed'));
      },
    });
  }

  useMyLocation() {
    if (!navigator.geolocation) {
      this.toastr.error(this.translate.instant('common.geoUnsupported'));
      return;
    }
    this.locating.set(true);
    navigator.geolocation.getCurrentPosition(
      (pos) => {
        this.lat.set(pos.coords.latitude);
        this.lng.set(pos.coords.longitude);
        this.locating.set(false);
        this.toastr.success(this.translate.instant('common.locationCaptured'));
      },
      () => {
        this.locating.set(false);
        this.toastr.error(this.translate.instant('common.locationFailed'));
      }
    );
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    if (this.lat() == null || this.lng() == null) {
      this.toastr.error(this.translate.instant('common.locationRequired'));
      return;
    }

    const v = this.form.getRawValue();
    const payload: CreateServiceRequest = {
      categoryId: v.categoryId!,
      problemTypeId: v.problemTypeId!,
      description: v.description!,
      address: v.address!,
      budget: v.budget!,
      image: v.image?.trim() ? v.image.trim() : null,
      scheduledAt: new Date(v.scheduledAt!).toISOString(),
      latitude: this.lat()!,
      longitude: this.lng()!,
    };

    this.submitting.set(true);
    this.service.create(payload).subscribe({
      next: (res) => {
        this.submitting.set(false);
        this.activeModal.close(res.id);
      },
      error: () => this.submitting.set(false),
    });
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
