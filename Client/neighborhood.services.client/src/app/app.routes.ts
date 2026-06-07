import { Routes } from '@angular/router';

import { PublicLayoutComponent } from './layouts/public-layout/public-layout.component';
import { CustomerLayoutComponent } from './layouts/customer-layout/customer-layout.component';
import { TechnicianLayoutComponent } from './layouts/technician-layout/technician-layout.component';
import { StaffLayoutComponent } from './layouts/staff-layout/staff-layout.component';

import { HomeComponent } from './features/public/pages/home/home.component';
import { ServicesComponent } from './features/public/pages/services/services.component';
import { AboutComponent } from './features/public/pages/about/about.component';
import { ContactComponent } from './features/public/pages/contact/contact.component';
import { CustomerDashboardComponent } from './features/customer/pages/dashboard/customer-dashboard.component';
import { TechnicianDashboardComponent } from './features/technician/pages/dashboard/technician-dashboard.component';
import { StaffDashboardComponent } from './features/staff/pages/dashboard/staff-dashboard.component';

export const routes: Routes = [
  // PUBLIC (navbar + footer)
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      { path: '', component: HomeComponent },
      { path: 'services', component: ServicesComponent },
      { path: 'about', component: AboutComponent },
      { path: 'contact', component: ContactComponent },
    ],
  },

  // CUSTOMER dashboard
  {
    path: 'customer',
    component: CustomerLayoutComponent,
    // canActivate: [authGuard, roleGuard], data: { role: 'Customer' }  ← add when auth exists
    children: [
      { path: '', component: CustomerDashboardComponent },
    ],
  },

  // TECHNICIAN dashboard
  {
    path: 'technician',
    component: TechnicianLayoutComponent,
    children: [
      { path: '', component: TechnicianDashboardComponent },
    ],
  },

  // STAFF dashboard
  {
    path: 'staff',
    component: StaffLayoutComponent,
    children: [
      { path: '', component: StaffDashboardComponent },
    ],
  },

  // Unknown URL → home
  { path: '**', redirectTo: '' },
];
