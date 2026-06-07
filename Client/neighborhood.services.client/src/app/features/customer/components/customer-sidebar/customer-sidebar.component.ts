import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-customer-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './customer-sidebar.component.html',
  styleUrl: './customer-sidebar.component.css',
})
export class CustomerSidebarComponent {
  collapsed = signal(false);

  toggle() {
    this.collapsed.update(v => !v);
  }
}
