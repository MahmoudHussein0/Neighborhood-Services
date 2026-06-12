import { Component, inject } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

type ConfirmVariant = 'primary' | 'danger' | 'success';

/**
 * Generic confirm/cancel modal. Opened via ConfirmService — callers set the i18n
 * keys + variant through componentInstance. Closes with `true` on confirm and is
 * dismissed (rejected) on cancel.
 */
@Component({
  selector: 'app-confirm-dialog',
  imports: [TranslatePipe],
  templateUrl: './confirm-dialog.component.html',
})
export class ConfirmDialogComponent {
  private readonly activeModal = inject(NgbActiveModal);

  // i18n keys, set by the opener.
  titleKey = 'common.confirmTitle';
  messageKey = '';
  messageParams: Record<string, unknown> = {};
  confirmKey = 'common.confirm';
  cancelKey = 'common.cancel';
  variant: ConfirmVariant = 'primary';

  get confirmButtonClass(): string {
    return `btn btn-${this.variant}`;
  }

  confirm() {
    this.activeModal.close(true);
  }

  dismiss() {
    this.activeModal.dismiss(false);
  }
}
