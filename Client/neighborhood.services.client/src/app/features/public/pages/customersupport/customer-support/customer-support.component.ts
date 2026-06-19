import { Component,Input,OnInit,NgZone,inject,signal } from '@angular/core';
import { TicketDto } from '../../../models/ticket-dto';
import {CustomerSupportService} from '../../../services/customer-support.service'
import { FormsModule } from '@angular/forms'
import { TicketMessageDto } from '../../../models/ticket-message-dto';


@Component({
  selector: 'app-customer-support',
  imports: [FormsModule],
  templateUrl: './customer-support.component.html',
  styleUrl: './customer-support.component.css',
})
 

export class CustomerSupportComponent {
    @Input({ required: true })
  ticket!: TicketDto;
  messageToSend:string="";
  private ngZone = inject(NgZone);
  isLoding=signal(false);
// messages:string[]=[]
// message:string="hello";
public mssgs: TicketMessageDto[]=[]
public mssgSignals=signal<TicketMessageDto[]>([])

 
  constructor(private myService:CustomerSupportService) {
     myService.incomingMessages$.subscribe(messages=>{
  this.mssgs=messages;
  console.log("Messages from Modal constructor");
  console.log(this.mssgs)
});
   
  }
  ngOnInit(){
    setTimeout(() => {
  this.myService.sendTicket(this.ticket)
   this.myService.incomingMessages$.subscribe(messages=>{
     this.ngZone.run(() => {
      this.isLoding.set(true);
  this.mssgs=messages;
  this.mssgSignals.set(this.mssgs);

  console.log("Messages from Modal Init");
 
    this.isLoding.set(false);

  console.log(this.mssgs)})
   })
}, 5000);
  }

  sendMessage(){
    this.myService.sendMessage(this.ticket.senderEmail,this.messageToSend,"g");
  }
}
 