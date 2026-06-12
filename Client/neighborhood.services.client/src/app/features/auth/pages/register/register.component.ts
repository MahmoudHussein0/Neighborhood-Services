import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { switchMap } from 'rxjs';
import { ApplicationUserRole, RegisterRequest } from '../../models/auth.models';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly isSubmitting = signal(false);
  readonly apiError = signal<string | null>(null);
  readonly selectedRole = signal<Extract<ApplicationUserRole, 'Customer' | 'Technician'>>('Customer');

  readonly form = this.formBuilder.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    age: [0, [Validators.required, Validators.min(13), Validators.max(120)]],
    applicationUserRole: ['Customer' as ApplicationUserRole, [Validators.required]],
    address: ['', [Validators.required, Validators.minLength(3)]],
    nationalId: [''],
    experience: [''],
    maxTravelDistance: [0],
  });

  setRole(role: Extract<ApplicationUserRole, 'Customer' | 'Technician'>): void {
    this.selectedRole.set(role);
    this.form.controls.applicationUserRole.setValue(role);
    this.form.controls.nationalId.clearValidators();
    this.form.controls.experience.clearValidators();
    this.form.controls.maxTravelDistance.clearValidators();

    if (role === 'Technician') {
      this.form.controls.nationalId.setValidators([Validators.required, Validators.minLength(8)]);
      this.form.controls.experience.setValidators([Validators.required, Validators.minLength(10)]);
      this.form.controls.maxTravelDistance.setValidators([Validators.required, Validators.min(1)]);
    }

    this.form.controls.nationalId.updateValueAndValidity();
    this.form.controls.experience.updateValueAndValidity();
    this.form.controls.maxTravelDistance.updateValueAndValidity();
  }

  submit(): void {
    this.apiError.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const formValue = this.form.getRawValue();

    this.authService
      .geocodeAddress(formValue.address)
      .pipe(
        switchMap((location) => {
          const request: RegisterRequest = {
            fullName: formValue.fullName,
            email: formValue.email,
            password: formValue.password,
            age: formValue.age,
            applicationUserRole: formValue.applicationUserRole,
            latitude: location.latitude,
            longitude: location.longitude,
            nationalId: formValue.nationalId,
            experience: formValue.experience,
            maxTravelDistance: formValue.maxTravelDistance,
          };

          return this.authService.register(request);
        }),
      )
      .subscribe({
        next: () => {
          this.isSubmitting.set(false);
          this.router.navigate(['/auth/login'], {
            queryParams: { registered: 'true' },
          });
        },
        error: (error) => {
          this.isSubmitting.set(false);
          this.apiError.set(this.getErrorMessage(error));
        },
      });
  }

  hasError(
    controlName:
      | 'fullName'
      | 'email'
      | 'password'
      | 'age'
      | 'applicationUserRole'
      | 'address'
      | 'nationalId'
      | 'experience'
      | 'maxTravelDistance',
    errorName: string,
  ): boolean {
    const control = this.form.controls[controlName];
    return control.touched && control.hasError(errorName);
  }

  private getErrorMessage(error: unknown): string {
    if (this.isHttpError(error)) {
      const apiError = error.error;

      if (typeof apiError === 'string') {
        return apiError;
      }

      if (this.isErrorObject(apiError)) {
        if (typeof apiError['message'] === 'string') {
          return apiError['message'];
        }

        if (typeof apiError['Message'] === 'string') {
          return apiError['Message'];
        }

        if (Array.isArray(apiError['errors'])) {
          return apiError['errors'].join(' ');
        }

        if (Array.isArray(apiError['Errors'])) {
          return apiError['Errors'].join(' ');
        }
      }
    }

    return 'Unable to create the account. Please review your details and try again.';
  }

  private isHttpError(error: unknown): error is {
    error?: unknown;
  } {
    return typeof error === 'object' && error !== null && 'error' in error;
  }

  private isErrorObject(error: unknown): error is Record<string, unknown> {
    return typeof error === 'object' && error !== null;
  }
}
