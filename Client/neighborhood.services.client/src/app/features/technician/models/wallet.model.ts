// Reusing the same models as customer for consistency, since the backend uses the same structure.
export type { 
  Wallet, 
  Transaction 
} from '../../customer/models/wallet.model';

export {
  TransactionType, 
  TransactionStatus 
} from '../../customer/models/wallet.model';
