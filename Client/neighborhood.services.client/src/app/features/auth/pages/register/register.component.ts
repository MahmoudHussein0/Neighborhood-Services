import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
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
  readonly roleOptions: ApplicationUserRole[] = ['Customer', 'Technician', 'Staff'];

  readonly form = this.formBuilder.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    age: [18, [Validators.required, Validators.min(13), Validators.max(120)]],
    applicationUserRole: ['Customer' as ApplicationUserRole, [Validators.required]],
    latitude: [30.0444, [Validators.required, Validators.min(-90), Validators.max(90)]],
    longitude: [31.2357, [Validators.required, Validators.min(-180), Validators.max(180)]],
  });

  submit(): void {
    this.apiError.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const request: RegisterRequest = this.form.getRawValue();

    this.authService.register(request).subscribe({
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
      | 'latitude'
      | 'longitude',
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
