import { Component, OnInit, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { BookingService } from '../../services/booking.service';
import { CatalogService } from '../../../../shared/services/catalog.service';
import { TechnicianService } from '../../../../shared/services/technician.service';
import { Category, ProblemType } from '../../../../core/models/catalog.model';
import { TechnicianAvailabilitySlot } from '../../../../core/models/technician-availability.model';
import { CreateBooking } from '../../models/booking.model';

@Component({
  selector: 'app-create-booking-modal',
  imports: [ReactiveFormsModule, TranslatePipe],
  templateUrl: './create-booking-modal.component.html',
})
export class CreateBookingModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly activeModal = inject(NgbActiveModal);
  private readonly service = inject(BookingService);
  private readonly catalog = inject(CatalogService);
  private readonly tech = inject(TechnicianService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  private readonly DAY_NAMES = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

  categories = signal<Category[]>([]);
  problemTypes = signal<ProblemType[]>([]);
  availability = signal<TechnicianAvailabilitySlot[]>([]);
  submitting = signal(false);

  estimatedPrice = signal<number | null>(null);
  estimating = signal(false);

  lat = signal<number | null>(null);
  lng = signal<number | null>(null);
  locating = signal(false);

  // Earliest selectable datetime (local "now") for the min attribute — "yyyy-MM-ddTHH:mm".
  readonly minDateTime = new Date(Date.now() - new Date().getTimezoneOffset() * 60000)
    .toISOString()
    .slice(0, 16);

  form = this.fb.group({
    categoryId: this.fb.control<number | null>(null, Validators.required),
    problemTypeId: this.fb.control<number | null>(null, Validators.required),
    scheduledAt: ['', Validators.required],
    description: ['', Validators.required],
    address: ['', Validators.required],
    promoCode: [''], // UI only for now — wiring TBD
  });

  // Pre-selected technician — set by the opener via componentInstance (from the Find Technician card).
  technicianName: string | null = null;
  private _technicianId = 0;
  set technicianId(value: number) {
    this._technicianId = value;
    if (value) {
      this.tech.getAvailability(value).subscribe({
        next: (a) => this.availability.set(a ?? []),
        error: () => this.availability.set([]),
      });
    }
  }
  get technicianId(): number {
    return this._technicianId;
  }

  ngOnInit() {
    this.catalog.getCategories().subscribe({ next: (c) => this.categories.set(c) });
  }

  onCategoryChange() {
    this.form.controls.problemTypeId.setValue(null);
    this.problemTypes.set([]);
    this.estimatedPrice.set(null);
    const categoryId = this.form.controls.categoryId.value;
    if (categoryId == null) return;
    this.catalog.getCategory(categoryId).subscribe({ next: (d) => this.problemTypes.set(d.problemTypes) });
  }

  onProblemTypeChange() {
    this.estimatedPrice.set(null); // stale once the problem type changes
  }

  getEstimate() {
    const problemTypeId = this.form.controls.problemTypeId.value;
    if (problemTypeId == null) return;
    this.estimating.set(true);
    this.service.estimate(problemTypeId).subscribe({
      next: (r) => {
        this.estimatedPrice.set(r.estimatedPrice);
        this.estimating.set(false);
      },
      error: () => this.estimating.set(false),
    });
  }

  /** Informational warning if the chosen slot is outside the technician's working days/hours. */
  availabilityWarning(): string | null {
    const dt = this.form.controls.scheduledAt.value;
    const avail = this.availability();
    if (!dt || avail.length === 0) return null;

    const d = new Date(dt);
    const slot = avail.find((a) => a.dayOfWeek === this.DAY_NAMES[d.getDay()]);
    if (!slot) return this.translate.instant('bookings.bookNow.notWorkingDay');

    const time = `${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:00`;
    if (time < slot.startTime || time > slot.endTime) return this.translate.instant('bookings.bookNow.outsideHours');
    return null;
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
      },
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
    const payload: CreateBooking = {
      technicianId: this._technicianId,
      problemTypeId: v.problemTypeId!,
      description: v.description!,
      address: v.address!,
      latitude: this.lat()!,
      longitude: this.lng()!,
      scheduledAt: `${v.scheduledAt}:00`,
      region: null,
      promoCodeId: null, // TODO: resolve v.promoCode → id once wiring is decided
    };

    this.submitting.set(true);
    this.service.create(payload).subscribe({
      next: (res) => {
        this.submitting.set(false);
        this.activeModal.close(res.id);
      },
      error: () => this.submitting.set(false), // error interceptor surfaces the backend message
    });
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
