import { CurrencyPipe } from '@angular/common';
import { Component, ElementRef, inject, OnDestroy, OnInit, Signal, signal, viewChild, viewChildren, WritableSignal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { firstValueFrom, skip, Subject, Subscription, takeUntil } from 'rxjs';
import { CategoriesService } from '../../../../../core/services/categories.service';
import { ProblemTypeService } from '../../../../../core/services/problem-type.service';
import { CategoryDetails, ProblemTypes } from '../../../models/category-details';
import { TranslatePipe } from '@ngx-translate/core';
import { LangService } from '../../../../../core/services/lang.service';
import { DeleteComponent } from "../../../../../shared/components/delete/delete.component";
import { UploadService } from '../../../../../shared/services/upload.service';


@Component({
  selector: 'app-category-details',
  imports: [ReactiveFormsModule, TranslatePipe, DeleteComponent],
  templateUrl: './category-details.component.html',
  styleUrl: './category-details.component.css',
})
export class CategoryDetailsComponent implements OnInit, OnDestroy {

  private readonly categoriesService = inject(CategoriesService);
  private readonly problemTypeService = inject(ProblemTypeService);
  private readonly toastrService = inject(ToastrService);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly langService = inject(LangService);
  private readonly fb = inject(FormBuilder);
  private readonly uploadService = inject(UploadService);

  categoryDetails: WritableSignal<CategoryDetails> = signal<CategoryDetails>({} as CategoryDetails);
  closBtn: Signal<readonly ElementRef<HTMLButtonElement>[]> = viewChildren<ElementRef<HTMLButtonElement>>('closeBtn');
  deleteModal = viewChild(DeleteComponent);
  loadFlag: WritableSignal<boolean> = signal<boolean>(false);
  isLoadingAPI: WritableSignal<boolean> = signal<boolean>(false);
  destroy$ = new Subject<void>()
  ImagePreview: string | null = null;

  selectedFile !: File;

  mode!: "Edit" | "Add";

  categoryId!: number;
  selectedProblem!: ProblemTypes | null;

  form = this.fb.group({
    nameEn: ['', Validators.required],
    nameAr: ['', Validators.required],
    descriptionEn: ['', [Validators.required]],
    descriptionAr: ['', Validators.required],
    minPrice: [0, [Validators.min(1)]],
    maxPrice: [0, [Validators.min(1)]],
    ImageUrl: [''],
    categoryId: [this.categoryId]
  })


  ngOnInit(): void {
    this.getCategoryId();
    this.changeLanguage();
  }





  getCategoryId(): void {
    this.activatedRoute.paramMap.subscribe({
      next: (urlParams => {
        this.categoryId = Number(urlParams.get("categoryId"));
      })
    })
  }


  changeLanguage(): void {
    this.langService.lang$
      .pipe(takeUntil(this.destroy$))
      .subscribe((lang) => {
        this.getCategoryDetails();
      });
  }



  getCategoryDetails(): void {
    this.isLoadingAPI.set(true);
    this.categoriesService.getCategoryDetails(this.categoryId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res => {
        console.log((res));
        this.categoryDetails.set(res);
        this.isLoadingAPI.set(false);
      }),
      error: (error => {
        this.isLoadingAPI.set(false);
      })
    })
  }



  uploadImage(event: Event): void {
    const InputFile = event.target as HTMLInputElement;
    if (!InputFile?.files?.length) return;
    this.selectedFile = InputFile?.files[0];
    this.ImagePreview = URL.createObjectURL(InputFile.files[0]);
  }




  updateModal(problemType: ProblemTypes): void {
    this.mode = "Edit";
    this.form.patchValue({
      nameAr: problemType.nameAr,
      nameEn: problemType.nameEn,
      descriptionAr: problemType.descriptionAr,
      descriptionEn: problemType.descriptionEn,
      minPrice: problemType.minPrice,
      maxPrice: problemType.maxPrice,
      ImageUrl: problemType.imageUrl
    })
    this.selectedProblem = problemType;
    this.ImagePreview = null;
  }


  addModal(): void {
    this.mode = "Add";
    this.form.reset();
    this.form.patchValue({
      categoryId: this.categoryId
    })
    this.selectedProblem = null;
    this.ImagePreview = null;
  }


  async save() {
    if (this.form.valid) {
      const value = this.form.value;
      if (this.selectedFile) {
        this.loadFlag.set(true);
        const secretUrl = await firstValueFrom(this.uploadService.upload(this.selectedFile))
        value.ImageUrl = secretUrl;
        console.log(secretUrl);
        this.loadFlag.set(false);

      }




      if (this.mode === "Edit") {
        const hasChanged =
          this.selectedProblem?.descriptionAr == value.descriptionAr &&
          this.selectedProblem?.descriptionEn == value.descriptionEn &&
          this.selectedProblem?.nameAr == value.nameAr &&
          this.selectedProblem?.nameEn == value.nameEn &&
          this.selectedProblem?.minPrice == value.minPrice &&
          this.selectedProblem?.maxPrice == value.maxPrice &&
          this.selectedProblem?.imageUrl == value.ImageUrl;

        if (hasChanged) {
          this.closeModal();
          return;
        }
        this.updateProblemtype(value);
      }

      else
        this.AddProblemType(value);
    }




    else {
      this.form.markAllAsTouched();
    }
  }



  AddProblemType(value: object): void {
    this.loadFlag.set(true);

    this.form.patchValue({
      categoryId: this.categoryId
    });
    this.problemTypeService.add(value).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res => {
        if (res) {
          this.toastrService.success("Problem added successfully");
          this.form.reset();
          this.closeModal();
          this.getCategoryDetails();
          this.loadFlag.set(false);

        }
      }),
      error: (error => {
        console.log(error);
        this.loadFlag.set(false);

      })
    })

  }









  updateProblemtype(value: object): void {
    this.loadFlag.set(true);

    console.log(value);

    this.problemTypeService.updateProblemType(value, this.selectedProblem?.id!).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res => {
        console.log(res);
        this.toastrService.success("Problem Updated Successfull");
        this.closeModal();
        this.getCategoryDetails();
        this.loadFlag.set(false);


      }),
      error: (err => {
        console.log(err);
        this.loadFlag.set(false);

      })
    })
  }





  confirmDelete() {
    this.problemTypeService.delete(this.selectedProblem?.id!).subscribe({
      next: (res => {
        if (res) {
          this.toastrService.show("Problem deleted successfull");
          this.getCategoryDetails();
          this.deleteModal()?.close();
        }
      }),
      error: (err => {
      })
    })
  }



  closeModal(): void {
    this.closBtn().forEach(btn => btn.nativeElement.click());
  }

  ngOnDestroy(): void {

    this.destroy$.next();
    this.destroy$.complete();

  }


}
