import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';

@Injectable({
  providedIn: 'root',
})
export class OfferService {
  private readonly api = inject(ApiService);

  /** POST /api/offers/{id}/accept — accept an offer; returns the new booking id */
  accept(offerId: number): Observable<{ bookingId: number }> {
    return this.api.post<{ bookingId: number }>(`/offers/${offerId}/accept`, {});
  }
}
