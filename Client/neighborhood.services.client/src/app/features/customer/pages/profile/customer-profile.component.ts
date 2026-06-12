import { Component, OnDestroy, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable, forkJoin, of, switchMap } from 'rxjs';
import { AuthService } from '../../../auth/services/auth.service';
import { environment } from '../../../../environments/environment';
import {
  CustomerAddress,
  CustomerAddressLabel,
  CustomerProfile,
  CustomerRecord,
} from '../../models/customer-profile.model';
import { CustomerProfileService } from '../../services/customer-profile.service';

@Component({
  selector: 'app-customer-profile',
  imports: [ReactiveFormsModule],
  styles: `
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
  `,
  template: `
    <div class="d-flex flex-column gap-4">
      <section class="bg-white border rounded-3 shadow-sm p-4">
        <div class="d-flex flex-column flex-md-row justify-content-between gap-3 mb-4">
          <div>
            <h2 class="h4 fw-bold mb-1">Profile</h2>
            <p class="text-muted mb-0">Manage your account details and saved service addresses.</p>
          </div>
          @if (profile()) {
            <span class="badge align-self-start" [class.text-bg-success]="profile()?.isActive" [class.text-bg-secondary]="!profile()?.isActive">
              {{ profile()?.isActive ? 'Active' : 'Inactive' }}
            </span>
          }
        </div>

        @if (loading()) {
          <div class="text-muted">Loading profile...</div>
        } @else {
          @if (error()) {
            <div class="alert alert-danger">{{ error() }}</div>
          }
          @if (success()) {
            <div class="alert alert-success">{{ success() }}</div>
          }

          <form [formGroup]="profileForm" (ngSubmit)="saveProfile()" class="row g-3">
            <div class="col-12 d-flex justify-content-center mb-2">
              <label class="profile-avatar-picker" for="profileAvatarFile" aria-label="Choose profile photo">
                @if (avatarPreviewUrl() || getPhotoSrc(profile()?.photo ?? ''); as photoUrl) {
                  <img [src]="photoUrl" alt="Profile photo preview" />
                } @else {
                  <span>{{ fallbackInitials() }}</span>
                }
                <span class="profile-avatar-action">
                  <i class="bi bi-camera" aria-hidden="true"></i>
                </span>
              </label>
              <input
                id="profileAvatarFile"
                class="visually-hidden"
                type="file"
                accept="image/*"
                (change)="onAvatarSelected($event)"
              />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="fullName">Full name</label>
              <input id="fullName" class="form-control" formControlName="fullName" [class.is-invalid]="profileForm.controls.fullName.touched && profileForm.controls.fullName.invalid" />
              @if (profileForm.controls.fullName.touched && profileForm.controls.fullName.invalid) {
                <div class="invalid-feedback">Full name is required.</div>
              }
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="email">Email</label>
              <input id="email" class="form-control" formControlName="email" readonly />
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="age">Age</label>
              <input id="age" type="number" class="form-control" formControlName="age" [class.is-invalid]="profileForm.controls.age.touched && profileForm.controls.age.invalid" />
              @if (profileForm.controls.age.touched && profileForm.controls.age.invalid) {
                <div class="invalid-feedback">Age must be between 13 and 120.</div>
              }
            </div>
            <div class="col-12 col-md-6">
              <label class="form-label" for="role">Role</label>
              <input id="role" class="form-control" [value]="profile()?.applicationUserRole || 'Customer'" readonly />
            </div>
            <div class="col-12">
              <button class="btn btn-primary" type="submit" [disabled]="savingProfile()">
                @if (savingProfile()) {
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
            <h2 class="h5 fw-bold mb-1">Saved addresses</h2>
            <p class="text-muted mb-0">Enter a specific address and we will locate it automatically.</p>
          </div>
        </div>

        <form [formGroup]="addressForm" (ngSubmit)="saveAddress()" class="row g-3 mb-4">
          <div class="col-12 col-md-3">
            <label class="form-label" for="label">Label</label>
            <select id="label" class="form-select" formControlName="label">
              @for (label of addressLabels; track label) {
                <option [value]="label">{{ label }}</option>
              }
            </select>
          </div>
          <div class="col-12 col-md-9">
            <label class="form-label" for="address">Address</label>
            <div class="input-group">
              <input id="address" class="form-control" formControlName="address" [class.is-invalid]="addressForm.controls.address.touched && addressForm.controls.address.invalid" />
              <button
                class="btn btn-outline-primary"
                type="button"
                title="Use my current location"
                [disabled]="locatingCurrentAddress()"
                (click)="useCurrentLocation()"
              >
                @if (locatingCurrentAddress()) {
                  <span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>
                  Locating...
                } @else {
                  <i class="bi bi-crosshair me-2" aria-hidden="true"></i>
                  Use my location
                }
              </button>
            </div>
            @if (addressForm.controls.address.touched && addressForm.controls.address.invalid) {
              <div class="text-danger small mt-1">Enter a specific address, area, and city.</div>
            }
            <div class="form-text">Example: 15 Tahrir Street, Dokki, Giza</div>
          </div>
          <div class="col-12">
            <div class="form-check">
              <input id="isDefault" class="form-check-input" type="checkbox" formControlName="isDefault" />
              <label class="form-check-label" for="isDefault">Default address</label>
            </div>
          </div>
          <div class="col-12 d-flex gap-2">
            <button class="btn btn-primary" type="submit" [disabled]="savingAddress() || !customer()">
              @if (savingAddress()) {
                <span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>
                Locating address...
              } @else {
                {{ editingAddressId() ? 'Update address' : 'Add address' }}
              }
            </button>
            @if (editingAddressId()) {
              <button class="btn btn-outline-secondary" type="button" (click)="resetAddressForm()">Cancel</button>
            }
          </div>
        </form>

        @if (addresses().length === 0) {
          <div class="text-muted">No addresses yet.</div>
        } @else {
          <div class="row g-3">
            @for (address of addresses(); track address.id) {
              <div class="col-12 col-lg-6">
                <div class="border rounded-3 p-3 h-100">
                  <div class="d-flex justify-content-between gap-2 mb-2">
                    <strong>{{ address.label }}</strong>
                    @if (address.isDefault) {
                      <span class="badge text-bg-primary">Default</span>
                    }
                  </div>
                  <p class="mb-2">{{ address.address }}</p>
                  <div class="d-flex flex-wrap gap-2">
                    <button class="btn btn-sm btn-outline-primary" type="button" (click)="editAddress(address)">Edit</button>
                    <button class="btn btn-sm btn-outline-secondary" type="button" (click)="setDefault(address)" [disabled]="address.isDefault">Set default</button>
                    <button class="btn btn-sm btn-outline-danger" type="button" (click)="openDeleteModal(address)">Delete</button>
                  </div>
                </div>
              </div>
            }
          </div>
        }
      </section>
    </div>

    @if (addressPendingDelete(); as address) {
      <div class="modal fade show d-block" tabindex="-1" role="dialog" aria-modal="true" aria-labelledby="deleteAddressTitle">
        <div class="modal-dialog modal-dialog-centered">
          <div class="modal-content border-0 shadow-lg">
            <div class="modal-header border-0 pb-0">
              <div class="d-flex align-items-center gap-3">
                <span class="d-inline-flex align-items-center justify-content-center rounded-circle bg-danger-subtle text-danger fs-5" style="width: 2.75rem; height: 2.75rem;">
                  <i class="bi bi-trash3" aria-hidden="true"></i>
                </span>
                <div>
                  <h2 class="modal-title fs-5 fw-bold" id="deleteAddressTitle">Delete address?</h2>
                  <p class="text-muted small mb-0">This action cannot be undone.</p>
                </div>
              </div>
              <button class="btn-close" type="button" aria-label="Close" [disabled]="deletingAddress()" (click)="closeDeleteModal()"></button>
            </div>
            <div class="modal-body py-4">
              <div class="border rounded-3 p-3 bg-body-tertiary">
                <div class="fw-semibold mb-1">{{ address.label }}</div>
                <div class="text-muted">{{ address.address }}</div>
              </div>
            </div>
            <div class="modal-footer border-0 pt-0">
              <button class="btn btn-outline-secondary" type="button" [disabled]="deletingAddress()" (click)="closeDeleteModal()">Cancel</button>
              <button class="btn btn-danger" type="button" [disabled]="deletingAddress()" (click)="confirmDeleteAddress()">
                @if (deletingAddress()) {
                  <span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>
                  Deleting...
                } @else {
                  <i class="bi bi-trash3 me-2" aria-hidden="true"></i>
                  Delete address
                }
              </button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade show"></div>
    }
  `,
})
export class CustomerProfileComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly customerProfileService = inject(CustomerProfileService);
  private readonly formBuilder = inject(FormBuilder);

  readonly addressLabels: CustomerAddressLabel[] = ['Home', 'Work', 'Other'];
  readonly loading = signal(true);
  readonly savingProfile = signal(false);
  readonly savingAddress = signal(false);
  readonly locatingCurrentAddress = signal(false);
  readonly deletingAddress = signal(false);
  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);
  readonly profile = signal<CustomerProfile | null>(null);
  readonly customer = signal<CustomerRecord | null>(null);
  readonly addresses = signal<CustomerAddress[]>([]);
  readonly editingAddressId = signal<number | null>(null);
  readonly addressPendingDelete = signal<CustomerAddress | null>(null);
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

  readonly profileForm = this.formBuilder.nonNullable.group({
    fullName: ['', Validators.required],
    email: [''],
    age: [18, [Validators.required, Validators.min(13), Validators.max(120)]],
  });

  readonly addressForm = this.formBuilder.nonNullable.group({
    label: ['Home' as CustomerAddressLabel, Validators.required],
    address: ['', [Validators.required, Validators.minLength(5)]],
    isDefault: [false],
  });

  constructor() {
    this.profileForm.controls.fullName.valueChanges.subscribe((fullName) => {
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
      this.loading.set(false);
      this.error.set('You must be logged in to view your profile.');
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      profile: this.customerProfileService.getProfile(user.userId),
      customer: this.customerProfileService.getCurrentCustomer(),
      addresses: this.customerProfileService.getAddressesByUserId(user.userId),
    }).subscribe({
      next: ({ profile, customer, addresses }) => {
        this.profile.set(profile);
        this.customer.set(customer);
        this.addresses.set(addresses.filter((address) => !address.isDeleted));
        this.authService.updateSafeUser({ fullName: profile.fullName, photo: profile.photo });
        this.profileForm.patchValue({
          fullName: profile.fullName,
          email: profile.email,
          age: profile.age,
        });
        this.fullNameValue.set(profile.fullName);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Unable to load profile data.');
        this.loading.set(false);
      },
    });
  }

  saveProfile(): void {
    const user = this.authService.currentUser();

    if (!user || this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    const formValue = this.profileForm.getRawValue();
    this.savingProfile.set(true);
    this.error.set(null);
    this.success.set(null);

    this.customerProfileService
      .updateProfile(user.userId, {
        fullName: formValue.fullName,
        age: formValue.age,
      })
      .pipe(
        switchMap(() =>
          this.selectedAvatarFile()
            ? this.customerProfileService.uploadUserPhoto(this.selectedAvatarFile() as File).pipe(
                switchMap(({ photoUrl }) =>
                  this.customerProfileService.updatePhoto(user.userId, photoUrl).pipe(
                    switchMap(() => of(photoUrl)),
                  ),
                ),
              )
            : of(this.profile()?.photo ?? ''),
        ),
      )
      .subscribe({
      next: (photoUrl) => {
        this.authService.updateSafeUser({ fullName: formValue.fullName, photo: photoUrl });
        this.success.set('Profile updated.');
        this.savingProfile.set(false);
        this.selectedAvatarFile.set(null);
        this.revokeAvatarPreview();
        this.load();
      },
      error: () => {
        this.error.set('Unable to update profile.');
        this.savingProfile.set(false);
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

  saveAddress(): void {
    const user = this.authService.currentUser();

    if (!user || this.addressForm.invalid) {
      this.addressForm.markAllAsTouched();
      return;
    }

    const customer = this.customer();
    const editingId = this.editingAddressId();

    if (!editingId && !customer) {
      this.error.set('Unable to find your customer profile.');
      return;
    }

    this.savingAddress.set(true);
    this.error.set(null);
    this.success.set(null);

    const formValue = this.addressForm.getRawValue();
    this.customerProfileService.geocodeAddress(formValue.address.trim()).subscribe({
      next: (location) => {
        const addressRequest = {
          label: formValue.label,
          address: location.formattedAddress || formValue.address.trim(),
          latitude: location.latitude,
          longitude: location.longitude,
        };
        const request$: Observable<unknown> = editingId
          ? this.customerProfileService.updateAddress(editingId, addressRequest)
          : this.customerProfileService.createAddress(
              user.userId,
              customer?.id ?? 0,
              { ...addressRequest, isDefault: formValue.isDefault },
            );

        request$.subscribe({
          next: () => {
            this.success.set(editingId ? 'Address updated.' : 'Address added.');
            this.savingAddress.set(false);
            this.resetAddressForm();
            this.load();
          },
          error: () => {
            this.error.set('Unable to save address.');
            this.savingAddress.set(false);
          },
        });
      },
      error: () => {
        this.error.set('Could not find this address. Please enter a more specific address.');
        this.savingAddress.set(false);
      },
    });
  }

  useCurrentLocation(): void {
    this.error.set(null);
    this.success.set(null);

    if (!navigator.geolocation) {
      this.error.set('Location access is not supported by this browser.');
      return;
    }

    this.locatingCurrentAddress.set(true);
    navigator.geolocation.getCurrentPosition(
      ({ coords }) => {
        this.customerProfileService.reverseGeocode(coords.latitude, coords.longitude).subscribe({
          next: (location) => {
            this.addressForm.controls.address.setValue(location.formattedAddress);
            this.addressForm.controls.address.markAsTouched();
            this.success.set('Current location found. Review the address before saving.');
            this.locatingCurrentAddress.set(false);
          },
          error: () => {
            this.error.set('Could not find an address for your current location.');
            this.locatingCurrentAddress.set(false);
          },
        });
      },
      (positionError) => {
        this.error.set(this.getLocationErrorMessage(positionError));
        this.locatingCurrentAddress.set(false);
      },
      {
        enableHighAccuracy: true,
        timeout: 15000,
        maximumAge: 60000,
      },
    );
  }

  editAddress(address: CustomerAddress): void {
    this.editingAddressId.set(address.id);
    this.addressForm.patchValue({
      label: address.label,
      address: address.address,
      isDefault: address.isDefault,
    });
  }

  resetAddressForm(): void {
    this.editingAddressId.set(null);
    this.addressForm.reset({
      label: 'Home',
      address: '',
      isDefault: false,
    });
  }

  setDefault(address: CustomerAddress): void {
    this.customerProfileService.setDefaultAddress(address.id).subscribe({
      next: () => this.load(),
      error: () => this.error.set('Unable to set default address.'),
    });
  }

  openDeleteModal(address: CustomerAddress): void {
    this.addressPendingDelete.set(address);
  }

  closeDeleteModal(): void {
    if (this.deletingAddress()) {
      return;
    }

    this.addressPendingDelete.set(null);
  }

  confirmDeleteAddress(): void {
    const address = this.addressPendingDelete();

    if (!address) {
      return;
    }

    this.deletingAddress.set(true);
    this.error.set(null);
    this.success.set(null);

    this.customerProfileService.deleteAddress(address.id).subscribe({
      next: () => {
        this.deletingAddress.set(false);
        this.addressPendingDelete.set(null);
        this.success.set('Address deleted.');
        this.load();
      },
      error: () => {
        this.error.set('Unable to delete address.');
        this.deletingAddress.set(false);
      },
    });
  }

  private getLocationErrorMessage(error: GeolocationPositionError): string {
    if (error.code === error.PERMISSION_DENIED) {
      return 'Location permission was denied. Allow location access in your browser and try again.';
    }

    if (error.code === error.POSITION_UNAVAILABLE) {
      return 'Your current location is unavailable. Please enter your address manually.';
    }

    if (error.code === error.TIMEOUT) {
      return 'Finding your location took too long. Please try again.';
    }

    return 'Could not access your current location.';
  }

  private revokeAvatarPreview(): void {
    const previewUrl = this.avatarPreviewUrl();

    if (previewUrl) {
      URL.revokeObjectURL(previewUrl);
    }

    this.avatarPreviewUrl.set(null);
  }
}
