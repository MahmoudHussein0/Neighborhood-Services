import {
  Component, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef, inject
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { SupportTicketsService } from '../../services/support-ticket.service';
import { TicketDetailsComponent } from './ticket-details.component';
import { NotificationServiceService } from '../../../../shared/services/notification-service.service';
import { SupportTicket } from '../../models/support-ticket.model';

@Component({
  selector: 'app-support-tickets',
  standalone: true,
  imports: [CommonModule, FormsModule, TicketDetailsComponent],
  templateUrl: './support-tickets.component.html',
  styleUrl: './support-tickets.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SupportTicketsComponent implements OnInit, OnDestroy {

  filters = { status: '', priority: '', search: '' };

  allTickets: SupportTicket[] = [];
  tickets: SupportTicket[] = [];

  selectedTicketId: number | null = null;
  loading = false;

  private readonly notificationService = inject(NotificationServiceService);

  private destroy$ = new Subject<void>();
  private search$ = new Subject<string>();

  constructor(
    private ticketService: SupportTicketsService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.search$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(() => this.applyFilters());

    this.loadTickets();

    // Live refresh: any realtime notification that mentions a new support ticket
    // quietly reloads the list so staff see it appear without a manual refresh.
    // The toast itself is already shown by NotificationBellComponent.
    this.notificationService.notificationReceived$
      .pipe(takeUntil(this.destroy$))
      .subscribe((n) => {
        const text = (n?.message ?? '').toLowerCase();
        if (text.includes('ticket')) {
          this.loadTickets();
        }
      });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadTickets() {
    this.loading = true;
    this.cdr.markForCheck();

    this.ticketService.getTickets({}).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res) => {
        this.allTickets = res ?? [];
        this.loading = false;
        this.applyFilters();
      },
      error: () => {
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  applyFilters() {
    const { status, priority, search } = this.filters;
    const q = search.trim().toLowerCase();

    this.tickets = this.allTickets
      .filter(t => {
        if (status   && t.status   !== status)   return false;
        if (priority && t.priority !== priority) return false;
        if (q && !t.subject?.toLowerCase().includes(q)
              && !t.description?.toLowerCase().includes(q)
              && !t.senderName?.toLowerCase().includes(q)) return false;
        return true;
      })
      .sort((a, b) => new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime());

    this.cdr.markForCheck();
  }

  onSearchInput() {
    this.search$.next(this.filters.search);
  }

  onFilterChange() {
    this.applyFilters();
  }

  resetFilters() {
    this.filters = { status: '', priority: '', search: '' };
    this.applyFilters();
  }

  openTicket(id: number) {
    this.selectedTicketId = id;
    this.cdr.markForCheck();
  }

  closeTicket() {
    this.selectedTicketId = null;
    this.cdr.markForCheck();
  }

  /** Ticket-detail emits this after an action that may have changed list-level data (status, etc). */
  onTicketUpdated() {
    this.loadTickets();
  }

  trackByTicketId(_index: number, t: SupportTicket): number {
    return t.id;
  }

  getStatusLabel(status: string): string {
    const map: Record<string, string> = {
      Open: 'Open', InProgress: 'In progress',
      WaitingOnCustomer: 'Waiting on customer', Resolved: 'Resolved'
    };
    return map[status] ?? status;
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      Open: 'badge-open', InProgress: 'badge-inprogress',
      WaitingOnCustomer: 'badge-waiting', Resolved: 'badge-resolved'
    };
    return map[status] ?? 'badge-neutral';
  }

  getPriorityClass(priority: string): string {
    const map: Record<string, string> = {
      High: 'badge-high', Medium: 'badge-medium', Low: 'badge-low'
    };
    return map[priority] ?? 'badge-neutral';
  }

  initials(name: string): string {
    if (!name) return '?';
    return name.trim().split(/\s+/).map(w => w[0]).slice(0, 2).join('').toUpperCase();
  }

  /** Short relative-ish timestamp for the list row. */
  rowTime(dateStr: string): string {
    const date = new Date(dateStr);
    const now = new Date();
    const sameDay = date.toDateString() === now.toDateString();
    if (sameDay) {
      return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    }
    return date.toLocaleDateString([], { day: '2-digit', month: 'short' });
  }
}