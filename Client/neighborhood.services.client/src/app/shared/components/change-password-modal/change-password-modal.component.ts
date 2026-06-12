import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

export interface ChangePasswordFormValue {
  currentPassword: string;
  newPassword: string;
}

@Component({
  selector: 'app-change-password-modal',
  imports: [ReactiveFormsModule],
  template: `
    <div class="modal fade show d-block" tabindex="-1" role="dialog" aria-modal="true" aria-labelledby="changePasswordTitle">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg">
          <div class="modal-header border-0 pb-0">
            <div class="d-flex align-items-center gap-3">
              <span class="password-icon">
                <i class="bi bi-shield-lock" aria-hidden="true"></i>
              </span>
              <div>
                <h2 class="modal-title fs-5 fw-bold" id="changePasswordTitle">Change password</h2>
                <p class="text-muted small mb-0">Use your current password to set a new one.</p>
              </div>
            </div>
            <button class="btn-close" type="button" aria-label="Close" [disabled]="busy" (click)="cancel.emit()"></button>
          </div>

          <form [formGroup]="form" (ngSubmit)="submitForm()">
            <div class="modal-body py-4">
              @if (apiError) {
                <div class="alert alert-danger py-2">{{ apiError }}</div>
              }

              <div class="d-flex flex-column gap-3">
                <div>
                  <label class="form-label" for="currentPassword">Old password</label>
                  <input
                    id="currentPassword"
                    class="form-control"
                    type="password"
                    formControlName="currentPassword"
                    autocomplete="current-password"
                    [class.is-invalid]="form.controls.currentPassword.touched && form.controls.currentPassword.invalid"
                  />
                  @if (form.controls.currentPassword.touched && form.controls.currentPassword.invalid) {
                    <div class="invalid-feedback">Old password is required.</div>
                  }
                </div>

                <div>
                  <label class="form-label" for="newPassword">New password</label>
                  <input
                    id="newPassword"
                    class="form-control"
                    type="password"
                    formControlName="newPassword"
                    autocomplete="new-password"
                    [class.is-invalid]="form.controls.newPassword.touched && form.controls.newPassword.invalid"
                  />
                  @if (form.controls.newPassword.touched && form.controls.newPassword.hasError('required')) {
                    <div class="invalid-feedback">New password is required.</div>
                  } @else if (form.controls.newPassword.touched && form.controls.newPassword.hasError('minlength')) {
                    <div class="invalid-feedback">Password must be at least 6 characters.</div>
                  }
                </div>

                <div>
                  <label class="form-label" for="confirmPassword">Confirm password</label>
                  <input
                    id="confirmPassword"
                    class="form-control"
                    type="password"
                    formControlName="confirmPassword"
                    autocomplete="new-password"
                    [class.is-invalid]="form.controls.confirmPassword.touched && (form.controls.confirmPassword.invalid || passwordsDoNotMatch())"
                  />
                  @if (form.controls.confirmPassword.touched && form.controls.confirmPassword.hasError('required')) {
                    <div class="invalid-feedback">Confirm password is required.</div>
                  } @else if (form.controls.confirmPassword.touched && passwordsDoNotMatch()) {
                    <div class="invalid-feedback">Passwords do not match.</div>
                  }
                </div>
              </div>
            </div>

            <div class="modal-footer border-0 pt-0">
              <button class="btn btn-outline-secondary" type="button" [disabled]="busy" (click)="cancel.emit()">Cancel</button>
              <button class="btn btn-primary" type="submit" [disabled]="busy">
                @if (busy) {
                  <span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>
                  Updating...
                } @else {
                  Update password
                }
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
    <div class="modal-backdrop fade show"></div>
  `,
  styles: `
    .password-icon {
      align-items: center;
      background-color: #dbeafe;
      border-radius: 50%;
      color: #1d4ed8;
      display: flex;
      flex: 0 0 auto;
      font-size: 1.25rem;
      height: 2.75rem;
      justify-content: center;
      width: 2.75rem;
    }
  `,
})
export class ChangePasswordModalComponent {
  private readonly formBuilder = inject(FormBuilder);

  @Input() busy = false;
  @Input() apiError: string | null = null;
  @Output() submitPassword = new EventEmitter<ChangePasswordFormValue>();
  @Output() cancel = new EventEmitter<void>();

  readonly form = this.formBuilder.nonNullable.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required],
  });

  submitForm(): void {
    if (this.form.invalid || this.passwordsDoNotMatch()) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.submitPassword.emit({
      currentPassword: value.currentPassword,
      newPassword: value.newPassword,
    });
  }

  passwordsDoNotMatch(): boolean {
    const value = this.form.getRawValue();
    return !!value.confirmPassword && value.newPassword !== value.confirmPassword;
  }
}
