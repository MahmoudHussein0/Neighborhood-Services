import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

// Shared by both the customer bookings page and the technician jobs page.
// The opener doesn't tell us who's reviewing whom — the backend derives that
// from the auth token + booking. We just collect rating + comment.
@Component({
  selector: 'app-leave-review-modal',
  imports: [FormsModule, TranslatePipe],
  templateUrl: './leave-review-modal.component.html',
})
export class LeaveReviewModalComponent {
  private readonly activeModal = inject(NgbActiveModal);

  readonly stars = [1, 2, 3, 4, 5];
  rating = signal<number>(5);
  hoverRating = signal<number>(0);
  comment = signal('');

  setRating(n: number) { this.rating.set(n); }
  setHover(n: number) { this.hoverRating.set(n); }
  clearHover() { this.hoverRating.set(0); }

  submit() {
    const text = this.comment().trim();
    if (!text || this.rating() < 1 || this.rating() > 5) return;
    this.activeModal.close({ rating: this.rating(), comment: text });
  }

  dismiss() { this.activeModal.dismiss(); }
}
