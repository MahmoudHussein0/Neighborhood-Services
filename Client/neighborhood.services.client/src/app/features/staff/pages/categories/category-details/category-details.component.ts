import { CurrencyPipe } from '@angular/common';
import { Component, ElementRef, inject, OnDestroy, OnInit, Signal, signal, viewChildren, WritableSignal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import Swal from 'sweetalert2';
import { CategoriesService } from '../../../../../core/services/categories.service';
import { ProblemTypeService } from '../../../../../core/services/problem-type.service';
import { CategoryDetails, ProblemTypes } from '../../../models/category-details';
import { TranslatePipe } from '@ngx-translate/core';


@Component({
  selector: 'app-category-details',
  imports: [CurrencyPipe, ReactiveFormsModule, TranslatePipe],
  templateUrl: './category-details.component.html',
  styleUrl: './category-details.component.css',
})
export class CategoryDetailsComponent implements OnInit, OnDestroy {

  private readonly categoriesService = inject(CategoriesService);
  private readonly problemTypeService = inject(ProblemTypeService);
  private readonly toastrService = inject(ToastrService);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  categoryDetails: WritableSignal<CategoryDetails> = signal<CategoryDetails>({} as CategoryDetails);
  closBtn: Signal<readonly ElementRef<HTMLButtonElement>[]> = viewChildren<ElementRef<HTMLButtonElement>>('closeBtn');
  loadFlag: WritableSignal<boolean> = signal<boolean>(false);
  $UpdateSub: Subscription = new Subscription();
  $SubUpdate: Subscription = new Subscription();

  addProblemForm!: FormGroup;

  categoryId!: number;
  ProblemTypeId!: number;

  updateModalForm = this.fb.group({
    descriptionEn: ['', [Validators.required]],
    minPrice: [0, [Validators.required]],
    maxPrice: [0, [Validators.required]],
  })


  ngOnInit(): void {
    this.getCategoryId();
    this.getCategoryDetails();
    console.log((this.categoryId));

    this.addProblemForm = this.fb.group({
      nameEn: ['', Validators.required],
      nameAr: ['', Validators.required],
      descriptionEn: ['', [Validators.required]],
      descriptionAr: ['', Validators.required],
      minPrice: [0, [Validators.required]],
      maxPrice: [0, [Validators.required]],
      categoryId: [this.categoryId]
    })
  }

  getCategoryId(): void {
    this.activatedRoute.paramMap.subscribe({
      next: (urlParams => {
        this.categoryId = Number(urlParams.get("categoryId"));
      })
    })
  }

  getCategoryDetails(): void {
    const lang = localStorage.getItem("lang") || "en";
    this.categoriesService.getCategoryDetails(this.categoryId, lang).subscribe({
      next: (res => {
        console.log((res));
        if (res) {
          this.categoryDetails.set(res);
        }
      }),
      error: (error => {
      })
    })
  }



  AddProblemType(): void {
    console.log(this.addProblemForm.value);

    if (this.addProblemForm.valid) {
      this.$SubUpdate.unsubscribe();
      this.loadFlag.set(true);
      this.problemTypeService.add(this.addProblemForm.value).subscribe({
        next: (res => {
          this.loadFlag.set(false);
          if (res) {
            this.toastrService.success("Problem added successfully", "NS");
            this.addProblemForm.reset();
            this.closeModal();
            this.getCategoryDetails();
          }
        }),
        error: (error => {
          console.log(error);

          this.loadFlag.set(false);
          this.toastrService.error(error.error.detail);
        })
      })
    }
    else {
      this.addProblemForm.markAllAsTouched();
    }
  }




  updateModal(problemType: ProblemTypes): void {
    this.updateModalForm.patchValue({
      descriptionEn: problemType.description,
      minPrice: (problemType.minPrice),
      maxPrice: (problemType.maxPrice),
    })
    this.ProblemTypeId = problemType.id;
  }


  updateProblemtype(): void {
    if (this.updateModalForm.valid) {
      this.loadFlag.set(true);
      this.$UpdateSub.unsubscribe;

      this.$UpdateSub = this.problemTypeService.updateProblemType(this.updateModalForm.value, this.ProblemTypeId).subscribe({
        next: (res => {
          console.log(res);
          this.toastrService.success("Problem Updated Successfull");
          this.closeModal();
          this.loadFlag.set(false);
          this.getCategoryDetails();
        }),
        error: (err => {

          this.toastrService.error(err.error.detail)
          console.log(err);

          this.loadFlag.set(false);

        })
      })
    }
  }





  confirmDelete() {
    this.problemTypeService.delete(this.ProblemTypeId).subscribe({
      next: (res => {
        if (res) {
          this.toastrService.success("Problem deleted successfull");
          this.closeModal();
          this.getCategoryDetails();
        }

      }),
      error: (err => {
        this.toastrService.error("Can't Delete Problem");
      })
    })
  }



  closeModal(): void {
    this.closBtn().forEach(btn => btn.nativeElement.click());
  }

  ngOnDestroy(): void {
    this.$UpdateSub.unsubscribe
  }


}
