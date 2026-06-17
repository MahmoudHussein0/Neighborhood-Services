import { Injectable } from '@angular/core';
import { FavoriteItem } from '../models/favorite-item';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../environments/environment.prod';
import { HttpClient } from '@microsoft/signalr/dist/esm/HttpClient';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FavoritesService {
// private apiUrl: string = environment.apiUrl.concat('/');
private Endpoint = '/Favorites';
constructor(private apiService: ApiService) { }
favItems?: FavoriteItem[];

  getAll(): Observable<FavoriteItem[]> {
      return this.apiService.get<FavoriteItem[]>(this.Endpoint+'/GetAllFavoriteItems');
    }

  getForCurrentUser():Observable<FavoriteItem[]> {
   return this.apiService.get<FavoriteItem[]>(this.Endpoint + '/GetMyFavorites');
  }

  AddToMyFavorites(technicianId: number): Observable<FavoriteItem[]> {
  return this.apiService.put<FavoriteItem[]>(
    `${this.Endpoint}/${technicianId}`,
  null
  );
}

  Delete(id: number):Observable<FavoriteItem> {
    return this.apiService.delete(`${this.Endpoint}/${id}`);
  }

}
