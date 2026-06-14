import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { RecurringBookingService } from '../../services/recurring-booking.service';
import { CatalogService } from '../../../../shared/services/catalog.service';
import { Category, ProblemType } from '../../../../core/models/catalog.model';
import { TechnicianCardCategory } from '../../../../core/models/technician-card.model';
import { CreateRecurringBooking, RecurringPattern } from '../../models/recurring-booking.model';

@Component({
  selector: 'app-create-recurring-booking-modal',
  imports: [ReactiveFormsModule, TranslatePipe],
  templateUrl: './create-recurring-booking-modal.component.html',
})
export class CreateRecurringBookingModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly activeModal = inject(NgbActiveModal);
  private readonly service = inject(RecurringBookingService);
  private readonly catalog = inject(CatalogService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  readonly daysOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  readonly patterns: RecurringPattern[] = ['Daily', 'Weekly', 'Monthly'];

  categories = signal<Category[]>([]);
  problemTypes = signal<ProblemType[]>([]);
  submitting = signal(false);

  lat = signal<number | null>(null);
  lng = signal<number | null>(null);
  locating = signal(false);

  // Set once the user tries to submit, so the (required) location error can show inline.
  submitAttempted = signal(false);

  // Earliest selectable date (local "today") for the min attribute — "yyyy-MM-dd".
  readonly minDate = new Date(Date.now() - new Date().getTimezoneOffset() * 60000)
    .toISOString()
    .slice(0, 10);

  form = this.fb.group({
    technicianId: this.fb.control<number | null>(null, Validators.required),
    categoryId: this.fb.control<number | null>(null, Validators.required),
    problemTypeId: this.fb.control<number | null>(null, Validators.required),
    address: ['', Validators.required],
    pattern: this.fb.control<RecurringPattern | null>(null, Validators.required),
    dayOfWeek: this.fb.control<string | null>(null),
    dayOfMonth: this.fb.control<number | null>(null),
    timeOfDay: ['', Validators.required],
    durationMinutes: this.fb.control<number | null>(null, [Validators.required, Validators.min(15)]),
    startDate: ['', Validators.required],
    endDate: [''],
  });

  // Pre-selected technician — set by the opener via componentInstance (from the Find Technician card).
  // The technician is chosen there, so this modal has no technician dropdown.
  technicianName: string | null = null;
  private _technicianId: number | null = null;
  set technicianId(value: number | null) {
    this._technicianId = value;
    this.form.controls.technicianId.setValue(value);
  }
  get technicianId(): number | null {
    return this._technicianId;
  }

  // Only the categories this technician actually works in (passed from the Find Technician card).
  set technicianCategories(cats: TechnicianCardCategory[]) {
    const ar = (this.translate.currentLang || 'en') === 'ar';
    this.categories.set((cats ?? []).map((c) => ({ id: c.id, name: ar ? c.nameAr : c.nameEn, icon: c.icon })));
  }

  onCategoryChange() {
    const categoryId = this.form.controls.categoryId.value;
    this.form.controls.problemTypeId.setValue(null);
    this.problemTypes.set([]);
    if (categoryId == null) return;
    const lang = this.translate.currentLang || 'en';
    this.catalog.getCategory(categoryId, lang).subscribe({ next: (d) => this.problemTypes.set(d.problemTypes) });
  }

  onPatternChange() {
    const pattern = this.form.controls.pattern.value;
    const dow = this.form.controls.dayOfWeek;
    const dom = this.form.controls.dayOfMonth;

    dow.clearValidators();
    dom.clearValidators();
    dow.setValue(null);
    dom.setValue(null);

    if (pattern === 'Weekly') dow.setValidators(Validators.required);
    if (pattern === 'Monthly') dom.setValidators([Validators.required, Validators.min(1), Validators.max(31)]);

    dow.updateValueAndValidity();
    dom.updateValueAndValidity();
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
    this.submitAttempted.set(true);
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    if (this.lat() == null || this.lng() == null) {
      this.toastr.error(this.translate.instant('common.locationRequired'));
      return;
    }

    const v = this.form.getRawValue();
    const payload: CreateRecurringBooking = {
      technicianId: v.technicianId!,
      problemTypeId: v.problemTypeId!,
      address: v.address!,
      latitude: this.lat()!,
      longitude: this.lng()!,
      pattern: v.pattern!,
      dayOfWeek: v.pattern === 'Weekly' ? v.dayOfWeek! : null,
      dayOfMonth: v.pattern === 'Monthly' ? v.dayOfMonth! : null,
      timeOfDay: `${v.timeOfDay}:00`,
      durationMinutes: v.durationMinutes!,
      startDate: v.startDate!,
      endDate: v.endDate ? v.endDate : null,
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
