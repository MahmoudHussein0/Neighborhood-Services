import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { LayoutService } from '../../../../core/services/layout.service';

@Component({
  selector: 'app-staff-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './staff-sidebar.component.html',
  styleUrl: './staff-sidebar.component.css',
})
export class StaffSidebarComponent {
  readonly layout = inject(LayoutService);
  collapsed = signal(false);

  toggle() {
    this.collapsed.update(v => !v);
  }
}
