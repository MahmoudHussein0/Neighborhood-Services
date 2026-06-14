import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
  OnChanges,
  SimpleChanges
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';

import { DisputeService } from '../../services/disputes.service';
import { DisputeDto } from '../../models/staff-disput.model';

type DisputeAction = 'banCustomer' | 'banTech' | 'refund' | 'release';

@Component({
  selector: 'app-dispute-details',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './dispute-details.component.html',
  styleUrls: ['./dispute-details.component.css']
})
export class DisputeDetailsComponent implements OnChanges {

  @Input() disputeId!: number;

  /** Emitted when the user dismisses the modal. */
  @Output() closed = new EventEmitter<void>();

  /** Emitted after an action succeeds, so the parent list can refresh. */
  @Output() changed = new EventEmitter<void>();

  dispute: DisputeDto | null = null;
  loading = false;
  error = '';

  /** Which action is currently in-flight (disables the buttons + shows a spinner). */
  actionInProgress: DisputeAction | null = null;
  actionError = '';
  actionMessage = '';

  /** The confirmation step currently shown over the modal (replaces native confirm()). */
  pendingConfirm: {
    action: DisputeAction;
    title: string;
    message: string;
    confirmLabel: string;
    danger: boolean;
  } | null = null;

  constructor(
    private disputeService: DisputeService,
    private cdr: ChangeDetectorRef,
    private translate: TranslateService
  ) {}

  private t(key: string, params?: object): string {
    return this.translate.instant('staffDisputes.details.' + key, params);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['disputeId']?.currentValue) {
      this.loadDispute();
    }
  }

  loadDispute(): void {
    this.loading = true;
    this.error = '';
    this.actionError = '';
    this.actionMessage = '';

    this.disputeService
      .getById(this.disputeId)
      .subscribe({
        next: (res) => {
          this.dispute = res;
          this.loading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.error = this.t('loadFail');
          this.loading = false;
          this.cdr.markForCheck();
        }
      });
  }

  close(): void {
    this.closed.emit();
  }

  get canSettleEscrow(): boolean {
    return !!this.dispute?.escrowId && this.dispute?.escrowStatus === 'Held';
  }

  // ── Action triggers: open the in-modal confirmation step ────────────────────

  askBanCustomer(): void {
    if (!this.dispute?.customerUserId) return;
    const name = this.dispute.customerName ?? this.dispute.customerUserId;
    this.pendingConfirm = {
      action: 'banCustomer',
      title: this.t('banCustomer'),
      message: this.t('banCustomerMsg', { name }),
      confirmLabel: this.t('banCustomer'),
      danger: true
    };
  }

  askBanTech(): void {
    if (!this.dispute?.technicianUserId) return;
    const name = this.dispute.technicianName ?? this.dispute.technicianUserId;
    this.pendingConfirm = {
      action: 'banTech',
      title: this.t('banTech'),
      message: this.t('banTechMsg', { name }),
      confirmLabel: this.t('banTech'),
      danger: true
    };
  }

  askRefund(): void {
    if (!this.canSettleEscrow) return;
    this.pendingConfirm = {
      action: 'refund',
      title: this.t('refundTitle'),
      message: this.t('refundMsg'),
      confirmLabel: this.t('refundConfirm'),
      danger: false
    };
  }

  askComplete(): void {
    if (!this.canSettleEscrow) return;
    this.pendingConfirm = {
      action: 'release',
      title: this.t('completeTitle'),
      message: this.t('completeMsg'),
      confirmLabel: this.t('releaseConfirm'),
      danger: false
    };
  }

  cancelConfirm(): void {
    this.pendingConfirm = null;
  }

  /** Runs the action the user just confirmed in the in-modal dialog. */
  confirmAction(): void {
    const action = this.pendingConfirm?.action;
    this.pendingConfirm = null;
    if (!action || !this.dispute) return;

    switch (action) {
      case 'banCustomer':
        if (this.dispute.customerUserId)
          this.runAction('banCustomer',
            this.disputeService.banUser(this.dispute.customerUserId),
            this.t('bannedCustomer'));
        break;
      case 'banTech':
        if (this.dispute.technicianUserId)
          this.runAction('banTech',
            this.disputeService.banUser(this.dispute.technicianUserId),
            this.t('bannedTech'));
        break;
      case 'refund':
        if (this.dispute.escrowId)
          this.runAction('refund',
            this.disputeService.refundEscrow(this.dispute.escrowId),
            this.t('refunded'));
        break;
      case 'release':
        if (this.dispute.escrowId)
          this.runAction('release',
            this.disputeService.releaseEscrow(this.dispute.escrowId),
            this.t('released'));
        break;
    }
  }

  private runAction(action: DisputeAction, call: Observable<unknown>, successMsg: string): void {
    this.actionInProgress = action;
    this.actionError = '';
    this.actionMessage = '';

    call.subscribe({
      next: () => {
        this.actionInProgress = null;
        this.actionMessage = successMsg;
        this.cdr.markForCheck();
        // Refresh the dispute (escrow status may have changed) and the parent list.
        this.loadDisputeKeepingMessage(successMsg);
        this.changed.emit();
      },
      error: (err) => {
        this.actionInProgress = null;
        this.actionError =
          err?.error?.detail ||
          err?.error?.message ||
          this.t('actionFail');
        this.cdr.markForCheck();
      }
    });
  }

  /** Reload the dispute but preserve the success banner the action just set. */
  private loadDisputeKeepingMessage(msg: string): void {
    this.disputeService.getById(this.disputeId).subscribe({
      next: (res) => {
        this.dispute = res;
        this.actionMessage = msg;
        this.cdr.markForCheck();
      }
    });
  }
}
