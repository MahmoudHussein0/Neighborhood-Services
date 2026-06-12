import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SupportMessagesService {

private readonly baseUrl = `${environment.apiUrl}/api/supporttickets`;
  constructor(private http: HttpClient) {}

  getMessages(ticketId: number) {
    return this.http.get(`${this.baseUrl}/${ticketId}/messages`);
  }

  sendMessage(ticketId: number, message: string) {
    return this.http.post(`${this.baseUrl}/${ticketId}/messages`, {
      message,
      senderId: null
    });
  }

  markAsRead(ticketId: number, messageId: number) {
    return this.http.patch(
      `${this.baseUrl}/${ticketId}/messages/${messageId}/read`,
      {}
    );
  }
}