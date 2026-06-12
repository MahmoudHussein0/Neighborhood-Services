import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable, forkJoin, of, switchMap } from 'rxjs';
import { AuthService } from '../../../auth/services/auth.service';
import { environment } from '../../../../environments/environment';
import {
  TechnicianPhoto,
  TechnicianProfile,
  TechnicianUserProfile,
} from '../../models/technician-profile.model';
import { TechnicianProfileService } from '../../services/technician-profile.service';

@Component({
  selector: 'app-technician-profile',
  imports: [DatePipe, ReactiveFormsModule],
  template: `
    <div class="d-flex flex-column gap-4">
      <section class="bg-white border rounded-3 shadow-sm p-4">
        <div class="d-flex flex-column flex-md-row justify-content-between gap-3 mb-4">
          <div>
            <h2 class="h4 fw-bold mb-1">Technician profile</h2>
            <p class="text-muted mb-0">Manage your account and public work profile.</p>
          </div>
          @if (technician()) {
            <span class="badge align-self-start" [class.text-bg-success]="technician()?.isAvailable" [class.text-bg-secondary]="!technician()?.isAvailable">
              {{ technician()?.isAvailable ? 'Available' : 'Unavailable' }}
            </span>
          }
        </div>

        @if (loading()) {
          <div class="text-muted">Loading technician profile...</div>
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
              <label class="form-label" for="fullName">Full name</label>
              <input id="fullName" class="form-control" formControlName="fullName" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="email">Email</label>
              <input id="email" class="form-control" formControlName="email" readonly />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="age">Age</label>
              <input id="age" type="number" class="form-control" formControlName="age" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="rating">Rating</label>
              <input id="rating" class="form-control" [value]="technician()?.rating ?? 0" readonly />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="nationalId">National ID</label>
              <input id="nationalId" class="form-control" formControlName="nationalId" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="maxTravelDistance">Max travel distance</label>
              <input id="maxTravelDistance" type="number" class="form-control" formControlName="maxTravelDistance" />
            </div>
            <div class="col-12">
              <label class="form-label" for="experience">Experience</label>
              <textarea id="experience" class="form-control" rows="4" formControlName="experience"></textarea>
            </div>
            <div class="col-12">
              <button class="btn btn-primary" type="submit" [disabled]="saving() || form.invalid">
                @if (saving()) {
                  <span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>
                }
                Save profile
              </button>
            </div>
          </form>
        }
      </section>

      <section class="bg-white border rounded-3 shadow-sm p-4">
        <div class="d-flex flex-column flex-lg-row justify-content-between gap-3 mb-4">
          <div>
            <h2 class="h5 fw-bold mb-1">Portfolio photos</h2>
            <p class="text-muted mb-0">Show work samples on your public profile.</p>
          </div>
        </div>

        <form [formGroup]="photoForm" (ngSubmit)="savePhoto()" class="row g-3 mb-4">
          <div class="col-12 col-lg-7">
            <label class="form-label" for="portfolioPhotoFile">Photo</label>
            <label class="portfolio-uploader" for="portfolioPhotoFile" [class.border-danger]="photoPickerTouched() && !selectedPhotoFile() && !editingPhotoId()">
              @if (selectedPhotoPreviewUrl() || editingPhotoPreviewUrl(); as previewUrl) {
                <img [src]="previewUrl" alt="Selected portfolio preview" />
                <span class="portfolio-uploader-action">
                  <i class="bi bi-camera" aria-hidden="true"></i>
                  Change photo
                </span>
              } @else {
                <span class="portfolio-uploader-empty">
                  <i class="bi bi-image fs-3" aria-hidden="true"></i>
                  <span class="fw-semibold">Add a work photo</span>
                  <span class="small text-muted">JPG, PNG, WebP, or GIF up to 5 MB</span>
                </span>
              }
            </label>
            <input
              id="portfolioPhotoFile"
              class="visually-hidden"
              type="file"
              accept="image/*"
              (change)="onPortfolioPhotoSelected($event)"
            />
            @if (photoPickerTouched() && !selectedPhotoFile() && !editingPhotoId()) {
              <div class="text-danger small mt-1">Choose a portfolio photo.</div>
            }
          </div>
          <div class="col-12 col-lg-5">
            <label class="form-label" for="caption">Caption</label>
            <input id="caption" class="form-control" formControlName="caption" />
          </div>
          <div class="col-12 d-flex gap-2">
            <button class="btn btn-primary" type="submit" [disabled]="savingPhoto() || photoForm.invalid || !technician()">
              @if (savingPhoto()) {
                <span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>
                Saving photo...
              } @else {
                {{ editingPhotoId() ? 'Update photo' : 'Add photo' }}
              }
            </button>
            @if (editingPhotoId()) {
              <button class="btn btn-outline-secondary" type="button" (click)="resetPhotoForm()">Cancel</button>
            }
          </div>
        </form>

        @if (photoLoading()) {
          <div class="text-muted">Loading portfolio photos...</div>
        } @else if (photos().length === 0) {
          <div class="text-muted">No portfolio photos yet.</div>
        } @else {
          <div class="row g-3">
            @for (photo of photos(); track photo.id) {
              <div class="col-12 col-md-6 col-xl-4">
                <div class="border rounded-3 overflow-hidden h-100 bg-white">
                  <div class="portfolio-image bg-light">
                    <img [src]="getPhotoSrc(photo.photoUrl)" [alt]="photo.caption || 'Technician work sample'" />
                  </div>
                  <div class="p-3">
                    <h3 class="h6 fw-bold mb-1">{{ photo.caption || 'Work sample' }}</h3>
                    <div class="text-muted small mb-3">{{ photo.createdAt | date: 'mediumDate' }}</div>
                    <div class="d-flex gap-2">
                      <button class="btn btn-sm btn-outline-primary" type="button" (click)="editPhoto(photo)">Edit</button>
                      <button class="btn btn-sm btn-outline-danger" type="button" (click)="deletePhoto(photo)">Delete</button>
                    </div>
                  </div>
                </div>
              </div>
            }
          </div>
        }
      </section>
    </div>
  `,
  styles: [`
    .portfolio-image {
      aspect-ratio: 4 / 3;
      overflow: hidden;
    }

    .portfolio-image img {
      height: 100%;
      object-fit: cover;
      width: 100%;
    }

    .portfolio-uploader {
      align-items: center;
      aspect-ratio: 16 / 9;
      background-color: #f8fafc;
      border: 1px dashed #bfdbfe;
      border-radius: 0.5rem;
      color: #1d4ed8;
      cursor: pointer;
      display: flex;
      justify-content: center;
      overflow: hidden;
      position: relative;
      width: 100%;
    }

    .portfolio-uploader img {
      height: 100%;
      object-fit: cover;
      width: 100%;
    }

    .portfolio-uploader-empty {
      align-items: center;
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
      padding: 1rem;
      text-align: center;
    }

    .portfolio-uploader-action {
      align-items: center;
      background: rgb(17 24 39 / 0.72);
      bottom: 0;
      color: #fff;
      display: flex;
      gap: 0.5rem;
      justify-content: center;
      left: 0;
      padding: 0.65rem;
      position: absolute;
      right: 0;
    }

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
  private readonly formBuilder = inject(FormBuilder);

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);
  readonly userProfile = signal<TechnicianUserProfile | null>(null);
  readonly technician = signal<TechnicianProfile | null>(null);
  readonly photos = signal<TechnicianPhoto[]>([]);
  readonly photoLoading = signal(false);
  readonly savingPhoto = signal(false);
  readonly editingPhotoId = signal<number | null>(null);
  readonly selectedPhotoFile = signal<File | null>(null);
  readonly selectedPhotoPreviewUrl = signal<string | null>(null);
  readonly editingPhotoPreviewUrl = signal<string | null>(null);
  readonly photoPickerTouched = signal(false);
  readonly avatarPreviewUrl = signal<string | null>(null);
  readonly selectedAvatarFile = signal<File | null>(null);
  readonly fullNameValue = signal('');

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

  readonly photoForm = this.formBuilder.nonNullable.group({
    caption: [''],
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
    this.revokeSelectedPhotoPreview();
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
        this.loadPhotos(technician.id);
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
            ? this.technicianProfileService.uploadUserPhoto(this.selectedAvatarFile() as File).pipe(
                switchMap(({ photoUrl }) =>
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

  loadPhotos(technicianId: number): void {
    this.photoLoading.set(true);

    this.technicianProfileService.getPhotosByTechnicianId(technicianId).subscribe({
      next: (photos) => {
        this.photos.set(photos);
        this.photoLoading.set(false);
      },
      error: () => {
        this.error.set('Unable to load portfolio photos.');
        this.photoLoading.set(false);
      },
    });
  }

  savePhoto(): void {
    const user = this.authService.currentUser();
    const technician = this.technician();

    this.photoPickerTouched.set(true);

    if (!user || !technician || this.photoForm.invalid || (!this.selectedPhotoFile() && !this.editingPhotoId())) {
      this.photoForm.markAllAsTouched();
      return;
    }

    const formValue = this.photoForm.getRawValue();
    const editingId = this.editingPhotoId();
    this.savingPhoto.set(true);
    this.error.set(null);
    this.success.set(null);

    const photoUpload$ = this.selectedPhotoFile()
      ? this.technicianProfileService.uploadPhoto(this.selectedPhotoFile() as File)
      : of({ photoUrl: this.editingPhotoPreviewUrl() ?? '' });

    const request$: Observable<unknown> = photoUpload$.pipe(
      switchMap(({ photoUrl }) =>
        editingId
          ? this.technicianProfileService.updatePhoto(editingId, {
              photoUrl,
              caption: formValue.caption,
            })
          : this.technicianProfileService.addPhoto({
              photoUrl,
              caption: formValue.caption,
              applicationUserId: user.userId,
              technicianId: technician.id,
            }),
      ),
    );

    request$.subscribe({
      next: () => {
        this.success.set(editingId ? 'Portfolio photo updated.' : 'Portfolio photo added.');
        this.savingPhoto.set(false);
        this.resetPhotoForm();
        this.loadPhotos(technician.id);
      },
      error: () => {
        this.error.set('Unable to save portfolio photo.');
        this.savingPhoto.set(false);
      },
    });
  }

  editPhoto(photo: TechnicianPhoto): void {
    this.editingPhotoId.set(photo.id);
    this.revokeSelectedPhotoPreview();
    this.selectedPhotoFile.set(null);
    this.selectedPhotoPreviewUrl.set(null);
    this.editingPhotoPreviewUrl.set(this.getPhotoSrc(photo.photoUrl));
    this.photoPickerTouched.set(false);
    this.photoForm.patchValue({
      caption: photo.caption,
    });
  }

  resetPhotoForm(): void {
    this.editingPhotoId.set(null);
    this.revokeSelectedPhotoPreview();
    this.selectedPhotoFile.set(null);
    this.selectedPhotoPreviewUrl.set(null);
    this.editingPhotoPreviewUrl.set(null);
    this.photoPickerTouched.set(false);
    this.photoForm.reset({
      caption: '',
    });
  }

  onPortfolioPhotoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    this.photoPickerTouched.set(true);

    if (!file) {
      return;
    }

    if (!file.type.startsWith('image/')) {
      this.error.set('Please choose an image file.');
      input.value = '';
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      this.error.set('Portfolio photo must be 5 MB or smaller.');
      input.value = '';
      return;
    }

    this.revokeSelectedPhotoPreview();
    this.selectedPhotoFile.set(file);
    this.selectedPhotoPreviewUrl.set(URL.createObjectURL(file));
    this.error.set(null);
  }

  getPhotoSrc(photoUrl: string): string {
    if (!photoUrl || photoUrl.startsWith('http') || photoUrl.startsWith('blob:') || photoUrl.startsWith('data:')) {
      return photoUrl;
    }

    return `${environment.apiUrl}${photoUrl.startsWith('/') ? '' : '/'}${photoUrl}`;
  }

  deletePhoto(photo: TechnicianPhoto): void {
    const technician = this.technician();

    if (!technician || !confirm('Delete this portfolio photo?')) {
      return;
    }

    this.technicianProfileService.deletePhoto(photo.id).subscribe({
      next: () => {
        this.success.set('Portfolio photo deleted.');
        this.loadPhotos(technician.id);
      },
      error: () => this.error.set('Unable to delete portfolio photo.'),
    });
  }

  private revokeSelectedPhotoPreview(): void {
    const previewUrl = this.selectedPhotoPreviewUrl();

    if (previewUrl) {
      URL.revokeObjectURL(previewUrl);
    }
  }

  private revokeAvatarPreview(): void {
    const previewUrl = this.avatarPreviewUrl();

    if (previewUrl) {
      URL.revokeObjectURL(previewUrl);
    }

    this.avatarPreviewUrl.set(null);
  }
}
