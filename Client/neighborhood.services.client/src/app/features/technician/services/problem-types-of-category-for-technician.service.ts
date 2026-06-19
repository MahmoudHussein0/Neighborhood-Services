import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { Observable } from 'rxjs';
import { ProblemTypesOfCategoryForSpecificTechnician } from '../models/problem-types-of-category-for-specific-technician';

@Injectable({
  providedIn: 'root',
})
export class ProblemTypesOfCategoryForTechnicianService {
  private readonly apiService = inject(ApiService);



  getProblemTypesOfCategory(): Observable<ProblemTypesOfCategoryForSpecificTechnician[]> {
    return this.apiService.get('/TechnitianCategory');
  }



  assignTechnicianToCategory(data: object): Observable<any> {
    return this.apiService.post('/TechnitianCategory', data);
  }



  delete(id: number): Observable<any> {
    return this.apiService.delete(`/TechnitianCategory/${id}`);
  }





}
