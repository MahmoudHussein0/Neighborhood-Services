import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { TechnicianService } from '../../../../shared/services/technician.service';
import { TechnicianCard, TechnicianCardCategory } from '../../../../core/models/technician-card.model';
import { CreateRecurringBookingModalComponent } from '../../components/create-recurring-booking-modal/create-recurring-booking-modal.component';
import { CreateBookingModalComponent } from '../../components/create-booking-modal/create-booking-modal.component';
import { SmartMatchModalComponent, SmartMatchChoice } from '../../components/smart-match-modal/smart-match-modal.component';

type SortBy = 'rating' | 'available';

@Component({
  selector: 'app-find-technician',
  imports: [TranslatePipe, DecimalPipe],
  templateUrl: './find-technician.component.html',
})
export class FindTechnicianComponent implements OnInit {
  private readonly service = inject(TechnicianService);
  private readonly modal = inject(NgbModal);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);
  private readonly translate = inject(TranslateService);

  loading = signal(true);
  private readonly all = signal<TechnicianCard[]>([]);

  // filters / sort
  searchTerm = signal('');
  sortBy = signal<SortBy>('rating');
  availableOnly = signal(false);
  verifiedOnly = signal(false);
  selectedCategory = signal<number | null>(null);

  /** Distinct categories present across the loaded technicians (for the filter chips). */
  categories = computed<TechnicianCardCategory[]>(() => {
    const map = new Map<number, TechnicianCardCategory>();
    for (const t of this.all()) {
      for (const c of t.categories) if (!map.has(c.id)) map.set(c.id, c);
    }
    return [...map.values()].sort((a, b) => a.nameEn.localeCompare(b.nameEn));
  });

  /** The filtered + sorted list that the template renders. */
  filtered = computed<TechnicianCard[]>(() => {
    let list = this.all();

    const term = this.searchTerm().trim().toLowerCase();
    if (term) {
      list = list.filter(
        (t) =>
          this.displayName(t).toLowerCase().includes(term) ||
          t.categories.some((c) => c.nameEn.toLowerCase().includes(term) || c.nameAr.includes(term)),
      );
    }
    if (this.availableOnly()) list = list.filter((t) => t.isAvailable);
    if (this.verifiedOnly()) list = list.filter((t) => t.verificationStatus === 'Approved');

    const cat = this.selectedCategory();
    if (cat != null) list = list.filter((t) => t.categories.some((c) => c.id === cat));

    const sort = this.sortBy();
    return [...list].sort((a, b) =>
      sort === 'available'
        ? Number(b.isAvailable) - Number(a.isAvailable) || b.rating - a.rating
        : b.rating - a.rating,
    );
  });

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading.set(true);
    this.service.getForBrowse().subscribe({
      next: (techs) => {
        this.all.set(techs ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.all.set([]);
        this.loading.set(false);
      },
    });
  }

  // --- filter handlers ---

  onSearchInput(value: string) {
    this.searchTerm.set(value);
  }

  setSort(value: SortBy) {
    this.sortBy.set(value);
  }

  toggleAvailable() {
    this.availableOnly.update((v) => !v);
  }

  toggleVerified() {
    this.verifiedOnly.update((v) => !v);
  }

  selectCategory(id: number | null) {
    this.selectedCategory.set(id);
  }

  // --- Smart Match ---

  openSmartMatch() {
    const ref = this.modal.open(SmartMatchModalComponent, { size: 'lg' });
    ref.result.then(
      (choice: SmartMatchChoice) => {
        // The modal returns the chosen technician → open the existing Book / Recurring flow.
        if (choice?.action === 'book') this.bookNow(choice.tech);
        else if (choice?.action === 'recurring') this.setupRecurring(choice.tech);
      },
      () => {}, // dismissed
    );
  }

  // --- CTAs (wired in the next steps: recurring-modal refactor + direct booking) ---

  setupRecurring(tech: TechnicianCard) {
    const ref = this.modal.open(CreateRecurringBookingModalComponent, { size: 'lg' });
    ref.componentInstance.technicianId = tech.id;
    ref.componentInstance.technicianName = this.displayName(tech);
    ref.componentInstance.technicianCategories = tech.categories;
    ref.result.then(
      () => {
        this.toastr.success(this.translate.instant('recurring.created'));
        this.router.navigate(['/customer/recurring-bookings']);
      },
      () => {}, // dismissed — stay on Find Technician
    );
  }

  bookNow(tech: TechnicianCard) {
    const ref = this.modal.open(CreateBookingModalComponent, { size: 'lg' });
    ref.componentInstance.technicianId = tech.id;
    ref.componentInstance.technicianName = this.displayName(tech);
    ref.componentInstance.technicianCategories = tech.categories;
    ref.result.then(
      () => {
        this.toastr.success(this.translate.instant('bookings.requested'));
        this.router.navigate(['/customer/bookings']);
      },
      () => {}, // dismissed — stay on Find Technician
    );
  }

  // --- display helpers ---

  displayName(tech: TechnicianCard): string {
    return tech.fullName?.trim() || this.translate.instant('findTech.unnamed', { id: tech.id });
  }

  initials(tech: TechnicianCard): string {
    const name = tech.fullName?.trim();
    if (!name) return '#' + tech.id;
    return name
      .split(/\s+/)
      .slice(0, 2)
      .map((p) => p[0])
      .join('')
      .toUpperCase();
  }

  isVerified(tech: TechnicianCard): boolean {
    return tech.verificationStatus === 'Approved';
  }

  /** Category label in the active language (falls back to English). */
  categoryName(c: TechnicianCardCategory): string {
    return (this.translate.currentLang || 'en') === 'ar' ? c.nameAr || c.nameEn : c.nameEn;
  }
}
