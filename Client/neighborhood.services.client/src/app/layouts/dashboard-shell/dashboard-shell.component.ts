import { Component, computed, inject, input, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { NotificationBellComponent } from '../../shared/components/notification-bell/notification-bell.component';
import { LanguageSwitcherComponent } from '../../shared/components/language-switcher/language-switcher.component';
import { AuthService } from '../../features/auth/services/auth.service';
import { LayoutService } from '../../core/services/layout.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-dashboard-shell',
  imports: [RouterOutlet, NotificationBellComponent, LanguageSwitcherComponent],
  templateUrl: './dashboard-shell.component.html',
  styleUrl: './dashboard-shell.component.css',
})
export class DashboardShellComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  readonly layout = inject(LayoutService);

  /** Page title shown in the topbar. Each role's layout passes its own.
   *  Named `pageTitle` (not `title`) so it doesn't leak to the host element's
   *  native `title` attribute, which the browser renders as a hover tooltip. */
  pageTitle = input<string>('Dashboard');

  readonly isLoggingOut = signal(false);
  readonly currentUser = this.authService.currentUser;
  readonly userInitial = computed(() => {
    const fullName = this.currentUser()?.fullName?.trim();
    return fullName ? fullName.charAt(0).toUpperCase() : 'A';
  });
  readonly userPhoto = computed(() => {
    const photo = this.currentUser()?.photo ?? '';

    if (!photo || photo.startsWith('http') || photo.startsWith('blob:') || photo.startsWith('data:')) {
      return photo;
    }

    return `${environment.apiUrl}${photo.startsWith('/') ? '' : '/'}${photo}`;
  });

  logout(): void {
    if (this.isLoggingOut()) {
      return;
    }

    this.isLoggingOut.set(true);

    this.authService.logout().subscribe({
      next: () => {
        this.isLoggingOut.set(false);
        this.router.navigate(['/auth/login']);
      },
      error: () => {
        this.isLoggingOut.set(false);
        this.router.navigate(['/auth/login']);
      },
    });
  }
}
