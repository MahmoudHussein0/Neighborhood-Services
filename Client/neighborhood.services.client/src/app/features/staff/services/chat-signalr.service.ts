// chat-signalr.service.ts
import { Injectable, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ChatMessage {
  ticketId: string;
  senderId: string;
  message: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class ChatSignalRService implements OnDestroy {
  private hubConnection: signalR.HubConnection | null = null;
  
  // الـ components بتـ subscribe عليه
  message$ = new Subject<ChatMessage>();
  connectionState$ = new Subject<'connected' | 'disconnected' | 'reconnecting'>();

  startConnection(): Promise<void> {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/hubs/support-chat`)
      .withAutomaticReconnect()
      .build();

    // لما يوصل رسالة جديدة
    this.hubConnection.on('ReceiveMessage', (msg: ChatMessage) => {
      this.message$.next(msg);
    });

    this.hubConnection.onreconnecting(() => 
      this.connectionState$.next('reconnecting'));
    
    this.hubConnection.onreconnected(() => 
      this.connectionState$.next('connected'));
    
    this.hubConnection.onclose(() => 
      this.connectionState$.next('disconnected'));

    return this.hubConnection.start()
      .then(() => this.connectionState$.next('connected'));
  }

  joinTicket(ticketId: number): Promise<void> {
    return this.hubConnection!.invoke('JoinTicket', ticketId.toString());
  }

  leaveTicket(ticketId: number): Promise<void> {
    return this.hubConnection!.invoke('LeaveTicket', ticketId.toString());
  }

  sendMessage(ticketId: number, senderId: string, message: string): Promise<void> {
    return this.hubConnection!.invoke('SendMessage', 
      ticketId.toString(), senderId, message);
  }

  stopConnection(): Promise<void> {
    return this.hubConnection?.stop() ?? Promise.resolve();
  }

  ngOnDestroy() {
    this.stopConnection();
  }
}