import { Category } from './../../../../core/models/category';
import { Component, ElementRef, inject, input, InputSignal, OnDestroy, OnInit, output, OutputEmitterRef, signal, Signal, viewChild, WritableSignal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import Swal from 'sweetalert2';
import { CategoriesService } from '../../../../core/services/categories.service';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { RouterLink } from "@angular/router";
import { TranslatePipe } from '@ngx-translate/core';
import { ProblemTypesOfCategoryForSpecificTechnician } from '../../../technician/models/problem-types-of-category-for-specific-technician';

@Component({
  selector: 'app-card',
  imports: [ReactiveFormsModule, RouterLink, TranslatePipe],
  templateUrl: './card.component.html',
  styleUrl: './card.component.css',
})
export class CardComponent {

  private readonly categoriesService = inject(CategoriesService);
  private readonly toastrService = inject(ToastrService);
  private readonly fb = inject(FormBuilder);

  category: InputSignal<Category | ProblemTypesOfCategoryForSpecificTechnician> = input.required<Category | ProblemTypesOfCategoryForSpecificTechnician>();
  edit: OutputEmitterRef<Category> = output<Category>()
  delete: OutputEmitterRef<number> = output<number>()
  // deleteCategoryForTechnician: OutputEmitterRef<number> = output<number>()

  closeBtn: Signal<ElementRef<HTMLButtonElement> | undefined> = viewChild<ElementRef>('closeBtn');
  showActions: InputSignal<boolean> = input.required<boolean>();




  isTechnicianCategory(item: Category | ProblemTypesOfCategoryForSpecificTechnician)
    : item is ProblemTypesOfCategoryForSpecificTechnician {
    return 'technicianCategoryId' in item;
  }


  onDelete(item: Category | ProblemTypesOfCategoryForSpecificTechnician) {
    if (this.isTechnicianCategory(item)) {
      this.delete.emit(item.technicianCategoryId);
    } else {
      this.delete.emit(item.id);
    }
  }
}


