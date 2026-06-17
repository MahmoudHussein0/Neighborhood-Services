import { Category } from './../../../../../core/models/category';

import { Component, computed, ElementRef, inject, OnDestroy, OnInit, Signal, signal, viewChild, viewChildren, WritableSignal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { firstValueFrom, skip, Subject, Subscription, takeUntil } from 'rxjs';
import { CategoriesService } from '../../../../../core/services/categories.service';
import { CardComponent } from '../../../components/card/card.component';
import { TranslatePipe } from '@ngx-translate/core';
import { LangService } from '../../../../../core/services/lang.service';
import { DeleteComponent } from "../../../../../shared/components/delete/delete.component";
import { UploadService } from '../../../../../shared/services/upload.service';
import { ActivatedRoute } from '@angular/router';

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
  private readonly uploadService = inject(UploadService);
  private readonly destroy$ = new Subject<void>();


  loadFlag: WritableSignal<boolean> = signal<boolean>(false);
  ImagePreview: string | null = null;
  closBtn: Signal<readonly ElementRef<HTMLButtonElement>[]> = viewChildren<ElementRef<HTMLButtonElement>>('closeBtn');
  categories: Signal<Category[]> = computed(() => this.categoriesService.categories());


  isLoadingAPI = signal<boolean>(false);
  selectedCategory!: Category | null;
  categoryId!: number;
  selectedFile !: File;
  deleteModal = viewChild(DeleteComponent);
  mode!: "Edit" | "Add";


  form = this.fb.group({
    nameEn: ['', Validators.required],
    nameAr: ['', Validators.required],
    icon: ['', Validators.required],
    image: ['']
  });


  ngOnInit(): void {
    this.langService.lang$
      .pipe(takeUntil(this.destroy$)).subscribe(() => {
        this.getAllCategories();
      });
  }


  getAllCategories(): void {
    this.isLoadingAPI.set(true);
    this.categoriesService.getAllCategories().pipe(takeUntil(this.destroy$)).subscribe({
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
    this.mode = "Edit";
    this.selectedCategory = category;
    console.log(this.selectedCategory);
    this.form.patchValue({
      nameEn: category.nameEn,
      nameAr: category.nameAr,
      icon: category.icon,
      image: category.image
    });
    this.ImagePreview = null;

  }

  addModal(): void {
    this.mode = "Add";
    this.form.reset();
    this.selectedCategory = null;
    this.ImagePreview = null;
  }


  uploadImage(event: Event) {
    const inputFile = event.target as HTMLInputElement;
    if (!inputFile?.files?.length) return;
    this.selectedFile = inputFile.files[0];
    this.ImagePreview = URL.createObjectURL(inputFile.files[0]);
  }



  async save(): Promise<void> {
    console.log(this.form.value);
    if (this.form.valid) {
      const value = this.form.value;

      if (this.selectedFile) {
        this.loadFlag.set(true);

        const secrtUrl = await firstValueFrom(this.uploadService.upload(this.selectedFile))
        value.image = secrtUrl;
        this.loadFlag.set(false);

      }

      if (this.mode === "Edit") {
        const hasChanges = value.nameAr == this.selectedCategory?.nameAr &&
          value.nameEn == this.selectedCategory?.nameEn &&
          value.icon == this.selectedCategory?.icon &&
          value.image == this.selectedCategory?.image;
        if (hasChanges) {
          this.closeModal();
          return;
        }
        this.updateCategory(value);
      }

      else
        this.AddCategory(value)
    }
    else {
      this.form.markAllAsTouched();
    }
  }
  updateCategory(value: object): void {
    console.log(value);

    this.loadFlag.set(true);

    this.categoriesService.updateCategory(value, this.selectedCategory?.id!).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res => {
        if (res) {
          this.toastrService.success("Category updated successfully");
          this.closeModal();
          this.getAllCategories();

          this.loadFlag.set(false);

        }
      }),
      error: (err => {
        console.log(err);
        this.loadFlag.set(false);


      })
    })
  }






  AddCategory(value: object): void {
    console.log(value);
    this.loadFlag.set(true);


    this.categoriesService.AddCategory(value).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res => {
        if (res) {
          this.toastrService.success("Category added successfully");
          this.form.reset();
          this.closeModal();
          this.getAllCategories();
          this.loadFlag.set(false);

        }
      }),
      error: (error => {
        this.loadFlag.set(false);

      })
    })
  }


  confirmDelete() {
    console.log(this.categoryId);
    this.categoriesService.deletCategory(this.categoryId).pipe(takeUntil(this.destroy$)).subscribe({
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
    this.destroy$.next();
    this.destroy$.complete();
  }
}
