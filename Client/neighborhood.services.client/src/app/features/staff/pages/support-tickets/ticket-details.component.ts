import {
  Component, Input, Output, EventEmitter,
  OnChanges, SimpleChanges, OnDestroy,
  ViewChild, ElementRef,
  ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SupportTicketsService } from '../../services/support-ticket.service';
import { SupportMessagesService } from '../../services/support-messages.service';

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
  @ViewChild('scrollMe') private scrollContainer!: ElementRef;

  loading = false;
  ticket: any = null;
  messages: any[] = [];
  messageText = '';

  private destroy$ = new Subject<void>();

  constructor(
    private ticketService: SupportTicketsService,
    private messageService: SupportMessagesService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['ticketId'] && this.ticketId) {
      this.loadDetails();
    }
  }

  ngOnDestroy() {
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
          this.messages = res.messages ?? [];
          this.loading = false;
          this.cdr.markForCheck();
          setTimeout(() => this.scrollToBottom(), 60);
        },
        error: () => {
          this.loading = false;
          this.cdr.markForCheck();
        }
      });
  }

  sendMessage() {
    const text = this.messageText.trim();
    if (!text) return;

    const tempId = Date.now();
    const tempMsg = {
      id: tempId,
      message: text,
      senderType: 'Staff',
      createdAt: new Date().toISOString(),
      isTemp: true
    };

    this.messages = [...this.messages, tempMsg];
    this.messageText = '';
    this.cdr.markForCheck();
    setTimeout(() => this.scrollToBottom(), 60);

    this.messageService.sendMessage(this.ticketId, text)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.messages = this.messages.map(m =>
            m.id === tempId ? { ...(res ?? tempMsg), isTemp: false } : m
          );
          this.cdr.markForCheck();
        },
        error: () => {
          this.messages = this.messages.filter(m => m.id !== tempId);
          this.cdr.markForCheck();
          alert('Failed to send message.');
        }
      });
  }

  // ── Status workflow: Open(0)→InProgress(1)  |  InProgress(1)→Closed(3) ──
// 2. في updateStatus() → تحديث local فوري بدل loadDetails()
updateStatus(newStatus: number) {
  this.ticketService.updateTicketStatus(this.ticketId, newStatus)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: () => {
        // تحديث local فوري بدون reload كامل
        const statusMap: Record<number, string> = {
          1: 'Open', 2: 'InProgress', 3: 'WaitingOnCustomer', 4: 'Resolved'
        };
        this.ticket = { ...this.ticket, status: statusMap[newStatus] };
        this.cdr.markForCheck();
        this.ticketUpdated.emit();
      },
      error: () => alert('Failed to update status.')
    });
}
  changePriority(newPriority: string) {
    // map string → enum number expected by the API
    const priorityMap: Record<string, number> = { High: 0, Medium: 1, Low: 2 };
    const priorityNumber = priorityMap[newPriority] ?? 2;

    this.ticketService.updateTicketPriority(this.ticketId, priorityNumber)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          if (this.ticket) this.ticket = { ...this.ticket, priority: newPriority };
          this.cdr.markForCheck();
          this.ticketUpdated.emit();
        },
        error: () => alert('Failed to update priority.')
      });
  }

  deleteTicket() {
    if (!confirm('Delete this ticket permanently?')) return;

    this.ticketService.deleteTicket(this.ticketId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.ticketUpdated.emit();
          this.ticket = null;
          this.cdr.markForCheck();
        },
        error: () => alert('Failed to delete ticket.')
      });
  }

  onBack() {
    this.close.emit();
  }

  // ── Badge helpers ─────────────────────────────────────────────────────────

  getStatusLabel(status: string): string {
    const map: Record<string, string> = {
      Open: 'Open', InProgress: 'In progress', Closed: 'Closed'
    };
    return map[status] ?? status;
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      Open: 'badge-open', InProgress: 'badge-inprogress', Closed: 'badge-closed'
    };
    return map[status] ?? 'badge-neutral';
  }

  getPriorityClass(priority: string): string {
    const map: Record<string, string> = {
      High: 'badge-high', Medium: 'badge-medium', Low: 'badge-low'
    };
    return map[priority] ?? 'badge-neutral';
  }

  private scrollToBottom(): void {
    try {
      if (this.scrollContainer) {
        this.scrollContainer.nativeElement.scrollTop =
          this.scrollContainer.nativeElement.scrollHeight;
      }
    } catch {}
  }
}