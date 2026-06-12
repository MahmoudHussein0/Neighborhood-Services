import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import {
  CustomerAddress,
  CustomerAddressRequest,
  CustomerProfile,
  CustomerRecord,
} from '../models/customer-profile.model';

@Injectable({
  providedIn: 'root',
})
export class CustomerProfileService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private readonly http: HttpClient) {}

  getProfile(userId: string) {
    return this.http.get<CustomerProfile>(`${this.apiUrl}/api/Users/${userId}`, {
      withCredentials: true,
    });
  }

  getCurrentCustomer() {
    return this.http.get<CustomerRecord>(`${this.apiUrl}/api/customers/me`, {
      withCredentials: true,
    });
  }

  updateProfile(userId: string, body: { fullName: string; age: number }) {
    return this.http.put<void>(`${this.apiUrl}/api/Users/${userId}/profile`, body, {
      withCredentials: true,
    });
  }

  updatePhoto(userId: string, photo: string) {
    return this.http.put<void>(`${this.apiUrl}/api/Users/${userId}/photo`, { photo }, {
      withCredentials: true,
    });
  }

  getAddressesByUserId(userId: string) {
    return this.http.get<CustomerAddress[]>(`${this.apiUrl}/api/CustomerAddresses/user/${userId}`, {
      withCredentials: true,
    });
  }

  createAddress(userId: string, customerId: number, body: CustomerAddressRequest & { isDefault: boolean }) {
    return this.http.post<{ id: number }>(
      `${this.apiUrl}/api/CustomerAddresses`,
      { ...body, applicationUserId: userId, customerId },
      { withCredentials: true },
    );
  }

  updateAddress(id: number, body: CustomerAddressRequest) {
    return this.http.put<void>(`${this.apiUrl}/api/CustomerAddresses/${id}`, { id, ...body }, {
      withCredentials: true,
    });
  }

  deleteAddress(id: number) {
    return this.http.delete<void>(`${this.apiUrl}/api/CustomerAddresses/${id}`, {
      withCredentials: true,
    });
  }

  setDefaultAddress(id: number) {
    return this.http.patch<void>(`${this.apiUrl}/api/CustomerAddresses/${id}/default`, {}, {
      withCredentials: true,
    });
  }
}
