import { Component, ElementRef, inject, Signal, signal, viewChildren, WritableSignal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { Time12Pipe } from '../../../../../shared/pipes/time12-pipe';
import { Availiability } from '../../../models/availiability';
import { AvailabilityService } from '../../../services/availability.service';

@Component({
  selector: 'app-availiability',
  imports: [TranslatePipe, Time12Pipe, ReactiveFormsModule],
  templateUrl: './availiability.component.html',
  styleUrl: './availiability.component.css',
})
export class AvailiabilityComponent {

  private readonly availabilityService = inject(AvailabilityService);
  private readonly fb = inject(FormBuilder);
  private readonly toastrService = inject(ToastrService);


  availiabilityId!: number;
  loadFlag: WritableSignal<boolean> = signal<boolean>(false);
  $Sub: Subscription = new Subscription();
  availabilities: WritableSignal<Availiability[]> = signal<Availiability[]>([]);
  existingAvailiability: Availiability | null = null;
  closBtn: Signal<readonly ElementRef<HTMLButtonElement>[]> = viewChildren<ElementRef<HTMLButtonElement>>('closeBtn');
  mode: 'add' | 'edit' = 'add';

  form = this.fb.group({
    dayOfWeek: ['', [Validators.required]],
    startTime: ['', [Validators.required]],
    endTime: ['', [Validators.required]]
  });


  ngOnInit(): void {
    this.getAvailability();
  }

  getAvailability(): void {
    this.$Sub = this.availabilityService.getAvailability().subscribe({
      next: (res => {
        console.log(res);
        this.availabilities.set(res);
      }),
      error: (error => {
        console.log(error);
      })
    })
  }


  updateAvaModal(availability: Availiability): void {
    this.mode = 'edit';
    this.form.patchValue({
      dayOfWeek: availability.dayOfWeek,
      startTime: availability.startTime,
      endTime: availability.endTime
    });
    this.existingAvailiability = availability;
  }

  addAvaModal(): void {
    this.mode = 'add';
    this.existingAvailiability = null;
    this.form.reset();
  }

  save(): void {
    console.log(this.existingAvailiability?.id);
    const value = this.form.value;
    if (this.form.valid) {
      this.$Sub.unsubscribe();
      if (this.mode === 'edit') {
        const hasChanged = value.dayOfWeek === this.existingAvailiability?.dayOfWeek &&
          value.startTime === this.existingAvailiability?.startTime &&
          value.endTime === this.existingAvailiability?.endTime;
        if (hasChanged) {
          this.toastrService.info('No changes detected', 'NS');
          this.closeModal();
          return;
        }
        this.$Sub = this.availabilityService.updateAvailability(this.existingAvailiability?.id, this.form.value).subscribe({
          next: (res => {
            this.toastrService.success('Availiability updated successfully', 'NS');
            this.closeModal();
            this.getAvailability();
          }),
          error: (err => {
            console.log(err);
            this.toastrService.error(err.error.detail, 'NS');
          })
        })
      }
      else {
        this.$Sub = this.availabilityService.addAvailability(this.form.value).subscribe({
          next: (res => {
            this.toastrService.success("Availiability added Successfully", 'NS');
            this.getAvailability();
            this.closeModal();
          }),
          error: (err => {
            this.toastrService.error(err.error.detail, 'NS');
          })
        })
      }
    }
    else {
      this.form.markAllAsTouched();
    }
  }


  confirmDelete() {
    this.availabilityService.deleteAvailability(this.availiabilityId).subscribe({
      next: (res => {
        console.log(res);
        this.toastrService.show("Your Availiability is deleted ")
        this.getAvailability();
        this.closeModal();
      }),
      error: (err => {
        console.log(err);
      })
    })
  }


  closeModal(): void {
    this.closBtn().forEach(btn => btn.nativeElement.click());
  }


  ngOnDestroy(): void {
    this.$Sub.unsubscribe();
  }
}
