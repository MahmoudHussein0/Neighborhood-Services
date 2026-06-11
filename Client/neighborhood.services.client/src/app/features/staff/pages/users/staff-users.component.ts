import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../auth/services/auth.service';
import { StaffUserDetails, StaffUserSummary } from '../../models/staff-user.model';
import { StaffUsersService } from '../../services/staff-users.service';

@Component({
  selector: 'app-staff-users',
  imports: [FormsModule],
  template: `
    <section class="bg-white border rounded-3 shadow-sm p-4">
      <div class="d-flex flex-column flex-xl-row justify-content-between gap-3 mb-4">
        <div>
          <h2 class="h4 fw-bold mb-1">Users</h2>
          <p class="text-muted mb-0">View and manage application users.</p>
        </div>

        <div class="d-flex flex-column flex-md-row gap-2">
          <input
            class="form-control"
            type="search"
            placeholder="Search name or email"
            [ngModel]="searchTerm()"
            (ngModelChange)="searchTerm.set($event)"
          />
          <select class="form-select" [ngModel]="roleFilter()" (ngModelChange)="changeRoleFilter($event)">
            <option value="">All roles</option>
            <option value="Customer">Customer</option>
            <option value="Technician">Technician</option>
            <option value="Staff">Staff</option>
          </select>
        </div>
      </div>

      @if (error()) {
        <div class="alert alert-danger">{{ error() }}</div>
      }
      @if (success()) {
        <div class="alert alert-success">{{ success() }}</div>
      }

      @if (loading()) {
        <div class="text-muted">Loading users...</div>
      } @else if (filteredUsers().length === 0) {
        <div class="text-muted">No users found.</div>
      } @else {
        <div class="table-responsive">
          <table class="table align-middle">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Role</th>
                <th>Status</th>
                <th class="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (user of filteredUsers(); track user.id) {
                <tr>
                  <td>
                    <div class="d-flex align-items-center gap-2">
                      <div class="staff-avatar">{{ getInitial(user) }}</div>
                      <span class="fw-semibold">{{ user.fullName }}</span>
                    </div>
                  </td>
                  <td>{{ user.email }}</td>
                  <td><span class="badge text-bg-light border">{{ user.applicationUserRole }}</span></td>
                  <td>
                    <span class="badge" [class.text-bg-success]="user.isActive" [class.text-bg-secondary]="!user.isActive">
                      {{ user.isActive ? 'Active' : 'Inactive' }}
                    </span>
                  </td>
                  <td>
                    <div class="d-flex justify-content-end gap-2 flex-wrap">
                      <button class="btn btn-sm btn-outline-primary" type="button" (click)="viewDetails(user)">Details</button>
                      @if (canManageUsers()) {
                        <button class="btn btn-sm btn-outline-success" type="button" (click)="activate(user)" [disabled]="user.isActive">Activate</button>
                        <button class="btn btn-sm btn-outline-warning" type="button" (click)="deactivate(user)" [disabled]="!user.isActive">Deactivate</button>
                        <button class="btn btn-sm btn-outline-danger" type="button" (click)="deleteUser(user)">Delete</button>
                      }
                    </div>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }
    </section>

    @if (selectedUser()) {
      <section class="bg-white border rounded-3 shadow-sm p-4 mt-4">
        <div class="d-flex justify-content-between gap-3 mb-3">
          <h3 class="h5 fw-bold mb-0">User details</h3>
          <button class="btn btn-sm btn-outline-secondary" type="button" (click)="selectedUser.set(null)">Close</button>
        </div>
        @if (detailsLoading()) {
          <div class="text-muted">Loading details...</div>
        } @else {
          <div class="row g-3">
            <div class="col-12 col-md-6">
              <div class="text-muted small">Full name</div>
              <div class="fw-semibold">{{ selectedUser()?.fullName }}</div>
            </div>
            <div class="col-12 col-md-6">
              <div class="text-muted small">Email</div>
              <div class="fw-semibold">{{ selectedUser()?.email }}</div>
            </div>
            <div class="col-12 col-md-4">
              <div class="text-muted small">Age</div>
              <div class="fw-semibold">{{ selectedUser()?.age ?? 'N/A' }}</div>
            </div>
            <div class="col-12 col-md-4">
              <div class="text-muted small">Role</div>
              <div class="fw-semibold">{{ selectedUser()?.applicationUserRole }}</div>
            </div>
            <div class="col-12 col-md-4">
              <div class="text-muted small">Status</div>
              <div class="fw-semibold">{{ selectedUser()?.isActive ? 'Active' : 'Inactive' }}</div>
            </div>
            <div class="col-12">
              <div class="text-muted small">Photo</div>
              <div class="fw-semibold text-break">{{ selectedUser()?.photo || 'No photo set' }}</div>
            </div>
          </div>
        }
      </section>
    }
  `,
  styles: [`
    .staff-avatar {
      align-items: center;
      background-color: #dbeafe;
      border-radius: 50%;
      color: #1d4ed8;
      display: flex;
      font-size: 0.75rem;
      font-weight: 700;
      height: 2rem;
      justify-content: center;
      width: 2rem;
    }
  `],
})
export class StaffUsersComponent implements OnInit {
  private readonly staffUsersService = inject(StaffUsersService);
  private readonly authService = inject(AuthService);

  readonly loading = signal(true);
  readonly detailsLoading = signal(false);
  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);
  readonly users = signal<StaffUserSummary[]>([]);
  readonly selectedUser = signal<StaffUserDetails | null>(null);
  readonly roleFilter = signal('');
  readonly searchTerm = signal('');
  readonly canManageUsers = computed(() => {
    const role = this.authService.currentUser()?.role.toLowerCase();
    return role === 'staff' || role === 'admin';
  });
  readonly filteredUsers = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();

    if (!term) {
      return this.users();
    }

    return this.users().filter((user) =>
      user.fullName.toLowerCase().includes(term) ||
      user.email.toLowerCase().includes(term),
    );
  });

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading.set(true);
    this.error.set(null);
    const role = this.roleFilter();
    const request$ = role
      ? this.staffUsersService.getUsersByRole(role)
      : this.staffUsersService.getUsers();

    request$.subscribe({
      next: (users) => {
        this.users.set(users);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load users.');
        this.loading.set(false);
      },
    });
  }

  changeRoleFilter(role: string): void {
    this.roleFilter.set(role);
    this.loadUsers();
  }

  viewDetails(user: StaffUserSummary): void {
    this.detailsLoading.set(true);
    this.selectedUser.set({ ...user, age: 0 });

    this.staffUsersService.getUser(user.id).subscribe({
      next: (details) => {
        this.selectedUser.set(details);
        this.detailsLoading.set(false);
      },
      error: () => {
        this.error.set('Unable to load user details.');
        this.detailsLoading.set(false);
      },
    });
  }

  activate(user: StaffUserSummary): void {
    this.staffUsersService.activateUser(user.id).subscribe({
      next: () => this.afterUserAction('User activated.'),
      error: () => this.error.set('Unable to activate user.'),
    });
  }

  deactivate(user: StaffUserSummary): void {
    if (!confirm('Deactivate this user?')) {
      return;
    }

    this.staffUsersService.deactivateUser(user.id).subscribe({
      next: () => this.afterUserAction('User deactivated.'),
      error: () => this.error.set('Unable to deactivate user.'),
    });
  }

  deleteUser(user: StaffUserSummary): void {
    if (!confirm('Delete this user?')) {
      return;
    }

    this.staffUsersService.deleteUser(user.id).subscribe({
      next: () => this.afterUserAction('User deleted.'),
      error: () => this.error.set('Unable to delete user.'),
    });
  }

  getInitial(user: StaffUserSummary): string {
    return user.fullName.trim().charAt(0).toUpperCase() || 'U';
  }

  private afterUserAction(message: string): void {
    this.success.set(message);
    this.selectedUser.set(null);
    this.loadUsers();
  }
}
