import { Component, computed, inject, OnInit, Signal, signal, WritableSignal } from '@angular/core';
import { CarouselModule, OwlOptions } from 'ngx-owl-carousel-o';
import { CategoriesService } from '../../../../../core/services/categories.service';
import { Category } from '../../../../../core/models/category';
import { skip, Subscription } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { LangService } from '../../../../../core/services/lang.service';
@Component({
  selector: 'ourservices',
  imports: [CarouselModule, TranslatePipe],
  templateUrl: './ourservices.component.html',
  styleUrl: './ourservices.component.css',
})
export class OurservicesComponent implements OnInit {

  private readonly categoriesService = inject(CategoriesService);
  private readonly LangService = inject(LangService);


  categories: WritableSignal<Category[]> = signal<Category[]>([]);
  $Sub: Subscription = new Subscription();
  isLoading = signal<boolean>(false);

  ngOnInit(): void {
    this.LangService.lang$
      .subscribe(() => {
        this.getAllCategories();
      });
  }



  getAllCategories(): void {
    this.isLoading.set(true);

    this.$Sub = this.categoriesService.getAllCategories().subscribe({
      next: (res => {
        this.categories.set(res);
        this.isLoading.set(false);
      })
    })
  }



  customOptions: OwlOptions = {
    loop: true,
    autoplay: true,
    autoplayTimeout: 2100,
    autoplaySpeed: 1200,
    rtl: true,
    autoplayHoverPause: true,
    mouseDrag: true,
    touchDrag: true,
    pullDrag: false,
    dots: false,
    navSpeed: 700,
    navText: ['<i class="fa-solid fa-caret-left" ></i>', '<i class="fa-solid fa-caret-right"></i>'],
    responsive: {
      0: {
        items: 1
      },
      400: {
        items: 2
      },
      740: {
        items: 3
      },
      940: {
        items: 4
      }
    },
    nav: true
  }
}
