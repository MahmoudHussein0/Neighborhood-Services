# Frontend Feature Requirements

## Scope

This document defines the frontend work required for the following Angular features:

1. `customer/profile` — customer profile + addresses
2. `technician/profile` — technician profile + photos/portfolio
3. `staff/users` — staff user management

The project uses:

- Angular frontend inside `Client/`
- ASP.NET Core backend inside `Provider/`
- JWT stored in an HttpOnly cookie named `access_token`
- Safe user data stored on the frontend after login
- Role-based navigation/guards
- Backend API endpoints exposed through controllers such as `UsersController`, `TechniciansController`, `CustomerAddressesController`, and `TechnicianPhotosController`

---

# General Frontend Rules

## Authentication

The JWT token must **not** be stored in `localStorage` or `sessionStorage`.

The backend stores the JWT in an HttpOnly cookie. Angular cannot read this cookie directly. Angular should only store safe user info such as:

```ts
{
  userId: string;
  fullName: string;
  email: string;
  role: string;
  expiresAt: string;
}
```

All authenticated API calls must use:

```ts
{ withCredentials: true }
```

## Routing

Use Angular router navigation, not raw anchor reloads.

Correct:

```html
<a routerLink="/customer/profile">Profile</a>
```

Avoid:

```html
<a href="/customer/profile">Profile</a>
```

## API Base URL

Use `environment.apiUrl`.

Example:

```ts
`${environment.apiUrl}/Users/${id}`
```

## UI Consistency

Before creating new UI, inspect and reuse existing:

- layouts
- colors
- buttons
- cards
- forms
- shared components
- validation styles
- loading/error states

Do not create a completely different design style for these pages.

---

# 1. Customer Profile

## Route

```text
/customer/profile
```

## Purpose

This page allows the logged-in customer to view and update their personal profile and manage their saved addresses.

## Expected UI Sections

### A. Profile Information

Display customer/user data:

- Full name
- Email
- Age
- Photo
- Role
- Active status if needed
- Current location if available

Possible actions:

- Update full name
- Update age
- Update photo
- Update location if supported

### B. Customer Addresses

Display a list of customer addresses.

Each address card/table row should show:

- Label
- Address text
- Latitude
- Longitude
- Default address flag
- Actions

Actions:

- Add address
- Edit address
- Delete address
- Set as default address

---

## Backend Data Source

Use the existing backend routes from:

- `UsersController`
- `CustomerAddressesController`
- optionally `GeocodingController`

The exact routes must be inspected from the backend controllers before implementation.

Expected route mapping is likely similar to:

```text
GET    /api/Users/{id}
PUT    /api/Users/{id}/profile
PUT    /api/Users/{id}/photo
PUT    /api/Users/{id}/location

GET    /api/CustomerAddresses/customer/{customerId}
POST   /api/CustomerAddresses
PUT    /api/CustomerAddresses/{id}
DELETE /api/CustomerAddresses/{id}
PATCH  /api/CustomerAddresses/{id}/default
```

Do not invent routes. Inspect the actual controllers.

---

## Angular Structure

Recommended structure:

```text
src/app/features/customer/profile/
├── pages/
│   └── customer-profile-page/
├── components/
│   ├── customer-profile-form/
│   ├── customer-address-list/
│   └── customer-address-form/
├── models/
│   ├── customer-profile.model.ts
│   └── customer-address.model.ts
└── services/
    └── customer-profile.service.ts
```

If the project already has a different customer feature structure, follow the existing structure instead.

---

## Suggested Angular Models

```ts
export interface CustomerProfile {
  id: string;
  fullName: string;
  email: string;
  age: number;
  photo: string;
  role: string;
  latitude?: number;
  longitude?: number;
}
```

```ts
export interface CustomerAddress {
  id: number | string;
  customerId?: number | string;
  label: string;
  address: string;
  latitude: number;
  longitude: number;
  isDefault: boolean;
}
```

---

## Required Behavior

When the page loads:

1. Read current safe user data from `AuthService`.
2. Load profile details.
3. Load customer addresses.
4. Show loading state.
5. Show error message if API fails.

When updating profile:

1. Validate form.
2. Call update profile endpoint.
3. Refresh displayed data.
4. Update stored safe user data if full name changed.

When adding address:

1. Validate address fields.
2. If geocoding exists, optionally convert address to coordinates.
3. Submit address to backend.
4. Refresh address list.

When setting default address:

1. Call backend endpoint.
2. Refresh address list.
3. Ensure only one address appears as default.

---

## Permissions

Only authenticated customers should access:

```text
/customer/profile
```

Allowed roles:

```text
Customer
```

Admin/Staff access depends on the business decision. Do not assume unless already defined.

---

## Acceptance Criteria

- Customer can open `/customer/profile` after login.
- Page does not redirect to login during normal navigation.
- Profile data is displayed correctly.
- Addresses are displayed correctly.
- Customer can add/edit/delete address if endpoints exist.
- Customer can set default address if backend supports it.
- All API calls use `withCredentials: true`.
- No JWT token is stored in frontend storage.

---

# 2. Technician Profile

## Route

```text
/technician/profile
```

## Purpose

This page allows the logged-in technician to view and update their profile and manage portfolio/work photos.

---

## Expected UI Sections

### A. Technician Profile Information

Display:

- Full name
- Email
- Age
- Photo
- Location
- Technician-specific fields
- Bio/description if available
- Skills/services if available
- Rating if available
- Availability if available

The technician profile may need data from both:

- `ApplicationUser`
- `Technician`

If `TechnicianDetailsDTO` does not include name, photo, email, or location, the backend should map those fields from `ApplicationUser`.

### B. Portfolio / Photos

Display technician photos/work samples.

Each photo card should show:

- Image
- Caption/description if available
- Created date if available
- Actions

Actions:

- Add/upload photo
- Edit caption/details if supported
- Delete photo

---

## Backend Data Source

Use routes from:

- `TechniciansController`
- `TechnicianPhotosController`
- optionally `UsersController`

Expected route mapping is likely similar to:

```text
GET    /api/Technicians/{id}
PUT    /api/Technicians/{id}

GET    /api/TechnicianPhotos/technician/{technicianId}
POST   /api/TechnicianPhotos
PUT    /api/TechnicianPhotos/{id}
DELETE /api/TechnicianPhotos/{id}
```

Do not invent routes. Inspect actual controllers first.

---

## Angular Structure

Recommended structure:

```text
src/app/features/technician/profile/
├── pages/
│   └── technician-profile-page/
├── components/
│   ├── technician-profile-form/
│   ├── technician-portfolio-grid/
│   └── technician-photo-form/
├── models/
│   ├── technician-profile.model.ts
│   └── technician-photo.model.ts
└── services/
    └── technician-profile.service.ts
```

Follow existing frontend architecture if it differs.

---

## Suggested Angular Models

```ts
export interface TechnicianProfile {
  id: number | string;
  userId?: string;
  fullName: string;
  email: string;
  photo: string;
  latitude?: number;
  longitude?: number;
  role?: string;
  bio?: string;
  rating?: number;
  isActive?: boolean;
}
```

```ts
export interface TechnicianPhoto {
  id: number | string;
  technicianId: number | string;
  photoUrl: string;
  caption?: string;
  createdAt?: string;
}
```

---

## Required Behavior

When the page loads:

1. Read current safe user data from `AuthService`.
2. Load technician details.
3. Load technician photos.
4. Render profile and portfolio.

When updating profile:

1. Validate form.
2. Call update technician/profile endpoint.
3. Refresh profile data.

When adding photo:

1. If backend supports upload, use the existing file upload/cloud service endpoint.
2. If backend expects a URL only, submit `photoUrl` as a string.
3. Refresh portfolio list.

When deleting photo:

1. Confirm deletion.
2. Call delete endpoint.
3. Refresh portfolio list.

---

## Permissions

Only authenticated technicians should access:

```text
/technician/profile
```

Allowed roles:

```text
Technician
```

Admin/Staff access to technician details should be handled separately in staff/admin pages.

---

## Acceptance Criteria

- Technician can open `/technician/profile` after login.
- Page does not redirect to login during normal navigation.
- Profile data is displayed correctly.
- Name, email, photo, and location are shown if backend returns them.
- Portfolio photos are displayed.
- Technician can add/delete/update photos if backend endpoints support it.
- All API calls use `withCredentials: true`.
- No JWT token is stored in frontend storage.

---

# 3. Staff Users Management

## Route

```text
/staff/users
```

## Purpose

This page allows staff/admin users to manage application users.

This is an admin/staff management page, not a normal user profile page.

---

## Expected UI Sections

### A. Users Table

Display all users.

Columns:

- Full name
- Email
- Role
- Active status
- Deleted status if needed
- Created at if available
- Actions

Actions:

- View user details
- Activate user
- Deactivate user
- Soft delete user
- Filter by role
- Search by name/email if supported

### B. Filters

Possible filters:

- Role
- Active/inactive
- Search text

### C. User Details Modal/Page

Show selected user details:

- Full name
- Email
- Age
- Photo
- Role
- Status
- Location if available

---

## Backend Data Source

Use routes from:

- `UsersController`

Expected route mapping is likely similar to:

```text
GET    /api/Users
GET    /api/Users/{id}
GET    /api/Users/role/{role}
PATCH  /api/Users/{id}/activate
PATCH  /api/Users/{id}/deactivate
DELETE /api/Users/{id}
```

Do not invent routes. Inspect actual controller first.

---

## Angular Structure

Recommended structure:

```text
src/app/features/staff/users/
├── pages/
│   └── staff-users-page/
├── components/
│   ├── users-table/
│   ├── user-details-modal/
│   └── users-filter-bar/
├── models/
│   └── staff-user.model.ts
└── services/
    └── staff-users.service.ts
```

Follow existing staff feature structure if different.

---

## Suggested Angular Models

```ts
export interface StaffUserSummary {
  id: string;
  fullName: string;
  email: string;
  photo?: string;
  role: string;
  isActive: boolean;
  isDeleted?: boolean;
}
```

```ts
export interface StaffUserDetails extends StaffUserSummary {
  age?: number;
  latitude?: number;
  longitude?: number;
  createdAt?: string;
  updatedAt?: string;
}
```

---

## Required Behavior

When the page loads:

1. Confirm user has a staff/admin role using frontend guard.
2. Fetch users list.
3. Render users table.
4. Show loading and error states.

When filtering by role:

1. Use backend role query endpoint if available.
2. Otherwise filter locally only if the full list is already loaded.

When activating/deactivating user:

1. Confirm action if destructive.
2. Call backend endpoint.
3. Refresh table.

When deleting user:

1. Confirm deletion.
2. Call delete endpoint.
3. Refresh table.
4. Prefer soft delete if backend uses it.

---

## Permissions

Only staff/admin users should access:

```text
/staff/users
```

Allowed roles depend on the project enum.

Common allowed roles:

```text
Admin
TechnicalSupport
Staff
```

If `TechnicalSupport` should only view users and not modify them, split UI permissions:

```text
Admin:
- View users
- Activate/deactivate users
- Delete users

TechnicalSupport:
- View users
- View user details
- No delete
- No activate/deactivate unless approved
```

Do not assume final permissions without the team permission matrix.

---

## Acceptance Criteria

- Staff user can open `/staff/users` after login.
- Unauthorized roles cannot access `/staff/users`.
- Users table loads from backend.
- Staff/Admin can view user details.
- Activate/deactivate/delete buttons appear only for allowed roles.
- Actions refresh the users table after success.
- All API calls use `withCredentials: true`.
- No JWT token is stored in frontend storage.

---

# Shared Implementation Notes

## Services Should Be Thin

Angular services should only call APIs and return observables.

Example:

```ts
getUsers() {
  return this.http.get<StaffUserSummary[]>(`${this.apiUrl}/Users`, {
    withCredentials: true
  });
}
```

Business decisions should stay in components or dedicated state services, not mixed randomly in API services.

---

## Components Should Handle UI State

Each page should handle:

- loading
- error
- empty state
- success message
- form validation

---

## Error Handling

For every API call:

- Show readable error message.
- Do not expose raw stack traces.
- Handle `401 Unauthorized` by redirecting to login.
- Handle `403 Forbidden` by showing access denied.
- Handle `404 Not Found` with a friendly message.

---

## Recommended Testing Checklist

### Customer Profile

- Login as Customer.
- Navigate to `/customer/profile`.
- Refresh page.
- Update profile.
- Add address.
- Set default address.
- Delete address.
- Logout.
- Try opening `/customer/profile` again.

### Technician Profile

- Login as Technician.
- Navigate to `/technician/profile`.
- Refresh page.
- Update profile.
- Add portfolio photo.
- Delete portfolio photo.
- Logout.
- Try opening `/technician/profile` again.

### Staff Users

- Login as Staff/Admin.
- Navigate to `/staff/users`.
- Load all users.
- Filter by role.
- View user details.
- Activate/deactivate user.
- Try accessing `/staff/users` as Customer and verify it is blocked.

---

# Codex Implementation Prompt

Use the following prompt when asking Codex to implement these features.

```text
Implement the following Angular frontend features based on the existing project structure and backend routes:

1. customer/profile — profile + addresses
2. technician/profile — profile + photos/portfolio
3. staff/users — user management

Repository structure:
- Client/ contains Angular frontend
- Provider/ contains ASP.NET Core backend

Important:
- First inspect existing frontend layouts, routes, guards, interceptors, services, models, shared components, colors, and styling.
- Then inspect backend controllers and exact API routes.
- Do not invent backend routes.
- Do not modify backend unless absolutely required for a compile-breaking mismatch.
- Use existing Angular architecture and style.
- JWT is stored in HttpOnly cookie named access_token.
- Do not store JWT in localStorage/sessionStorage.
- Use withCredentials: true for all authenticated API calls.
- Use stored safe user info only for frontend guards and UI state.

Feature 1: customer/profile
- Show customer profile information.
- Show customer addresses.
- Allow add/edit/delete address if backend endpoints exist.
- Allow set default address if backend endpoint exists.
- Use CustomerAddressesController and UsersController routes.

Feature 2: technician/profile
- Show technician profile information.
- Show technician portfolio/photos.
- Allow add/edit/delete photo if backend endpoints exist.
- Use TechniciansController and TechnicianPhotosController routes.
- Ensure name, photo, email, and location are displayed if backend DTO returns them.

Feature 3: staff/users
- Show users table.
- Allow staff/admin to view users.
- Allow activate/deactivate/delete only if role permissions allow.
- Use UsersController routes.
- Respect existing role/permission model.

Routing:
- /customer/profile
- /technician/profile
- /staff/users

After implementation:
- List files created.
- List files modified.
- List backend endpoints used.
- Explain auth/cookie handling.
- Explain route guard behavior.
- Explain how each page works.
- Confirm build passes.
```

---

# Final Notes

These three pages are important because they connect the core role-based experience:

- Customer manages personal data and addresses.
- Technician manages public work profile and portfolio.
- Staff/Admin manages users and operational access.

The main rule is: keep the frontend aligned with the backend contracts. Do not guess route names or DTO shapes. Always inspect the existing controllers and response models before implementation.
