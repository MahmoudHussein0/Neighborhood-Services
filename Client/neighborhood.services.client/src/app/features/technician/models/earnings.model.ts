export enum InvoiceStatus {
  Unpaid = 0,
  Paid = 1,
  Refunded = 2,
  Voided = 3
}

export interface Invoice {
  id: number;
  bookingId: number;
  transactionId?: number;
  customerId: number;
  technicianId: number;
  amount: number;
  tax: number;
  totalAmount: number;
  status: InvoiceStatus;
  issuedAt: string;
  paidAt?: string;
  voidedAt?: string;
}
