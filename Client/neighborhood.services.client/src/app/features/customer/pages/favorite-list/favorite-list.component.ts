import { Component ,signal,OnInit} from '@angular/core';
import { FavoritesService } from '../../services/favorites.service';
import { FavoriteItem } from '../../models/favorite-item';
import { NgClass,CommonModule } from '@angular/common';

@Component({
  selector: 'app-favorite-list',
  imports: [CommonModule, NgClass ],
  templateUrl: './favorite-list.component.html',
  styleUrl: './favorite-list.component.css',
})
export class FavoriteListComponent {
  favorites = signal<FavoriteItem[]>([]);
  isLoading = signal(false);
  public favs: FavoriteItem[] = [];

  constructor(private favoritesService: FavoritesService) {}

  ngOnInit(): void {
    this.loadFavorites();
  }

  loadFavorites(): void {
    this.isLoading.set(true);

    this.favoritesService.getAll().subscribe({
      next: data => {
        console.log(data);
        this.favorites.set(data);
        this.favs = [...data];
        console.log('Favorites loaded:', data);
        this.isLoading.set(false);
        
      },
      error: err => {
        console.error(err);
        this.isLoading.set(false);
      }
    });
  }

  DeleteFromFavs(id: number): void {
    this.favoritesService.Delete(id).subscribe({
      next: (data) => {
        console.log('Delete response:', data);
        console.log(`Favorite with id ${id} deleted successfully.`);
        this.loadFavorites(); 
      },
      error: err => {
        console.error(`Error deleting favorite with id ${id}:`, err);
      }
    });
}

}// end of component class
