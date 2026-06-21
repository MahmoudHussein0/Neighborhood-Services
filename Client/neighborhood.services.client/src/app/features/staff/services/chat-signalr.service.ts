import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Subject } from 'rxjs';

import { environment } from '../../../environments/environment';

export interface ChatMessage {
  id?: number;

  ticketId: number;

  senderId?: string;

  senderType: string;

  message?: string;

  channel: string;

  createdAt: string;

  attachments?: any[];
}

@Injectable({
  providedIn: 'root'
})
export class ChatSignalRService {

  private hubConnection?: signalR.HubConnection;

  private readonly hubUrl =
    `${environment.apiUrl}/hubs/SupportChatHub`;

  private messageSubject =
    new Subject<ChatMessage>();

  message$ =
    this.messageSubject.asObservable();

  private connectionStateSubject =
    new BehaviorSubject<
      'connected' |
      'disconnected' |
      'reconnecting'
    >('disconnected');

  connectionState$ =
    this.connectionStateSubject.asObservable();

  async startConnection() {

    if (
      this.hubConnection &&
      this.hubConnection.state !== signalR.HubConnectionState.Disconnected
    ) {
      return;
    }

    this.hubConnection =
      new signalR.HubConnectionBuilder()
        .withUrl(this.hubUrl, {
          withCredentials: true
        })
        .withAutomaticReconnect()
        .build();

    this.registerEvents();

    await this.hubConnection.start();

    this.connectionStateSubject.next(
      'connected'
    );
  }

  async stopConnection() {

    if (!this.hubConnection)
      return;

    await this.hubConnection.stop();

    this.connectionStateSubject.next(
      'disconnected'
    );
  }

  async joinTicket(ticketId: number) {

    if (!this.hubConnection)
      return;

    await this.hubConnection.invoke(
      'JoinTicket',
      ticketId.toString()
    );
  }

  async leaveTicket(ticketId: number) {

    if (!this.hubConnection)
      return;

    await this.hubConnection.invoke(
      'LeaveTicket',
      ticketId.toString()
    );
  }

  async sendMessage(
    ticketId: number,
    message: ChatMessage
  ) {

    if (!this.hubConnection)
      return;

    await this.hubConnection.invoke(
      'SendMessage',
      ticketId.toString(),
      message
    );
  }

  private registerEvents() {

    if (!this.hubConnection)
      return;

    this.hubConnection.on(
      'ReceiveMessage',
      (message: ChatMessage) => {

        this.messageSubject.next(
          message
        );
      }
    );

    this.hubConnection.onreconnecting(() => {

      this.connectionStateSubject.next(
        'reconnecting'
      );
    });

    this.hubConnection.onreconnected(() => {

      this.connectionStateSubject.next(
        'connected'
      );
    });

    this.hubConnection.onclose(() => {

      this.connectionStateSubject.next(
        'disconnected'
      );
    });
  }
}