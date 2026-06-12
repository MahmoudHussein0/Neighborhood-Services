import {
  Component, Input, Output, EventEmitter,
  OnChanges, SimpleChanges, OnDestroy,
  ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { SupportTicketsService } from '../../services/support-ticket.service';

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

  private destroy$ = new Subject<void>();

  constructor(
    private ticketService: SupportTicketsService,
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
    this.cdr.markForCheck();

    this.ticketService.getTicketDetails(this.ticketId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res: any) => {
          this.ticket = res;
          this.loading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.loading = false;
          this.cdr.markForCheck();
        }
      });
  }

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

  copyEmail(email: string) {
    navigator.clipboard.writeText(email).then(() => {
      // optional: add toast notification here
    });
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