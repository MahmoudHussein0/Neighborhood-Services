import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { guestGuard } from './core/guards/guest.guard';
import { permissionGuard } from './core/guards/permission.guard';
import { PublicLayoutComponent } from './layouts/public-layout/public-layout.component';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';
import { CustomerLayoutComponent } from './layouts/customer-layout/customer-layout.component';
import { TechnicianLayoutComponent } from './layouts/technician-layout/technician-layout.component';
import { StaffLayoutComponent } from './layouts/staff-layout/staff-layout.component';
import { HomeComponent } from './features/public/pages/home/home.component';
import { ServicesComponent } from './features/public/pages/services/services.component';
import { AboutComponent } from './features/public/pages/about/about.component';
import { ContactComponent } from './features/public/pages/contact/contact.component';
import { LoginComponent } from './features/auth/pages/login/login.component';
import { RegisterComponent } from './features/auth/pages/register/register.component';
import { ExternalCallbackComponent } from './features/auth/pages/external-callback/external-callback.component';
import { CustomerDashboardComponent } from './features/customer/pages/dashboard/customer-dashboard.component';
import { BookingsComponent } from './features/customer/pages/bookings/bookings.component';
import { ServiceRequestsComponent } from './features/customer/pages/service-requests/service-requests.component';
import { ServiceRequestDetailsComponent } from './features/customer/pages/service-requests/service-request-details.component';
import { RecurringBookingsComponent } from './features/customer/pages/recurring-bookings/recurring-bookings.component';
import { FindTechnicianComponent } from './features/customer/pages/find-technician/find-technician.component';
import { CustomerPlaceholderComponent } from './features/customer/pages/customer-placeholder/customer-placeholder.component';
import { CustomerProfileComponent } from './features/customer/pages/profile/customer-profile.component';
import { CustomerWalletComponent } from './features/customer/pages/wallet/customer-wallet.component';
import { TechnicianDashboardComponent } from './features/technician/pages/dashboard/technician-dashboard.component';
import { TechnicianJobsComponent } from './features/technician/pages/jobs/technician-jobs.component';
import { TechnicianOffersComponent } from './features/technician/pages/offers/technician-offers.component';
import { TechnicianBrowseRequestsComponent } from './features/technician/pages/browse-requests/technician-browse-requests.component';
import { TechnicianRecurringJobsComponent } from './features/technician/pages/recurring-jobs/technician-recurring-jobs.component';
import { StaffDashboardComponent } from './features/staff/pages/dashboard/staff-dashboard.component';
import { FlaggedRequestsComponent } from './features/staff/pages/flagged-requests/flagged-requests.component';
import { StaffBookingsComponent } from './features/staff/pages/bookings/staff-bookings.component';
import { TechnicianProfileComponent } from './features/technician/pages/profile/technician-profile.component';
import { TechnicianGalleryComponent } from './features/technician/pages/gallery/technician-gallery.component';
import { TechnicianWalletComponent } from './features/technician/pages/wallet/technician-wallet.component';
import { TechnicianEarningsComponent } from './features/technician/pages/earnings/technician-earnings.component';

import { StaffUsersComponent } from './features/staff/pages/users/staff-users.component';
import { StaffPromoCodesComponent } from './features/staff/pages/promo-codes/staff-promo-codes.component';

import { CategoryComponent } from './features/staff/pages/categories/category/category.component';
import { CategoryDetailsComponent } from './features/staff/pages/categories/category-details/category-details.component';
import { AvailiabilityAndExceptionComponent } from './features/technician/pages/availabilities/availiability-and-exception/availiability-and-exception.component';
import { PricingComponent } from './features/technician/pages/pricing/pricing.component';
import { ProblemTypeComponent } from './features/public/pages/services/problem-type/problem-type.component';
import { PoliciesComponent } from './features/staff/pages/policies/policies.component';
import { TechReviewsComponent } from './features/technician/pages/reviews/tech-review';
import { PublicProfileComponent } from './shared/components/public-profile/public-profile.component';
import { categoriesResolver } from './core/resolvers/categories-resolver';
import {FavoriteListComponent} from '../app/features/customer/pages/favorite-list/favorite-list.component';
import {CustomerChatsComponent} from '../app/features/customer/pages/customer-chats/customer-chats.component'
import{NewsletterpublishingComponent} from'../app/features/staff/pages/newsletter/newsletterpublishing/newsletterpublishing.component'

export const routes: Routes = [
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      { path: '', component: HomeComponent, resolve: { "categories": categoriesResolver } },
      { path: 'services', component: ServicesComponent, resolve: { "categories": categoriesResolver } },
      { path: 'problemType/:id', component: ProblemTypeComponent },
      { path: 'about', component: AboutComponent },
      { path: 'contact', component: ContactComponent },
    ],
  },

  {
    path: 'auth',
    component: AuthLayoutComponent,
    children: [
      { path: 'login', component: LoginComponent, canActivate: [guestGuard] },
      { path: 'register', component: RegisterComponent, canActivate: [guestGuard] },
      { path: 'external-callback', component: ExternalCallbackComponent },
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
      { path: 'bookings', component: BookingsComponent },
      { path: 'service-requests', component: ServiceRequestsComponent },
      { path: 'service-requests/:id', component: ServiceRequestDetailsComponent },
      { path: 'find-technician', component: FindTechnicianComponent },
      { path: 'technician/:id', component: PublicProfileComponent, data: { role: 'technician' } },
      { path: 'recurring-bookings', component: RecurringBookingsComponent },
      { path: 'favorites', component: FavoriteListComponent, data: { title: 'Favorites' } },
      { path: 'wallet', component: CustomerWalletComponent },
      { path: 'chat', component:CustomerChatsComponent, data: { title: 'Chat' } },
      { path: 'chats', redirectTo: 'chat', pathMatch: 'full' },
      { path: 'notifications', component: CustomerPlaceholderComponent, data: { title: 'Notifications' } },
      { path: 'profile', component: CustomerProfileComponent },
      { path: '**', redirectTo: '' }
    ],
  },


  {
    path: 'technician',
    component: TechnicianLayoutComponent,
    canActivate: [authGuard, roleGuard],
    canActivateChild: [authGuard, roleGuard],
    data: { roles: ['Technician'] },
    children: [
      { path: '', component: TechnicianDashboardComponent },
      { path: 'jobs', component: TechnicianJobsComponent },
      { path: 'browse-requests', component: TechnicianBrowseRequestsComponent },
      { path: 'offers', component: TechnicianOffersComponent },
      { path: 'recurring-jobs', component: TechnicianRecurringJobsComponent },
      { path: 'customer/:id', component: PublicProfileComponent, data: { role: 'customer' } },
      { path: 'profile', component: TechnicianProfileComponent },
      { path: 'gallery', component: TechnicianGalleryComponent },
      { path: 'wallet', component: TechnicianWalletComponent },
      { path: 'earnings', component: TechnicianEarningsComponent },
      { path: 'availability', component: AvailiabilityAndExceptionComponent },
      { path: 'reviews', component: TechReviewsComponent },
      { path: 'pricing', component: PricingComponent },
      { path: 'chat', component: CustomerChatsComponent, data: { title: 'Chat' } },
    ]
  },

  {
    path: 'staff',
    component: StaffLayoutComponent,
    canActivate: [authGuard, roleGuard],
    canActivateChild: [authGuard, roleGuard, permissionGuard],
    data: { roles: ['Staff', 'Admin', 'TechnicalSupport'] },
    children: [
      // Overview has no permission requirement — every staffer can see it.
      { path: '', component: StaffDashboardComponent },
      { path: 'bookings', component: StaffBookingsComponent, data: { permission: 'ManageBookings' } },
      { path: 'flagged-requests', component: FlaggedRequestsComponent, data: { permission: 'ManageFlagedReq' } },
      { path: 'users', component: StaffUsersComponent, data: { permission: 'ManageUsers' } },
      { path: 'categories', component: CategoryComponent, title: 'Staff Categories', data: { permission: 'ManageCategories' } },
      { path: 'details/:categoryId', component: CategoryDetailsComponent, title: 'Category Details ', data: { permission: 'ManageCategories' } },
      { path: 'policies', component: PoliciesComponent, title: 'Staff Policies', data: { permission: 'ManagePolicies' } },
      { path: 'promo-codes', component: StaffPromoCodesComponent, data: { permission: 'ManagePromos' } },
      {
        path: 'staff-management',
        data: { permission: 'FullAccess' },
        loadComponent: () =>
          import('./features/staff/pages/staff-management/staff-management.component')
            .then(m => m.StaffManagementComponent)
      },
      {
        path: 'support-tickets',
        data: { permission: 'ManageTickets' },
        loadComponent: () =>
          import('./features/staff/pages/support-tickets/support-tickets.component')
            .then(m => m.SupportTicketsComponent)
      },
      {
        path: 'disputes',
        data: { permission: 'ManageDisputes' },
        loadComponent: () =>
          import('./features/staff/pages/disputes/disputes.component')
            .then(m => m.DisputesComponent)
      },
      {
        path: 'reviews',
        data: { permission: 'MangeReviews' },
        loadComponent: () =>
          import('./features/staff/pages/reviews/reviews.component')
            .then(m => m.ReviewsComponent)
      },
      // Newsletters has no matching PermissionType — left open to all staff.
      {path:'newsletters',component:NewsletterpublishingComponent},
      { path: '**', redirectTo: '' }
    ],
  },

  { path: '**', redirectTo: '' },
]
