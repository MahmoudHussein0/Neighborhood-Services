import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LayoutService } from '../../../../core/services/layout.service';
import {CustomerSupportService} from '../../../public/services/customer-support.service'
import {TicketDto} from '../../../public/models/ticket-dto';

import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';

@Component({
  selector: 'app-staff-sidebar',
  imports: [RouterLink, RouterLinkActive, TranslatePipe, HasPermissionDirective],
  templateUrl: './staff-sidebar.component.html',
  styleUrl: './staff-sidebar.component.css',
})
export class StaffSidebarComponent {
  readonly layout = inject(LayoutService);
  readonly customerSupportService = inject(CustomerSupportService);
  collapsed = signal(false);

  toggle() {
    this.collapsed.update(v => !v);
  }
}
