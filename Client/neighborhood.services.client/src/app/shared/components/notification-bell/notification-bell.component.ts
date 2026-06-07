import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-notification-bell',
  imports: [],
  templateUrl: './notification-bell.component.html',
  styleUrl: './notification-bell.component.css',
})
export class NotificationBellComponent {
  // TODO (Arwa): replace with real unread count from the notifications service.
  unreadCount = signal(3);

  // TODO (Arwa): open a dropdown listing notifications, mark-as-read, etc.
  open() {
    // placeholder
  }
}
