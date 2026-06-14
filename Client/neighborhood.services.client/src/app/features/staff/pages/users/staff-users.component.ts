import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ConfirmModalComponent } from '../../../../shared/components/confirm-modal/confirm-modal.component';
import { AuthService } from '../../../auth/services/auth.service';
import { environment } from '../../../../environments/environment';
import { StaffUserDetails, StaffUserSummary } from '../../models/staff-user.model';
import { StaffUsersService } from '../../services/staff-users.service';

type StaffUserConfirmAction = 'deactivate' | 'delete';

@Component({
  selector: 'app-staff-users',
  imports: [FormsModule, ConfirmModalComponent, TranslatePipe],
  template: `
    <section class="bg-white border rounded-3 shadow-sm p-4">
      <div class="d-flex flex-column flex-xl-row justify-content-between gap-3 mb-4">
        <div>
          <h2 class="h4 fw-bold mb-1">{{ 'staffUsers.title' | translate }}</h2>
          <p class="text-muted mb-0">{{ 'staffUsers.subtitle' | translate }}</p>
        </div>

        <div class="d-flex flex-column flex-md-row gap-2">
          <input
            class="form-control"
            type="search"
            [placeholder]="'staffUsers.searchPlaceholder' | translate"
            [ngModel]="searchTerm()"
            (ngModelChange)="searchTerm.set($event)"
          />
          <select class="form-select" [ngModel]="roleFilter()" (ngModelChange)="changeRoleFilter($event)">
            <option value="">{{ 'staffUsers.allRoles' | translate }}</option>
            <option value="Customer">{{ 'staffUsers.roles.Customer' | translate }}</option>
            <option value="Technician">{{ 'staffUsers.roles.Technician' | translate }}</option>
            <option value="Staff">{{ 'staffUsers.roles.Staff' | translate }}</option>
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
        <div class="text-muted">{{ 'staffUsers.loading' | translate }}</div>
      } @else if (filteredUsers().length === 0) {
        <div class="text-muted">{{ 'staffUsers.empty' | translate }}</div>
      } @else {
        <div class="table-responsive">
          <table class="table align-middle">
            <thead>
              <tr>
                <th>{{ 'staffUsers.name' | translate }}</th>
                <th>{{ 'staffUsers.email' | translate }}</th>
                <th>{{ 'staffUsers.role' | translate }}</th>
                <th>{{ 'staffUsers.status' | translate }}</th>
                <th class="text-end">{{ 'staffUsers.actions' | translate }}</th>
              </tr>
            </thead>
            <tbody>
              @for (user of filteredUsers(); track user.id) {
                <tr>
                  <td>
                    <div class="d-flex align-items-center gap-2">
                      @if (user.photo) {
                        <div class="staff-avatar">
                          <img [src]="getPhotoSrc(user.photo)" alt="User photo" />
                        </div>
                      } @else {
                        <div class="staff-avatar">{{ getInitial(user) }}</div>
                      }
                      <span class="fw-semibold">{{ user.fullName }}</span>
                    </div>
                  </td>
                  <td>{{ user.email }}</td>
                  <td><span class="badge text-bg-light border">{{ ('staffUsers.roles.' + user.applicationUserRole) | translate }}</span></td>
                  <td>
                    <span class="badge" [class.text-bg-success]="user.isActive" [class.text-bg-secondary]="!user.isActive">
                      {{ (user.isActive ? 'staffUsers.active' : 'staffUsers.inactive') | translate }}
                    </span>
                  </td>
                  <td>
                    <div class="d-flex justify-content-end gap-2 flex-wrap">
                      <button class="btn btn-sm btn-outline-primary" type="button" (click)="viewDetails(user)">{{ 'staffUsers.details' | translate }}</button>
                      @if (canManageUsers()) {
                        <button class="btn btn-sm btn-outline-success" type="button" (click)="activate(user)" [disabled]="user.isActive">{{ 'staffUsers.activate' | translate }}</button>
                        <button class="btn btn-sm btn-outline-warning" type="button" (click)="deactivate(user)" [disabled]="!user.isActive">{{ 'staffUsers.deactivate' | translate }}</button>
                        <button class="btn btn-sm btn-outline-danger" type="button" (click)="deleteUser(user)">{{ 'staffUsers.delete' | translate }}</button>
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

    @if (selectedUser(); as user) {
      <div class="modal fade show d-block" tabindex="-1" role="dialog" aria-modal="true" aria-labelledby="userDetailsTitle">
        <div class="modal-dialog modal-dialog-centered modal-lg">
          <div class="modal-content border-0 shadow-lg">
            <div class="modal-header border-0 pb-0">
              <div class="d-flex align-items-center gap-3">
                <div class="staff-avatar-lg">
                  @if (user.photo) {
                    <img [src]="getPhotoSrc(user.photo)" alt="User photo" />
                  } @else {
                    {{ getInitial(user) }}
                  }
                </div>
                <div>
                  <h3 class="modal-title h5 fw-bold mb-1" id="userDetailsTitle">{{ user.fullName }}</h3>
                  <div class="d-flex flex-wrap gap-2">
                    <span class="badge text-bg-light border">{{ ('staffUsers.roles.' + user.applicationUserRole) | translate }}</span>
                    <span class="badge" [class.text-bg-success]="user.isActive" [class.text-bg-secondary]="!user.isActive">
                      {{ (user.isActive ? 'staffUsers.active' : 'staffUsers.inactive') | translate }}
                    </span>
                  </div>
                </div>
              </div>
              <button class="btn-close" type="button" aria-label="Close" (click)="closeDetails()"></button>
            </div>
            <div class="modal-body">
              @if (detailsLoading()) {
                <div class="d-flex align-items-center gap-2 text-muted py-4">
                  <span class="spinner-border spinner-border-sm" aria-hidden="true"></span>
                  {{ 'staffUsers.loadingDetails' | translate }}
                </div>
              } @else {
                <div class="row g-3">
                  <div class="col-12 col-md-6">
                    <div class="detail-card">
                      <div class="text-muted small">{{ 'staffUsers.fullName' | translate }}</div>
                      <div class="fw-semibold text-break">{{ user.fullName }}</div>
                    </div>
                  </div>
                  <div class="col-12 col-md-6">
                    <div class="detail-card">
                      <div class="text-muted small">{{ 'staffUsers.email' | translate }}</div>
                      <div class="fw-semibold text-break">{{ user.email }}</div>
                    </div>
                  </div>
                  <div class="col-12 col-md-4">
                    <div class="detail-card">
                      <div class="text-muted small">{{ 'staffUsers.age' | translate }}</div>
                      <div class="fw-semibold">{{ user.age || ('staffUsers.na' | translate) }}</div>
                    </div>
                  </div>
                  <div class="col-12 col-md-4">
                    <div class="detail-card">
                      <div class="text-muted small">{{ 'staffUsers.role' | translate }}</div>
                      <div class="fw-semibold">{{ ('staffUsers.roles.' + user.applicationUserRole) | translate }}</div>
                    </div>
                  </div>
                  <div class="col-12 col-md-4">
                    <div class="detail-card">
                      <div class="text-muted small">{{ 'staffUsers.status' | translate }}</div>
                      <div class="fw-semibold">{{ (user.isActive ? 'staffUsers.active' : 'staffUsers.inactive') | translate }}</div>
                    </div>
                  </div>
                </div>
              }
            </div>
            <div class="modal-footer border-0 pt-0">
              <button class="btn btn-outline-secondary" type="button" (click)="closeDetails()">{{ 'staffUsers.close' | translate }}</button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade show"></div>
    }

    @if (pendingUserAction(); as pending) {
      <app-confirm-modal
        [title]="(pending.action === 'delete' ? 'staffUsers.deleteTitle' : 'staffUsers.deactivateTitle') | translate"
        [subtitle]="pending.user.fullName"
        [message]="(pending.action === 'delete' ? 'staffUsers.deleteMsg' : 'staffUsers.deactivateMsg') | translate"
        [confirmText]="(pending.action === 'delete' ? 'staffUsers.deleteConfirm' : 'staffUsers.deactivateConfirm') | translate"
        [busyText]="(pending.action === 'delete' ? 'staffUsers.deletingBusy' : 'staffUsers.deactivatingBusy') | translate"
        [busy]="userActionBusy()"
        [variant]="pending.action === 'delete' ? 'danger' : 'warning'"
        (confirm)="confirmUserAction()"
        (cancel)="closeUserActionModal()"
      />
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
      overflow: hidden;
      width: 2rem;
    }

    .staff-avatar img,
    .staff-avatar-lg img {
      height: 100%;
      object-fit: cover;
      width: 100%;
    }

    .staff-avatar-lg {
      align-items: center;
      background-color: #dbeafe;
      border-radius: 50%;
      color: #1d4ed8;
      display: flex;
      flex: 0 0 auto;
      font-size: 1.35rem;
      font-weight: 700;
      height: 4rem;
      justify-content: center;
      overflow: hidden;
      width: 4rem;
    }

    .detail-card {
      background-color: #f8fafc;
      border: 1px solid #e5e7eb;
      border-radius: 0.5rem;
      height: 100%;
      padding: 1rem;
    }
  `],
})
export class StaffUsersComponent implements OnInit {
  private readonly staffUsersService = inject(StaffUsersService);
  private readonly authService = inject(AuthService);
  private readonly translate = inject(TranslateService);

  readonly loading = signal(true);
  readonly detailsLoading = signal(false);
  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);
  readonly users = signal<StaffUserSummary[]>([]);
  readonly selectedUser = signal<StaffUserDetails | null>(null);
  readonly pendingUserAction = signal<{ user: StaffUserSummary; action: StaffUserConfirmAction } | null>(null);
  readonly userActionBusy = signal(false);
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
        this.error.set(this.translate.instant('staffUsers.msgLoadFail'));
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
        this.error.set(this.translate.instant('staffUsers.msgDetailsFail'));
        this.detailsLoading.set(false);
      },
    });
  }

  closeDetails(): void {
    this.selectedUser.set(null);
    this.detailsLoading.set(false);
  }

  activate(user: StaffUserSummary): void {
    this.staffUsersService.activateUser(user.id).subscribe({
      next: () => this.afterUserAction(this.translate.instant('staffUsers.msgActivated')),
      error: () => this.error.set(this.translate.instant('staffUsers.msgActivateFail')),
    });
  }

  deactivate(user: StaffUserSummary): void {
    this.pendingUserAction.set({ user, action: 'deactivate' });
  }

  deleteUser(user: StaffUserSummary): void {
    this.pendingUserAction.set({ user, action: 'delete' });
  }

  closeUserActionModal(): void {
    if (this.userActionBusy()) {
      return;
    }

    this.pendingUserAction.set(null);
  }

  confirmUserAction(): void {
    const pending = this.pendingUserAction();

    if (!pending) {
      return;
    }

    this.userActionBusy.set(true);
    this.error.set(null);
    this.success.set(null);

    const request$ = pending.action === 'delete'
      ? this.staffUsersService.deleteUser(pending.user.id)
      : this.staffUsersService.deactivateUser(pending.user.id);

    request$.subscribe({
      next: () => {
        this.userActionBusy.set(false);
        this.pendingUserAction.set(null);
        this.afterUserAction(this.translate.instant(pending.action === 'delete' ? 'staffUsers.msgDeleted' : 'staffUsers.msgDeactivated'));
      },
      error: () => {
        this.userActionBusy.set(false);
        this.error.set(this.translate.instant(pending.action === 'delete' ? 'staffUsers.msgDeleteFail' : 'staffUsers.msgDeactivateFail'));
      },
    });
  }

  getInitial(user: StaffUserSummary): string {
    return user.fullName.trim().charAt(0).toUpperCase() || 'U';
  }

  getPhotoSrc(photoUrl: string): string {
    if (!photoUrl || photoUrl.startsWith('http') || photoUrl.startsWith('blob:') || photoUrl.startsWith('data:')) {
      return photoUrl;
    }

    return `${environment.apiUrl}${photoUrl.startsWith('/') ? '' : '/'}${photoUrl}`;
  }

  private afterUserAction(message: string): void {
    this.success.set(message);
    this.selectedUser.set(null);
    this.loadUsers();
  }
}
