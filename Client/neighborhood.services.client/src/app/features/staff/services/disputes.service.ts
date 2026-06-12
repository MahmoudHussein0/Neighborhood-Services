import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { DisputeDto } from '../models/staff-disput.model';

@Injectable({
  providedIn: 'root'
})
export class DisputeService {

  private readonly baseUrl =
    `${environment.apiUrl}/api/Disputes`;

  constructor(private http: HttpClient) {}

  getDisputes(): Observable<DisputeDto[]> {
    return this.http.get<DisputeDto[]>(this.baseUrl);
  }

  getDispute(id: number): Observable<DisputeDto> {
    return this.http.get<DisputeDto>(
      `${this.baseUrl}/${id}`
    );
  }

  getByStatus(status: string): Observable<DisputeDto[]> {
    return this.http.get<DisputeDto[]>(
      `${this.baseUrl}/status/${status}`
    );
  }

  getByType(type: string): Observable<DisputeDto[]> {
    return this.http.get<DisputeDto[]>(
      `${this.baseUrl}/type/${type}`
    );
  }

  updateDispute(id: number, body: any) {
    return this.http.put(
      `${this.baseUrl}/${id}`,
      body
    );
  }

  deleteDispute(id: number) {
    return this.http.delete(
      `${this.baseUrl}/${id}`
    );
  }
  getById(id: number): Observable<any> {
  return this.http.get<any>(`${this.baseUrl}/${id}`);
}
}