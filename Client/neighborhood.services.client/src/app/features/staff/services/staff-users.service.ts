import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { StaffUserDetails, StaffUserSummary } from '../models/staff-user.model';

@Injectable({
  providedIn: 'root',
})
export class StaffUsersService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private readonly http: HttpClient) {}

  getUsers() {
    return this.http.get<StaffUserSummary[]>(`${this.apiUrl}/api/Users`, {
      withCredentials: true,
    });
  }

  getUsersByRole(role: string) {
    return this.http.get<StaffUserSummary[]>(`${this.apiUrl}/api/Users/role/${role}`, {
      withCredentials: true,
    });
  }

  getUser(id: string) {
    return this.http.get<StaffUserDetails>(`${this.apiUrl}/api/Users/${id}`, {
      withCredentials: true,
    });
  }

  activateUser(id: string) {
    return this.http.patch<void>(`${this.apiUrl}/api/Users/${id}/activate`, {}, {
      withCredentials: true,
    });
  }

  deactivateUser(id: string) {
    return this.http.patch<void>(`${this.apiUrl}/api/Users/${id}/deactivate`, {}, {
      withCredentials: true,
    });
  }

  deleteUser(id: string) {
    return this.http.delete<void>(`${this.apiUrl}/api/Users/${id}`, {
      withCredentials: true,
    });
  }
}
