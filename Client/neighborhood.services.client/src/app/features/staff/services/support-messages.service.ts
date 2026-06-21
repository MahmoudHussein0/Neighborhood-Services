import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateSupportMessage, SupportMessage } from '../models/support-message.model';

@Injectable({
  providedIn: 'root'
})
export class SupportMessagesService {

  private readonly baseUrl =
    `${environment.apiUrl}/api/supporttickets`;

  constructor(
    private http: HttpClient
  ) {}

  getMessages(ticketId: number): Observable<SupportMessage[]> {
    return this.http.get<SupportMessage[]>(
      `${this.baseUrl}/${ticketId}/messages`
    );
  }

  sendMessage(
    ticketId: number,
    payload: CreateSupportMessage
  ): Observable<SupportMessage> {
    return this.http.post<SupportMessage>(
      `${this.baseUrl}/${ticketId}/messages`,
      payload
    );
  }

  markAsRead(
    ticketId: number,
    messageId: number
  ): Observable<void> {
    return this.http.patch<void>(
      `${this.baseUrl}/${ticketId}/messages/${messageId}/read`,
      {}
    );
  }
}