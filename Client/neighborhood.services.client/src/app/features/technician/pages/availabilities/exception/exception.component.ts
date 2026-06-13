import { DatePipe } from '@angular/common';
import { Component, ElementRef, inject, Signal, signal, viewChildren, WritableSignal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { Time12Pipe } from '../../../../../shared/pipes/time12-pipe';
import { ExceptionService } from '../../../services/exception.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Exception } from '../../../models/exception';

@Component({
  selector: 'app-exception',
  imports: [ReactiveFormsModule, TranslatePipe, Time12Pipe, DatePipe],
  templateUrl: './exception.component.html',
  styleUrl: './exception.component.css',
})
export class ExceptionComponent {


  private readonly fb = inject(FormBuilder);
  private readonly exceptionService = inject(ExceptionService);
  private readonly toastrService = inject(ToastrService);
  private readonly activatedRoute = inject(ActivatedRoute);

  exceptionId!: number;
  loadFlag: WritableSignal<boolean> = signal<boolean>(false);
  $SubException: Subscription = new Subscription();
  exceptions: WritableSignal<Exception[]> = signal<Exception[]>([]);
  existingException: Exception | null = null;
  closBtn: Signal<readonly ElementRef<HTMLButtonElement>[]> = viewChildren<ElementRef<HTMLButtonElement>>('closeBtn');
  exceptiomMode: 'edit' | 'add' = 'add';

  exceptionForm = this.fb.group({
    date: ['', [Validators.required]],
    isAvailable: [false],
    startTime: [''],
    endTime: [''],
    reason: ['']
  });

  ngOnInit(): void {

    this.getException();
  }


  getException(): void {
    this.exceptionService.getException().subscribe({
      next: (res => {
        console.log(res);
        this.exceptions.set(res);
      })
    })
  }



  updateExceptiomModal(exception: Exception): void {
    this.exceptiomMode = 'edit';
    this.existingException = exception;
    this.exceptionForm.patchValue({
      date: exception.date,
      isAvailable: exception.isAvailable,
      startTime: exception.startTime,
      endTime: exception.endTime,
      reason: exception.reason
    })
  }

  addExceptiomModal(): void {
    this.exceptiomMode = 'add';
    this.existingException = null;
    this.exceptionForm.reset();
  }



  saveException(): void {
    console.log(this.existingException?.id);
    const value = this.exceptionForm.value;
    if (this.exceptionForm.valid) {
      this.$SubException.unsubscribe();
      if (this.exceptiomMode === 'edit') {
        const hasChanged = this.existingException?.date === value.date &&
          this.existingException?.isAvailable === value.isAvailable &&
          this.existingException?.startTime === value.startTime &&
          this.existingException?.endTime === value.endTime &&
          this.existingException?.reason === value.reason;
        if (hasChanged) {
          this.toastrService.info('No changes detected', 'NS');
          this.closeModal();
          return;
        }
        this.$SubException = this.exceptionService.updateException(this.existingException?.id, this.exceptionForm.value).subscribe({
          next: (res => {
            console.log(res);
            this.toastrService.success('Exception updates Successfully', 'NS');
            this.closeModal();
          }),
          error: (err => {
            console.log(err);

          })
        })

      }
      else {
        const payload = {
          date: value.date,
          isAvailable: value.isAvailable || false,
          startTime: value.startTime || null,
          endTime: value.endTime || null,
          reason: value.reason || null,
        }
        this.$SubException = this.exceptionService.addException(payload).subscribe({
          next: (res => {
            console.log(res);
            this.toastrService.success("Exception added successfully", "NS");
            this.closeModal();
            console.log(this.closBtn);

            this.getException();

          }), error: (err => {
            console.log(err);
            this.toastrService.error(err.error.detail, "NS");
          })
        })
      }
    } else {
      this.exceptionForm.markAllAsTouched();
    }
  }





  confirmDelete() {
    this.exceptionService.deleteException(this.exceptionId).subscribe({
      next: (res => {
        console.log(res);
        this.toastrService.show("exception is deleted");
        this.getException();
      })
    })
  }



  closeModal(): void {
    this.closBtn().forEach(btn => btn.nativeElement.click());
  }



  ngOnDestroy(): void {
    this.$SubException.unsubscribe();
  }



}
