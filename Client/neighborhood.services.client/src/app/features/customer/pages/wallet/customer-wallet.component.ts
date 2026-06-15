import { Component, OnInit, OnDestroy, signal, computed, AfterViewInit } from '@angular/core';
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
export class CustomerWalletComponent implements OnInit, OnDestroy, AfterViewInit {
  wallet = signal<Wallet | null>(null);
  transactions = signal<Transaction[]>([]);
  paymentMethods = signal<PaymentMethod[]>([]);

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
  // Top Up Modal State
  topUpAmount = signal<number | null>(null);
  selectedPaymentMethodId = signal<number | null>(null);
  isToppingUp = signal<boolean>(false);

  // Withdraw Modal State
  withdrawAmount = signal<number | null>(null);
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
    // Paymob may redirect to either:
    //   http://localhost:4200/#/customer/wallet?merchant_order_id=...  (hash routing - route.queryParams works)
    //   http://localhost:4200/#/customer/wallet  (MPGS 3DS - NO params, use localStorage)
    // So we check BOTH sources.
    const pathParams = new URLSearchParams(window.location.search);
    const pathMerchantOrderId = pathParams.get('merchant_order_id');
    const pathSuccess = pathParams.get('success');

    if (pathMerchantOrderId && pathSuccess !== null) {
      // Came from Paymob redirect via path-level URL (non-hash)
      const orderId = pathMerchantOrderId;
      const success = pathSuccess === 'true';
      const token = pathParams.get('token') || '';
      const maskedPan = pathParams.get('masked_pan') || '';
      window.history.replaceState({}, '', '/#/customer/wallet');
      this.walletService.finalizeTransaction(orderId, success, token, maskedPan).subscribe({
        next: () => {
          if (success) {
            this.toastr.success('Payment completed successfully!');
          } else {
            this.toastr.error('Payment failed. Please try again.');
          }
          this.loadData();
          if (window.opener) {
            setTimeout(() => window.close(), 2000);
          }
        },
        error: (err) => {
          console.error('Finalize error (path params)', err);
          this.loadData();
          if (window.opener) {
            setTimeout(() => window.close(), 2000);
          }
        }
      });
    } else {
      // Check Angular route queryParams (hash routing)
      this.route.queryParams.subscribe(params => {
        if (params['merchant_order_id'] && params['success']) {
          const orderId = params['merchant_order_id'] as string;
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
              this.router.navigate([], { queryParams: {} });
              this.loadData();
              if (window.opener) {
                setTimeout(() => window.close(), 2000);
              }
            },
            error: (err) => {
              console.error('Finalize error (route params)', err);
              this.loadData();
              if (window.opener) {
                setTimeout(() => window.close(), 2000);
              }
            }
          });
        } else {
          // Check localStorage for a pending Paymob top-up (MPGS 3DS redirect has NO query params)
          const pending = localStorage.getItem('pending_paymob_topup');
          if (pending) {
            const { localTransactionId, paymobOrderId } = JSON.parse(pending);
            localStorage.removeItem('pending_paymob_topup');
            // Load existing data immediately so screen isn't blank
            this.loadData();
            
            this.walletService.verifyPayment(localTransactionId, paymobOrderId).subscribe({
              next: (res) => {
                if (res.success) {
                  this.toastr.success('Payment completed successfully!');
                } else {
                  this.toastr.warning('Payment could not be confirmed. Please check your transaction history.');
                }
                // Silently refresh to show the updated transaction status
                this.refreshTransactions();
                if (window.opener) {
                  setTimeout(() => window.close(), 2000);
                }
              },
              error: (err) => {
                console.error('Verify payment error', err);
                this.refreshTransactions();
                if (window.opener) {
                  setTimeout(() => window.close(), 2000);
                }
              }
            });
          } else {
            this.loadData();
          }
        }
      });
    }

    // Poll transactions every 10 seconds
    this.pollingInterval = setInterval(() => {
      this.refreshTransactions();
    }, 10000);
  }

  ngAfterViewInit(): void {
    // Event listeners removed due to unreliability across language changes
  }

  focusInput(inputId: string): void {
    setTimeout(() => {
      document.getElementById(inputId)?.focus();
    }, 500);
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
      next: (t) => {
        this.transactions.set(t);
        // Adjust current page if it's out of bounds after refresh
        if (this.currentPage() > this.totalPages()) {
          this.currentPage.set(Math.max(1, this.totalPages()));
        }
      }
    });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
    }
  }

  initiateTopUp(): void {
    const amount = this.topUpAmount();
    if (!amount || amount < 10) {
      this.toastr.warning('Please enter an amount of at least 10 EGP.');
      return;
    }

    this.isToppingUp.set(true);
    const methodId = this.selectedPaymentMethodId();
    this.walletService.topUp(amount, methodId !== null ? methodId : undefined, PaymentProvider.Paymob).subscribe({
      next: (res) => {
        // Redirect to Paymob payment page or append query string for instant pay
        if (res.redirectUrl.startsWith('?')) {
            const urlParams = new URLSearchParams(res.redirectUrl);
            const orderId = urlParams.get('merchant_order_id');
            const success = urlParams.get('success');
            this.router.navigate([], { queryParams: { merchant_order_id: orderId, success: success } });
        } else {
            // Store payment info BEFORE redirect so we can verify on return
            // Paymob MPGS (3DS) flow does NOT append params to redirect URL
            localStorage.setItem('pending_paymob_topup', JSON.stringify({
              localTransactionId: res.transactionId,
              paymobOrderId: res.providerReference
            }));
            window.location.href = res.redirectUrl;
        }
      },
      error: (err) => {
        const msg = err?.error?.errorMessage || err?.error?.title || 'Failed to initiate top-up';
        this.toastr.error(msg);
        this.isToppingUp.set(false);
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
        this.toastr.success('Withdrawal request submitted successfully!');
        this.withdrawAmount.set(null);
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
