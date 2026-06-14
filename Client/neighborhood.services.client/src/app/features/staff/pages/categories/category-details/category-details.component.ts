import { CurrencyPipe } from '@angular/common';
import { Component, ElementRef, inject, OnDestroy, OnInit, Signal, signal, viewChild, viewChildren, WritableSignal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { skip, Subscription } from 'rxjs';
import { CategoriesService } from '../../../../../core/services/categories.service';
import { ProblemTypeService } from '../../../../../core/services/problem-type.service';
import { CategoryDetails, ProblemTypes } from '../../../models/category-details';
import { TranslatePipe } from '@ngx-translate/core';
import { LangService } from '../../../../../core/services/lang.service';
import { DeleteComponent } from "../../../../../shared/components/delete/delete.component";


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

  categoryDetails: WritableSignal<CategoryDetails> = signal<CategoryDetails>({} as CategoryDetails);
  closBtn: Signal<readonly ElementRef<HTMLButtonElement>[]> = viewChildren<ElementRef<HTMLButtonElement>>('closeBtn');
  deleteModal = viewChild(DeleteComponent);
  loadFlag: WritableSignal<boolean> = signal<boolean>(false);
  isLoadingAPI: WritableSignal<boolean> = signal<boolean>(false);
  $UpdateSub: Subscription = new Subscription();
  $addSub: Subscription = new Subscription();

  addProblemForm!: FormGroup;

  categoryId!: number;
  selectedProblem!: ProblemTypes;

  updateModalForm = this.fb.group({
    nameEn: ['', Validators.required],
    nameAr: ['', Validators.required],
    descriptionEn: ['', [Validators.required]],
    descriptionAr: ['', Validators.required],
    minPrice: [0, [Validators.min(1)]],
    maxPrice: [0, [Validators.min(1)]],
  })


  ngOnInit(): void {
    this.getCategoryId();
    this.getCategoryDetails();
    this.addProblemForm = this.fb.group({
      nameEn: ['', Validators.required],
      nameAr: ['', Validators.required],
      descriptionEn: ['', [Validators.required]],
      descriptionAr: ['', Validators.required],
      minPrice: [0, [Validators.min(1)]],
      maxPrice: [0, [Validators.min(1)]],
      categoryId: [this.categoryId]
    })
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
      .pipe(skip(1))
      .subscribe((lang) => {
        this.getCategoryDetails();
      });
  }



  getCategoryDetails(): void {
    this.isLoadingAPI.set(true);
    this.categoriesService.getCategoryDetails(this.categoryId).subscribe({
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



  AddProblemType(): void {
    console.log(this.addProblemForm);
    if (this.addProblemForm.valid) {
      this.addProblemForm.patchValue({
        categoryId: this.categoryId
      });


      this.$addSub.unsubscribe();
      this.loadFlag.set(true);
      this.$addSub = this.problemTypeService.add(this.addProblemForm.value).subscribe({
        next: (res => {
          this.loadFlag.set(false);
          if (res) {
            this.toastrService.success("Problem added successfully");
            this.addProblemForm.reset();
            this.closeModal();
            this.getCategoryDetails();
          }
        }),
        error: (error => {
          console.log(error);
          this.loadFlag.set(false);
        })
      })
    }
    else {
      this.addProblemForm.markAllAsTouched();
    }
  }




  updateModal(problemType: ProblemTypes): void {
    this.updateModalForm.patchValue({
      nameAr: problemType.nameAr,
      nameEn: problemType.nameEn,
      descriptionAr: problemType.descriptionAr,
      descriptionEn: problemType.descriptionEn,
      minPrice: problemType.minPrice,
      maxPrice: problemType.maxPrice,
    })
    this.selectedProblem = problemType;
  }





  updateProblemtype(): void {
    if (this.updateModalForm.valid) {
      const value = this.updateModalForm.value;
      this.$UpdateSub.unsubscribe();

      const hasChanged =
        this.selectedProblem.descriptionAr == value.descriptionAr &&
        this.selectedProblem.descriptionEn == value.descriptionEn &&
        this.selectedProblem.nameAr == value.nameAr &&
        this.selectedProblem.nameEn == value.nameEn &&
        this.selectedProblem.minPrice == value.minPrice &&
        this.selectedProblem.maxPrice == value.maxPrice


      if (hasChanged) {
        this.closeModal();
        return;
      }





      this.loadFlag.set(true);
      this.$UpdateSub = this.problemTypeService.updateProblemType(value, this.selectedProblem.id).subscribe({
        next: (res => {
          console.log(res);
          this.toastrService.success("Problem Updated Successfull");
          this.closeModal();
          this.loadFlag.set(false);
          this.getCategoryDetails();

        }),
        error: (err => {
          console.log(err);
          this.loadFlag.set(false);
        })
      })
    } else {
      this.updateModalForm.markAllAsTouched();
    }
  }





  confirmDelete() {
    this.problemTypeService.delete(this.selectedProblem.id).subscribe({
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
    this.$UpdateSub.unsubscribe()
    this.$addSub.unsubscribe()

  }


}
