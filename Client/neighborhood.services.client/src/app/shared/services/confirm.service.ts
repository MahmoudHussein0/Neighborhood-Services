import { Injectable, inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from '../components/confirm-dialog/confirm-dialog.component';

export interface ConfirmOptions {
  /** i18n key for the message body (required). */
  messageKey: string;
  /** Interpolation params for the message, e.g. { price: 350 }. */
  messageParams?: Record<string, unknown>;
  /** i18n key for the title. Defaults to a generic "Please confirm". */
  titleKey?: string;
  /** i18n key for the confirm button. Defaults to "Confirm". */
  confirmKey?: string;
  /** i18n key for the cancel button. Defaults to "Cancel". */
  cancelKey?: string;
  /** Confirm button colour. Use 'danger' for destructive actions. */
  variant?: 'primary' | 'danger' | 'success';
}

/**
 * Opens a confirm/cancel modal and resolves to true (confirmed) or false (cancelled/dismissed).
 * Replaces native window.confirm() so dialogs match the app's look.
 */
@Injectable({ providedIn: 'root' })
export class ConfirmService {
  private readonly modal = inject(NgbModal);

  confirm(opts: ConfirmOptions): Promise<boolean> {
    const ref = this.modal.open(ConfirmDialogComponent, { centered: true });
    const ci = ref.componentInstance as ConfirmDialogComponent;
    ci.messageKey = opts.messageKey;
    ci.messageParams = opts.messageParams ?? {};
    ci.titleKey = opts.titleKey ?? 'common.confirmTitle';
    ci.confirmKey = opts.confirmKey ?? 'common.confirm';
    ci.cancelKey = opts.cancelKey ?? 'common.cancel';
    ci.variant = opts.variant ?? 'primary';
    return ref.result.then(
      () => true,
      () => false,
    );
  }
}
