import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Wallet, Transaction, PaymentMethod, TopUpResponse, PaymentProvider, PaymentType } from '../models/wallet.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerWalletService {

  constructor(private api: ApiService) {}

  getMyWallet(): Observable<Wallet> {
    return this.api.get<Wallet>('/wallets/me');
  }

  topUp(amount: number, paymentMethodId: number | undefined, provider: PaymentProvider): Observable<{transactionId: number, redirectUrl: string, providerReference: string}> {
    return this.api.post<{transactionId: number, redirectUrl: string, providerReference: string}>('/wallets/me/topup', { amount, paymentMethodId, provider });
  }

  getMyTransactions(): Observable<Transaction[]> {
    return this.api.get<Transaction[]>('/transactions/me');
  }

  getMyPaymentMethods(): Observable<PaymentMethod[]> {
    return this.api.get<PaymentMethod[]>('/paymentmethods/me');
  }

  addPaymentMethod(paymentType: PaymentType, paymentProvider: PaymentProvider, providerToken: string, lastFourDigits?: string, expiryMonth?: number, expiryYear?: number): Observable<PaymentMethod> {
    return this.api.post<PaymentMethod>('/paymentmethods', {
      paymentType,
      paymentProvider,
      providerToken,
      lastFourDigits,
      expiryMonth,
      expiryYear
    });
  }

  deletePaymentMethod(id: number): Observable<void> {
    return this.api.delete<void>(`/paymentmethods/${id}`);
  }

  withdraw(amount: number): Observable<void> {
    return this.api.post<void>('/wallets/me/withdraw', { amount });
  }

  finalizeTransaction(merchantOrderId: string, success: boolean, token?: string, maskedPan?: string): Observable<{message: string, status: string}> {
    let url = `/wallets/me/transactions/finalize?merchant_order_id=${merchantOrderId}&success=${success}`;
    if (token && maskedPan) {
      url += `&token=${token}&masked_pan=${maskedPan}`;
    }
    return this.api.get<{message: string, status: string}>(url);
  }

  verifyPayment(localTransactionId: number, paymobOrderId: string): Observable<{message: string, status: string, success: boolean}> {
    return this.api.post<{message: string, status: string, success: boolean}>('/wallets/me/transactions/verify-payment', {
      localTransactionId,
      paymobOrderId
    });
  }
}
