import { Component, inject, OnDestroy, OnInit, signal, WritableSignal } from '@angular/core';
import { CategoriesService } from '../../../../core/services/categories.service';
import { Category } from '../../../../core/models/category';
import { ProblemTypes } from '../../../staff/models/category-details';


import { LangService } from '../../../../core/services/lang.service';
import { skip, Subscription } from 'rxjs';


import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../../auth/services/auth.service';


@Component({
  selector: 'app-services',
  imports: [RouterLink, TranslatePipe],
  templateUrl: './services.component.html',
  styleUrl: './services.component.css',
})
export class ServicesComponent implements OnInit, OnDestroy {


  private readonly categoriesService = inject(CategoriesService);
  private readonly LangService = inject(LangService);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  categories: WritableSignal<Category[]> = signal<Category[]>([]);
  selectedProblem: WritableSignal<ProblemTypes> = signal<ProblemTypes>({} as ProblemTypes);
  selectedCategory: WritableSignal<Category> = signal<Category>({} as Category);

  $SubCategories: Subscription = new Subscription()

  ngOnInit(): void {
    this.LangService.lang$
      .subscribe(() => {

        this.activatedRoute.data.subscribe({
          next: (data => {
            this.categories.set(data["categories"])
          })
        })

      });
  }


  ngOnDestroy(): void {
    this.$SubCategories.unsubscribe();
  }

  /** Routes the "Book Now" CTA by role: guest→login, customer→Find Tech, tech/staff→their area. */
  bookNow(): void {
    this.router.navigateByUrl(this.auth.getBookNowUrl());
  }





}
