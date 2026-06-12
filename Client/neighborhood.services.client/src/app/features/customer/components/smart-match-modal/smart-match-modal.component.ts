import { Component, OnInit, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { BookingService } from '../../services/booking.service';
import { CatalogService } from '../../../../shared/services/catalog.service';
import { Category, ProblemType } from '../../../../core/models/catalog.model';
import { TechnicianMatchResult } from '../../models/booking.model';
import { TechnicianCard } from '../../../../core/models/technician-card.model';

// Result the modal hands back to Find Technician so it can open the existing Book / Recurring flow.
export interface SmartMatchChoice {
  action: 'book' | 'recurring';
  tech: TechnicianCard;
}

@Component({
  selector: 'app-smart-match-modal',
  imports: [ReactiveFormsModule, TranslatePipe, DecimalPipe],
  templateUrl: './smart-match-modal.component.html',
})
export class SmartMatchModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly activeModal = inject(NgbActiveModal);
  private readonly booking = inject(BookingService);
  private readonly catalog = inject(CatalogService);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  categories = signal<Category[]>([]);
  problemTypes = signal<ProblemType[]>([]);
  matching = signal(false);
  result = signal<TechnicianMatchResult | null>(null);

  // Optional location — improves distance ranking, not required.
  lat = signal<number | null>(null);
  lng = signal<number | null>(null);
  locating = signal(false);

  form = this.fb.group({
    categoryId: this.fb.control<number | null>(null, Validators.required),
    problemTypeId: this.fb.control<number | null>(null, Validators.required),
    description: [''],
  });

  ngOnInit() {
    this.catalog.getCategories().subscribe({ next: (c) => this.categories.set(c) });
  }

  onCategoryChange() {
    this.form.controls.problemTypeId.setValue(null);
    this.problemTypes.set([]);
    this.result.set(null);
    const categoryId = this.form.controls.categoryId.value;
    if (categoryId == null) return;
    this.catalog.getCategory(categoryId).subscribe({ next: (d) => this.problemTypes.set(d.problemTypes) });
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

  findMatch() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    this.matching.set(true);
    this.result.set(null);
    this.booking
      .smartMatch({
        categoryId: v.categoryId!,
        problemTypeId: v.problemTypeId!,
        latitude: this.lat(),
        longitude: this.lng(),
        description: v.description?.trim() ? v.description.trim() : null,
        topN: 2,
      })
      .subscribe({
        next: (r) => {
          this.result.set(r);
          this.matching.set(false);
          if (!r?.matches?.length) this.toastr.info(this.translate.instant('smartMatch.none'));
        },
        error: () => {
          this.matching.set(false);
          this.toastr.error(this.translate.instant('smartMatch.failed'));
        },
      });
  }

  // Hand the chosen technician back to Find Technician, which opens the existing modal.
  book(tech: TechnicianCard) {
    this.activeModal.close({ action: 'book', tech } satisfies SmartMatchChoice);
  }

  recurring(tech: TechnicianCard) {
    this.activeModal.close({ action: 'recurring', tech } satisfies SmartMatchChoice);
  }

  refine() {
    this.result.set(null);
  }

  dismiss() {
    this.activeModal.dismiss();
  }

  displayName(t: TechnicianCard): string {
    return t.fullName?.trim() || '#' + t.id;
  }

  isVerified(t: TechnicianCard): boolean {
    return t.verificationStatus === 'Approved';
  }
}
