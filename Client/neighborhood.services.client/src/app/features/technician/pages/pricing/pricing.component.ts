import { DeleteComponent } from './../../../../shared/components/delete/delete.component';
import { Pricing } from './../../models/pricing';
import { Component, ElementRef, inject, OnDestroy, OnInit, Signal, signal, viewChild, viewChildren, WritableSignal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PricingService } from '../../services/pricing.service';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProblemTypeService } from '../../../../core/services/problem-type.service';
import { ProblemTypes } from '../../../staff/models/category-details';
import { ToastrService } from 'ngx-toastr';
import Swal from 'sweetalert2';
import { Subscription } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { LangService } from '../../../../core/services/lang.service';

@Component({
  selector: 'app-pricing',
  imports: [ReactiveFormsModule, TranslatePipe, DeleteComponent],
  templateUrl: './pricing.component.html',
  styleUrl: './pricing.component.css',
})
export class PricingComponent implements OnInit, OnDestroy {

  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly pricingService = inject(PricingService);
  private readonly problemTypeService = inject(ProblemTypeService);
  private readonly toastrService = inject(ToastrService);
  private readonly langService = inject(LangService);
  private readonly fb = inject(FormBuilder);

  pricingId!: number;
  isEditMode: boolean = false;
  existingprice: Pricing | null = null;
  $Sub: Subscription = new Subscription();
  lang: string | null = localStorage.getItem("lang");

  deleteModal = viewChild(DeleteComponent);
  closeBtn: Signal<readonly ElementRef<HTMLButtonElement>[]> = viewChildren<ElementRef<HTMLButtonElement>>('closeBtn');
  pricing: WritableSignal<Pricing[]> = signal<Pricing[]>([]);
  problemTypes: WritableSignal<ProblemTypes[]> = signal<ProblemTypes[]>([]);

  pricingForm = this.fb.group({
    problemTypeId: [0, [Validators.required]],
    techMinPrice: [0, [Validators.required]],
    techMaxPrice: [0, [Validators.required]]
  });

  ngOnInit(): void {



    this.langService.lang$
      .subscribe(() => {
        this.getPricing();
        this.getProblemTypes();

      });





  }


  getPricing(): void {

    this.pricingService.getPricing().subscribe({
      next: (res => {
        console.log(res);
        this.pricing.set(res);
      })

    })
  }


  getProblemTypes(): void {
    this.problemTypeService.getProblemTypes().subscribe({
      next: (res => {
        console.log(res);
        this.problemTypes.set(res);

      })
    })
  }


  editModal(pricing: Pricing): void {
    this.isEditMode = true;
    this.pricingForm.patchValue({
      problemTypeId: pricing.problemTypeId,
      techMinPrice: pricing.techMinPrice,
      techMaxPrice: pricing.techMaxPrice
    })
    this.existingprice = pricing;
  }


  addModal(): void {
    this.isEditMode = false;
    this.pricingForm.reset();
  }

  savePricing(): void {
    const value = this.pricingForm.value;

    if (this.pricingForm.valid) {
      if (this.isEditMode) {
        //edit
        const hasChanged =
          this.existingprice?.problemTypeId == value.problemTypeId &&
          this.existingprice?.techMinPrice == value.techMinPrice &&
          this.existingprice?.techMaxPrice == value.techMaxPrice;
        if (hasChanged) {

          if (this.lang == "en") {

            this.toastrService.info('No changes detected', 'NS');
          } else {

            this.toastrService.info('لم يتم رصد أي تغييرات', 'NS');
          }
          this.closeModal();
          return;
        }
        this.$Sub.unsubscribe();
        this.$Sub = this.pricingService.updatePricing(this.existingprice?.id, value).subscribe({
          next: (res => {
            console.log(res);
            this.toastrService.success("Pricing updated successfully");
            this.closeModal();
            this.getPricing();

          }),
          error: (err => {
          })
        })
      }

      else {
        //add
        this.$Sub = this.pricingService.addPricing(this.pricingForm.value).subscribe({
          next: (res => {
            console.log(res);
            this.toastrService.success("Pricing added successfully");
            this.closeModal();
            this.getPricing();

          }),
          error: (err => {
            console.log(err);
            this.toastrService.error(err.error.detail);

          })
        })
      }



    }

  }


  confirmDelete(): void {
    this.pricingService.deletePricing(this.pricingId).subscribe({
      next: (res => {
        console.log(res);
        if (res) {
          this.toastrService.show("your Price for this problem deleted successfully");
          this.getPricing();
          this.deleteModal()?.close();
        }

      }),
      error: (err => {

      })
    })
  }


  closeModal(): void {
    this.closeBtn()?.forEach(btn => btn.nativeElement.click())
  }


  ngOnDestroy(): void {
    this.$Sub.unsubscribe();
  }
}