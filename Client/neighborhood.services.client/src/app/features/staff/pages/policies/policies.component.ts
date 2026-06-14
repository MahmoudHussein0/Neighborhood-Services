import { Exception } from './../../../technician/models/exception';
import { HomeComponent } from './../../../public/pages/home/home.component';
import { HttpClient } from '@angular/common/http';
import { debounce, debounceTime, distinctUntilChanged, distinctUntilKeyChanged, filter, Subscription } from 'rxjs';
import { Component, ElementRef, inject, OnInit, QueryList, Signal, signal, viewChild, viewChildren, WritableSignal } from '@angular/core';
import { PoliciesService } from '../../services/policies.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { Policies } from '../../models/policies';
import { ToastrService } from 'ngx-toastr';
import { TranslatePipe } from '@ngx-translate/core';
import { DeleteComponent } from "../../../../shared/components/delete/delete.component";

@Component({
  selector: 'app-policies',
  imports: [FormsModule, DatePipe, ReactiveFormsModule, TranslatePipe, DeleteComponent],
  templateUrl: './policies.component.html',
  styleUrl: './policies.component.css',
})
export class PoliciesComponent implements OnInit {


  private readonly policiesService = inject(PoliciesService);
  private readonly fb = inject(FormBuilder);
  private readonly toastrService = inject(ToastrService);


  filteredPolicies: WritableSignal<Policies[]> = signal<Policies[]>([]);
  filteredPolicy: WritableSignal<Policies | null> = signal<Policies | null>(null);
  hours: WritableSignal<number[]> = signal<number[]>([]);
  isLoading = signal<boolean>(false);
  uniqueHours: WritableSignal<number[]> = signal<number[]>([]);
  isEdit: WritableSignal<boolean> = signal<boolean>(true);
  deleteModal = viewChild(DeleteComponent);
  closeBtns: Signal<readonly ElementRef<HTMLButtonElement>[]> = viewChildren<ElementRef>('closeBtn')



  policyForm = this.fb.group({
    appliesTo: this.fb.control<null | string>(null, [Validators.required]),
    hoursBeforeBooking: [0, [Validators.min(1)]],
    penaltyPct: [0, [Validators.min(1)]]
  });

  filterPolicy = {
    hoursBeforeBooking: 0,
    appliesTo: 'Customer'
  }

  existingPolicy: Policies = {} as Policies;
  $Sub: Subscription = new Subscription();

  ngOnInit(): void {
    this.getPolicies();
  }


  getPolicies(): void {
    this.isLoading.set(true);
    this.policiesService.getPolicies().subscribe({
      next: (res => {
        console.log(res);
        this.hours.set(res.map(record => record.hoursBeforeBooking));
        this.uniqueHours.set([... new Set(this.hours())])
        this.filteredPolicies.set(res);
        this.isLoading.set(false);
      }),
      error: (err => {
        this.isLoading.set(false);
      })
    })
  }

  openAddModal(): void {
    this.isEdit.set(false);
    this.policyForm.reset();
  }



  applyFilter(): void {
    const params = {
      appliesTo: 'Customer',
      hoursBeforeBooking: 0
    };


    if (this.filterPolicy.appliesTo)
      params.appliesTo = this.filterPolicy.appliesTo;


    if (this.filterPolicy.hoursBeforeBooking != 0)
      params.hoursBeforeBooking = this.filterPolicy.hoursBeforeBooking;

    this.policiesService.lookUpPolicy(params).subscribe({
      next: (res => {
        console.log(res);
        this.filteredPolicy.set(res);

      }),
      error: (err => {
        console.log(err);
      })
    })

  }


  resetFilter(): void {
    this.filteredPolicy.set(null)
    this.getPolicies();
  }


  openEditModal(policy: Policies): void {
    this.isEdit.set(true);
    this.policyForm.patchValue({
      appliesTo: policy.appliesTo,
      hoursBeforeBooking: policy.hoursBeforeBooking,
      penaltyPct: policy.penaltyPct
    });
    this.existingPolicy = policy;
  }

  openDeleteModal(policy: Policies): void {
    console.log(policy);
    this.existingPolicy.id = policy.id;
  }

  savePolicy(): void {
    if (this.policyForm.valid) {
      this.$Sub.unsubscribe();
      const value = this.policyForm.value;
      console.log(this.policyForm.value);
      if (!this.isEdit()) {
        this.$Sub = this.policiesService.addPolicy(value).subscribe({
          next: (res => {
            this.toastrService.success('Policy added successfully');
            this.closeModal();
            this.getPolicies();
          }), error: (err => {
            console.log(err);

          })
        })
      } else {
        // edit
        const hasChanged =
          this.existingPolicy.appliesTo == value.appliesTo
          &&
          this.existingPolicy.hoursBeforeBooking == value.hoursBeforeBooking
          &&
          this.existingPolicy.penaltyPct == value.penaltyPct;
        if (hasChanged) {
          this.closeModal();
          return;
        } else {
          this.$Sub = this.policiesService.editPolicy(this.existingPolicy.id, value).subscribe({
            next: (res => {
              console.log(res);
              this.toastrService.success("Policy updated successfully");
              this.closeModal();
              this.getPolicies();
            })
          })
        }
      }
    } else {
      this.policyForm.markAsTouched();
    }
  }

  confirmDelete(): void {
    console.log(this.existingPolicy.id);
    this.policiesService.deletePolicy(this.existingPolicy.id).subscribe({
      next: (res => {
        console.log(res);

        this.toastrService.show("Policy deleted successfully");
        this.getPolicies();
        this.deleteModal()?.close();

      }),
      error: (err => {
        console.log(err);
      })
    })
  }

  closeModal(): void {
    this.closeBtns().forEach(btn => btn.nativeElement.click())
  }
}
