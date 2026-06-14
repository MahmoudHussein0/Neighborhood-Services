import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LayoutService } from '../../../../core/services/layout.service';

@Component({
  selector: 'app-customer-sidebar',
  imports: [RouterLink, RouterLinkActive, TranslatePipe],
  templateUrl: './customer-sidebar.component.html',
  styleUrl: './customer-sidebar.component.css',
})
export class CustomerSidebarComponent {
  readonly layout = inject(LayoutService);
  collapsed = signal(false);

  toggle() {
    this.collapsed.update(v => !v);
  }
}
