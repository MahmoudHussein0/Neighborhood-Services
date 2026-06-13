import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ExceptionService {

  private readonly apiService = inject(ApiService);



  getException(): Observable<any> {
    return this.apiService.get(`/AvilabilityException`);
  }


  updateException(id: number | undefined, data: object): Observable<any> {
    return this.apiService.put(`/AvilabilityException/${id}`, data);
  }

  addException(data: object): Observable<any> {
    return this.apiService.post(`/AvilabilityException`, data);
  }

  deleteException(id: number): Observable<any> {
    return this.apiService.delete(`/AvilabilityException/${id}`);
  }



}
