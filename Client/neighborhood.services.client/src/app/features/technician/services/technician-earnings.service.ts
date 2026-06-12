import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { Invoice } from '../models/earnings.model';

@Injectable({
  providedIn: 'root'
})
export class TechnicianEarningsService {

  constructor(private api: ApiService) {}

  getMyInvoices(technicianId: number): Observable<Invoice[]> {
    return this.api.get<Invoice[]>(`/invoices/technician/${technicianId}`);
  }
}
