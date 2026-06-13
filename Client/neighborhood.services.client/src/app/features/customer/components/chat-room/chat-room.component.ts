import { Component, computed, NgModule, signal, inject, Signal, NgZone } from '@angular/core';
import { MdbCollapseModule } from 'mdb-angular-ui-kit/collapse';
import { ChatService } from '../../services/chat.service';
import { MessageDto } from '../../../../core/models/message-dto';
import { NgModel } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { MessagesService } from '../../services/messages.service';
import { MessageSelectedDto } from '../../../../core/models/message-selected-dto';
import { NgClass, CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';



// import { MdbScrollbarModule } from 'mdb-angular-ui-kit/scrollbar';



@Component({
  selector: 'app-chat-room',
  imports: [MdbCollapseModule, FormsModule, CommonModule, NgClass],
  templateUrl: './chat-room.component.html',
  styleUrl: './chat-room.component.css',
})
export class ChatRoomComponent {
  private ngZone = inject(NgZone);
  private route = inject(ActivatedRoute);
  // private router = inject(Router);
  public messageContent = '';
  //data holders
  public bookingId: number = 0; // For group chat
  public myId: string = null!;

  //all messages received and sent
  public ReceivedMessages = computed(() => this.chatService.messages());
  // public AllMessagesForBooking: MessageSelectedDto[] = [];
  public AllMessagesForBooking = signal<any[]>([]);


  public allmssgs = computed(() => this.AllMessagesForBooking);
  // public MyMessages = computed(() => this.AllMessagesForBooking.filter(m => m.senderId === this.myId));
  // public OtherMessages = computed(() => this.AllMessagesForBooking.filter(m => m.senderId !== this.myId));
  //
  isLoading = signal(false);

  public MessageToBeSent: MessageDto = {
    "senderId": '',
    "content": '',
    "bookingId": this.bookingId
  }


  constructor(private chatService: ChatService, private messagesService: MessagesService) {
    this.isLoading = signal(false);
    this.AllMessagesForBooking = this.chatService.AllMessagesForBooking;

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

  ngOnInit(): void {
    this.AllMessagesForBooking = this.chatService.AllMessagesForBooking;

    // this.chatService.initializeChat(this.bookingId);

    const bookingId = +this.route.snapshot.paramMap.get('bookingId')!;
    console.log("initiazled");

    this.chatService.initializeChat(bookingId);
    this.route.params.subscribe((params) => {
      const bookingId = +params['bookingId'];
      if (bookingId) {
        this.chatService.initializeChat(bookingId);
      }
    });
    this.getMyId();
    console.log("from init", this.chatService.AllMessagesForBooking);
    // this.loadMessagesForBooking(1);
    // console.log(this.MyMessages);
    // console.log(this.OtherMessages)
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
    }
  }


  // public loadMessagesForBooking(bookingId: number): void {
  //   this.isLoading.set(true);
  //   this.messagesService.getMessagesForBooking(bookingId).subscribe(
  //     {
  //       next: (messages) => {
  //         console.log('Messages loaded for booking:', messages);
  //         this.AllMessagesForBooking = messages.sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime());
  //         //  this.MyMessages = computed(() => this.AllMessagesForBooking.filter(m => m.senderId === this.myId));
  //         //  this.OtherMessages = computed(() => this.AllMessagesForBooking.filter(m => m.senderId !== this.myId));
  //         this.isLoading.set(false);

  //       },
  //       error: (error) => {
  //         console.error('Error loading messages for booking:', error);
  //         this.isLoading.set(false);
  //       }
  //     }
  //   );

  // }


}