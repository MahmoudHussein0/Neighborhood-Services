import { ApiService } from './../../../core/services/api.service';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Policies } from '../models/policies';

@Injectable({
  providedIn: 'root',
})
export class PoliciesService {

  private readonly apiService = inject(ApiService);

  getPolicies(): Observable<Policies[]> {
    return this.apiService.get<Policies[]>('/CancellationPolicies');
  }

  addPolicy(data: object): Observable<any> {
    return this.apiService.post('/CancellationPolicies', data);
  }

  editPolicy(id: number, data: object): Observable<any> {
    return this.apiService.put(`/CancellationPolicies/${id}`, data);
  }

  deletePolicy(id: number): Observable<any> {
    return this.apiService.delete(`/CancellationPolicies/${id}`);
  }

  lookUpPolicy(params: any): Observable<any> {
    return this.apiService.get(`/CancellationPolicies/lookup`, params);
  }

}
