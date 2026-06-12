// src/app/features/staff/models/staff-management.model.ts

// ==========================================
// 1. Enums (مطابقة تماماً للـ Backend Enums)
// ==========================================
export enum PermissionType {
  ManageDisputes = 1,
  ManageTickets = 2,
  ViewTransactions = 3,
  ManagePromos = 4,
  ApproveTechnicians = 5,
  FlagReviews = 6,
  ManageUsers = 7,
  ManageBookings = 8,
  FullAccess = 9
}

export enum StaffRole {
  Admin = 1,
  TechnicalSupport = 2
}

// ==========================================
// 2. Data Transfer Objects (الـ Responses القادمة من الـ API)
// ==========================================

// يمثل كل عنصر داخل قائمة صلاحيات الموظف
export interface StaffPermissionDto {
  id: number;
  staffId: number;
  permission: string; // تأتي كـ string بناءً على الـ DTO (مثل "ManageTickets")
}

// يمثل الموظف بالكامل مع بيانات الـ User الخاصة به (المستخدم في الـ Data Tables)
export interface StaffDto {
  id: number;
  applicationUserId: string;
  staffRole: string; // تأتي كـ string (مثل "Admin" أو "TechnicalSupport")
  isActive: boolean;
  permissions: StaffPermissionDto[];
  createdAt: Date;
  updatedAt: Date;
  
  // بيانات الـ ApplicationUser المحقونة داخل الـ DTO
  fullName: string;
  email: string;
}

// ==========================================
// 3. Commands / Requests (الـ Payloads المرسلة للـ API)
// ==========================================

// مطابق لـ CreateStaffCommand عند إضافة موظف جديد
export interface CreateStaffCommand {
  userId: string;        // الـ Id الخاص بالمستخدم بعد إنشائه في نظام الـ Identity
  role: StaffRole;       // نرسله كـ رقم Enum (1 أو 2)
  permissions: PermissionType[]; // مصفوفة أرقام الـ Enums المختارة للـ الصلاحيات
}

// مطابق لـ UpdateStaffCommand عند تعديل بيانات أو صلاحيات موظف حالي
export interface UpdateStaffCommand {
  id: number;            // معرف الموظف المراد تعديله
  role: StaffRole;       // الدور الجديد أو الحالي
  isActive: boolean;     // حالة الحساب (تفعيل/تعطيل)
  permissions: PermissionType[]; // القائمة الجديدة الكاملة للصلاحيات كأرقام
}
export interface UserLookupDto {

  id: string;

  fullName: string;

  email: string;

  photo: string;

  isActive: boolean;

  applicationUserRole: string;
}
// ملاحظة: الـ DeleteStaffCommand لا يحتاج لـ Interface مستقل لأنه يرسل الـ Id مباشرة في الـ URL كـ Parameter