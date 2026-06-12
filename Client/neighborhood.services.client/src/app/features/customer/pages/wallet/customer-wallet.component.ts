import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { CustomerWalletService } from '../../services/customer-wallet.service';
import { Wallet, Transaction, PaymentMethod, PaymentProvider, PaymentType, TransactionStatus, TransactionType } from '../../models/wallet.model';

@Component({
  selector: 'app-customer-wallet',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './customer-wallet.component.html',
  styleUrls: ['./customer-wallet.component.css']
})
export class CustomerWalletComponent implements OnInit, OnDestroy {
  wallet = signal<Wallet | null>(null);
  transactions = signal<Transaction[]>([]);
  paymentMethods = signal<PaymentMethod[]>([]);

  // Top Up Modal State
  topUpAmount = signal<number>(100);
  selectedPaymentMethodId = signal<number | null>(null);

  // Withdraw Modal State
  withdrawAmount = signal<number>(0);
  isWithdrawing = signal<boolean>(false);

  // Add Payment Method Modal State
  newCardNumber = signal<string>('');
  newCardExpiryMonth = signal<number>(1);
  newCardExpiryYear = signal<number>(new Date().getFullYear());

  // Enums for template
  TransactionStatus = TransactionStatus;
  TransactionType = TransactionType;
  PaymentProvider = PaymentProvider;
  PaymentType = PaymentType;

  private pollingInterval: any = null;

  constructor(
    private walletService: CustomerWalletService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Process Paymob Callback redirect
    this.route.queryParams.subscribe(params => {
      if (params['merchant_order_id'] && params['success']) {
        const orderId = +params['merchant_order_id'];
        const success = params['success'] === 'true';
        const token = params['token'] || '';
        const maskedPan = params['masked_pan'] || '';

        this.walletService.finalizeTransaction(orderId, success, token, maskedPan).subscribe({
          next: () => {
            if (success) {
              this.toastr.success('Payment completed successfully!');
            } else {
              this.toastr.error('Payment failed. Please try again.');
            }
            // Clear query params so refresh doesn't trigger it again
            this.router.navigate([], { queryParams: {} });
            this.loadData();
          },
          error: (err) => {
            console.error('Finalize error', err);
            // Still load data even if finalize fails
            this.loadData();
          }
        });
      } else {
        this.loadData();
      }
    });

    // Poll every 5 seconds to pick up new transactions (e.g., after Paymob callback)
    this.pollingInterval = setInterval(() => this.refreshTransactions(), 5000);
  }

  ngOnDestroy(): void {
    if (this.pollingInterval) {
      clearInterval(this.pollingInterval);
    }
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

    this.walletService.getMyPaymentMethods().subscribe({
      next: (pm) => this.paymentMethods.set(pm),
      error: () => this.toastr.error('Failed to load payment methods')
    });
  }

  refreshTransactions(): void {
    this.walletService.getMyWallet().subscribe({
      next: (w) => this.wallet.set(w)
    });
    this.walletService.getMyTransactions().subscribe({
      next: (t) => this.transactions.set(t)
    });
  }

  initiateTopUp(): void {
    const methodId = this.selectedPaymentMethodId();
    // methodId can be null if user wants to use a new card
    this.walletService.topUp(this.topUpAmount(), methodId !== null ? methodId : undefined, PaymentProvider.Paymob).subscribe({
      next: (res) => {
        // Redirect to Paymob payment page or append query string for instant pay
        if (res.redirectUrl.startsWith('?')) {
            const urlParams = new URLSearchParams(res.redirectUrl);
            const orderId = urlParams.get('merchant_order_id');
            const success = urlParams.get('success');
            this.router.navigate([], { queryParams: { merchant_order_id: orderId, success: success } });
        } else {
            window.location.href = res.redirectUrl;
        }
      },
      error: (err) => {
        const msg = err?.error?.errorMessage || err?.error?.title || 'Failed to initiate top-up';
        this.toastr.error(msg);
        // Refresh immediately to show the failed transaction
        this.refreshTransactions();
        console.error('Top-up error:', err);
      }
    });
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
        this.toastr.success(`Withdrawal of EGP ${amount.toFixed(2)} initiated successfully`);
        this.withdrawAmount.set(0);
        this.isWithdrawing.set(false);
        this.refreshTransactions();
      },
      error: (err) => {
        const msg = err?.error?.message || err?.error?.title || 'Failed to process withdrawal';
        this.toastr.error(msg);
        this.isWithdrawing.set(false);
      }
    });
  }

  deletePaymentMethod(id: number): void {
    if(confirm('Are you sure you want to delete this payment method?')) {
      this.walletService.deletePaymentMethod(id).subscribe({
        next: () => {
          this.toastr.success('Payment method deleted');
          this.walletService.getMyPaymentMethods().subscribe(pm => this.paymentMethods.set(pm));
          if (this.selectedPaymentMethodId() === id) {
            this.selectedPaymentMethodId.set(null);
          }
        },
        error: () => this.toastr.error('Failed to delete payment method')
      });
    }
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
    if (s === 'Pending' || s === '0') return 'Pending';
    if (s === 'Completed' || s === '1') return 'Completed';
    if (s === 'Failed' || s === '2') return 'Failed';
    if (s === 'Reversed' || s === '3') return 'Reversed';
    return s ?? 'Unknown';
  }

  getTypeLabel(type: any): string {
    const t = type?.toString();
    if (t === 'TopUp' || t === '0') return 'Top Up';
    if (t === 'Transfer' || t === '1') return 'Transfer';
    if (t === 'Withdrawal' || t === '2') return 'Withdrawal';
    if (t === 'BookingPayment' || t === '3') return 'Booking Payment';
    if (t === 'Refund' || t === '4') return 'Refund';
    if (t === 'Reversal' || t === '5') return 'Reversal';
    return t ?? 'Unknown';
  }
}
