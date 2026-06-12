import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { PromoCode } from '../models/promo-code.model';

@Injectable({
  providedIn: 'root'
})
export class StaffPromoCodeService {

  constructor(private api: ApiService) {}

  getAll(): Observable<PromoCode[]> {
    return this.api.get<PromoCode[]>('/promocodes');
  }

  create(code: string, discountPercentage: number, maxUses: number, expiresAt: string): Observable<PromoCode> {
    return this.api.post<PromoCode>('/promocodes', { code, discountPercentage, maxUses, expiresAt });
  }

  delete(id: number): Observable<void> {
    return this.api.delete<void>(`/promocodes/${id}`);
  }
}
