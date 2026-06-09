import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { PublicLayoutComponent } from './layouts/public-layout/public-layout.component';
import { CustomerLayoutComponent } from './layouts/customer-layout/customer-layout.component';
import { TechnicianLayoutComponent } from './layouts/technician-layout/technician-layout.component';
import { StaffLayoutComponent } from './layouts/staff-layout/staff-layout.component';

import { HomeComponent } from './features/public/pages/home/home.component';
import { ServicesComponent } from './features/public/pages/services/services.component';
import { AboutComponent } from './features/public/pages/about/about.component';
import { ContactComponent } from './features/public/pages/contact/contact.component';
import { LoginComponent } from './features/auth/pages/login/login.component';
import { RegisterComponent } from './features/auth/pages/register/register.component';
import { CustomerDashboardComponent } from './features/customer/pages/dashboard/customer-dashboard.component';
import { CustomerPlaceholderComponent } from './features/customer/pages/customer-placeholder/customer-placeholder.component';
import { CustomerWalletComponent } from './features/customer/pages/wallet/customer-wallet.component';
import { TechnicianDashboardComponent } from './features/technician/pages/dashboard/technician-dashboard.component';
import { TechnicianWalletComponent } from './features/technician/pages/wallet/technician-wallet.component';
import { TechnicianEarningsComponent } from './features/technician/pages/earnings/technician-earnings.component';
import { StaffDashboardComponent } from './features/staff/pages/dashboard/staff-dashboard.component';
import { StaffPromoCodesComponent } from './features/staff/pages/promo-codes/staff-promo-codes.component';

export const routes: Routes = [
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      { path: '', component: HomeComponent },
      { path: 'services', component: ServicesComponent },
      { path: 'about', component: AboutComponent },
      { path: 'contact', component: ContactComponent },
      { path: 'auth/login', component: LoginComponent },
      { path: 'auth/register', component: RegisterComponent },
    ],
  },
  {
    path: 'customer',
    component: CustomerLayoutComponent,
    canActivate: [authGuard, roleGuard],
    canActivateChild: [authGuard, roleGuard],
    data: { roles: ['Customer'] },
    children: [
      { path: '', component: CustomerDashboardComponent },
      { path: 'bookings', component: CustomerPlaceholderComponent, data: { title: 'My Bookings' } },
      { path: 'service-requests', component: CustomerPlaceholderComponent, data: { title: 'Service Requests' } },
      { path: 'find-technician', component: CustomerPlaceholderComponent, data: { title: 'Find Technician' } },
      { path: 'recurring-bookings', component: CustomerPlaceholderComponent, data: { title: 'Recurring Bookings' } },
      { path: 'favorites', component: CustomerPlaceholderComponent, data: { title: 'Favorites' } },
      { path: 'wallet', component: CustomerPlaceholderComponent, data: { title: 'Wallet' } },
      { path: 'chat', component: CustomerPlaceholderComponent, data: { title: 'Chat' } },
      { path: 'chats', redirectTo: 'chat', pathMatch: 'full' },
      { path: 'notifications', component: CustomerPlaceholderComponent, data: { title: 'Notifications' } },
      { path: 'profile', component: CustomerPlaceholderComponent, data: { title: 'Profile' } },
      { path: '**', redirectTo: '' },
      { path: 'wallet', component: CustomerWalletComponent },
    ],
  },
  {
    path: 'technician',
    component: TechnicianLayoutComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Technician'] },
    children: [
      { path: '', component: TechnicianDashboardComponent },
      { path: 'wallet', component: TechnicianWalletComponent },
      { path: 'earnings', component: TechnicianEarningsComponent },
    ],
  },
  {
    path: 'staff',
    component: StaffLayoutComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Staff', 'Admin', 'TechnicalSupport'] },
    children: [
      { path: '', component: StaffDashboardComponent },
      { path: 'promo-codes', component: StaffPromoCodesComponent },
    ],
  },
  { path: '**', redirectTo: '' },
];
