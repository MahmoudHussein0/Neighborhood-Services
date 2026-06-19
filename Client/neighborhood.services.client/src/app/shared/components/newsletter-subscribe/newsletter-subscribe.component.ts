import { Component, Signal,signal } from '@angular/core';
import {NewsletterService} from'../../services/newsletter.service'
import { FormsModule } from '@angular/forms';
import { ElementRef, ViewChild } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { Toast, ToastrService } from 'ngx-toastr';




@Component({
  selector: 'app-newsletter-subscribe',
  imports: [FormsModule,TranslatePipe],
  templateUrl: './newsletter-subscribe.component.html',
  styleUrl: './newsletter-subscribe.component.css',
})
export class NewsletterSubscribeComponent {
  /**
   *
   */
 
  constructor(private myService:NewsletterService,private toastr: ToastrService) {}

objectToSend={
  "email":" "
}
email = signal<string>('');
  subs():void{
    if(this.email.length==0){this.toastr.error("Enter a Valid Email"); return;}
    this.objectToSend.email=this.email()
    console.log(this.email)
    console.log(this.objectToSend)

    this.myService.subscribe(this.objectToSend)
  .subscribe({
    
    next: res => {console.log(res);this.toastr.success("Subscribed Susceccfully");this.email.set(" ")},
    error: err => console.error(err)
    
  });
    
  }
    
    
    
  
}
