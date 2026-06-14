import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LayoutService } from '../../../../core/services/layout.service';

@Component({
  selector: 'app-staff-sidebar',
  imports: [RouterLink, RouterLinkActive, TranslatePipe],
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
