import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin, of, switchMap } from 'rxjs';
import { ChangePasswordFormValue, ChangePasswordModalComponent } from '../../../../shared/components/change-password-modal/change-password-modal.component';
import { AuthService } from '../../../auth/services/auth.service';
import { environment } from '../../../../environments/environment';
import {
  TechnicianProfile,
  TechnicianUserProfile,
} from '../../models/technician-profile.model';
import { TechnicianProfileService } from '../../services/technician-profile.service';
import { UploadService } from '../../../../shared/services/upload.service';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-technician-profile',
  imports: [ReactiveFormsModule, ChangePasswordModalComponent, TranslatePipe],
  template: `
    <div class="d-flex flex-column gap-4">
      <section class="bg-white border rounded-3 shadow-sm p-4">
        <div class="d-flex flex-column flex-md-row justify-content-between gap-3 mb-4">
          <div>
            <h2 class="h4 fw-bold mb-1">{{ 'profile.techTitle' | translate }}</h2>
            <p class="text-muted mb-0">{{ 'profile.techSubtitle' | translate }}</p>
          </div>
          @if (technician()) {
            <span class="badge align-self-start" [class.text-bg-success]="technician()?.isAvailable" [class.text-bg-secondary]="!technician()?.isAvailable">
              {{ (technician()?.isAvailable ? 'profile.available' : 'profile.unavailable') | translate }}
            </span>
          }
        </div>

        @if (loading()) {
          <div class="text-muted">{{ 'profile.loadingTech' | translate }}</div>
        } @else {
          @if (error()) {
            <div class="alert alert-danger">{{ error() }}</div>
          }
          @if (success()) {
            <div class="alert alert-success">{{ success() }}</div>
          }

          <form [formGroup]="form" (ngSubmit)="save()" class="row g-3">
            <div class="col-12 d-flex justify-content-center mb-2">
              <label class="profile-avatar-picker" for="technicianAvatarFile" aria-label="Choose profile photo">
                @if (avatarPreviewUrl() || getPhotoSrc(userProfile()?.photo ?? ''); as photoUrl) {
                  <img [src]="photoUrl" alt="Profile photo preview" />
                } @else {
                  <span>{{ fallbackInitials() }}</span>
                }
                <span class="profile-avatar-action">
                  <i class="bi bi-camera" aria-hidden="true"></i>
                </span>
              </label>
              <input
                id="technicianAvatarFile"
                class="visually-hidden"
                type="file"
                accept="image/*"
                (change)="onAvatarSelected($event)"
              />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="fullName">{{ 'profile.fullName' | translate }}</label>
              <input id="fullName" class="form-control" formControlName="fullName" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="email">{{ 'profile.email' | translate }}</label>
              <input id="email" class="form-control" formControlName="email" readonly />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="age">{{ 'profile.age' | translate }}</label>
              <input id="age" type="number" class="form-control" formControlName="age" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="rating">{{ 'profile.rating' | translate }}</label>
              <input id="rating" class="form-control" [value]="technician()?.rating ?? 0" readonly />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="nationalId">{{ 'profile.nationalId' | translate }}</label>
              <input id="nationalId" class="form-control" formControlName="nationalId" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="maxTravelDistance">{{ 'profile.maxTravelDistance' | translate }}</label>
              <input id="maxTravelDistance" type="number" class="form-control" formControlName="maxTravelDistance" />
            </div>
            <div class="col-12">
              <label class="form-label" for="experience">{{ 'profile.experience' | translate }}</label>
              <textarea id="experience" class="form-control" rows="4" formControlName="experience"></textarea>
            </div>
            <div class="col-12 d-flex flex-wrap gap-2">
              <button class="btn btn-primary" type="submit" [disabled]="saving() || form.invalid">
                @if (saving()) {
                  <span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>
                }
                {{ 'profile.saveProfile' | translate }}
              </button>
              <button class="btn btn-outline-primary" type="button" (click)="openChangePasswordModal()">
                <i class="bi bi-shield-lock me-2" aria-hidden="true"></i>
                {{ 'profile.changePassword' | translate }}
              </button>
            </div>
          </form>
        }
      </section>

      @if (changePasswordModalOpen()) {
        <app-change-password-modal
          [busy]="changingPassword()"
          [apiError]="passwordError()"
          (submitPassword)="changePassword($event)"
          (cancel)="closeChangePasswordModal()"
        />
      }
    </div>
  `,
  styles: [`
    .profile-avatar-picker {
      align-items: center;
      background-color: #dbeafe;
      border: 2px solid #bfdbfe;
      border-radius: 50%;
      color: #1d4ed8;
      cursor: pointer;
      display: flex;
      font-size: 1.75rem;
      font-weight: 700;
      height: 7rem;
      justify-content: center;
      position: relative;
      width: 7rem;
    }

    .profile-avatar-picker img {
      border-radius: 50%;
      height: 100%;
      object-fit: cover;
      width: 100%;
    }

    .profile-avatar-action {
      align-items: center;
      background-color: #1d4ed8;
      border: 2px solid #fff;
      border-radius: 50%;
      bottom: 0.2rem;
      color: #fff;
      display: flex;
      font-size: 0.9rem;
      height: 2rem;
      justify-content: center;
      position: absolute;
      right: 0.2rem;
      width: 2rem;
    }
  `],
})
export class TechnicianProfileComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly technicianProfileService = inject(TechnicianProfileService);
  private readonly uploadService = inject(UploadService);
  private readonly formBuilder = inject(FormBuilder);

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly changingPassword = signal(false);
  readonly error = signal<string | null>(null);
  readonly passwordError = signal<string | null>(null);
  readonly success = signal<string | null>(null);
  readonly userProfile = signal<TechnicianUserProfile | null>(null);
  readonly technician = signal<TechnicianProfile | null>(null);
  readonly avatarPreviewUrl = signal<string | null>(null);
  readonly selectedAvatarFile = signal<File | null>(null);
  readonly fullNameValue = signal('');
  readonly changePasswordModalOpen = signal(false);

  readonly fallbackInitials = computed(() => {
    const fullName = this.fullNameValue().trim();

    if (!fullName) {
      return 'NS';
    }

    const nameParts = fullName.split(/\s+/).filter(Boolean);
    return nameParts.length > 1
      ? `${nameParts[0].charAt(0)}${nameParts[1].charAt(0)}`.toUpperCase()
      : fullName.slice(0, Math.min(2, fullName.length)).toUpperCase();
  });

  readonly form = this.formBuilder.nonNullable.group({
    fullName: ['', Validators.required],
    email: [''],
    age: [18, [Validators.required, Validators.min(13), Validators.max(120)]],
    nationalId: ['', Validators.required],
    experience: ['', Validators.required],
    maxTravelDistance: [10, [Validators.required, Validators.min(0)]],
  });

  constructor() {
    this.form.controls.fullName.valueChanges.subscribe((fullName) => {
      this.fullNameValue.set(fullName);
    });
  }

  ngOnInit(): void {
    this.load();
  }

  ngOnDestroy(): void {
    this.revokeAvatarPreview();
  }

  load(): void {
    const user = this.authService.currentUser();

    if (!user) {
      this.error.set('You must be logged in to view your profile.');
      this.loading.set(false);
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      userProfile: this.technicianProfileService.getUserProfile(user.userId),
      technician: this.technicianProfileService.getTechnicianByUserId(user.userId),
    }).subscribe({
      next: ({ userProfile, technician }) => {
        this.userProfile.set(userProfile);
        this.technician.set(technician);
        this.authService.updateSafeUser({ fullName: userProfile.fullName, photo: userProfile.photo });
        this.form.patchValue({
          fullName: userProfile.fullName,
          email: userProfile.email,
          age: userProfile.age,
          nationalId: technician.nationalId,
          experience: technician.experience,
          maxTravelDistance: technician.maxTravelDistance,
        });
        this.fullNameValue.set(userProfile.fullName);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load technician profile.');
        this.loading.set(false);
      },
    });
  }

  save(): void {
    const user = this.authService.currentUser();
    const technician = this.technician();

    if (!user || !technician || this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const formValue = this.form.getRawValue();
    this.saving.set(true);
    this.error.set(null);
    this.success.set(null);

    this.technicianProfileService
      .updateUserProfile(user.userId, {
        fullName: formValue.fullName,
        age: formValue.age,
      })
      .pipe(
        switchMap(() =>
          this.selectedAvatarFile()
            ? this.uploadService.upload(this.selectedAvatarFile() as File).pipe(
                switchMap((photoUrl) =>
                  this.technicianProfileService.updateUserPhoto(user.userId, photoUrl).pipe(
                    switchMap(() => of(photoUrl)),
                  ),
                ),
              )
            : of(this.userProfile()?.photo ?? ''),
        ),
        switchMap((photoUrl) =>
          this.technicianProfileService.updateTechnician(technician.id, {
            nationalId: formValue.nationalId,
            experience: formValue.experience,
            maxTravelDistance: formValue.maxTravelDistance,
          }).pipe(switchMap(() => of(photoUrl))),
        ),
      )
      .subscribe({
      next: (photoUrl) => {
        this.authService.updateSafeUser({ fullName: formValue.fullName, photo: photoUrl });
        this.success.set('Technician profile updated.');
        this.saving.set(false);
        this.selectedAvatarFile.set(null);
        this.revokeAvatarPreview();
        this.load();
      },
      error: () => {
        this.error.set('Unable to update technician profile.');
        this.saving.set(false);
      },
      });
  }

  openChangePasswordModal(): void {
    this.passwordError.set(null);
    this.changePasswordModalOpen.set(true);
  }

  closeChangePasswordModal(): void {
    if (this.changingPassword()) {
      return;
    }

    this.changePasswordModalOpen.set(false);
    this.passwordError.set(null);
  }

  changePassword(value: ChangePasswordFormValue): void {
    this.changingPassword.set(true);
    this.passwordError.set(null);
    this.error.set(null);
    this.success.set(null);

    this.authService.changePassword({
      currentPassword: value.currentPassword,
      newPassword: value.newPassword,
    }).subscribe({
      next: (response) => {
        this.changingPassword.set(false);
        this.changePasswordModalOpen.set(false);
        this.success.set(response.message || 'Password changed successfully.');
      },
      error: (error) => {
        this.changingPassword.set(false);
        this.passwordError.set(this.getPasswordError(error));
      },
    });
  }

  onAvatarSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;

    if (!file) {
      return;
    }

    if (!file.type.startsWith('image/')) {
      this.error.set('Please choose an image file.');
      input.value = '';
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      this.error.set('Profile photo must be 5 MB or smaller.');
      input.value = '';
      return;
    }

    this.revokeAvatarPreview();
    this.selectedAvatarFile.set(file);
    this.avatarPreviewUrl.set(URL.createObjectURL(file));
    this.error.set(null);
  }

  getPhotoSrc(photoUrl: string): string {
    if (!photoUrl || photoUrl.startsWith('http') || photoUrl.startsWith('blob:') || photoUrl.startsWith('data:')) {
      return photoUrl;
    }

    return `${environment.apiUrl}${photoUrl.startsWith('/') ? '' : '/'}${photoUrl}`;
  }

  private revokeAvatarPreview(): void {
    const previewUrl = this.avatarPreviewUrl();

    if (previewUrl) {
      URL.revokeObjectURL(previewUrl);
    }

    this.avatarPreviewUrl.set(null);
  }

  private getPasswordError(error: unknown): string {
    if (typeof error === 'object' && error && 'error' in error) {
      const response = (error as { error?: { message?: string; errors?: string[] } }).error;
      const firstError = response?.errors?.[0];

      return firstError || response?.message || 'Unable to change password.';
    }

    return 'Unable to change password.';
  }
}
