import {Component, OnInit, inject, signal, computed} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReviewsService } from '../../../services/Reviews.service';
import { ReviewDto, ReviewFilters, ReviewStatus } from '../../../models/Review.model';

@Component({
  selector: 'app-reviewstab',
  imports: [CommonModule, FormsModule],
  templateUrl: './reviewstab.component.html',
  styleUrl: './reviewstab.component.css',
})



export class ReviewsTabComponent implements OnInit {
  private svc = inject(ReviewsService);

  reviews = signal<ReviewDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  currentPage = signal(1);
  readonly perPage = 8;

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
      },
      error: () => alert('Failed to update status.')
    });
  }

  deleteReview(review: ReviewDto) {
    if (!confirm(`Delete review #${review.id}?`)) return;
    this.svc.delete(review.id).subscribe({
      next: () => this.reviews.update(list => list.filter(r => r.id !== review.id)),
      error: () => alert('Failed to delete review.')
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

