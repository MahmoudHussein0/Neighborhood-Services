import { Component, computed, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { Router, RouterLink } from "@angular/router";
import { AuthService } from '../../../../auth/services/auth.service';

@Component({
  selector: 'hero-section',
  imports: [TranslatePipe, RouterLink],
  templateUrl: './hero-section.component.html',
  styleUrl: './hero-section.component.css',
})
export class HeroSectionComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  /** Show "Book Now" to guests and customers; hide it for technicians/staff. */
  readonly showBookNow = computed(() => {
    const user = this.auth.currentUser();
    return !user || user.role.trim().toLowerCase() === 'customer';
  });

  /** Guests go to login; logged-in customers go straight to Find Technician. */
  bookNow(): void {
    if (this.auth.isAuthenticated()) {
      this.router.navigate(['/customer/find-technician']);
    } else {
      this.router.navigate(['/auth/login']);
    }
  }
}
