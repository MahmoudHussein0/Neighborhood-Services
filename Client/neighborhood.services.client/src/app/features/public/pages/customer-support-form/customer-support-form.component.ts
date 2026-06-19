import { Component,OnInit, signal } from '@angular/core';
import { Router,RouterLink,RouterLinkActive } from '@angular/router'; 
import { TicketDto } from '../../models/ticket-dto';
import { FormsModule } from '@angular/forms'
import {CustomerSupportComponent} from '../customersupport/customer-support/customer-support.component'

@Component({
  selector: 'app-customer-support-form',
  imports: [FormsModule,RouterLink,RouterLinkActive,CustomerSupportComponent],
  templateUrl: './customer-support-form.component.html',
  styleUrl: './customer-support-form.component.css',
})
export class CustomerSupportFormComponent {
  isLoading=signal(false);
   ngOnInit() {this.showChat = false; this.isLoading.set(true);}

  public ticket: TicketDto = new TicketDto();
  public showChat = false;

  constructor(private router: Router) {this.showChat = false;}

  goToLiveChat(): void {

     this.showChat = true;

    // Navigate to chat page
   // this.router.navigate(['/customer-service-chat']);
  }
}
