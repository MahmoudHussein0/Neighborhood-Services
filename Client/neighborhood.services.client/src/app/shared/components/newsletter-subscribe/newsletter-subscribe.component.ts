import { Component } from '@angular/core';
import {NewsletterService} from'../../services/newsletter.service'
import { FormsModule } from '@angular/forms';
import { ElementRef, ViewChild } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';



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
  isSaving = false;
  isDone = false;
  isHidden=""
  constructor(private myService:NewsletterService) {}
  @ViewChild('emailInput')
emailInput!: ElementRef<HTMLInputElement>;


onSubmit(event: Event) {
   
this.isHidden="none";
    this.isSaving = true;
  this.emailInput.nativeElement.blur();
  this.myService
  .subscribbel(this.email)
  .subscribe({
    next: res => console.log(res),
    error: err => console.error(err)
  });
   event.preventDefault();

    // Call your API here

    setTimeout(() => {
      this.isDone = true;
    }, 200);
  }
  onAnimationEnd() {
    setTimeout(() => {
      this.isDone = true;
    }, 200);
  }

email:string='';
  subs(inputemail:string):void{
    
    this.myService
  .subscribbel(inputemail)
  .subscribe({
    next: res => console.log(res),
    error: err => console.error(err)
  });
    
  }
    
    
    
  
}
