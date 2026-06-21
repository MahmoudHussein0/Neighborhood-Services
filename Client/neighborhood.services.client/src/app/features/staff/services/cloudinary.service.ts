import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CloudinaryService {

  private readonly baseUrl =
    `${environment.apiUrl}/api/files`;

  constructor(
    private http: HttpClient
  ) {}

  getSignature() {
    return this.http.post(
      `${this.baseUrl}/signature`,
      {}
    );
  }
}