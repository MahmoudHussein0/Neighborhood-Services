import { inject, Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProblemTypeService {
  private readonly apiService = inject(ApiService);




  updateProblemType(data: object, id: number): Observable<any> {
    return this.apiService.put(`/ProblemTypes/${id}`, data);
  }



  delete(id: number): Observable<any> {
    return this.apiService.delete(`/ProblemTypes/${id}`);
  }


  add(data: object): Observable<any> {
    return this.apiService.post('/ProblemTypes', data);

  }


  getProblemTypes(): Observable<any> {
    return this.apiService.get(`/ProblemTypes`);
  }


  getProblemTypeById(id: number): Observable<any> {
    return this.apiService.get(`/ProblemTypes/${id}`);
  }




}
