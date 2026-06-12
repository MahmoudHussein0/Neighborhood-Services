import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { StaffManagementService } from '../../services/staff-management.service';
import { StaffDto, StaffRole, PermissionType, UserLookupDto } from '../../models/staff-management.model';
import { finalize } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-staff-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './staff-management.component.html',
  styleUrls: ['./staff-management.component.css']
})
export class StaffManagementComponent implements OnInit {
  private staffService = inject(StaffManagementService);
  private fb = inject(FormBuilder);
  private toastr = inject(ToastrService);
  staffList: StaffDto[] = [];
  usersList: UserLookupDto[] = [];
  isLoading = false;
  isLoadingUsers = false;
  isSubmitting = false;
  errorMessage = '';
  private cdr = inject(ChangeDetectorRef);
  staffForm!: FormGroup;
  showModal = false;

  roles = Object.keys(StaffRole).filter(k => isNaN(Number(k)));
  permissionsList = [
    { id: PermissionType.ManageDisputes, name: 'Manage Disputes' },
    { id: PermissionType.ManageTickets, name: 'Manage Tickets' },
    { id: PermissionType.ViewTransactions, name: 'View Transactions' },
    { id: PermissionType.ApproveTechnicians, name: 'Approve Technicians' },
    { id: PermissionType.FlagReviews, name: 'Flag Reviews' },
    { id: PermissionType.ManageUsers, name: 'Manage Users' },
    { id: PermissionType.ManageBookings, name: 'Manage Bookings' },
    { id: PermissionType.FullAccess, name: 'Full Access' }
  ];

  ngOnInit(): void {
    this.loadAllStaff();

    this.initForm();
  }

  initForm(): void {
    this.staffForm = this.fb.group({
      userId: ['', [Validators.required]],
      role: [StaffRole.TechnicalSupport, [Validators.required]],
      selectedPermissions: [[]]
    });
  }

  loadAllStaff(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.staffService.getAll()
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: (response: any) => {

          const data = response?.data || response?.result || response;

          this.staffList = Array.isArray(data) ? data : [];
          this.loadAvailableUsers();
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.errorMessage = 'Failed to load staff members data.';
          console.error(err);

        }
      });

  }
  loadAvailableUsers(): void {

    this.isLoadingUsers = true;

    this.staffService
      .getUsersByRole('Staff')
      .pipe(
        finalize(() => {
          this.isLoadingUsers = false;
        })
      )
      .subscribe({
        next: (users) => {

          const assignedUserIds =
            this.staffList.map(
              s => s.applicationUserId
            );

          this.usersList = users.filter(
            user => !assignedUserIds.includes(user.id)
          );

        },
        error: (err) => {
          console.error(err);
        }
      });
  }
  toggleModal(isOpen: boolean): void {

    this.showModal = isOpen;

    if (!isOpen) {

      this.isEditMode = false;

      this.selectedStaff = null;

      this.staffForm.reset({
        role: StaffRole.TechnicalSupport,
        selectedPermissions: []
      });
    }
  }

  onPermissionChange(permissionId: number, event: any): void {
    const current: number[] = this.staffForm.get('selectedPermissions')?.value || [];
    if (event.target.checked) {
      this.staffForm.get('selectedPermissions')?.setValue([...current, permissionId]);
    } else {
      this.staffForm.get('selectedPermissions')?.setValue(current.filter(id => id !== permissionId));
    }
  }

  onSubmit(): void {

    if (this.staffForm.invalid) return;

    if (this.isEditMode && this.selectedStaff) {
      this.updateStaff();
      return;
    }

    this.isSubmitting = true;

    const formValue = this.staffForm.value;

    const command = {
      userId: formValue.userId,
      role: Number(formValue.role),
      permissions: formValue.selectedPermissions
    };

    this.staffService.create(command).subscribe({
      next: () => {

        this.isSubmitting = false;

        this.toastr.success(
          'Staff member assigned successfully!',
          'Success'
        );

        this.loadAllStaff();

        this.toggleModal(false);
      },
      error: (err) => {

        this.isSubmitting = false;

        this.toastr.error(
          'Failed to add staff member.',
          'Error'
        );

        console.error(err);
      }
    });
  }
  onDeleteStaff(id: number): void {
    if (confirm('Are you sure you want to permanently delete this staff member?')) {
      this.staffService.delete(id).subscribe({
        next: () => {
          this.staffList = this.staffList.filter(s => s.id !== id);
        },
        error: (err) => {
          this.toastr.error('Delete failed. This account might be linked to existing operations.', 'Error');
        }
      });
    }
  }

  formatRole(role: string | number): string {
    if (this.isAdminRole(role)) return 'Admin';
    const r = String(role).trim();
    if (r === 'TechnicalSupport' || r === '2') return 'Technical Support';
    return r;
  }
  formatPermission(permission: string | number): string {
    if (!permission) return '';
    let permStr = String(permission);
    const num = Number(permission);
    if (!isNaN(num) && PermissionType[num]) {
      permStr = PermissionType[num];
    }
    return permStr
      .replace(/([A-Z])/g, ' $1')
      .trim();
  }

  mapPermissionsToIds(permissions: any[]): number[] {
    return (permissions || []).map(p => {
      const val = p && typeof p === 'object' ? p.permission : p;
      if (val === undefined || val === null) return null;
      const num = Number(val);
      if (!isNaN(num)) {
        return num;
      }
      if (typeof val === 'string' && val in PermissionType) {
        return PermissionType[val as keyof typeof PermissionType] as any as number;
      }
      return null;
    }).filter(v => v !== null) as number[];
  }
  get activeStaffCount(): number {
    return this.staffList.filter(s => s.isActive).length;
  }

  get adminCount(): number {
    return this.staffList.filter(s => this.isAdminRole(s.staffRole)).length;
  }
  selectedStaff: StaffDto | null = null;
  isEditMode = false;

  searchTerm = '';
  selectedRoleFilter = '';
  get filteredStaffList(): StaffDto[] {

    let result = [...this.staffList];

    if (this.searchTerm.trim()) {

      const search = this.searchTerm.toLowerCase();

      result = result.filter(staff =>
        staff.fullName.toLowerCase().includes(search) ||
        staff.email.toLowerCase().includes(search)
      );
    }

    if (this.selectedRoleFilter) {

      result = result.filter(
        s => s.staffRole === this.selectedRoleFilter
      );
    }

    return result;
  }
  openEditModal(staff: StaffDto): void {

    this.isEditMode = true;

    this.selectedStaff = staff;


    this.staffForm.patchValue({
      role: this.isAdminRole(staff.staffRole) ? StaffRole.Admin : StaffRole.TechnicalSupport
    });

    const permissions = this.mapPermissionsToIds(staff.permissions);

    this.staffForm.patchValue({
      selectedPermissions: permissions
    });

    this.showModal = true;
  }
  updateStaff(): void {

    const formValue = this.staffForm.value;

    const command = {
      id: this.selectedStaff!.id,
      role: Number(formValue.role),
      isActive: this.selectedStaff!.isActive,
      permissions: formValue.selectedPermissions
    };

    this.staffService.update(command)
      .subscribe({

        next: () => {

          this.toastr.success(
            'Staff updated successfully'
          );

          this.loadAllStaff();

          this.toggleModal(false);

        },
        error: () => {

          this.toastr.error(
            'Failed to update staff',
            'Error'
          );
        }

      });
  }
  private isAdminRole(role: any): boolean {
    const r = String(role).trim();
    return r === 'Admin' || r === '1';
  }

  toggleStatus(staff: StaffDto): void {
    const command = {
      id: staff.id,
      role: this.isAdminRole(staff.staffRole) ? 1 : 2,
      isActive: !staff.isActive,
      permissions: this.mapPermissionsToIds(staff.permissions)
    };

    this.staffService.update(command).subscribe({
      next: () => {
        staff.isActive = !staff.isActive;
        this.toastr.success(
          `Staff ${staff.isActive ? 'activated' : 'deactivated'} successfully`,
          'Success'
        );
      },
      // ✅ أضف الـ error
      error: () => {
        this.toastr.error('Failed to update staff status', 'Error');
      }
    });
  }
}