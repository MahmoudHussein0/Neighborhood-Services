import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AvailabilityService {

  private readonly apiService = inject(ApiService);


  getAvailability(): Observable<any> {
    return this.apiService.get(`/TechnitianAvailability`);
  }

  updateAvailability(id: number | undefined, body: object): Observable<any> {
    return this.apiService.put(`/TechnitianAvailability/${id}`, body);
  }


  addAvailability(body: object): Observable<any> {
    return this.apiService.post(`/TechnitianAvailability/`, body);
  }

  deleteAvailability(id: number): Observable<any> {
    return this.apiService.delete(`/TechnitianAvailability/${id}`);
  }


}
