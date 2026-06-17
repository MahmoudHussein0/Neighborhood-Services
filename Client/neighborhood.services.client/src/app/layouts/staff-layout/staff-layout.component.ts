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
// Permissions are loaded by permissionGuard (canActivateChild) before any staff route
// activates, so the sidebar's *appHasPermission checks have data to read on first paint.
export class StaffLayoutComponent {}
