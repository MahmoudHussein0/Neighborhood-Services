import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { switchMap } from 'rxjs';
import { RegisterRequest } from '../../models/auth.models';
import { AuthService } from '../../services/auth.service';
import { LanguageSwitcherComponent } from '../../../../shared/components/language-switcher/language-switcher.component';

@Component({
  selector: 'app-register-customer',
  imports: [ReactiveFormsModule, RouterLink, TranslatePipe, LanguageSwitcherComponent],
  templateUrl: './register-customer.component.html',
  styleUrl: './register-customer.component.css',
})
export class RegisterCustomerComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly translate = inject(TranslateService);

  readonly isSubmitting = signal(false);
  readonly apiError = signal<string | null>(null);

  readonly form = this.formBuilder.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    age: [0, [Validators.required, Validators.min(13), Validators.max(120)]],
    address: ['', [Validators.required, Validators.minLength(3)]],
  });

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
            applicationUserRole: 'Customer',
            latitude: location.latitude,
            longitude: location.longitude,
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
    controlName: 'fullName' | 'email' | 'password' | 'age' | 'address',
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

    return this.translate.instant('register.submitError');
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
