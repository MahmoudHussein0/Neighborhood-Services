import {
  Component, Input, Output, EventEmitter,
  OnChanges, SimpleChanges, OnDestroy,
  ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { SupportTicketsService } from '../../services/support-ticket.service';
import { SupportMessagesService } from '../../services/support-messages.service';
import { ChatSignalRService, ChatMessage } from '../../services/chat-signalr.service';

@Component({
  selector: 'app-ticket-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ticket-details.component.html',
  styleUrl: './ticket-details.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TicketDetailsComponent implements OnChanges, OnDestroy {
  @Input() ticketId!: number;
  @Output() ticketUpdated = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();

  loading = false;
  ticket: any = null;

  // ── Chat ──
  messages: ChatMessage[] = [];
  newMessage = '';
  sendingMessage = false;
  connectionState: 'connected' | 'disconnected' | 'reconnecting' = 'disconnected';
  readonly senderId = 'admin'; // TODO: استبدليه بالـ auth user id

  private destroy$ = new Subject<void>();

  constructor(
    private ticketService: SupportTicketsService,
    private messagesService: SupportMessagesService,
    private chatSignalR: ChatSignalRService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['ticketId'] && this.ticketId) {
      this.loadDetails();
    }
  }

  ngOnDestroy() {
    this.chatSignalR.leaveTicket(this.ticketId);
    this.chatSignalR.stopConnection();
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadDetails() {
    this.loading = true;
    this.ticket = null;
    this.messages = [];
    this.cdr.markForCheck();

    this.ticketService.getTicketDetails(this.ticketId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.ticket = res;
          this.loading = false;
          this.cdr.markForCheck();
          this.initChat(); // ← ابدأ الشات بعد ما الـ ticket يتحمل
        },
        error: () => {
          this.loading = false;
          this.cdr.markForCheck();
        }
      });
  }

  private async initChat() {
    // 1) جيبي الرسايل القديمة من REST
    this.messagesService.getMessages(this.ticketId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (msgs: any) => {
          this.messages = msgs ?? [];
          this.cdr.markForCheck();
        }
      });

    // 2) ابدأ SignalR
    try {
      await this.chatSignalR.startConnection();
      await this.chatSignalR.joinTicket(this.ticketId);
    } catch (err) {
      console.error('SignalR connection failed', err);
    }

    // 3) اسمعي الرسايل الجديدة
    this.chatSignalR.message$
      .pipe(takeUntil(this.destroy$))
      .subscribe((msg: ChatMessage) => {
        this.messages = [...this.messages, msg];
        this.cdr.markForCheck();
      });

    // 4) اسمعي حالة الـ connection
    this.chatSignalR.connectionState$
      .pipe(takeUntil(this.destroy$))
      .subscribe((state: 'connected' | 'disconnected' | 'reconnecting') => {
        this.connectionState = state;
        this.cdr.markForCheck();
      });
  }

  async sendMessage() {
    const text = this.newMessage.trim();
    if (!text || this.connectionState !== 'connected') return;

    this.newMessage = '';
    this.sendingMessage = true;

    try {
      // بعتي عن طريق SignalR (real-time)
      await this.chatSignalR.sendMessage(this.ticketId, this.senderId, text);

      // احفظي في الـ DB عن طريق REST
      this.messagesService.sendMessage(this.ticketId, text)
        .pipe(takeUntil(this.destroy$))
        .subscribe();
    } catch (err) {
      console.error('Failed to send message', err);
    } finally {
      this.sendingMessage = false;
      this.cdr.markForCheck();
    }
  }

  // ── Ticket actions ──

  updateStatus(newStatus: number) {
    this.ticketService.updateTicketStatus(this.ticketId, newStatus)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.ticket = { ...this.ticket, status: this.mapStatus(newStatus) };
          this.ticketUpdated.emit();
          this.cdr.markForCheck();
        }
      });
  }

  changePriority(priority: string) {
    const map: Record<string, number> = { High: 0, Medium: 1, Low: 2 };
    this.ticketService.updateTicketPriority(this.ticketId, map[priority])
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.ticket = { ...this.ticket, priority };
          this.ticketUpdated.emit();
          this.cdr.markForCheck();
        }
      });
  }

  deleteTicket() {
    if (!confirm('Delete this ticket? This action cannot be undone.')) return;
    this.ticketService.deleteTicket(this.ticketId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.ticketUpdated.emit();
          this.close.emit();
          this.ticket = null;
          this.cdr.markForCheck();
        }
      });
  }

  // ── Helpers ──

  copyEmail(email: string) {
    navigator.clipboard.writeText(email);
  }

  getInitials(name: string): string {
    if (!name) return '?';
    return name.trim()
      .split(/\s+/)
      .map(w => w[0])
      .slice(0, 2)
      .join('')
      .toUpperCase();
  }

  private mapStatus(status: number): string {
    return ['Open', 'InProgress', 'WaitingOnCustomer', 'Resolved'][status] ?? 'Open';
  }

  getStatusLabel(status: string): string {
    const map: Record<string, string> = {
      Open: 'Open',
      InProgress: 'In progress',
      WaitingOnCustomer: 'Waiting on customer',
      Resolved: 'Resolved',
    };
    return map[status] ?? status;
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      Open: 'badge-open',
      InProgress: 'badge-inprogress',
      WaitingOnCustomer: 'badge-waiting',
      Resolved: 'badge-closed',
    };
    return map[status] ?? 'badge-neutral';
  }

  getPriorityClass(priority: string): string {
    const map: Record<string, string> = {
      High: 'badge-high',
      Medium: 'badge-medium',
      Low: 'badge-low',
    };
    return map[priority] ?? 'badge-neutral';
  }
}