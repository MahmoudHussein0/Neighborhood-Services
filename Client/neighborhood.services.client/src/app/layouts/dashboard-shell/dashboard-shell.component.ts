import { Component, input } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotificationBellComponent } from '../../shared/components/notification-bell/notification-bell.component';

@Component({
  selector: 'app-dashboard-shell',
  imports: [RouterOutlet, NotificationBellComponent],
  templateUrl: './dashboard-shell.component.html',
  styleUrl: './dashboard-shell.component.css',
})
export class DashboardShellComponent {
  /** Page title shown in the topbar. Each role's layout passes its own. */
  title = input<string>('Dashboard');
}
