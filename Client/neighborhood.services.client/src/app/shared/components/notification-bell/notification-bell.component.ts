import { Component, signal,inject,computed } from '@angular/core';
// import { MaterialModule } from '../../shared/material.module';
import { CommonModule } from '@angular/common';
import { NotificationServiceService } from '../../../shared/services/notification-service.service';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { HttpErrorResponse } from '@angular/common/http';
import { NotificationMessage } from './../../../core/models/notification-message';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../../features/auth/services/auth.service';
import { RouterLink, RouterLinkActive } from '@angular/router';




@Component({
  selector: 'app-notification-bell',
  imports: [CommonModule, NgbDropdownModule,RouterLink, RouterLinkActive],
  templateUrl: './notification-bell.component.html',
  styleUrl: './notification-bell.component.css',

})
export class NotificationBellComponent {
  // // TODO (Arwa): replace with real unread count from the notifications service.
  // unreadCount = signal(3);

  // // TODO (Arwa): open a dropdown listing notifications, mark-as-read, etc.
  // open() {
  //   // placeholder
  // }
 
  notifications$ = new BehaviorSubject<NotificationMessage[]>([]);
  unreadCount$ = new BehaviorSubject<number>(0);

//the auth service
 private readonly authService = inject(AuthService);
  readonly currentUser = this.authService.currentUser;
  //to get current role
  readonly currentUserRole = computed(() => {
    const role = this.currentUser()?.role;
    return role;
  });
  //to get current id
  readonly currentUserId = computed(() => {
    const Id = this.currentUser()?.userId;
    return Id;
  });

  
  private subscriptions: Subscription[] = [];

  constructor(private notificationService: NotificationServiceService, private toastr: ToastrService) { }

  ngOnInit(): void {   

    this.notificationService.startConnection(); 

    // Subscribe to SignalR
    this.subscriptions.push(
      this.notificationService.notifications$.subscribe((data) => {
        this.notifications$.next(data);
        if(data!=null){
        const unread = data.filter((n) => !n.isRead).length;
        this.unreadCount$.next(unread);}
        else{this.unreadCount$.next(0)}
        
        console.log('Received notifications via SignalR:', data);
      })
    );

    this.loadNotifications();
  } 

  loadNotifications() {
    this.notificationService.GetAllNotifications().subscribe({
      next: (data: NotificationMessage[]) => {
        if(data!=null){
        data=data.filter(e=>e.isRead==false);
        data.sort((a, b) => new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime());}
        console.log(this.unreadCount$.value)
        this.notificationService.setNotifications(data);
        // this.toastr.success('Notifications loaded successfully.');
      },
      error: (error: HttpErrorResponse) => {
        this.toastr.error(error.error.detail || 'An error occurred while processing your request.');
      }
    })
  }


  markAllAsRead() {
    this.notificationService.markAllAsRead().subscribe({
      next: () => {
        this.notificationService.markAllLocalAsRead();
      },
    });
  }


//Marking a notification as read 
  onNotificationClick(notification: NotificationMessage) {
    if (!notification.isRead) {
      this.notificationService.markAsRead(notification.id).subscribe({
        next: () => {
          this.notificationService.updateNotificationAsRead(notification.id);
          console.log(`Marked notification ${notification.id} as read`);
          this.notificationService.setNotifications(this.notifications$.value.map(n => n.id === notification.id ? { ...n, isRead: true } : n));
          notification.isRead = true;
          console.log('Updated notifications after marking as read:', this.notifications$.value);
          console.log('Updated unread count:', this.unreadCount$.value);
        },
        error: () => this.toastr.error('Failed to mark as read'),
      });
    }
  }

  ngOnDestroy(): void {
    // Prevent memory leaks
    this.subscriptions.forEach(s => s.unsubscribe());   
  }
}
