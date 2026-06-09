import { Component, computed, inject, input, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { NotificationBellComponent } from '../../shared/components/notification-bell/notification-bell.component';
import { AuthService } from '../../features/auth/services/auth.service';

@Component({
  selector: 'app-dashboard-shell',
  imports: [RouterOutlet, NotificationBellComponent],
  templateUrl: './dashboard-shell.component.html',
  styleUrl: './dashboard-shell.component.css',
})
export class DashboardShellComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  /** Page title shown in the topbar. Each role's layout passes its own. */
  title = input<string>('Dashboard');

  readonly isLoggingOut = signal(false);
  readonly currentUser = this.authService.currentUser;
  readonly userInitial = computed(() => {
    const fullName = this.currentUser()?.fullName?.trim();
    return fullName ? fullName.charAt(0).toUpperCase() : 'A';
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
