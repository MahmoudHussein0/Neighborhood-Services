import { Component } from '@angular/core';
import { DashboardShellComponent } from '../dashboard-shell/dashboard-shell.component';
import { StaffSidebarComponent } from '../../features/staff/components/staff-sidebar/staff-sidebar.component';

@Component({
  selector: 'app-staff-layout',
  imports: [DashboardShellComponent, StaffSidebarComponent],
  template: `
    <app-dashboard-shell pageTitle="Staff Dashboard">
      <app-staff-sidebar sidebar />
    </app-dashboard-shell>
  `,
})
export class StaffLayoutComponent {}
