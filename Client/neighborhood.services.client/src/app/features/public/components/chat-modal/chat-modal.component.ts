import { Component,Input,OnInit,NgZone,inject, signal, Signal } from '@angular/core';
import { FormsModule } from '@angular/forms'
import {CustomerSupportService} from '../../services/customer-support.service'
import { TicketMessageDto } from '../../models/ticket-message-dto';
import { TicketDto } from '../../models/ticket-dto';



@Component({
  selector: 'app-chat-modal',
  imports: [FormsModule],
  templateUrl: './chat-modal.component.html',
  styleUrl: './chat-modal.component.css',
  providers: [],
})
export class ChatModalComponent {
/**
 *
 */
 @Input() senderEmail!: string;
  private ngZone = inject(NgZone);
  isLoding=signal(false);
messages:string[]=[]
message:string="hello";
public mssgs: TicketMessageDto[]=[]
public mssgSignals=signal<TicketMessageDto[]>([])
messageToSend:string=" ";

constructor(
	
    private readonly sb:CustomerSupportService) {
      console.log(this.senderEmail)
		// customize default values of modals used by this component tree
	
    sb.incomingMessages$.subscribe(messages=>{
  this.mssgs=messages.filter(e=>e.email==this.senderEmail)
  console.log("Messages from Modal constructor");
  console.log(this.mssgs)
});
      
	} //end of constructor
ngOnInit(){
   this.sb.incomingMessages$.subscribe(messages=>{
     this.ngZone.run(() => {
      this.isLoding.set(true);
  this.mssgs=messages.filter(e=>e.email==this.senderEmail)
  this.mssgSignals.set(this.mssgs);

  console.log("Messages from Modal Init");
  console.log(this.senderEmail)
    this.isLoding.set(false);

  console.log(this.mssgs)})
   })
}


  sendMessage(){
    this.sb.sendMessage(this.senderEmail,this.messageToSend,"a");
  }

  

}
