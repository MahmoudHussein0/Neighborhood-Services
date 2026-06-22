import {Component, OnInit, inject, signal, computed} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ReviewsService } from '../../../services/Reviews.service';
import { ReviewAnalysisService } from '../../../services/Review-analysis.service';
import { ReviewDto, ReviewFilters, ReviewStatus } from '../../../models/Review.model';
import { ReviewAnalysisDto } from '../../../models/ReviewAnalysis.model';

@Component({
  selector: 'app-reviewstab',
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './reviewstab.component.html',
  styleUrl: './reviewstab.component.css',
})



export class ReviewsTabComponent implements OnInit {
  private svc = inject(ReviewsService);
  private analysisSvc = inject(ReviewAnalysisService);
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

  // Bulk selection: ids of reviews ticked for a bulk action.
  selectedIds = signal<Set<number>>(new Set<number>());
  bulkProcessing = signal(false);

  // The review whose full details modal is open.
  viewing = signal<ReviewDto | null>(null);
  // AI analyses keyed by reviewId (loaded once; details modal looks the row up here).
  private analysisByReview = signal<Map<number, ReviewAnalysisDto>>(new Map());

  // The AI analysis for the currently-viewed review, if one exists.
  viewingAnalysis = computed(() => {
    const r = this.viewing();
    return r ? this.analysisByReview().get(r.id) ?? null : null;
  });

  filters: ReviewFilters = { search: '', status: '', rating: '', revieweeId: '' };
  // Bumped whenever `filters` is mutated, so the `filtered` computed re-runs
  // (it can't track the plain `filters` object on its own).
  private filterTick = signal(0);

  filtered = computed(() => {
    this.filterTick(); // register dependency so dropdown/search changes invalidate this
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

  selectedCount = computed(() => this.selectedIds().size);

  // True when every row on the current page is selected (drives the header checkbox).
  allPageSelected = computed(() => {
    const page = this.paginated();
    const sel = this.selectedIds();
    return page.length > 0 && page.every(r => sel.has(r.id));
  });

  ngOnInit() {
    this.loadAll();
    this.loadAnalyses();
  }

  // Fail-silent: AI analyses enrich the details modal but must not block the reviews list.
  private loadAnalyses() {
    this.analysisSvc.getAll().subscribe({
      next: data => this.analysisByReview.set(new Map(data.map(a => [a.reviewId, a]))),
      error: () => {}
    });
  }

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
    this.clearSelection();
    this.svc.getFlagged().subscribe({
      next: data => { this.reviews.set(data); this.loading.set(false); },
      error: () => { this.error.set('Failed to load flagged reviews.'); this.loading.set(false); }
    });
  }

  onFilterChange() {
    this.currentPage.set(1);
    this.clearSelection();
    this.filterTick.update(v => v + 1);
  }

  resetFilters() {
    this.filters = { search: '', status: '', rating: '', revieweeId: '' };
    this.clearSelection();
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

  // ── Bulk selection ─────────────────────────────────────────────────────────

  isSelected(id: number): boolean {
    return this.selectedIds().has(id);
  }

  toggleSelect(id: number) {
    this.selectedIds.update(set => {
      const next = new Set(set);
      next.has(id) ? next.delete(id) : next.add(id);
      return next;
    });
  }

  // Select/clear every row on the current page (leaves other pages' selection intact).
  toggleSelectAllPage() {
    const page = this.paginated();
    this.selectedIds.update(set => {
      const next = new Set(set);
      const allSelected = page.every(r => next.has(r.id));
      page.forEach(r => allSelected ? next.delete(r.id) : next.add(r.id));
      return next;
    });
  }

  clearSelection() {
    this.selectedIds.set(new Set<number>());
  }

  // Apply one status to every selected review, in parallel.
  bulkUpdate(status: ReviewStatus) {
    const ids = [...this.selectedIds()];
    if (ids.length === 0) return;

    this.bulkProcessing.set(true);
    forkJoin(ids.map(id => this.svc.update(id, { status }))).subscribe({
      next: updated => {
        this.reviews.update(list =>
          list.map(r => updated.find(u => u.id === r.id) ?? r)
        );
        this.bulkProcessing.set(false);
        this.clearSelection();
        this.toastr.success(this.translate.instant('reviewsTab.bulkDone', {
          count: updated.length,
          status: this.translate.instant('reviewsTab.status_map.' + status)
        }));
      },
      error: () => {
        this.bulkProcessing.set(false);
        this.toastr.error(this.translate.instant('reviewsTab.bulkFail'));
      }
    });
  }

  // ── Details modal ──────────────────────────────────────────────────────────

  openDetails(review: ReviewDto) {
    this.viewing.set(review);
  }

  closeDetails() {
    this.viewing.set(null);
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

  sentimentClass(sentiment: ReviewAnalysisDto['sentiment']): Record<string, boolean> {
    return {
      'bg-success': sentiment === 'Positive',
      'bg-secondary': sentiment === 'Neutral',
      'bg-danger': sentiment === 'Negative',
    };
  }
}

