import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import {NewsletterService} from '../../../../../shared/services/newsletter.service'
import { TranslatePipe } from '@ngx-translate/core';
import { Toast, ToastrService } from 'ngx-toastr';




@Component({
  selector: 'app-newsletterpublishing',
  imports: [FormsModule,TranslatePipe],
  templateUrl: './newsletterpublishing.component.html',
  styleUrl: './newsletterpublishing.component.css',
})
export class NewsletterpublishingComponent {

    rawHtml: string = '<h1 style="color: blue;">Hello Preview</h1><p>This is live HTML.</p>';

    tamplateHtml:string=""

  constructor(private sanitizer: DomSanitizer, private myService:NewsletterService, private tostr:ToastrService) {
  }
  subject = '';

  get sanitizedHtml():SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(this.rawHtml);
  }

  publishNewsletter():void{
    const  subject=this.subject;
    const content= this.rawHtml;
  
    const obj={
      "subj":this.subject,
      "content":this.rawHtml
    }
  this.myService.publish(obj).subscribe({
    next: ()=>this.tostr.success("Newsletter Sent Successfully"),
    error: err =>this.tostr.error(err)
  });
  }
}


