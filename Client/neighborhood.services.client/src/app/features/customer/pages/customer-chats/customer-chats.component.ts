import {ConversationDto} from '../../models/conversation-dto'
import { Component ,signal,OnInit} from '@angular/core';
import {ConversationService} from '../../services/conversation.service'
import { NgClass,CommonModule } from '@angular/common';
import { MessagesService } from '../../services/messages.service';
import { ChatService } from '../../services/chat.service';
import { computed, NgModule,  inject, Signal, NgZone } from '@angular/core';
import { MessageDto } from '../../../../core/models/message-dto';
import { FormsModule } from '@angular/forms';







@Component({
  selector: 'app-customer-chats',
  imports: [NgClass,CommonModule,FormsModule],
  templateUrl: './customer-chats.component.html',
  styleUrl: './customer-chats.component.css',
})
export class CustomerChatsComponent {

  ConvSignals = signal<ConversationDto[]>([]);
  isLoading = signal(false);
  public ConvDtos: ConversationDto[] = [];
   public myId: string = "";

private ngZone = inject(NgZone);
 
  // private router = inject(Router);
  public messageContent = '';
  //data holders
  public bookingId: number = 0; // For group chat


  //all messages received and sent
  public ReceivedMessages = computed(() => this.chatService.messages());
  // public AllMessagesForBooking: MessageSelectedDto[] = [];
  public AllMessagesForBooking = signal<any[]>([]);


  public allmssgs = computed(() => this.AllMessagesForBooking);
  // public MyMessages = computed(() => this.AllMessagesForBooking.filter(m => m.senderId === this.myId));
  // public OtherMessages = computed(() => this.AllMessagesForBooking.filter(m => m.senderId !== this.myId));
  //
 

  public MessageToBeSent: MessageDto = {
    "senderId": '',
    "content": '',
    "bookingId": this.bookingId
  }


  constructor(private conversationService: ConversationService,
    private chatService: ChatService, private messagesService: MessagesService) {
      this.isLoading = signal(false);
    this.AllMessagesForBooking = this.chatService.AllMessagesForBooking;
    }

  ngOnInit(): void {
    this.loadConversations();
        this.chatService.initializeChat(1)
        this.getMyId();
    console.log("from init", this.chatService.AllMessagesForBooking());
    this.loadConversationMessages(1);

  }

  loadConversations(): void {
    this.isLoading.set(true);

    this.conversationService.getMyConversations().subscribe({
      next: data => {
       
        this.ConvSignals.set(data);
        this.ConvDtos = [...data];
        console.log('Conversations loaded:', data);
        this.isLoading.set(false);
        
      },
      error: err => {
        console.error(err);
        this.isLoading.set(false);
      }
    });
  }

  loadConversationMessages(bookingId:number){

  }

getMyId(): void {
    this.messagesService.GetCurrentUserId().subscribe(
      {
        next: (userId) => {
          this.myId = userId;
          console.log('Current user ID:', this.myId);
        },
        error: (error) => {
          console.error('Error fetching current user ID:', error);
        }
      }
    );
  }

  public sendMessage(message: MessageDto): void {
    this.isLoading.set(false);
    console.log('Attempting to send message:', message);
    //هنا بنجربها عالهاب بس

    // if (message.content.trim()) {
    //     this.chatService.sendMessageForAll(message).subscribe({
    //         next: () => {
    //             this.ngZone.run(() => {  // ✅ back inside Angular's zone
    //                 console.log("message sent!");
    //                 this.messageContent = '';
    //                 this.MessageToBeSent = new MessageDto();
    //                 this.isLoading.set(true);
    //             });
    //         },
    //         error: (err) => {
    //             this.ngZone.run(() => {
    //                 console.error("send failed:", err);
    //                 this.isLoading.set(false);
    //             });
    //         }
    //     });
    // }

    //هنا بنجربها بالمسدج سيرفس

    if (message.content.trim()) {
      this.MessageToBeSent.bookingId=this.bookingId;
      this.MessageToBeSent.senderId=this.myId;
      this.messagesService.CreateMessagesOnBooking(this.MessageToBeSent).subscribe({
        next: () => {
          this.ngZone.run(() => {  
            console.log("message sent!");
            this.ngZone.run(() => {
                    this.AllMessagesForBooking.update(msgs => [...msgs, this.MessageToBeSent]);
                });
            //this.AllMessagesForBooking().push(this.MessageToBeSent);
             this.isLoading.set(true);
             
            this.messageContent = '';
            this.MessageToBeSent = new MessageDto();
            this.isLoading.set(true);
          });
        },
        error: (err) => {
          this.ngZone.run(() => {
            console.error("send failed:", err);
            this.isLoading.set(false);
          });
        }
      });
    }}

}


