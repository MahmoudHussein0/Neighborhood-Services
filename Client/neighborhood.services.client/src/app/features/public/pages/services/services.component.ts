import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { CategoriesService } from '../../../../core/services/categories.service';
import { Category } from '../../../../core/models/category';
import { ProblemTypes } from '../../../staff/models/category-details';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { LangService } from '../../../../core/services/lang.service';
import { skip } from 'rxjs';

@Component({
  selector: 'app-services',
  imports: [RouterLink, TranslatePipe],
  templateUrl: './services.component.html',
  styleUrl: './services.component.css',
})
export class ServicesComponent implements OnInit {

  private readonly categoriesService = inject(CategoriesService);
  private readonly LangService = inject(LangService);

  categories: WritableSignal<Category[]> = signal<Category[]>([]);
  isLoading = signal<boolean>(false);
  selectedProblem: WritableSignal<ProblemTypes> = signal<ProblemTypes>({} as ProblemTypes);
  selectedCategory: WritableSignal<Category> = signal<Category>({} as Category);

  ngOnInit(): void {
    this.LangService.lang$
      .subscribe(() => {
        this.getCategories();
      });
  }


  getCategories(): void {
    this.isLoading.set(true);
    this.categoriesService.getAllCategories().subscribe({
      next: (res => {
        console.log(res);
        this.categories.set(res);
        this.isLoading.set(false);
      }),
      error: (er => {

      })
    })
  }





}
