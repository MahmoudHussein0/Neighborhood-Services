import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { TechnicianWalletService } from '../../services/technician-wallet.service';
import { Wallet, Transaction, TransactionStatus, TransactionType } from '../../models/wallet.model';

@Component({
  selector: 'app-technician-wallet',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './technician-wallet.component.html',
  styleUrls: ['./technician-wallet.component.css']
})
export class TechnicianWalletComponent implements OnInit {
  wallet = signal<Wallet | null>(null);
  transactions = signal<Transaction[]>([]);
  
  TransactionStatus = TransactionStatus;
  TransactionType = TransactionType;

  constructor(
    private walletService: TechnicianWalletService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.walletService.getMyWallet().subscribe({
      next: (w) => this.wallet.set(w),
      error: () => this.toastr.error('Failed to load wallet')
    });

    this.walletService.getMyTransactions().subscribe({
      next: (t) => this.transactions.set(t),
      error: () => this.toastr.error('Failed to load transactions')
    });
  }

  getStatusBadgeClass(status: TransactionStatus): string {
    switch(status) {
      case TransactionStatus.Completed: return 'bg-success';
      case TransactionStatus.Pending: return 'bg-warning text-dark';
      case TransactionStatus.Failed: return 'bg-danger';
      case TransactionStatus.Reversed: return 'bg-secondary';
      default: return 'bg-primary';
    }
  }

  getTypeLabel(type: TransactionType): string {
    return TransactionType[type];
  }
}
