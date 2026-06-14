import {Component, OnInit, inject, signal, computed} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ReviewsService } from '../../../services/Reviews.service';
import { ReviewDto, ReviewFilters, ReviewStatus } from '../../../models/Review.model';

@Component({
  selector: 'app-reviewstab',
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './reviewstab.component.html',
  styleUrl: './reviewstab.component.css',
})



export class ReviewsTabComponent implements OnInit {
  private svc = inject(ReviewsService);
  private toastr = inject(ToastrService);
  private translate = inject(TranslateService);

  reviews = signal<ReviewDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  currentPage = signal(1);
  readonly perPage = 8;

  // The review pending delete confirmation (drives the in-app modal; replaces native confirm()).
  pendingDelete = signal<ReviewDto | null>(null);
  deleting = signal(false);

  filters: ReviewFilters = { search: '', status: '', rating: '', revieweeId: '' };

  filtered = computed(() => {
    const { search, status, rating } = this.filters;
    return this.reviews().filter(r => {
      if (search && !r.comment.toLowerCase().includes(search.toLowerCase())) return false;
      if (status && r.status !== status) return false;
      if (rating !== '' && r.rating !== +rating) return false;
      return true;
    });
  });

  totalPages = computed(() => Math.max(1, Math.ceil(this.filtered().length / this.perPage)));

  pages = computed(() =>
    Array.from({ length: this.totalPages() }, (_, i) => i + 1)
  );

  paginated = computed(() => {
    const start = (this.currentPage() - 1) * this.perPage;
    return this.filtered().slice(start, start + this.perPage);
  });

  ngOnInit() { this.loadAll(); }

  loadAll() {
    this.loading.set(true);
    this.error.set(null);
    this.svc.getAll().subscribe({
      next: data => { this.reviews.set(data); this.loading.set(false); },
      error: () => { this.error.set('Failed to load reviews.'); this.loading.set(false); }
    });
  }

  loadFlagged() {
    this.loading.set(true);
    this.svc.getFlagged().subscribe({
      next: data => { this.reviews.set(data); this.loading.set(false); },
      error: () => { this.error.set('Failed to load flagged reviews.'); this.loading.set(false); }
    });
  }

  onFilterChange() { this.currentPage.set(1); }

  resetFilters() {
    this.filters = { search: '', status: '', rating: '', revieweeId: '' };
    this.loadAll();
  }

  goPage(p: number) {
    if (p >= 1 && p <= this.totalPages()) this.currentPage.set(p);
  }

  updateStatus(review: ReviewDto, status: ReviewStatus) {
    this.svc.update(review.id, { status }).subscribe({
      next: updated => {
        this.reviews.update(list =>
          list.map(r => r.id === updated.id ? updated : r)
        );
        this.toastr.success(this.translate.instant('reviewsTab.statusUpdated', {
          id: updated.id,
          status: this.translate.instant('reviewsTab.status_map.' + status)
        }));
      },
      error: () => this.toastr.error(this.translate.instant('reviewsTab.statusFail'))
    });
  }

  // ── Delete with in-app confirmation modal ──────────────────────────────────

  askDelete(review: ReviewDto) {
    this.pendingDelete.set(review);
  }

  cancelDelete() {
    this.pendingDelete.set(null);
  }

  confirmDelete() {
    const review = this.pendingDelete();
    if (!review) return;

    this.deleting.set(true);
    this.svc.delete(review.id).subscribe({
      next: () => {
        this.reviews.update(list => list.filter(r => r.id !== review.id));
        this.deleting.set(false);
        this.pendingDelete.set(null);
        this.toastr.success(this.translate.instant('reviewsTab.deleted', { id: review.id }));
      },
      error: () => {
        this.deleting.set(false);
        this.toastr.error(this.translate.instant('reviewsTab.deleteFail'));
      }
    });
  }

  starsOf(n: number): string { return '★'.repeat(n); }
  emptyStars(n: number): string { return '★'.repeat(5 - n); }

  statusClass(status: ReviewStatus): Record<string, boolean> {
    return {
      'bg-warning text-dark': status === 'Pending',
      'bg-success': status === 'Approved',
      'bg-danger': status === 'Rejected',
      'bg-secondary': status === 'Flagged',
    };
  }
}

