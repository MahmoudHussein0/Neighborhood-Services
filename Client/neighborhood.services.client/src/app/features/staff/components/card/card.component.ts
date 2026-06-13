import { Category } from './../../../../core/models/category';
import { Component, ElementRef, inject, input, InputSignal, OnDestroy, OnInit, output, OutputEmitterRef, Signal, viewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import Swal from 'sweetalert2';
import { CategoriesService } from '../../../../core/services/categories.service';
import { ToastRef, ToastrService } from 'ngx-toastr';
import { RouterLink } from "@angular/router";
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-card',
  imports: [ReactiveFormsModule, RouterLink, TranslatePipe],
  templateUrl: './card.component.html',
  styleUrl: './card.component.css',
})
export class CardComponent implements OnInit {

  private readonly categoriesService = inject(CategoriesService);
  private readonly toastrService = inject(ToastrService);
  private readonly fb = inject(FormBuilder);

  category: InputSignal<Category> = input.required<Category>();
  edit: OutputEmitterRef<Category> = output<Category>()
  closeBtn: Signal<ElementRef<HTMLButtonElement> | undefined> = viewChild<ElementRef>('closeBtn');


  updateCategoryForm!: FormGroup;
  categoryId!: number;
  ngOnInit(): void {
    this.updateCategoryForm = this.fb.group({
      name: ['', [Validators.required]],
      icon: ['', Validators.required]
    })

  }

  confirmDelete() {
    this.categoriesService.deletCategory(this.categoryId).subscribe({
      next: (res => {
        if (res) {
          this.toastrService.success("Category Deleted Successful");
          this.categoriesService.categories.update(categories => categories.filter(c => c.id !== this.categoryId))
          this.closeBtn()?.nativeElement.click()
        }
      }),
      error: (err => {
        this.toastrService.error("Can't Delete Category");
      })
    })
  }
}


