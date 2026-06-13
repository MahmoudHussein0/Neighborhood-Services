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
import { Observable } from 'rxjs';

import { DisputeService } from '../../services/disputes.service';
import { DisputeDto } from '../../models/staff-disput.model';

type DisputeAction = 'banCustomer' | 'banTech' | 'refund' | 'release';

@Component({
  selector: 'app-dispute-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
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
    private cdr: ChangeDetectorRef
  ) {}

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
          this.error = 'Failed to load dispute details';
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
      title: 'Ban Customer',
      message: `Ban the customer (${name})? They will no longer be able to use the platform.`,
      confirmLabel: 'Ban Customer',
      danger: true
    };
  }

  askBanTech(): void {
    if (!this.dispute?.technicianUserId) return;
    const name = this.dispute.technicianName ?? this.dispute.technicianUserId;
    this.pendingConfirm = {
      action: 'banTech',
      title: 'Ban Technician',
      message: `Ban the technician (${name})? They will no longer be able to use the platform.`,
      confirmLabel: 'Ban Technician',
      danger: true
    };
  }

  askRefund(): void {
    if (!this.canSettleEscrow) return;
    this.pendingConfirm = {
      action: 'refund',
      title: 'Refund Customer',
      message: 'Refund the held funds back to the customer? This cannot be undone.',
      confirmLabel: 'Refund',
      danger: false
    };
  }

  askComplete(): void {
    if (!this.canSettleEscrow) return;
    this.pendingConfirm = {
      action: 'release',
      title: 'Complete Transaction',
      message: 'Release the held funds to the technician? This cannot be undone.',
      confirmLabel: 'Release Funds',
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
            'Customer has been banned.');
        break;
      case 'banTech':
        if (this.dispute.technicianUserId)
          this.runAction('banTech',
            this.disputeService.banUser(this.dispute.technicianUserId),
            'Technician has been banned.');
        break;
      case 'refund':
        if (this.dispute.escrowId)
          this.runAction('refund',
            this.disputeService.refundEscrow(this.dispute.escrowId),
            'Funds refunded to the customer.');
        break;
      case 'release':
        if (this.dispute.escrowId)
          this.runAction('release',
            this.disputeService.releaseEscrow(this.dispute.escrowId),
            'Funds released to the technician.');
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
          'The action could not be completed.';
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
