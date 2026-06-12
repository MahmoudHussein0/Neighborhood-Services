import { Component, OnDestroy, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { of, switchMap } from 'rxjs';
import { ApplicationUserRole, RegisterRequest } from '../../models/auth.models';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnDestroy {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly isSubmitting = signal(false);
  readonly apiError = signal<string | null>(null);
  readonly avatarPreviewUrl = signal<string | null>(null);
  readonly selectedAvatarFile = signal<File | null>(null);
  readonly fullNameValue = signal('');
  readonly roleOptions: ApplicationUserRole[] = ['Customer', 'Technician', 'Staff'];

  readonly form = this.formBuilder.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    age: [0, [Validators.required, Validators.min(13), Validators.max(120)]],
    applicationUserRole: ['Customer' as ApplicationUserRole, [Validators.required]],
    address: ['', [Validators.required, Validators.minLength(3)]],
  });

  readonly fallbackInitials = computed(() => {
    const fullName = this.fullNameValue().trim();

    if (!fullName) {
      return 'NS';
    }

    const nameParts = fullName.split(/\s+/).filter(Boolean);

    if (nameParts.length > 1) {
      return `${nameParts[0].charAt(0)}${nameParts[1].charAt(0)}`.toUpperCase();
    }

    return fullName.slice(0, Math.min(2, fullName.length)).toUpperCase();
  });

  constructor() {
    this.form.controls.fullName.valueChanges.subscribe((fullName) => {
      this.fullNameValue.set(fullName);
    });
  }

  ngOnDestroy(): void {
    this.revokeAvatarPreview();
  }

  onAvatarSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;

    if (!file) {
      return;
    }

    if (!file.type.startsWith('image/')) {
      this.apiError.set('Please choose an image file.');
      input.value = '';
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      this.apiError.set('Profile photo must be 5 MB or smaller.');
      input.value = '';
      return;
    }

    this.revokeAvatarPreview();
    this.selectedAvatarFile.set(file);
    this.avatarPreviewUrl.set(URL.createObjectURL(file));
    this.apiError.set(null);
  }

  submit(): void {
    this.apiError.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const formValue = this.form.getRawValue();

    const photoUpload$ = this.selectedAvatarFile()
      ? this.authService.uploadUserPhoto(this.selectedAvatarFile() as File)
      : of({ photoUrl: '' });

    photoUpload$
      .pipe(
        switchMap(({ photoUrl }) =>
          this.authService.geocodeAddress(formValue.address).pipe(
            switchMap((location) => {
              const request: RegisterRequest = {
                fullName: formValue.fullName,
                email: formValue.email,
                photo: photoUrl,
                password: formValue.password,
                age: formValue.age,
                applicationUserRole: formValue.applicationUserRole,
                latitude: location.latitude,
                longitude: location.longitude,
              };

              return this.authService.register(request);
            }),
          ),
        ),
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
      | 'address',
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

  private revokeAvatarPreview(): void {
    const previewUrl = this.avatarPreviewUrl();

    if (previewUrl) {
      URL.revokeObjectURL(previewUrl);
    }

    this.avatarPreviewUrl.set(null);
  }
}
