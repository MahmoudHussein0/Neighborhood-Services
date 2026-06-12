import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-confirm-modal',
  template: `
    <div class="modal fade show d-block" tabindex="-1" role="dialog" aria-modal="true" [attr.aria-labelledby]="titleId">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg">
          <div class="modal-header border-0 pb-0">
            <div class="d-flex align-items-center gap-3">
              <span class="confirm-icon" [class.danger]="variant === 'danger'" [class.warning]="variant === 'warning'">
                <i class="bi" [class.bi-trash3]="variant === 'danger'" [class.bi-exclamation-triangle]="variant === 'warning'" aria-hidden="true"></i>
              </span>
              <div>
                <h2 class="modal-title fs-5 fw-bold" [id]="titleId">{{ title }}</h2>
                @if (subtitle) {
                  <p class="text-muted small mb-0">{{ subtitle }}</p>
                }
              </div>
            </div>
            <button class="btn-close" type="button" aria-label="Close" [disabled]="busy" (click)="cancel.emit()"></button>
          </div>

          <div class="modal-body py-4">
            <p class="mb-0">{{ message }}</p>
          </div>

          <div class="modal-footer border-0 pt-0">
            <button class="btn btn-outline-secondary" type="button" [disabled]="busy" (click)="cancel.emit()">
              {{ cancelText }}
            </button>
            <button class="btn" [class.btn-danger]="variant === 'danger'" [class.btn-warning]="variant === 'warning'" type="button" [disabled]="busy" (click)="confirm.emit()">
              @if (busy) {
                <span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>
                {{ busyText }}
              } @else {
                {{ confirmText }}
              }
            </button>
          </div>
        </div>
      </div>
    </div>
    <div class="modal-backdrop fade show"></div>
  `,
  styles: `
    .confirm-icon {
      align-items: center;
      border-radius: 50%;
      display: flex;
      flex: 0 0 auto;
      font-size: 1.25rem;
      height: 2.75rem;
      justify-content: center;
      width: 2.75rem;
    }

    .confirm-icon.danger {
      background-color: #fee2e2;
      color: #dc2626;
    }

    .confirm-icon.warning {
      background-color: #fef3c7;
      color: #b45309;
    }
  `,
})
export class ConfirmModalComponent {
  @Input({ required: true }) title = '';
  @Input() subtitle = '';
  @Input({ required: true }) message = '';
  @Input() confirmText = 'Confirm';
  @Input() cancelText = 'Cancel';
  @Input() busyText = 'Working...';
  @Input() busy = false;
  @Input() variant: 'danger' | 'warning' = 'danger';
  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  readonly titleId = `confirm-modal-${Math.random().toString(36).slice(2)}`;
}
