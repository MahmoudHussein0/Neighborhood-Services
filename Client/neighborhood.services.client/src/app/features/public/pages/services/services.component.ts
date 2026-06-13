import { Component, inject, OnInit, signal, WritableSignal } from '@angular/core';
import { CategoriesService } from '../../../../core/services/categories.service';
import { Category } from '../../../../core/models/category';
import { ProblemTypes } from '../../../staff/models/category-details';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-services',
  imports: [RouterLink, TranslatePipe],
  templateUrl: './services.component.html',
  styleUrl: './services.component.css',
})
export class ServicesComponent implements OnInit {

  private readonly categoriesService = inject(CategoriesService);

  categories: WritableSignal<Category[]> = signal<Category[]>([]);
  selectedProblem: WritableSignal<ProblemTypes> = signal<ProblemTypes>({} as ProblemTypes);
  selectedCategory: WritableSignal<Category> = signal<Category>({} as Category);

  ngOnInit(): void {
    this.getCategories();
  }


  getCategories(): void {
    // const lang = localStorage.getItem("lang") || "en";
    this.categoriesService.getAllCategories().subscribe({
      next: (res => {
        console.log(res);
        this.categories.set(res);
      }),
      error: (er => {

      })
    })
  }





}
