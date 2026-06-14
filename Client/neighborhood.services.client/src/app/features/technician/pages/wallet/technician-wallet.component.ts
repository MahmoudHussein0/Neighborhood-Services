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

  // Filters
  filterType = signal<TransactionType | ''>('');
  filterStatus = signal<TransactionStatus | ''>('');

  filteredTransactions = computed(() => {
    let list = this.transactions();
    const type = this.filterType();
    const status = this.filterStatus();
    
    if (type !== '') {
      list = list.filter(t => {
        const tTypeStr = t.type?.toString();
        return tTypeStr === type.toString() || tTypeStr === TransactionType[Number(type)];
      });
    }
    if (status !== '') {
      list = list.filter(t => {
        const tStatusStr = t.status?.toString();
        return tStatusStr === status.toString() || tStatusStr === TransactionStatus[Number(status)];
      });
    }
    return list;
  });

  paginatedTransactions = computed(() => {
    const start = (this.currentPage() - 1) * this.pageSize;
    return this.filteredTransactions().slice(start, start + this.pageSize);
  });

  getFilterTypeLabel(type: TransactionType | ''): string {
    if (type === '') return 'wallet.allTypes';
    switch (Number(type)) {
      case TransactionType.TopUp: return 'wallet.transactionType.topUp';
      case TransactionType.Withdrawal: return 'wallet.transactionType.withdrawal';
      case TransactionType.BookingPayment: return 'wallet.transactionType.bookingPayment';
      case TransactionType.Refund: return 'wallet.transactionType.refund';
      case TransactionType.Transfer: return 'wallet.transactionType.transfer';
      default: return 'wallet.allTypes';
    }
  }

  getFilterStatusLabel(status: TransactionStatus | ''): string {
    if (status === '') return 'wallet.allStatuses';
    switch (Number(status)) {
      case TransactionStatus.Pending: return 'wallet.statusType.pending';
      case TransactionStatus.Completed: return 'wallet.statusType.completed';
      case TransactionStatus.Failed: return 'wallet.statusType.failed';
      case TransactionStatus.Reversed: return 'wallet.statusType.reversed';
      default: return 'wallet.allStatuses';
    }
  }

  emptyRows = computed(() => {
    const currentRecords = this.paginatedTransactions().length;
    if (currentRecords === 0 || currentRecords === this.pageSize) return [];
    return Array(this.pageSize - currentRecords).fill(0);
  });

  totalPages = computed(() => {
    return Math.ceil(this.filteredTransactions().length / this.pageSize) || 1;
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
    if (s === 'Completed' || s === '1') return 'bg-success bg-opacity-10 text-success fw-bold px-3 py-2 rounded-pill';
    if (s === 'Pending' || s === '0') return 'bg-warning bg-opacity-10 text-pending-brown fw-bold px-3 py-2 rounded-pill';
    if (s === 'Failed' || s === '2') return 'bg-danger bg-opacity-10 text-danger fw-bold px-3 py-2 rounded-pill';
    if (s === 'Reversed' || s === '3') return 'bg-secondary bg-opacity-10 text-secondary fw-bold px-3 py-2 rounded-pill';
    return 'bg-secondary bg-opacity-10 text-secondary fw-bold px-3 py-2 rounded-pill';
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
