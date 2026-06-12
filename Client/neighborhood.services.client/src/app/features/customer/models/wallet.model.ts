export interface Wallet {
  id: number;
  userId: string;
  balance: number;
  createdAt: string;
  updatedAt: string;
}

export enum TransactionType {
  TopUp = 0,
  Transfer = 1,
  Withdrawal = 2,
  BookingPayment = 3,
  Refund = 4,
  Reversal = 5
}

export enum TransactionStatus {
  Pending = 0,
  Completed = 1,
  Failed = 2,
  Reversed = 3
}

export interface Transaction {
  id: number;
  fromWalletId?: number;
  toWalletId?: number;
  paymentMethodId?: number;
  amount: number;
  fee: number;
  currency: string;
  type: TransactionType;
  status: TransactionStatus;
  createdAt: string;
}

export enum PaymentType {
  VodafoneCash = 0,
  CreditCard = 1,
  InstaPay = 2
}

export enum PaymentProvider {
  Paymob = 0,
  Fawry = 1
}

export interface PaymentMethod {
  id: number;
  userId: string;
  paymentType: PaymentType;
  paymentProvider: PaymentProvider;
  lastFourDigits?: string;
  expiryMonth?: number;
  expiryYear?: number;
  createdAt: string;
}

export interface TopUpResponse {
  transactionId: number;
  redirectUrl: string;
  providerReference: string;
}
