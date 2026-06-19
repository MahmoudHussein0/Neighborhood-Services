import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
  StaffDto,
  CreateStaffCommand,
  UpdateStaffCommand,
  CreateUserCommand
} from '../models/staff-management.model';

@Injectable({
  providedIn: 'root'
})
export class StaffManagementService {

  private http = inject(HttpClient);

  private readonly apiUrl = `${environment.apiUrl}/api/Staff`;
  private readonly usersApiUrl = `${environment.apiUrl}/api/Users`;

  getAll(): Observable<StaffDto[]> {
    return this.http.get<StaffDto[]>(this.apiUrl);
  }

  // إنشاء يوزر جديد بدور Staff عبر /api/Users
  // (الباك إند بيعمل Staff record تلقائياً بحالة TechnicalSupport / inactive / بدون صلاحيات)
  createUser(request: CreateUserCommand): Observable<any> {
    return this.http.post<any>(this.usersApiUrl, request);
  }

  getById(id: number): Observable<StaffDto> {
    return this.http.get<StaffDto>(`${this.apiUrl}/${id}`);
  }

  // جلب Staff record عن طريق applicationUserId
  // (نستخدمها بعد إنشاء اليوزر عشان نعرف id بتاع الـ Staff record اللي اتعمل تلقائي)
  getByUserId(userId: string): Observable<StaffDto> {
    return this.http.get<StaffDto>(`${this.apiUrl}/user/${userId}`);
  }

  create(request: CreateStaffCommand): Observable<StaffDto> {
    return this.http.post<StaffDto>(this.apiUrl, request);
  }

  update(request: UpdateStaffCommand): Observable<StaffDto> {
    return this.http.put<StaffDto>(
      `${this.apiUrl}/${request.id}`,
      request
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getActive(): Observable<StaffDto[]> {
    return this.http.get<StaffDto[]>(`${this.apiUrl}/active`);
  }

  getByRole(role: string): Observable<StaffDto[]> {
    return this.http.get<StaffDto[]>(
      `${this.apiUrl}/role/${role}`
    );
  }
}