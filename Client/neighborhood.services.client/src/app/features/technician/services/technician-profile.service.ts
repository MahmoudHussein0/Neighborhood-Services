import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import {
  TechnicianPhoto,
  TechnicianProfile,
  TechnicianUserProfile,
} from '../models/technician-profile.model';

@Injectable({
  providedIn: 'root',
})
export class TechnicianProfileService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private readonly http: HttpClient) {}

  getUserProfile(userId: string) {
    return this.http.get<TechnicianUserProfile>(`${this.apiUrl}/api/Users/${userId}`, {
      withCredentials: true,
    });
  }

  updateUserProfile(userId: string, body: { fullName: string; age: number }) {
    return this.http.put<void>(`${this.apiUrl}/api/Users/${userId}/profile`, body, {
      withCredentials: true,
    });
  }

  updateUserPhoto(userId: string, photo: string) {
    return this.http.put<void>(`${this.apiUrl}/api/Users/${userId}/photo`, { photo }, {
      withCredentials: true,
    });
  }

  getTechnicianByUserId(userId: string) {
    return this.http.get<TechnicianProfile>(`${this.apiUrl}/api/Technicians/user/${userId}`, {
      withCredentials: true,
    });
  }

  updateTechnician(id: number, body: { nationalId: string; experience: string; maxTravelDistance: number }) {
    return this.http.put<void>(`${this.apiUrl}/api/Technicians/${id}`, { id, ...body }, {
      withCredentials: true,
    });
  }

  getPhotosByTechnicianId(technicianId: number) {
    return this.http.get<TechnicianPhoto[]>(`${this.apiUrl}/api/technician-photos/technician/${technicianId}`, {
      withCredentials: true,
    });
  }

  addPhoto(body: { photoUrl: string; caption: string; applicationUserId: string; technicianId: number }) {
    return this.http.post<{ id: number }>(`${this.apiUrl}/api/technician-photos`, body, {
      withCredentials: true,
    });
  }

  updatePhoto(id: number, body: { photoUrl: string; caption: string }) {
    return this.http.put<void>(`${this.apiUrl}/api/technician-photos/${id}`, body, {
      withCredentials: true,
    });
  }

  deletePhoto(id: number) {
    return this.http.delete<void>(`${this.apiUrl}/api/technician-photos/${id}`, {
      withCredentials: true,
    });
  }
}
