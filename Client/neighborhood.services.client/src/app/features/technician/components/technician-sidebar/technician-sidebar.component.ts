import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-technician-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './technician-sidebar.component.html',
  styleUrl: './technician-sidebar.component.css',
})
export class TechnicianSidebarComponent {
  collapsed = signal(false);

  toggle() {
    this.collapsed.update(v => !v);
  }
}
