
import { Component, computed, ElementRef, inject, OnDestroy, OnInit, Signal, signal, viewChildren, WritableSignal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { Category } from '../../../../../core/models/category';
import { CategoriesService } from '../../../../../core/services/categories.service';
import { CardComponent } from '../../../components/card/card.component';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';


@Component({
  selector: 'app-categories',
  imports: [CardComponent, ReactiveFormsModule, TranslatePipe],
  templateUrl: './category.component.html',
  styleUrl: './category.component.css',
})
export class CategoryComponent implements OnInit, OnDestroy {

  private readonly categoriesService = inject(CategoriesService);
  private readonly translate = inject(TranslateService);
  private readonly toastrService = inject(ToastrService);
  private readonly fb = inject(FormBuilder);

  $SubUpdate: Subscription = new Subscription();
  $SubAdd: Subscription = new Subscription();
  loadFlag: WritableSignal<boolean> = signal<boolean>(false);
  closBtn: Signal<readonly ElementRef<HTMLButtonElement>[]> = viewChildren<ElementRef<HTMLButtonElement>>('closeBtn');
  categories: Signal<Category[]> = computed(() => this.categoriesService.categories());

  selectedCategory!: Category;

  updateCategoryForm = this.fb.group({
    nameEn: ['', Validators.required],
    icon: ['', Validators.required]
  });


  addCategoryForm = this.fb.group({
    nameEn: ['', Validators.required],
    nameAr: ['', Validators.required],
    icon: ['', Validators.required]
  });





  ngOnInit(): void {
    this.getAllCategories();


    console.log('Current Lang:', this.translate.currentLang);
    console.log('Default Lang:', this.translate.getDefaultLang());
    console.log('Local Storage:', localStorage.getItem('lang'));
  }

  getAllCategories(): void {
    // const lang = localStorage.getItem('lang') || 'en';
    this.categoriesService.getAllCategories().subscribe({
      next: (res => {
        this.categoriesService.categories.set(res);
        console.log(res);
      }),
      error: (err => {
        console.log(err);
      })
    })
  }

  updateModal(category: Category) {
    this.selectedCategory = category;
    console.log(this.selectedCategory);
    this.updateCategoryForm.patchValue({
      nameEn: category.name,
      icon: category.icon
    });
  }

  updateCategory(): void {
    if (this.updateCategoryForm.valid) {
      console.log(this.updateCategoryForm.value);
      this.$SubUpdate.unsubscribe();
      this.loadFlag.set(true);
      this.$SubUpdate = this.categoriesService.updateCategory(this.updateCategoryForm.value, this.selectedCategory.id).subscribe({
        next: (res => {
          if (res) {
            this.loadFlag.set(false);
            this.toastrService.success("Category updated Successfully", "NS");
            this.closeModal();
            this.getAllCategories();
          }
        }),
        error: (err => {
          this.loadFlag.set(false);
          this.toastrService.error(err.error.detail);
        })
      })
    }
  }


  AddCategory(): void {
    if (this.addCategoryForm.valid) {
      this.$SubAdd.unsubscribe();
      this.loadFlag.set(true);
      this.categoriesService.AddCategory(this.addCategoryForm.value).subscribe({
        next: (res => {
          this.loadFlag.set(false);
          if (res) {
            this.toastrService.success("Category added successfully", "NS");
            this.addCategoryForm.reset();
            this.closeModal();
            this.getAllCategories();
          }
        }),
        error: (error => {
          this.loadFlag.set(false);
          this.toastrService.error(error.error.detail);
        })
      })
    }
    else {
      this.addCategoryForm.markAllAsTouched();
    }
  }


  closeModal(): void {
    this.closBtn().forEach(btn => btn.nativeElement.click());
  }


  ngOnDestroy(): void {
    this.$SubUpdate.unsubscribe();
  }
}
