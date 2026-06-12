import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Wallet, Transaction } from '../models/wallet.model';

@Injectable({
  providedIn: 'root'
})
export class TechnicianWalletService {

  constructor(private api: ApiService) {}

  getMyWallet(): Observable<Wallet> {
    return this.api.get<Wallet>('/wallets/me');
  }

  getMyTransactions(): Observable<Transaction[]> {
    return this.api.get<Transaction[]>('/transactions/me');
  }
}
