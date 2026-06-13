import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../shared/components/footer/footer.component';
import {NotificationBellComponent} from '../../shared/components/notification-bell/notification-bell.component'
import { FavoriteListComponent } from '../../features/customer/pages/favorite-list/favorite-list.component';
import { CustomerChatsComponent } from '../../features/customer/pages/customer-chats/customer-chats.component';
import { ChatRoomComponent } from '../../features/customer/components/chat-room/chat-room.component';


@Component({
  selector: 'app-public-layout',
  imports: [RouterOutlet, NavbarComponent, FooterComponent, 
    NotificationBellComponent, 
    FavoriteListComponent, 
    CustomerChatsComponent,
    ChatRoomComponent
  ],
  template: `
    <app-navbar />
  
    <main class="flex-grow-1">
    <app-chat-room />
   <app-customer-chats />
  
   <!--  <app-favorite-list />-->
      <router-outlet />
    </main>
    <app-footer />
  `,
  styles: [`
    :host {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }
  `],
})
export class PublicLayoutComponent {}
