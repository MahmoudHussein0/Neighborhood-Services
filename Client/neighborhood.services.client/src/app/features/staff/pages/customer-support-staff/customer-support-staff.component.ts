import { Component,signal,computed ,Signal} from '@angular/core';
import { TicketDto } from '../../../public/models/ticket-dto';
import { CustomerSupportService } from '../../../public/services/customer-support.service';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import {ChatModalComponent} from '../../../public/components/chat-modal/chat-modal.component'
import {TicketMessageDto} from'../../../public/models/ticket-message-dto'


@Component({
  selector: 'app-customer-support-staff',
  imports: [CommonModule,ChatModalComponent],
  templateUrl: './customer-support-staff.component.html',
  styleUrl: './customer-support-staff.component.css',
})
export class CustomerSupportStaffComponent {
  /**
   *
   */
  public MessagesForConv: Record<string, TicketMessageDto[]>={"":[]}
  public MessagesSignalForConfv!: Signal<Record<string, TicketMessageDto[]>>
  
    public allTickets!:Signal<TicketDto[]>;
    public allMessages!:Signal<TicketMessageDto[]>;
    public ticks: TicketDto[]=[]
    public mssgs: TicketMessageDto[]=[]
  constructor(private readonly sb:CustomerSupportService) {
//this.allTickets = sb.receivedTickets;
//this.ticks = this.allTickets();
sb.incomingTickets$.subscribe(tickets => {
  this.ticks = tickets;
  console.log("Tickets from constructor")
  console.log(this.ticks)
});
sb.incomingMessages$.subscribe(messages=>{
  this.mssgs=messages;
  console.log("Messages from constructor");
  console.log(this.mssgs)
});
  }//end of constructor
  openChatEmail: string | null = null;

  toggleChat(tickeEmail: string): void {
    // if same ticket clicked again, close it — otherwise open new one
    this.openChatEmail = this.openChatEmail === tickeEmail ? null : tickeEmail;
  }




}
