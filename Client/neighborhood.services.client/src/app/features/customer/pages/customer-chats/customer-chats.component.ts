import {ConversationDto} from '../../models/conversation-dto'
import { Component ,signal,OnInit} from '@angular/core';
import {ConversationService} from '../../services/conversation.service'
import { NgClass,CommonModule } from '@angular/common';
import { MessagesService } from '../../services/messages.service';
import { ChatService } from '../../services/chat.service';
import { computed, NgModule,  inject, Signal, NgZone } from '@angular/core';
import { MessageDto } from '../../../../core/models/message-dto';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
import { AfterViewInit,ViewChild, ViewChildren, ElementRef,   QueryList, HostListener
 } from '@angular/core';
 import { UploadService } from '../../../../shared/services/upload.service';



@Component({
  selector: 'app-customer-chats',
  imports: [NgClass,CommonModule,FormsModule,DatePipe],
  templateUrl: './customer-chats.component.html',
  styleUrl: './customer-chats.component.css',
})
export class CustomerChatsComponent implements AfterViewInit{

  @ViewChild('scrollframe', {static: false}) scrollFrame!: ElementRef;
  @ViewChildren('item') itemElements!: QueryList<any>;

  private itemContainer: any;
  private scrollContainer: any;
  //public items :string[]=[];
  private isNearBottom = true;

  ngAfterViewInit() {
    this.isLoading.set(true)
    this.scrollContainer = this.scrollFrame.nativeElement;
    this.itemElements.changes.subscribe(_ => this.onItemElementsChanged());    

    // Add a new item every 2 seconds for demo purposes
    // setInterval(() => {
      
    //   this.items.push("hi");
    //    this.isLoading.set(false);
     
    // }, 2000);
      this.isLoading.set(false);
  }

  private onItemElementsChanged(): void {
    if (this.isNearBottom) {
      this.scrollToBottom();
    }
  }

  private scrollToBottom(): void {
    this.scrollContainer.scroll({
      top: this.scrollContainer.scrollHeight,
      left: 0,
      behavior: 'smooth'
    });
  }
 private isUserNearBottom(): boolean {
    const threshold = 30;
    const position = this.scrollContainer.scrollTop + this.scrollContainer.offsetHeight;
    const height = this.scrollContainer.scrollHeight;
    return position > height - threshold;
  }

  scrolled(event: any): void {
    this.isNearBottom = this.isUserNearBottom();
  }




  ////////////////
  ConvSignals = signal<ConversationDto[]>([]);
  chosenBookingId = signal<number>(0);

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
    public CurrentConversationMessages = signal<any[]>([]);



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
    private chatService: ChatService, private messagesService: MessagesService,private cdr: ChangeDetectorRef,
  private uploadService:UploadService) {
      this.isLoading = signal(false);
    this.AllMessagesForBooking = this.chatService.AllMessagesForBooking;
    this.CurrentConversationMessages=this.AllMessagesForBooking;
    /////
   
    }

  ngOnInit(): void {
    console.log("initiating chats..");
    this.loadConversations();
       // this.chatService.initializeChat(this.bookingId)
        this.getMyId();
    console.log("from init", this.chatService.AllMessagesForBooking());
    //this.loadConversationMessages(this.bookingId);

  }

  loadConversations(): void {
    this.isLoading.set(true);

    this.conversationService.getMyConversations().subscribe({
      next: data => {
       
        this.ConvSignals.set(data);
        this.ConvDtos = [...data];
        console.log('Conversations loaded:', data);
        if(data.at(0)!=null){this.bookingId=data.at(0)?.bookingId!}
        console.log('BookingId:', this.bookingId);
        //  this.chatService.initializeChat(this.bookingId)
        this.getMyId();
    // console.log("from init", this.chatService.AllMessagesForBooking());
    //this.loadConversationMessages(this.bookingId);


        this.isLoading.set(false);
        
      },
      error: err => {
        console.error(err);
        this.isLoading.set(false);
      }
    });
  }

  // loadConversationMessages(bookingId:number){

  // }
  reload(id:number) :void{

this.bookingId=id;
console.log(this.bookingId);
this.chatService.deinitializeChat().subscribe({
      next:()=>{ console.log("deinitialized");this.AllMessagesForBooking=signal<any[]>([]);
        this.chatService.AllMessagesForBooking=signal<any[]>([]); this.chatService.initializeChat(this.bookingId); 
        this.AllMessagesForBooking.set(this.chatService.AllMessagesForBooking())
}
    })
this.chosenBookingId.set(this.bookingId);
 this.isLoading.set(true);
 this.messagesService.getMessagesForBooking(id).subscribe({
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

    // this.conversationService.getMyConversations().subscribe({
    //   next: data => {
       
    //     this.ConvSignals.set(data);
    //     this.ConvDtos = [...data];
    //     console.log('Conversations loaded:', data);
        
    //     console.log('BookingId:', this.chosenBookingId());
      
    //      this.isLoading.set(false);
    // console.log("from init", this.chatService.AllMessagesForBooking());
    // //this.loadConversationMessages(this.chosenBookingId());


    //     this.isLoading.set(false);
        
    //   },
    //   error: err => {
    //     console.error(err);
    //     this.isLoading.set(false);
    //   }
    // });
//     this.chatService.deinitializeChat().subscribe({
//       next:()=>{ console.log("deinitialized");this.AllMessagesForBooking=signal<any[]>([]);this.chatService.initializeChat(this.bookingId); 
// }
//     })
        //  this.chatService.initializeChat(this.bookingId)


this.isLoading.set(!this.isLoading())

  }

getMyId(): void {
     this.messagesService.getCurrentUserId1().subscribe({
      next: (raw) => {console.log('Raw response:', raw); this.myId=raw; console.log(this.myId);},
      error: (err) => console.error('Error:', err)
    });
  }

     selectedFile = signal<File | null>(null);


  onFileSelected(event: Event) {
   const input = event.target as HTMLInputElement;
   if (input.files && input.files.length > 0) {
     this.selectedFile.set(input.files[0]);
    this.MessageToBeSent.hasImage=true;
   }
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

    if (message.content||this.selectedFile()) {
      this.MessageToBeSent.bookingId=this.bookingId;
      this.MessageToBeSent.senderId=this.myId;
       if(this.selectedFile()!=null){
            this.MessageToBeSent.hasImage=true;
            if(message.content?.trim().length==0){message.content="image"}
        
            this.uploadService.uploadArwaEdit(this.selectedFile()).subscribe({
             next: (res) => {
             this.MessageToBeSent.imageUrl=res;
             console.log(res)
              console.log(res)
              this.messagesService.CreateMessagesOnBooking(this.MessageToBeSent).subscribe({
        next: (res) => {
          this.ngZone.run(() => {  
            console.log("message sent!");
            console.log(res);
         
          this.AllMessagesForBooking=this.chatService.AllMessagesForBooking;
          this.AllMessagesForBooking=this.chatService.AllMessagesForBooking;
           console.log("after send");
           console.log(this.AllMessagesForBooking())
          this.MessageToBeSent.content = '';
          this.selectedFile.set(null);

           this.cdr.detectChanges();
        
            // this.ngZone.run(() => {
           
              
            //         //this.AllMessagesForBooking.update(msgs => [...msgs, this.MessageToBeSent]);
            //     });
            //this.AllMessagesForBooking().push(this.MessageToBeSent);
             this.isLoading.set(false);
             
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
      }); //end of subs
               },
             error: (err) => {
             console.error('Upload failed:', err);
                 }
                  });
             
      
    }
    else{
      this.messagesService.CreateMessagesOnBooking(this.MessageToBeSent).subscribe({
        next: (res) => {
          this.ngZone.run(() => {  
            console.log("message sent!");
            console.log(res);
         
          this.AllMessagesForBooking=this.chatService.AllMessagesForBooking;
          this.AllMessagesForBooking=this.chatService.AllMessagesForBooking;
           console.log("after send");
           console.log(this.AllMessagesForBooking())
          this.MessageToBeSent.content = '';

           this.cdr.detectChanges();
        
            // this.ngZone.run(() => {
           
              
            //         //this.AllMessagesForBooking.update(msgs => [...msgs, this.MessageToBeSent]);
            //     });
            //this.AllMessagesForBooking().push(this.MessageToBeSent);
             this.isLoading.set(false);
             
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
      }); //end of subs

    }
    }}

}


