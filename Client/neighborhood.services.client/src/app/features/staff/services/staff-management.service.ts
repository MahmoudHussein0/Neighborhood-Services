import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
  StaffDto,
  CreateStaffCommand,
  UpdateStaffCommand,
  UserLookupDto
} from '../models/staff-management.model';

@Injectable({
  providedIn: 'root'
})
export class StaffManagementService {

  private http = inject(HttpClient);

  private readonly apiUrl = `${environment.apiUrl}/api/Staff`;

  getAll(): Observable<StaffDto[]> {
    return this.http.get<StaffDto[]>(this.apiUrl);
  }

getUsersByRole(role: string) {
  return this.http.get<UserLookupDto[]>(
    `${environment.apiUrl}/api/Users/role/${role}`
  );
}
  getById(id: number): Observable<StaffDto> {
    return this.http.get<StaffDto>(`${this.apiUrl}/${id}`);
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