import { Category } from './../../../../../core/models/category';

import { Component, computed, ElementRef, inject, OnDestroy, OnInit, Signal, signal, viewChild, viewChildren, WritableSignal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { skip, Subscription } from 'rxjs';
import { CategoriesService } from '../../../../../core/services/categories.service';
import { CardComponent } from '../../../components/card/card.component';
import { TranslatePipe } from '@ngx-translate/core';
import { LangService } from '../../../../../core/services/lang.service';
import { DeleteComponent } from "../../../../../shared/components/delete/delete.component";


@Component({
  selector: 'app-categories',
  imports: [CardComponent, ReactiveFormsModule, TranslatePipe, DeleteComponent],
  templateUrl: './category.component.html',
  styleUrl: './category.component.css',
})
export class CategoryComponent implements OnInit, OnDestroy {

  private readonly categoriesService = inject(CategoriesService);
  private readonly toastrService = inject(ToastrService);
  private readonly fb = inject(FormBuilder);
  private readonly langService = inject(LangService);

  $SubUpdate: Subscription = new Subscription();
  $SubAdd: Subscription = new Subscription();
  loadFlag: WritableSignal<boolean> = signal<boolean>(false);
  isLoadingAPI = signal<boolean>(false);

  deleteModal = viewChild(DeleteComponent);
  closBtn: Signal<readonly ElementRef<HTMLButtonElement>[]> = viewChildren<ElementRef<HTMLButtonElement>>('closeBtn');
  categories: Signal<Category[]> = computed(() => this.categoriesService.categories());

  selectedCategory!: Category;
  categoryId!: number;

  updateCategoryForm = this.fb.group({
    nameEn: ['', Validators.required],
    nameAr: ['', Validators.required],
    icon: ['', Validators.required],
    image: ['']
  });


  addCategoryForm = this.fb.group({
    nameEn: ['', Validators.required],
    nameAr: ['', Validators.required],
    icon: ['', Validators.required],
    image: ['']
  });

  ngOnInit(): void {
    this.langService.lang$
      .subscribe(() => {
        this.getAllCategories();
      });
  }


  getAllCategories(): void {
    this.isLoadingAPI.set(true);
    this.categoriesService.getAllCategories().subscribe({
      next: (res => {
        this.categoriesService.categories.set(res);
        this.isLoadingAPI.set(false);
        console.log(res);
      }),
      error: (err => {
        this.isLoadingAPI.set(false);
        console.log(err);
      })
    })
  }

  updateModal(category: Category) {
    this.selectedCategory = category;
    console.log(this.selectedCategory);
    this.updateCategoryForm.patchValue({
      nameEn: category.nameEn,
      nameAr: category.nameAr,
      icon: category.icon,
      image: category.image
    });
  }

  updateCategory(): void {
    if (this.updateCategoryForm.valid) {
      const value = this.updateCategoryForm.value;
      this.$SubUpdate.unsubscribe();
      const hasChanges = value.nameAr == this.selectedCategory.nameAr &&
        value.nameEn == this.selectedCategory.nameEn &&
        value.icon == this.selectedCategory.icon &&
        value.image == this.selectedCategory.image;



      if (hasChanges) {

        this.closeModal();
        return;
      }

      this.loadFlag.set(true)
      this.$SubUpdate = this.categoriesService.updateCategory(this.updateCategoryForm.value, this.selectedCategory.id).subscribe({
        next: (res => {
          if (res) {
            this.loadFlag.set(true);
            this.toastrService.success("Category updated successfully");
            this.closeModal();
            this.categoriesService.categories.update(categories => res);
            this.getAllCategories();
            this.loadFlag.set(false);
          }
        }),
        error: (err => {
          this.loadFlag.set(false);
        })
      })
    }
    else {
      this.updateCategoryForm.markAllAsTouched();
    }
  }


  AddCategory(): void {
    if (this.addCategoryForm.valid) {
      this.$SubAdd.unsubscribe();
      this.loadFlag.set(true);
      this.$SubAdd = this.categoriesService.AddCategory(this.addCategoryForm.value).subscribe({
        next: (res => {
          this.loadFlag.set(false);
          if (res) {
            this.toastrService.success("Category added successfully");
            this.addCategoryForm.reset();
            this.closeModal();
            this.getAllCategories();
          }
        }),
        error: (error => {
          this.loadFlag.set(false);
        })
      })
    }
    else {
      this.addCategoryForm.markAllAsTouched();
    }
  }


  confirmDelete() {
    console.log(this.categoryId);
    this.categoriesService.deletCategory(this.categoryId).subscribe({
      next: (res) => {
        if (res) {
          this.toastrService.show('Category Deleted Successful');
          this.categoriesService.categories.update(categories =>
            categories.filter(c => c.id !== this.categoryId)
          );

          this.deleteModal()?.close();



        }
      },
      error: (err) => {
      }
    });
  }


  closeModal(): void {
    this.closBtn().forEach(btn => btn.nativeElement.click());
  }

  ngOnDestroy(): void {
    this.$SubUpdate.unsubscribe();
    this.$SubAdd.unsubscribe();
  }
}
