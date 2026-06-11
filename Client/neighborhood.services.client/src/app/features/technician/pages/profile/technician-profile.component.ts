import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable, forkJoin } from 'rxjs';
import { AuthService } from '../../../auth/services/auth.service';
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
            <div class="col-12 col-md-6">
              <label class="form-label" for="fullName">Full name</label>
              <input id="fullName" class="form-control" formControlName="fullName" />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="email">Email</label>
              <input id="email" class="form-control" formControlName="email" readonly />
            </div>
            <div class="col-12 col-md-4">
              <label class="form-label" for="age">Age</label>
              <input id="age" type="number" class="form-control" formControlName="age" />
            </div>
            <div class="col-12 col-md-4">
              <label class="form-label" for="photo">Photo URL</label>
              <input id="photo" class="form-control" formControlName="photo" />
            </div>
            <div class="col-12 col-md-4">
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
            <label class="form-label" for="photoUrl">Photo URL</label>
            <input id="photoUrl" class="form-control" formControlName="photoUrl" [class.is-invalid]="photoForm.controls.photoUrl.touched && photoForm.controls.photoUrl.invalid" />
            @if (photoForm.controls.photoUrl.touched && photoForm.controls.photoUrl.invalid) {
              <div class="invalid-feedback">Photo URL is required.</div>
            }
          </div>
          <div class="col-12 col-lg-5">
            <label class="form-label" for="caption">Caption</label>
            <input id="caption" class="form-control" formControlName="caption" />
          </div>
          <div class="col-12 d-flex gap-2">
            <button class="btn btn-primary" type="submit" [disabled]="savingPhoto() || photoForm.invalid || !technician()">
              {{ editingPhotoId() ? 'Update photo' : 'Add photo' }}
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
                    <img [src]="photo.photoUrl" [alt]="photo.caption || 'Technician work sample'" />
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
  `],
})
export class TechnicianProfileComponent implements OnInit {
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

  readonly form = this.formBuilder.nonNullable.group({
    fullName: ['', Validators.required],
    email: [''],
    age: [18, [Validators.required, Validators.min(13), Validators.max(120)]],
    photo: [''],
    nationalId: ['', Validators.required],
    experience: ['', Validators.required],
    maxTravelDistance: [10, [Validators.required, Validators.min(0)]],
  });

  readonly photoForm = this.formBuilder.nonNullable.group({
    photoUrl: ['', Validators.required],
    caption: [''],
  });

  ngOnInit(): void {
    this.load();
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
        this.form.patchValue({
          fullName: userProfile.fullName,
          email: userProfile.email,
          age: userProfile.age,
          photo: userProfile.photo,
          nationalId: technician.nationalId,
          experience: technician.experience,
          maxTravelDistance: technician.maxTravelDistance,
        });
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

    forkJoin([
      this.technicianProfileService.updateUserProfile(user.userId, {
        fullName: formValue.fullName,
        age: formValue.age,
      }),
      this.technicianProfileService.updateUserPhoto(user.userId, formValue.photo),
      this.technicianProfileService.updateTechnician(technician.id, {
        nationalId: formValue.nationalId,
        experience: formValue.experience,
        maxTravelDistance: formValue.maxTravelDistance,
      }),
    ]).subscribe({
      next: () => {
        this.authService.updateSafeUser({ fullName: formValue.fullName });
        this.success.set('Technician profile updated.');
        this.saving.set(false);
        this.load();
      },
      error: () => {
        this.error.set('Unable to update technician profile.');
        this.saving.set(false);
      },
    });
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

    if (!user || !technician || this.photoForm.invalid) {
      this.photoForm.markAllAsTouched();
      return;
    }

    const formValue = this.photoForm.getRawValue();
    const editingId = this.editingPhotoId();
    this.savingPhoto.set(true);
    this.error.set(null);
    this.success.set(null);

    const request$: Observable<unknown> = editingId
      ? this.technicianProfileService.updatePhoto(editingId, formValue)
      : this.technicianProfileService.addPhoto({
          ...formValue,
          applicationUserId: user.userId,
          technicianId: technician.id,
        });

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
    this.photoForm.patchValue({
      photoUrl: photo.photoUrl,
      caption: photo.caption,
    });
  }

  resetPhotoForm(): void {
    this.editingPhotoId.set(null);
    this.photoForm.reset({
      photoUrl: '',
      caption: '',
    });
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
}
