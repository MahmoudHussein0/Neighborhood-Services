import { Component } from '@angular/core';
import { DashboardShellComponent } from '../dashboard-shell/dashboard-shell.component';
import { CustomerSidebarComponent } from '../../features/customer/components/customer-sidebar/customer-sidebar.component';

@Component({
  selector: 'app-customer-layout',
  imports: [DashboardShellComponent, CustomerSidebarComponent],
  template: `
    <app-dashboard-shell pageTitle="Customer Dashboard">
      <app-customer-sidebar sidebar />
    </app-dashboard-shell>
  `,
})
export class CustomerLayoutComponent {}
