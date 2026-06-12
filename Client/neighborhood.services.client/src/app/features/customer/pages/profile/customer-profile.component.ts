import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable, forkJoin } from 'rxjs';
import { AuthService } from '../../../auth/services/auth.service';
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
            <div class="col-12 col-md-4">
              <label class="form-label" for="age">Age</label>
              <input id="age" type="number" class="form-control" formControlName="age" [class.is-invalid]="profileForm.controls.age.touched && profileForm.controls.age.invalid" />
              @if (profileForm.controls.age.touched && profileForm.controls.age.invalid) {
                <div class="invalid-feedback">Age must be between 13 and 120.</div>
              }
            </div>
            <div class="col-12 col-md-4">
              <label class="form-label" for="role">Role</label>
              <input id="role" class="form-control" [value]="profile()?.applicationUserRole || 'Customer'" readonly />
            </div>
            <div class="col-12 col-md-4">
              <label class="form-label" for="photo">Photo URL</label>
              <input id="photo" class="form-control" formControlName="photo" />
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
                    <button class="btn btn-sm btn-outline-danger" type="button" (click)="deleteAddress(address)">Delete</button>
                  </div>
                </div>
              </div>
            }
          </div>
        }
      </section>
    </div>
  `,
})
export class CustomerProfileComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly customerProfileService = inject(CustomerProfileService);
  private readonly formBuilder = inject(FormBuilder);

  readonly addressLabels: CustomerAddressLabel[] = ['Home', 'Work', 'Other'];
  readonly loading = signal(true);
  readonly savingProfile = signal(false);
  readonly savingAddress = signal(false);
  readonly locatingCurrentAddress = signal(false);
  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);
  readonly profile = signal<CustomerProfile | null>(null);
  readonly customer = signal<CustomerRecord | null>(null);
  readonly addresses = signal<CustomerAddress[]>([]);
  readonly editingAddressId = signal<number | null>(null);

  readonly profileForm = this.formBuilder.nonNullable.group({
    fullName: ['', Validators.required],
    email: [''],
    age: [18, [Validators.required, Validators.min(13), Validators.max(120)]],
    photo: [''],
  });

  readonly addressForm = this.formBuilder.nonNullable.group({
    label: ['Home' as CustomerAddressLabel, Validators.required],
    address: ['', [Validators.required, Validators.minLength(5)]],
    isDefault: [false],
  });

  ngOnInit(): void {
    this.load();
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
        this.profileForm.patchValue({
          fullName: profile.fullName,
          email: profile.email,
          age: profile.age,
          photo: profile.photo,
        });
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

    forkJoin([
      this.customerProfileService.updateProfile(user.userId, {
        fullName: formValue.fullName,
        age: formValue.age,
      }),
      this.customerProfileService.updatePhoto(user.userId, formValue.photo),
    ]).subscribe({
      next: () => {
        this.authService.updateSafeUser({ fullName: formValue.fullName });
        this.success.set('Profile updated.');
        this.savingProfile.set(false);
        this.load();
      },
      error: () => {
        this.error.set('Unable to update profile.');
        this.savingProfile.set(false);
      },
    });
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

  deleteAddress(address: CustomerAddress): void {
    if (!confirm('Delete this address?')) {
      return;
    }

    this.customerProfileService.deleteAddress(address.id).subscribe({
      next: () => this.load(),
      error: () => this.error.set('Unable to delete address.'),
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
}
