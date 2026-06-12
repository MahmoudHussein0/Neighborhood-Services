import {
  Component, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { SupportTicketsService } from '../../services/support-ticket.service';
import { TicketDetailsComponent } from './ticket-details.component';

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

  allTickets: any[] = [];      // ← الداتا الكاملة من الـ API
  tickets: any[] = [];         // ← اللي بيتعرض بعد الفلترة

  selectedTicketId: number | null = null;
  loading = false;

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
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadTickets() {
    this.loading = true;
    this.allTickets = [];
    this.tickets = [];
    this.cdr.markForCheck();

    this.ticketService.getTickets({}).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: any) => {
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

    this.tickets = this.allTickets.filter(t => {
      if (status   && t.status   !== status)   return false;
      if (priority && t.priority !== priority) return false;
      if (q && !t.subject?.toLowerCase().includes(q)
            && !t.description?.toLowerCase().includes(q)) return false;
      return true;
    });

    this.cdr.markForCheck();
  }

  onSearchInput() {
    this.search$.next(this.filters.search);
  }

  // الـ select بيستدعي applyFilters مباشرة بدون debounce
  onFilterChange() {
    this.applyFilters();
  }

  openTicket(id: number) {
    this.selectedTicketId = id;
    this.cdr.markForCheck();
  }

  getStatusLabel(status: string): string {
    const map: Record<string, string> = {
      Open: 'Open', InProgress: 'In progress',
      WaitingOnCustomer: 'Waiting', Resolved: 'Resolved'
    };
    return map[status] ?? status;
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      Open: 'badge-open', InProgress: 'badge-inprogress',
      WaitingOnCustomer: 'badge-waiting', Resolved: 'badge-closed'
    };
    return map[status] ?? 'badge-neutral';
  }

  getPriorityClass(priority: string): string {
    const map: Record<string, string> = {
      High: 'badge-high', Medium: 'badge-medium', Low: 'badge-low'
    };
    return map[priority] ?? 'badge-neutral';
  }
}