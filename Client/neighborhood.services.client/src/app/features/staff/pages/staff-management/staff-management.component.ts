import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { StaffManagementService } from '../../services/staff-management.service';
import { StaffDto, StaffRole, PermissionType, CreateUserCommand } from '../../models/staff-management.model';
import { finalize, switchMap } from 'rxjs/operators';
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
  private cdr = inject(ChangeDetectorRef);

  staffList: StaffDto[] = [];
  isLoading = false;
  isSubmitting = false;
  errorMessage = '';

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

  selectedStaff: StaffDto | null = null;
  isEditMode = false;

  searchTerm = '';
  selectedRoleFilter = '';

  // ====== Permissions الخاصة باليوزر الحالي ======
  private currentUserPermissions: PermissionType[] = [];

  ngOnInit(): void {
    this.loadAllStaff();
    this.initForm();
    this.loadCurrentUserPermissions();
  }

  // بيانات اليوزر الحالي متخزنة كـ JSON object تحت مفتاح 'ns_auth_user'
  private getCurrentUserIdFromToken(): string | null {
    const raw = localStorage.getItem('ns_auth_user');
    if (!raw) return null;

    try {
      const user = JSON.parse(raw);
      return user?.userId || null;
    } catch {
      return null;
    }
  }

  // بنجيب الـ Staff record بتاع اليوزر الحالي عشان نعرف الصلاحيات بتاعته
  private loadCurrentUserPermissions(): void {
    const userId = this.getCurrentUserIdFromToken();
    if (!userId) {
      console.warn('Could not resolve current user id from token.');
      return;
    }

    this.staffService.getByUserId(userId).subscribe({
      next: (staff: StaffDto) => {
        this.currentUserPermissions = this.mapPermissionsToIds(staff.permissions);
      },
      error: (err: any) => {
        console.error('Failed to load current user permissions', err);
      }
    });
  }

  initForm(): void {
    this.staffForm = this.fb.group({
      // Staff fields
      role: [StaffRole.TechnicalSupport, [Validators.required]],
      selectedPermissions: [[]],

      // New user creation fields (مطلوبة فقط عند الإضافة)
      fullName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      age: ['', [Validators.required, Validators.min(18)]]
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
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.errorMessage = 'Failed to load staff members data.';
          console.error(err);
        }
      });
  }

  // ====== فحص الصلاحيات ======
  canManageStaff(): boolean {
    return this.currentUserPermissions.includes(PermissionType.FullAccess) ||
           this.currentUserPermissions.includes(PermissionType.ManageUsers);
  }

  private guardAccess(): boolean {
    if (!this.canManageStaff()) {
      this.toastr.error('you are not allowed', 'Error');
      return false;
    }
    return true;
  }

  toggleModal(isOpen: boolean): void {
    if (isOpen && !this.guardAccess()) {
      return;
    }

    this.showModal = isOpen;

    this.isEditMode = false;
    this.selectedStaff = null;

    this.staffForm.reset({
      role: StaffRole.TechnicalSupport,
      selectedPermissions: []
    });

    // فتح المودال لإضافة موظف جديد => نفعّل الـ validators بتاعة بيانات اليوزر الجديد
    if (isOpen) {
      this.setNewUserValidators(true);
    }
  }

  // تفعيل/تعطيل الـ validators الخاصة بحقول إنشاء اليوزر الجديد
  // (تكون مطلوبة عند الإضافة فقط، وتُلغى عند التعديل)
  private setNewUserValidators(enable: boolean): void {
    const requiredFields = ['fullName', 'email', 'password', 'age'];

    requiredFields.forEach(field => {
      const control = this.staffForm.get(field);
      if (!control) return;

      if (enable) {
        if (field === 'email') {
          control.setValidators([Validators.required, Validators.email]);
        } else if (field === 'password') {
          control.setValidators([Validators.required, Validators.minLength(6)]);
        } else if (field === 'age') {
          control.setValidators([Validators.required, Validators.min(18)]);
        } else {
          control.setValidators([Validators.required]);
        }
      } else {
        control.clearValidators();
      }

      control.updateValueAndValidity();
    });
  }

  onPermissionChange(permissionId: number, event: any): void {
    const current: number[] = this.staffForm.get('selectedPermissions')?.value || [];
    const isChecked = event.target.checked;

    if (permissionId === PermissionType.FullAccess) {
      // لو حدد Full Access، بقية الصلاحيات تصبح غير ضرورية
      this.staffForm.get('selectedPermissions')?.setValue(
        isChecked ? [PermissionType.FullAccess] : []
      );
      return;
    }

    if (isChecked) {
      this.staffForm.get('selectedPermissions')?.setValue([...current, permissionId]);
    } else {
      this.staffForm.get('selectedPermissions')?.setValue(current.filter(id => id !== permissionId));
    }
  }

  // بنستخدمها في الـ template لتعطيل باقي الصلاحيات لما يكون Full Access متحدد
  isFullAccessSelected(): boolean {
    const current: number[] = this.staffForm.get('selectedPermissions')?.value || [];
    return current.includes(PermissionType.FullAccess);
  }

  onSubmit(): void {
    if (!this.guardAccess()) return;

    if (this.staffForm.invalid) return;

    if (this.isEditMode && this.selectedStaff) {
      this.updateStaff();
      return;
    }

    this.isSubmitting = true;

    const formValue = this.staffForm.value;

    // 1) إنشاء اليوزر الجديد بدور Staff
    //    (الباك إند بيعمل Staff record تلقائياً: TechnicalSupport / inactive / بدون صلاحيات)
    const createUserCommand: CreateUserCommand = {
      fullName: formValue.fullName,
      email: formValue.email,
      photo: '',
      password: formValue.password,
      age: Number(formValue.age),
      applicationUserRole: 'Staff',
      latitude: 0,
      longitude: 0,
      nationalId: '',
      experience: '',
      maxTravelDistance: 0
    };

    this.staffService.createUser(createUserCommand)
      .pipe(
        // 2) نجيب الـ Staff record اللي اتعمل تلقائياً عشان نعرف الـ id بتاعه
        switchMap((newUser: any) => {
          const userId = newUser?.id || newUser?.data?.id || newUser?.result?.id;
          return this.staffService.getByUserId(userId);
        }),
        // 3) نحدّث الـ Role والصلاحيات ونفعّل الحساب بالقيم اللي اختارها الأدمن
        switchMap((staffRecord: StaffDto) => {
          const command = {
            id: staffRecord.id,
            role: Number(formValue.role),
            isActive: true,
            permissions: formValue.selectedPermissions
          };
          return this.staffService.update(command);
        }),
        finalize(() => {
          this.isSubmitting = false;
        })
      )
      .subscribe({
        next: () => {
          this.toastr.success(
            'Staff member created and assigned successfully!',
            'Success'
          );

          this.loadAllStaff();
          this.toggleModal(false);
        },
        error: (err) => {
          this.toastr.error(
            'Failed to create the user or assign the staff role.',
            'Error'
          );
          console.error(err);
        }
      });
  }

  onDeleteStaff(id: number): void {
    if (!this.guardAccess()) return;

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
      result = result.filter(s => s.staffRole === this.selectedRoleFilter);
    }

    return result;
  }

  openEditModal(staff: StaffDto): void {
    if (!this.guardAccess()) return;

    this.isEditMode = true;
    this.selectedStaff = staff;

    // مفيش إنشاء يوزر هنا، خالص نلغي الـ validators الخاصة بيه
    this.setNewUserValidators(false);

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
          this.toastr.success('Staff updated successfully');
          this.loadAllStaff();
          this.toggleModal(false);
        },
        error: () => {
          this.toastr.error('Failed to update staff', 'Error');
        }
      });
  }

  private isAdminRole(role: any): boolean {
    const r = String(role).trim();
    return r === 'Admin' || r === '1';
  }

  toggleStatus(staff: StaffDto): void {
    if (!this.guardAccess()) return;

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
      error: () => {
        this.toastr.error('Failed to update staff status', 'Error');
      }
    });
  }
}