import { Injectable,signal,computed,NgZone,inject } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import * as signalR from '@microsoft/signalr';
import { MessageDto } from '../../../core/models/message-dto';
import { MessageSelectedDto } from '../../../core/models/message-selected-dto';
import { MessagesService } from '../../customer/services/messages.service';

import { from, Observable } from 'rxjs';


@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private ngZone = inject(NgZone);


  // public myId:string=this.GetCurrentUserId().toString();
  public myId:string='1234';
  private Endpoint = '/api/Message';
  private hubConnection: signalR.HubConnection;
  public messages = signal<MessageDto[]>([]);
  public currentMessages : MessageDto[]=[];
  public AllMessagesForBooking = signal<any[]>([]);
  // public MyMessages = computed(() => this.AllMessagesForBooking.filter(m => m.senderId === this.myId));
  // public OtherMessages = computed(() => this.AllMessagesForBooking.filter(m => m.senderId !== this.myId));
  


constructor(private apiService: ApiService,private messagesService: MessagesService) { 
  this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7228/chatHub')
     
      .build();

    this.hubConnection.on('ReceiveMessage', (message: any) => {
      console.log("from receive");
     //  this.AllMessagesForBooking().push(message);
       console.log(this.AllMessagesForBooking());
       console.log(this.AllMessagesForBooking());
       console.log('Received message:', message);
    
    });
}

// public initializeChat(bookingId:number){
//     this.hubConnection.start().then(() => {
//       console.log("Chat service initialized and SignalR connection started.");
//       //أول ما الكونكشن يبدأ يعمل لود للرسايل كلها من الداتا بيز
//       this.messagesService.getMessagesForBooking(bookingId).subscribe(
//       {
// next: (messages) => {
//         console.log('Messages loaded for booking:', messages);
//         // this.AllMessagesForBooking = messages.sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime());
//         //  this.MyMessages = computed(() => this.AllMessagesForBooking.filter(m => m.senderId === this.myId));
//         //  this.OtherMessages = computed(() => this.AllMessagesForBooking.filter(m => m.senderId !== this.myId));
//         this.AllMessagesForBooking.set(
//         messages.sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime()));
       

//       },
//       error: (error) => {
//         console.error('Error loading messages for booking:', error);
       
//       }
//     }
//     );
//     }).catch(err => console.error(err));
// }


 initializeChat(bookingId: number) {
        this.hubConnection.start().then(() => {
      //     this.hubConnection.invoke('JoinGroup', bookingId.toString())
      // .catch(err => console.error(err));

            console.log("initializaing hub")
            this.hubConnection.on('ReceiveMessage', (message: MessageDto) => {
                this.ngZone.run(() => {
                    this.AllMessagesForBooking.update(msgs => [...msgs, message]);
                });
            });

            this.messagesService.getMessagesForBooking(bookingId).subscribe({
                next: (messages) => {
                  console.log(messages);
                    this.ngZone.run(() => {
                        this.AllMessagesForBooking.set(
                            messages.sort((a, b) =>
                                new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime()
                            )
                        );
                    });
                },
                error: (err) => console.error(err)
            });

        }).catch(err => console.error(err));
    }

public sendMessage(message: MessageDto, userId?: string, groupName?: string) {
    if (userId) {
      this.hubConnection.invoke('SendPrivateMessage', userId, message)
        .catch(err => console.error(err));
    } else if (groupName) {
      this.hubConnection.invoke('SendGroupMessage', groupName, message)
        .catch(err => console.error(err));
    } else {
      this.hubConnection.invoke('SendBroadcastMessage', message)
        .catch(err => console.error(err));
    }
  }

  public sendMessageUponBooking(groupName: string,message: MessageDto ) {
   
   
      this.hubConnection.invoke('SendGroupMessage', groupName, message)
        .catch(err => console.error(err));
    
  }

  public sendMessageForAll(message: MessageDto ):Observable<void> {
    console.log("from brodcast invoke");
      return from( this.hubConnection.invoke('SendBroadcastMessageDto',message));
       // .catch(err => console.error(err));
        
    
  }

  //For a user to join a group chat based on booking ID
  public joinGroupChat(groupName: string) {
    this.hubConnection.invoke('JoinGroup', groupName)
      .catch(err => console.error(err));
  }

  // GetCurrentUserId(): Observable<any>{
  //     return this.apiService.get(this.Endpoint + '/GetCurrentUserId');
  //   }

}
