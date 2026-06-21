import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { CreateSupportTicket } from '../models/create-support-ticket.model';
import { SupportTicket } from '../../staff/models/support-ticket.model';
import { SupportTicketDetails } from '../../staff/models/support-ticket-details.model';

/**
 * Customer/guest-facing slice of the support ticket API — only the endpoints
 * a non-staff caller is allowed to hit (no [HasPermission] guard):
 * create a ticket, and read back one ticket's own details + thread.
 */
@Injectable({ providedIn: 'root' })
export class CustomerSupportTicketService {
  private readonly baseUrl = `${environment.apiUrl}/api/SupportTickets`;

  constructor(private http: HttpClient) {}

  /** POST /api/supporttickets — open a new ticket (guest or logged-in customer). */
  createTicket(body: CreateSupportTicket): Observable<SupportTicket> {
    return this.http.post<SupportTicket>(this.baseUrl, body);
  }

  /** GET /api/supporttickets/{id}/details — the ticket + its full message thread. */
  getTicketDetails(id: number): Observable<SupportTicketDetails> {
    return this.http.get<SupportTicketDetails>(`${this.baseUrl}/${id}/details`);
  }
}