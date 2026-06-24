import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LayoutService } from '../../../../core/services/layout.service';
import { LogoComponent } from '../../../../shared/components/logo/logo.component';

@Component({
  selector: 'app-customer-sidebar',
  imports: [RouterLink, RouterLinkActive, TranslatePipe, LogoComponent],
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
