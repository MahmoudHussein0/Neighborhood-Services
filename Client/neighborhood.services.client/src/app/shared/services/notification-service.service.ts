import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { NotificationMessage } from './../../core/models/notification-message';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Toast, ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class NotificationServiceService {
  private hubConnection!: signalR.HubConnection;

  private notificationsSubject = new BehaviorSubject<NotificationMessage[]>([]);
  notifications$ = this.notificationsSubject.asObservable();

  // Fires once per realtime push so pages can refresh their list on a new notification.
  private notificationReceivedSubject = new Subject<NotificationMessage>();
  notificationReceived$ = this.notificationReceivedSubject.asObservable();

  baseUrl = environment.apiUrl;
  constructor(private httpClient: HttpClient, private toastr: ToastrService) { }


  startConnection() {

    //Assuming tokens are stored in local storage under 'AccessToken'
    const token = localStorage.getItem('AccessToken'); 

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7228/notificationHub', {
        accessTokenFactory: () => token || '',
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(err => console.error('SignalR error: ', err));

    this.hubConnection.on('ReceiveNotification', (data: any) => {  
      console.log("Receving ..")
      const updated = [data, ...this.notificationsSubject.value];
       this.toastr.success(data.message, 'New Notification');
      console.log("Updated list: ", updated);
      this.notificationsSubject.next(updated);
      this.notificationReceivedSubject.next(data);
    }); //end of receive function
  }// end of start connection

  GetAllNotifications() {
    return this.httpClient.get<NotificationMessage[]>(`${this.baseUrl}/api/Notifications/GetAll`);
  }

  setNotifications(list: NotificationMessage[]) {
    this.notificationsSubject.next(list);
  }

  markAllAsRead() {
    return this.httpClient.put(`${this.baseUrl}Notification/MarkAllAsRead`, {});
  }

  markAsRead(id: number) {
    return this.httpClient.put(`${this.baseUrl}/api/Notifications/MarkAsRead/${id}`, {});
  }

  markAllLocalAsRead() {
    const updated = this.notificationsSubject.value.map((n) => ({ ...n, isRead: true }));
    this.notificationsSubject.next(updated);
  }

  updateNotificationAsRead(id: number) {
    const updated = this.notificationsSubject.value.map((n) =>
      n.id === id ? { ...n, isRead: true } : n
    );
    this.notificationsSubject.next(updated);
  }
  
}
