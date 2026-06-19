import { Component, ElementRef, inject, OnDestroy, OnInit, signal, viewChild, WritableSignal } from '@angular/core';
import { ProblemTypesOfCategoryForTechnicianService } from '../../services/problem-types-of-category-for-technician.service';
import { Category } from '../../../../core/models/category';
import { ProblemTypesOfCategoryForSpecificTechnician } from '../../models/problem-types-of-category-for-specific-technician';
import { TranslatePipe } from '@ngx-translate/core';
import { CardComponent } from "../../../staff/components/card/card.component";
import { LangService } from '../../../../core/services/lang.service';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoriesService } from '../../../../core/services/categories.service';
import { ToastrService } from 'ngx-toastr';
import { DeleteComponent } from "../../../../shared/components/delete/delete.component";
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-your-categories',
  imports: [TranslatePipe, CardComponent, ReactiveFormsModule, DeleteComponent],
  templateUrl: './Technician-categories.component.html',
  styleUrl: './Technician-categories.component.css',
})
export class TechnicianCategoriesComponent implements OnInit, OnDestroy {


  private readonly ProblemTypesOfCategoryForTechnicianService = inject(ProblemTypesOfCategoryForTechnicianService);
  private readonly langService = inject(LangService);
  private readonly fb = inject(FormBuilder);
  private readonly categoriesService = inject(CategoriesService);
  private readonly toastrService = inject(ToastrService);

  categories: WritableSignal<ProblemTypesOfCategoryForSpecificTechnician[]> = signal<ProblemTypesOfCategoryForSpecificTechnician[]>([])
  allCategories: WritableSignal<Category[]> = signal<Category[]>([])
  deleteModal = viewChild(DeleteComponent);
  destroy$: Subject<void> = new Subject<void>();



  isLoadingAPI: WritableSignal<boolean> = signal<boolean>(false);
  loadFlag: WritableSignal<boolean> = signal<boolean>(false);
  closeBtn = viewChild<ElementRef>('closeBtn');

  technicianCategoryId!: number;
  form = this.fb.group({
    CategoryId: [-1, [Validators.required, Validators.min(1)]]
  })


  ngOnInit(): void {
    this.getTechnicianCategory();
  }


  getTechnicianCategory(): void {
    this.isLoadingAPI.set(true);
    this.ProblemTypesOfCategoryForTechnicianService.getProblemTypesOfCategory().pipe(takeUntil(this.destroy$)).subscribe({
      next: (res => {
        this.categories.set(res);
        console.log(res);
        this.isLoadingAPI.set(false);
        this.getAllCategories()
      }), error: (err => {
        this.isLoadingAPI.set(false);
      })
    })
  }

  getAllCategories(): void {
    this.categoriesService.getAllCategories().pipe(takeUntil(this.destroy$)).subscribe({
      next: (res => {
        // console.log(res);
        const assignedCategoryId = this.categories().map(c => c.id);
        console.log(res.filter((c: Category) => !assignedCategoryId.includes(c.id)));
        this.allCategories.set(res.filter((c: Category) => !assignedCategoryId.includes(c.id)));
      })
    })
  }


  save(): void {
    if (this.form.valid) {

      const value = this.form.value;
      console.log(value);
      this.loadFlag.set(true);
      this.ProblemTypesOfCategoryForTechnicianService.assignTechnicianToCategory(value).pipe(takeUntil(this.destroy$)).subscribe({
        next: (res => {
          console.log(res);
          this.toastrService.success("Assigned to category is successed");
          this.closeBtn()?.nativeElement.click();
          this.getTechnicianCategory();
          // this.getAllCategories();
          this.loadFlag.set(false);
        }),
        error: (err => {
          console.log(err);
          this.loadFlag.set(false);

        })
      })
    }
    else {
      console.log(this.form);

      this.form.markAllAsTouched();
    }
  }


  confirmDelete(): void {
    console.log(this.technicianCategoryId);
    this.ProblemTypesOfCategoryForTechnicianService.delete(this.technicianCategoryId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res => {
        this.toastrService.show("You no longer Assigned to this category");
        this.getTechnicianCategory();
        // this.getAllCategories();
        this.deleteModal()?.close();
      })



    })

  }


  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }














}
