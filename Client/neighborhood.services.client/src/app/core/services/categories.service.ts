import { inject, Injectable, signal, WritableSignal } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { Category } from '../models/category';

@Injectable({
  providedIn: 'root',
})
export class CategoriesService {
  private readonly apiService = inject(ApiService);

  categories: WritableSignal<Category[]> = signal<Category[]>([]);




  getAllCategories(): Observable<any> {
    return this.apiService.get(`/Categories`);
  }


  updateCategory(body: object, categoryId: number): Observable<any> {
    return this.apiService.put(`/Categories/${categoryId}`, body);
  }



  AddCategory(body: object): Observable<any> {
    return this.apiService.post("/Categories/", body);
  }


  deletCategory(id: number): Observable<any> {
    return this.apiService.delete(`/Categories/${id}`);
  }


  getCategoryDetails(id: number): Observable<any> {
    return this.apiService.get(`/Categories/${id}`)
  }


}
