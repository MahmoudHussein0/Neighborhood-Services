import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { switchMap } from 'rxjs';
import { RegisterRequest } from '../../models/auth.models';
import { AuthService } from '../../services/auth.service';
import { CategoriesService } from '../../../../core/services/categories.service';
import { Category } from '../../../../core/models/category';
import { LanguageSwitcherComponent } from '../../../../shared/components/language-switcher/language-switcher.component';

@Component({
  selector: 'app-register-technician',
  imports: [ReactiveFormsModule, RouterLink, TranslatePipe, LanguageSwitcherComponent],
  templateUrl: './register-technician.component.html',
  styleUrl: './register-technician.component.css',
})
export class RegisterTechnicianComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly categoriesService = inject(CategoriesService);
  private readonly router = inject(Router);
  private readonly translate = inject(TranslateService);

  readonly isSubmitting = signal(false);
  readonly apiError = signal<string | null>(null);

  readonly categories = signal<Category[]>([]);
  readonly selectedCategoryIds = signal<number[]>([]);
  // True once the user tries to submit, so we only flag the "pick a category" error after an attempt.
  readonly submitted = signal(false);

  readonly form = this.formBuilder.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    age: [0, [Validators.required, Validators.min(13), Validators.max(120)]],
    address: ['', [Validators.required, Validators.minLength(3)]],
    nationalId: ['', [Validators.required, Validators.minLength(8)]],
    experience: ['', [Validators.required, Validators.minLength(10)]],
    maxTravelDistance: [0, [Validators.required, Validators.min(1)]],
  });

  ngOnInit(): void {
    this.categoriesService.getAllCategories().subscribe({
      next: (categories: Category[]) => this.categories.set(categories ?? []),
      error: () => this.categories.set([]),
    });
  }

  categoryName(category: Category): string {
    return this.translate.getCurrentLang() === 'ar'
      ? category.nameAr || category.nameEn || category.name
      : category.nameEn || category.name || category.nameAr;
  }

  isSelected(categoryId: number): boolean {
    return this.selectedCategoryIds().includes(categoryId);
  }

  toggleCategory(categoryId: number): void {
    this.selectedCategoryIds.update((ids) =>
      ids.includes(categoryId) ? ids.filter((id) => id !== categoryId) : [...ids, categoryId],
    );
  }

  submit(): void {
    this.apiError.set(null);
    this.submitted.set(true);

    if (this.form.invalid || this.selectedCategoryIds().length === 0) {
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
            applicationUserRole: 'Technician',
            latitude: location.latitude,
            longitude: location.longitude,
            nationalId: formValue.nationalId,
            experience: formValue.experience,
            maxTravelDistance: formValue.maxTravelDistance,
            categoryIds: this.selectedCategoryIds(),
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
