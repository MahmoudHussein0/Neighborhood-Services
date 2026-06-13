import { Component, OnInit, signal, computed, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { TechnicianWalletService } from '../../services/technician-wallet.service';
import { Wallet, Transaction, TransactionStatus, TransactionType } from '../../models/wallet.model';

@Component({
  selector: 'app-technician-wallet',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './technician-wallet.component.html',
  styleUrls: ['./technician-wallet.component.css']
})
export class TechnicianWalletComponent implements OnInit, AfterViewInit {
  wallet = signal<Wallet | null>(null);
  transactions = signal<Transaction[]>([]);

  // Pagination
  currentPage = signal<number>(1);
  readonly pageSize = 6;

  paginatedTransactions = computed(() => {
    const start = (this.currentPage() - 1) * this.pageSize;
    return this.transactions().slice(start, start + this.pageSize);
  });

  emptyRows = computed(() => {
    const currentRecords = this.paginatedTransactions().length;
    if (currentRecords === 0 || currentRecords === this.pageSize) return [];
    return Array(this.pageSize - currentRecords).fill(0);
  });

  totalPages = computed(() => {
    return Math.ceil(this.transactions().length / this.pageSize) || 1;
  });

  // Withdraw Modal State
  withdrawAmount = signal<number | null>(null);
  isWithdrawing = signal<boolean>(false);

  // Enums for template
  TransactionStatus = TransactionStatus;
  TransactionType = TransactionType;

  constructor(
    private walletService: TechnicianWalletService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  ngAfterViewInit(): void {
    // Event listeners removed due to unreliability across language changes
  }

  focusInput(inputId: string): void {
    setTimeout(() => {
      document.getElementById(inputId)?.focus();
    }, 500);
  }

  loadData(): void {
    this.walletService.getMyWallet().subscribe({
      next: (w) => this.wallet.set(w),
      error: () => this.toastr.error('Failed to load wallet')
    });

    this.walletService.getMyTransactions().subscribe({
      next: (t) => this.transactions.set(t),
      error: () => this.toastr.error('Failed to load transactions')
    });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
    }
  }

  initiateWithdraw(): void {
    const amount = this.withdrawAmount();
    if (!amount || amount <= 0) {
      this.toastr.warning('Please enter a valid withdrawal amount');
      return;
    }
    const balance = this.wallet()?.balance ?? 0;
    if (amount > balance) {
      this.toastr.error('Withdrawal amount exceeds your current balance');
      return;
    }

    this.isWithdrawing.set(true);
    this.walletService.withdraw(amount).subscribe({
      next: () => {
        this.toastr.success('Withdrawal request submitted successfully!');
        this.withdrawAmount.set(null);
        this.isWithdrawing.set(false);
        this.loadData();
      },
      error: (err) => {
        const msg = err?.error?.message || err?.error?.title || 'Failed to process withdrawal';
        this.toastr.error(msg);
        this.isWithdrawing.set(false);
      }
    });
  }

  getStatusBadgeClass(status: any): string {
    const s = status?.toString();
    if (s === 'Completed' || s === '1') return 'bg-success';
    if (s === 'Pending' || s === '0') return 'bg-warning text-dark';
    if (s === 'Failed' || s === '2') return 'bg-danger';
    if (s === 'Reversed' || s === '3') return 'bg-secondary';
    return 'bg-secondary';
  }

  getStatusLabel(status: any): string {
    const s = status?.toString();
    if (s === 'Pending' || s === '0') return 'wallet.statusType.pending';
    if (s === 'Completed' || s === '1') return 'wallet.statusType.completed';
    if (s === 'Failed' || s === '2') return 'wallet.statusType.failed';
    if (s === 'Reversed' || s === '3') return 'wallet.statusType.reversed';
    return s ?? 'Unknown';
  }

  getTypeLabel(type: any): string {
    const t = type?.toString();
    if (t === 'TopUp' || t === '0') return 'wallet.transactionType.topUp';
    if (t === 'Transfer' || t === '1') return 'wallet.transactionType.transfer';
    if (t === 'Withdrawal' || t === '2') return 'wallet.transactionType.withdrawal';
    if (t === 'BookingPayment' || t === '3') return 'wallet.transactionType.bookingPayment';
    if (t === 'Refund' || t === '4') return 'wallet.transactionType.refund';
    if (t === 'Reversal' || t === '5') return 'wallet.transactionType.reversal';
    return t ?? 'Unknown';
  }
}
