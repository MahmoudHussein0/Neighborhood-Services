import { DatePipe } from '@angular/common';
import { Component, ElementRef, inject, Signal, signal, viewChild, viewChildren, WritableSignal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { Time12Pipe } from '../../../../../shared/pipes/time12-pipe';
import { ExceptionService } from '../../../services/exception.service';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute } from '@angular/router';
import { Subject, Subscription, takeUntil } from 'rxjs';
import { Exception } from '../../../models/exception';
import { DeleteComponent } from "../../../../../shared/components/delete/delete.component";

@Component({
  selector: 'app-exception',
  imports: [ReactiveFormsModule, TranslatePipe, Time12Pipe, DatePipe, DeleteComponent],
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
  deleteModal = viewChild(DeleteComponent)

  closBtn: Signal<readonly ElementRef<HTMLButtonElement>[]> = viewChildren<ElementRef<HTMLButtonElement>>('closeBtn');
  exceptiomMode: 'edit' | 'add' = 'add';

  destroy$ = new Subject<void>();


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
    this.exceptionService.getException().pipe(takeUntil(this.destroy$)).subscribe({
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

    if (this.exceptiomMode === 'edit') {
      if (this.exceptionForm.valid) {
        const hasChanged = this.existingException?.date === value.date &&
          this.existingException?.isAvailable === value.isAvailable &&
          this.existingException?.startTime === value.startTime &&
          this.existingException?.endTime === value.endTime &&
          this.existingException?.reason === value.reason;
        if (hasChanged) {
          this.closeModal();
          return;
        }

        this.updateException(value);
      }



      else {
        const payload = {
          date: value.date,
          isAvailable: value.isAvailable || false,
          startTime: value.startTime || null,
          endTime: value.endTime || null,
          reason: value.reason || null,
        }

        this.addException(payload);
      }
    } else {
      this.exceptionForm.markAllAsTouched();
    }
  }


  updateException(value: object): void {
    this.loadFlag.set(true);

    this.exceptionService.updateException(this.existingException?.id, value).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res => {
        console.log(res);
        this.toastrService.success('Exception updates Successfully');
        this.getException();
        this.closeModal();
        this.loadFlag.set(false);

      }),
      error: (err => {
        console.log(err);

      })
    })

  }


  addException(payload: object): void {
    this.loadFlag.set(true);
    this.exceptionService.addException(payload).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res => {
        console.log(res);
        this.toastrService.success("Exception added successfully");
        this.closeModal();
        console.log(this.closBtn);
        this.getException();
        this.loadFlag.set(false);
      }), error: (err => {
        console.log(err);
      })
    })
  }

  confirmDelete() {
    console.log(this.exceptionId);

    this.exceptionService.deleteException(this.exceptionId).subscribe({
      next: (res => {
        console.log(res);
        this.toastrService.show("exception is deleted");
        this.getException();
        this.deleteModal()?.close();
      })
    })
  }



  closeModal(): void {
    this.closBtn().forEach(btn => btn.nativeElement.click());
  }



  ngOnDestroy(): void {
    this.$SubException.unsubscribe();


    this.destroy$.next();
    this.destroy$.complete();
  }



}
