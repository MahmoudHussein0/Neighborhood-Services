import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { catchError, of, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AuthResponse,
  ChangePasswordRequest,
  ChangePasswordResponse,
  GeocodingResult,
  LoginRequest,
  RegisterRequest,
  RegisterResponse,
  SafeAuthUser,
} from '../models/auth.models';

const AUTH_USER_KEY = 'ns_auth_user';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly baseUrl = environment.apiUrl;
  private readonly currentUserSignal = signal<SafeAuthUser | null>(this.loadStoredUser());

  readonly currentUser = this.currentUserSignal.asReadonly();

  constructor(private readonly http: HttpClient) {}

  login(email: string, password: string) {
    const request: LoginRequest = { email, password };

    return this.http
      .post<AuthResponse>(`${this.baseUrl}/api/Auth/login`, request, {
        withCredentials: true,
      })
      .pipe(tap((user) => this.setCurrentUser(user)));
  }

  register(userData: RegisterRequest) {
    return this.http.post<RegisterResponse>(`${this.baseUrl}/api/Users`, userData, {
      withCredentials: true,
    });
  }

  uploadUserPhoto(file: File) {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<{ photoUrl: string }>(`${this.baseUrl}/api/Users/photo-upload`, formData, {
      withCredentials: true,
    });
  }

  completeExternalLogin(user: SafeAuthUser): void {
    this.setCurrentUser(user);
  }

  geocodeAddress(address: string) {
    return this.http.get<GeocodingResult>(`${this.baseUrl}/api/Geocoding/search`, {
      params: { address },
      withCredentials: true,
    });
  }

  changePassword(request: ChangePasswordRequest) {
    return this.http.post<ChangePasswordResponse>(`${this.baseUrl}/api/Auth/change-password`, request, {
      withCredentials: true,
    });
  }

  logout() {
    return this.http
      .post<{ message: string }>(
        `${this.baseUrl}/api/Auth/logout`,
        {},
        { withCredentials: true },
      )
      .pipe(
        tap(() => this.clearCurrentUser()),
        catchError(() => {
          this.clearCurrentUser();
          return of({ message: 'Logged out locally' });
        }),
      );
  }

  isAuthenticated(): boolean {
    const user = this.currentUserSignal();

    if (!user) {
      return false;
    }

    if (this.isExpired(user.expiresAt)) {
      this.clearCurrentUser();
      return false;
    }

    return true;
  }

  hasAnyRole(allowedRoles: string[]): boolean {
    const user = this.currentUserSignal();

    if (!user || !this.isAuthenticated()) {
      return false;
    }

    const normalizedRole = this.normalizeRole(user.role);
    return allowedRoles.some((role) => this.normalizeRole(role) === normalizedRole);
  }

  /**
   * Where a public "Book Now" CTA should send the user:
   * guests → login, customers → Find Technician, and other roles back to a
   * sensible spot in their own area (tech dashboard, staff categories).
   */
  getBookNowUrl(): string {
    if (!this.isAuthenticated()) {
      return '/auth/login';
    }

    const role = this.normalizeRole(this.currentUserSignal()?.role ?? '');

    if (role === 'customer') {
      return '/customer/find-technician';
    }

    if (role === 'technician') {
      return '/technician';
    }

    return '/staff/categories';
  }

  getRedirectUrlForRole(role: string): string {
    const normalizedRole = role.trim().toLowerCase();

    if (normalizedRole === 'customer') {
      return '/customer';
    }

    if (normalizedRole === 'technician') {
      return '/technician';
    }

    return '/staff';
  }

  updateSafeUser(user: Partial<SafeAuthUser>): void {
    const currentUser = this.currentUserSignal();

    if (!currentUser) {
      return;
    }

    this.setCurrentUser({ ...currentUser, ...user });
  }

  private setCurrentUser(user: SafeAuthUser): void {
    this.currentUserSignal.set(user);
    localStorage.setItem(AUTH_USER_KEY, JSON.stringify(user));
  }

  private clearCurrentUser(): void {
    this.currentUserSignal.set(null);
    localStorage.removeItem(AUTH_USER_KEY);
  }

  private loadStoredUser(): SafeAuthUser | null {
    const storedUser = localStorage.getItem(AUTH_USER_KEY);

    if (!storedUser) {
      return null;
    }

    try {
      const user = JSON.parse(storedUser) as SafeAuthUser;

      if (this.isExpired(user.expiresAt)) {
        localStorage.removeItem(AUTH_USER_KEY);
        return null;
      }

      return user;
    } catch {
      localStorage.removeItem(AUTH_USER_KEY);
      return null;
    }
  }

  private isExpired(expiresAt: string): boolean {
    const expiresAtTime = Date.parse(expiresAt);

    if (Number.isNaN(expiresAtTime)) {
      return true;
    }

    return expiresAtTime <= Date.now();
  }

  private normalizeRole(role: string): string {
    return role.trim().toLowerCase();
  }
}
