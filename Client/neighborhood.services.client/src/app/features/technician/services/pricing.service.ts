import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PricingService {
  private readonly apiService = inject(ApiService);

  getPricing(): Observable<any> {
    return this.apiService.get(`/TechnicianPricing`);
  }


  updatePricing(id: number | undefined, data: object): Observable<any> {
    return this.apiService.put(`/TechnicianPricing/${id}`, data);
  }


  deletePricing(id: number): Observable<any> {
    return this.apiService.delete(`/TechnicianPricing/${id}`);
  }


  addPricing(data: object): Observable<any> {
    return this.apiService.post(`/TechnicianPricing`, data);
  }


}
