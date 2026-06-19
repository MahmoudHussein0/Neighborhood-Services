import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { BookingService } from '../../services/booking.service';
import { CatalogService } from '../../../../shared/services/catalog.service';
import { TechnicianService } from '../../../../shared/services/technician.service';
import { UploadService } from '../../../../shared/services/upload.service';
import { Category, ProblemType } from '../../../../core/models/catalog.model';
import { TechnicianAvailabilitySlot } from '../../../../core/models/technician-availability.model';
import { TechnicianCardCategory } from '../../../../core/models/technician-card.model';
import { CreateBooking, TechnicianPricingRange } from '../../models/booking.model';

@Component({
  selector: 'app-create-booking-modal',
  imports: [ReactiveFormsModule, TranslatePipe],
  templateUrl: './create-booking-modal.component.html',
  styles: [`
    .slot-grid {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
      max-height: 12rem;
      overflow-y: auto;
      padding: 0.25rem;
      margin: -0.25rem;
    }
    .slot-chip {
      font-size: 0.85rem;
      font-weight: 500;
      padding: 0.4rem 0.85rem;
      border-radius: 0.6rem;
      border: 1px solid #e2e8f0;
      background: #fff;
      color: #334155;
      cursor: pointer;
      transition: border-color 0.12s ease, background-color 0.12s ease, color 0.12s ease;
      white-space: nowrap;
    }
    .slot-chip:hover {
      border-color: #93c5fd;
      background: #eff6ff;
      color: #1d4ed8;
    }
    .slot-chip.selected {
      background: linear-gradient(135deg, #3b82f6, #2563eb);
      border-color: #2563eb;
      color: #fff;
      box-shadow: 0 2px 8px rgba(37, 99, 235, 0.3);
    }
    .slots-state {
      font-size: 0.85rem;
      color: #64748b;
      padding: 0.5rem 0;
    }
  `],
})
export class CreateBookingModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly activeModal = inject(NgbActiveModal);
  private readonly service = inject(BookingService);
  private readonly catalog = inject(CatalogService);
  private readonly tech = inject(TechnicianService);
  private readonly uploadService = inject(UploadService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  private readonly DAY_NAMES = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

  categories = signal<Category[]>([]);
  problemTypes = signal<ProblemType[]>([]);
  availability = signal<TechnicianAvailabilitySlot[]>([]);
  submitting = signal(false);

  estimatedPrice = signal<number | null>(null);
  estimating = signal(false);

  // Tech's own MinPrice/MaxPrice for the currently selected problem type — null if
  // the tech hasn't priced this problem type yet.
  techPricing = signal<TechnicianPricingRange | null>(null);
  techPricingLoaded = signal(false);

  lat = signal<number | null>(null);
  lng = signal<number | null>(null);
  locating = signal(false);

  // Optional "Before" photo of the problem — uploaded to Cloudinary, sent with the booking.
  beforeImageUrl = signal<string | null>(null);
  uploadingPhoto = signal(false);

  // Set once the user tries to submit, so the (required) location error can show inline.
  submitAttempted = signal(false);

  // Slot granularity for the time picker (minutes between bookable start-times).
  readonly SLOT_MINUTES = 30;

  // Earliest selectable DAY (local "today") for the date input's min — "yyyy-MM-dd".
  readonly minDate = new Date(Date.now() - new Date().getTimezoneOffset() * 60000)
    .toISOString()
    .slice(0, 10);

  // The chosen day, the free start-times for it, and a loading flag for the slots fetch.
  selectedDate = signal<string>('');
  availableSlots = signal<string[]>([]);
  loadingSlots = signal(false);

  form = this.fb.group({
    categoryId: this.fb.control<number | null>(null, Validators.required),
    problemTypeId: this.fb.control<number | null>(null, Validators.required),
    scheduledAt: ['', Validators.required],
    description: ['', Validators.required],
    address: ['', Validators.required],
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

  // Only the categories this technician actually works in (passed from the Find Technician card),
  // so the customer can't pick a category the technician doesn't serve.
  set technicianCategories(cats: TechnicianCardCategory[]) {
    const ar = (this.translate.currentLang || 'en') === 'ar';
    this.categories.set((cats ?? []).map((c) => ({ id: c.id, name: ar ? c.nameAr : c.nameEn, icon: c.icon })));
  }

  onCategoryChange() {
    this.form.controls.problemTypeId.setValue(null);
    this.problemTypes.set([]);
    this.estimatedPrice.set(null);
    this.techPricing.set(null);
    this.techPricingLoaded.set(false);
    const categoryId = this.form.controls.categoryId.value;
    if (categoryId == null) return;
    const lang = this.translate.currentLang || 'en';
    this.catalog.getCategory(categoryId, lang).subscribe({ next: (d) => this.problemTypes.set(d.problemTypes) });
  }

  onProblemTypeChange() {
    this.estimatedPrice.set(null); // stale once the problem type changes
    this.techPricing.set(null);
    this.techPricingLoaded.set(false);

    const problemTypeId = this.form.controls.problemTypeId.value;
    if (problemTypeId == null || !this._technicianId) return;

    this.service.getTechPricingRange(this._technicianId, problemTypeId).subscribe({
      next: (range) => {
        this.techPricing.set(range);
        this.techPricingLoaded.set(true);
      },
      error: () => this.techPricingLoaded.set(true),
    });
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

  /** Day changed → clear any time picked for the old day and load this day's free slots. */
  onDateChange(value: string) {
    this.selectedDate.set(value);
    this.form.controls.scheduledAt.setValue('');
    this.availableSlots.set([]);
    if (!value || !this._technicianId) return;

    this.loadingSlots.set(true);
    this.tech.getAvailableSlots(this._technicianId, value, this.SLOT_MINUTES).subscribe({
      next: (slots) => {
        this.availableSlots.set(slots ?? []);
        this.loadingSlots.set(false);
      },
      error: () => {
        this.availableSlots.set([]);
        this.loadingSlots.set(false);
      },
    });
  }

  /** Picking a slot stores it as a wall-clock "yyyy-MM-ddTHH:mm" (submit appends ":00"). */
  selectSlot(iso: string) {
    this.form.controls.scheduledAt.setValue(iso.slice(0, 16));
  }

  isSelectedSlot(iso: string): boolean {
    return this.form.controls.scheduledAt.value === iso.slice(0, 16);
  }

  /** "2026-06-22T09:30:00" → "9:30 AM" (wall-clock, no timezone math). */
  slotLabel(iso: string): string {
    const [h, m] = iso.slice(11, 16).split(':').map(Number);
    const ampm = h >= 12 ? 'PM' : 'AM';
    const hour12 = h % 12 || 12;
    return `${hour12}:${String(m).padStart(2, '0')} ${ampm}`;
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

  onPhotoSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.uploadingPhoto.set(true);
    this.uploadService.upload(file).subscribe({
      next: (url) => {
        this.beforeImageUrl.set(url);
        this.uploadingPhoto.set(false);
      },
      error: () => {
        this.uploadingPhoto.set(false);
        this.toastr.error(this.translate.instant('bookings.bookNow.photoFailed'));
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
      },
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
    const payload: CreateBooking = {
      technicianId: this._technicianId,
      problemTypeId: v.problemTypeId!,
      description: v.description!,
      address: v.address!,
      latitude: this.lat()!,
      longitude: this.lng()!,
      scheduledAt: `${v.scheduledAt}:00`,
      region: null,
      beforeImageUrl: this.beforeImageUrl(),
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
