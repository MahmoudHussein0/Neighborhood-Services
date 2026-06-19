import { Injectable,signal,computed,NgZone,inject, Signal ,} from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import * as signalR from '@microsoft/signalr';
import {TicketDto} from '../../public/models/ticket-dto';
import {TicketMessageDto} from '../../public/models/ticket-message-dto';
//import { v4 as uuidv4 } from 'uuid';


import { BehaviorSubject, Observable, Subscription } from 'rxjs';


@Injectable({
  providedIn: 'root',
})
export class CustomerSupportService {
  private ngZone = inject(NgZone);
  private hubConnection: signalR.HubConnection;

    public receivedTickets = signal<TicketDto[]>([]);
  public TickDtos :TicketDto[]=[]
  public MssgDtos :TicketMessageDto[]=[]

  public incomingTickets$ = new BehaviorSubject<TicketDto[]>([]);
  public incomingMessages$ = new BehaviorSubject<TicketMessageDto[]>([]);

  

/**
 *
 */
userEmail:string="";
constructor() {
this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7228/customerServiceHub')
      .build();

 this.hubConnection.start().catch(err => console.error('SignalR error: ', err));
       this.hubConnection.on('ReceiveTicket', (ticket: TicketDto) => {
        this.hubConnection.invoke('JoinGroup',ticket.senderEmail)
       
         this.ngZone.run(() => {
      this.TickDtos.push({...ticket,localId:`ticket-${Date.now()}-${Math.random().toString(36).slice(2)}`})
      const updated = [ticket, ...this.incomingTickets$.value]
     console.log("Updated list of tickets: ", updated)
      console.log("Updated list of tickets: ", updated);
      this.incomingTickets$.next(updated);

      console.log("from receive ticket");
      console.log(ticket);
 // generate unique id on arrival});
    });

    
   
    
    });

     this.hubConnection.on('TicketChat', (message: any) => {
      this.ngZone.run(() => {
         console.log("from receive chat");
      console.log(message);
      this.MssgDtos.push({...message,localId:`mssg-${Date.now()}-${Math.random().toString(36).slice(2)}`})
      const updated = [message, ...this.incomingMessages$.value]
     console.log("Updated list of Messsges: ", updated)
      this.incomingMessages$.next(updated);

      console.log("from receive message");
      console.log(message);
 // generate unique id on arrival});
    });
     
    
    });
  
}

sendTicket(ticket:TicketDto):void{
  this.hubConnection.invoke("SendLiveTicket",ticket.senderEmail,ticket)
}

sendMessage(email:string,mssg:string,role:string):void{
  this.hubConnection.invoke("SendGroupMessage",email,mssg,role);
}

//when user enters his dispute, a notification is sent to admins, a new conversation of disputes chat component is created

}
