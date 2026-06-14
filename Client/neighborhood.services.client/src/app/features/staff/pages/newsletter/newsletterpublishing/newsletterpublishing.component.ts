import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import {NewsletterService} from '../../../../../shared/services/newsletter.service'


@Component({
  selector: 'app-newsletterpublishing',
  imports: [FormsModule],
  templateUrl: './newsletterpublishing.component.html',
  styleUrl: './newsletterpublishing.component.css',
})
export class NewsletterpublishingComponent {

    rawHtml: string = '<h1 style="color: blue;">Hello Preview</h1><p>This is live HTML.</p>';

    tamplateHtml:string=""

  constructor(private sanitizer: DomSanitizer, private myService:NewsletterService) {
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
  this.myService.publish(obj)
    .subscribe(next=>console.log("published"));
  }
}

