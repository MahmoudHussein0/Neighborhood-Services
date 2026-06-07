import { Component } from '@angular/core';
import { DashboardShellComponent } from '../dashboard-shell/dashboard-shell.component';
import { TechnicianSidebarComponent } from '../../features/technician/components/technician-sidebar/technician-sidebar.component';

@Component({
  selector: 'app-technician-layout',
  imports: [DashboardShellComponent, TechnicianSidebarComponent],
  template: `
    <app-dashboard-shell title="Technician Dashboard">
      <app-technician-sidebar sidebar />
    </app-dashboard-shell>
  `,
})
export class TechnicianLayoutComponent {}
