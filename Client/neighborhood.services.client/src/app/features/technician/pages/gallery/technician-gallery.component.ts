import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Observable, of, switchMap } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { ConfirmModalComponent } from '../../../../shared/components/confirm-modal/confirm-modal.component';
import { AuthService } from '../../../auth/services/auth.service';
import { environment } from '../../../../environments/environment';
import { TechnicianPhoto, TechnicianProfile } from '../../models/technician-profile.model';
import { TechnicianProfileService } from '../../services/technician-profile.service';

@Component({
  selector: 'app-technician-gallery',
  imports: [DatePipe, ReactiveFormsModule, ConfirmModalComponent, TranslatePipe],
  template: `
    <div class="d-flex flex-column gap-4">
      <section class="bg-white border rounded-3 shadow-sm p-4">
        <div class="d-flex flex-column flex-lg-row justify-content-between gap-3 mb-4">
          <div>
            <h2 class="h4 fw-bold mb-1">{{ 'gallery.title' | translate }}</h2>
            <p class="text-muted mb-0">{{ 'gallery.subtitle' | translate }}</p>
          </div>
        </div>

        @if (error()) {
          <div class="alert alert-danger">{{ error() }}</div>
        }
        @if (success()) {
          <div class="alert alert-success">{{ success() }}</div>
        }

        <form [formGroup]="photoForm" (ngSubmit)="savePhoto()" class="row g-3 mb-4">
          <div class="col-12 col-lg-7">
            <label class="form-label" for="portfolioPhotoFile">{{ 'gallery.photo' | translate }}</label>
            <label class="portfolio-uploader" for="portfolioPhotoFile" [class.border-danger]="photoPickerTouched() && !selectedPhotoFile() && !editingPhotoId()">
              @if (selectedPhotoPreviewUrl() || editingPhotoPreviewUrl(); as previewUrl) {
                <img [src]="previewUrl" alt="Selected portfolio preview" />
                <span class="portfolio-uploader-action">
                  <i class="bi bi-camera" aria-hidden="true"></i>
                  {{ 'gallery.changePhoto' | translate }}
                </span>
              } @else {
                <span class="portfolio-uploader-empty">
                  <i class="bi bi-image fs-3" aria-hidden="true"></i>
                  <span class="fw-semibold">{{ 'gallery.addPhoto' | translate }}</span>
                  <span class="small text-muted">{{ 'gallery.fileHint' | translate }}</span>
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
              <div class="text-danger small mt-1">{{ 'gallery.chooseError' | translate }}</div>
            }
          </div>
          <div class="col-12 col-lg-5">
            <label class="form-label" for="caption">{{ 'gallery.caption' | translate }}</label>
            <input id="caption" class="form-control" formControlName="caption" />
          </div>
          <div class="col-12 d-flex gap-2">
            <button class="btn btn-primary" type="submit" [disabled]="savingPhoto() || photoForm.invalid || !technician()">
              @if (savingPhoto()) {
                <span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>
                {{ 'gallery.saving' | translate }}
              } @else {
                {{ editingPhotoId() ? ('gallery.updatePhoto' | translate) : ('gallery.addPhoto' | translate) }}
              }
            </button>
            @if (editingPhotoId()) {
              <button class="btn btn-outline-secondary" type="button" (click)="resetPhotoForm()">{{ 'common.cancel' | translate }}</button>
            }
          </div>
        </form>

        @if (loading()) {
          <div class="text-muted">{{ 'gallery.loading' | translate }}</div>
        } @else if (photos().length === 0) {
          <div class="text-muted">{{ 'gallery.empty' | translate }}</div>
        } @else {
          <div class="row g-3">
            @for (photo of photos(); track photo.id) {
              <div class="col-12 col-md-6 col-xl-4">
                <div class="border rounded-3 overflow-hidden h-100 bg-white">
                  <div class="portfolio-image bg-light">
                    <img [src]="getPhotoSrc(photo.photoUrl)" [alt]="photo.caption || 'Technician work sample'" />
                  </div>
                  <div class="p-3">
                    <h3 class="h6 fw-bold mb-1">{{ photo.caption || ('gallery.workSample' | translate) }}</h3>
                    <div class="text-muted small mb-3">{{ photo.createdAt | date: 'mediumDate' }}</div>
                    <div class="d-flex gap-2">
                      <button class="btn btn-sm btn-outline-primary" type="button" (click)="editPhoto(photo)">{{ 'common.edit' | translate }}</button>
                      <button class="btn btn-sm btn-outline-danger" type="button" (click)="deletePhoto(photo)">{{ 'common.delete' | translate }}</button>
                    </div>
                  </div>
                </div>
              </div>
            }
          </div>
        }
      </section>

      @if (photoPendingDelete(); as photo) {
        <app-confirm-modal
          [title]="'gallery.deleteTitle' | translate"
          [subtitle]="'gallery.deleteSubtitle' | translate"
          [message]="photo.caption ? ('gallery.deleteMessageWithCaption' | translate: { caption: photo.caption }) : ('gallery.deleteMessage' | translate)"
          [confirmText]="'gallery.deleteConfirm' | translate"
          [busyText]="'gallery.deleting' | translate"
          [busy]="deletingPhoto()"
          variant="danger"
          (confirm)="confirmDeletePhoto()"
          (cancel)="closeDeletePhotoModal()"
        />
      }
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
  `],
})
export class TechnicianGalleryComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly technicianProfileService = inject(TechnicianProfileService);
  private readonly formBuilder = inject(FormBuilder);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);
  readonly technician = signal<TechnicianProfile | null>(null);
  readonly photos = signal<TechnicianPhoto[]>([]);
  readonly savingPhoto = signal(false);
  readonly editingPhotoId = signal<number | null>(null);
  readonly selectedPhotoFile = signal<File | null>(null);
  readonly selectedPhotoPreviewUrl = signal<string | null>(null);
  readonly editingPhotoPreviewUrl = signal<string | null>(null);
  readonly photoPickerTouched = signal(false);
  readonly photoPendingDelete = signal<TechnicianPhoto | null>(null);
  readonly deletingPhoto = signal(false);

  readonly photoForm = this.formBuilder.nonNullable.group({
    caption: [''],
  });

  ngOnInit(): void {
    this.load();
  }

  ngOnDestroy(): void {
    this.revokeSelectedPhotoPreview();
  }

  load(): void {
    const user = this.authService.currentUser();

    if (!user) {
      this.error.set('You must be logged in to view your gallery.');
      this.loading.set(false);
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    this.technicianProfileService.getTechnicianByUserId(user.userId).subscribe({
      next: (technician) => {
        this.technician.set(technician);
        this.loadPhotos(technician.id);
      },
      error: () => {
        this.error.set('Unable to load your gallery.');
        this.loading.set(false);
      },
    });
  }

  loadPhotos(technicianId: number): void {
    this.technicianProfileService.getPhotosByTechnicianId(technicianId).subscribe({
      next: (photos) => {
        this.photos.set(photos);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load portfolio photos.');
        this.loading.set(false);
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
    this.photoPendingDelete.set(photo);
  }

  closeDeletePhotoModal(): void {
    if (this.deletingPhoto()) {
      return;
    }

    this.photoPendingDelete.set(null);
  }

  confirmDeletePhoto(): void {
    const technician = this.technician();
    const photo = this.photoPendingDelete();

    if (!technician || !photo) {
      return;
    }

    this.deletingPhoto.set(true);
    this.error.set(null);
    this.success.set(null);

    this.technicianProfileService.deletePhoto(photo.id).subscribe({
      next: () => {
        this.success.set('Portfolio photo deleted.');
        this.deletingPhoto.set(false);
        this.photoPendingDelete.set(null);
        this.loadPhotos(technician.id);
      },
      error: () => {
        this.error.set('Unable to delete portfolio photo.');
        this.deletingPhoto.set(false);
      },
    });
  }

  private revokeSelectedPhotoPreview(): void {
    const previewUrl = this.selectedPhotoPreviewUrl();

    if (previewUrl) {
      URL.revokeObjectURL(previewUrl);
    }
  }
}
