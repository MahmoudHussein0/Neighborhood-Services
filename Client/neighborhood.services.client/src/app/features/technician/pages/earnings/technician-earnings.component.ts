import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { TechnicianEarningsService } from '../../services/technician-earnings.service';
import { Invoice, InvoiceStatus } from '../../models/earnings.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-technician-earnings',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './technician-earnings.component.html',
  styleUrls: ['./technician-earnings.component.css']
})
export class TechnicianEarningsComponent implements OnInit {
  invoices = signal<Invoice[]>([]);
  readonly Math = Math;
  InvoiceStatus = InvoiceStatus;

  // Handles both string ("Paid") and numeric (1) from backend JsonStringEnumConverter
  private statusMatches(invoiceStatus: any, enumValue: InvoiceStatus): boolean {
    const s = invoiceStatus?.toString();
    return s === InvoiceStatus[enumValue] || s === enumValue.toString();
  }

  totalEarned = computed(() =>
    this.invoices()
      .filter(i => this.statusMatches(i.status, InvoiceStatus.Paid))
      .reduce((sum, i) => sum + i.totalAmount, 0)
  );

  // Filters
  filterStatus = signal<InvoiceStatus | ''>('');

  filteredInvoices = computed(() => {
    const status = this.filterStatus();
    if (status === '') return this.invoices();
    return this.invoices().filter(i => this.statusMatches(i.status, status as InvoiceStatus));
  });

  // Pagination
  currentPage = signal<number>(1);
  readonly pageSize = 6;

  paginatedInvoices = computed(() => {
    const start = (this.currentPage() - 1) * this.pageSize;
    return this.filteredInvoices().slice(start, start + this.pageSize);
  });

  totalPages = computed(() =>
    Math.ceil(this.filteredInvoices().length / this.pageSize) || 1
  );

  emptyRows = computed(() => {
    const count = this.paginatedInvoices().length;
    if (count === 0 || count === this.pageSize) return [];
    return Array(this.pageSize - count).fill(0);
  });

  constructor(
    private earningsService: TechnicianEarningsService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.earningsService.getMyInvoices().subscribe({
      next: (inv) => this.invoices.set(inv),
      error: () => this.toastr.error('Failed to load invoices')
    });
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
    }
  }

  /** Returns the i18n key for any status value (string OR number from API) */
  getStatusLabel(status: any): string {
    const s = status?.toString();
    if (s === 'Paid'     || s === '1') return 'earnings.statusType.paid';
    if (s === 'Unpaid'   || s === '0') return 'earnings.statusType.unpaid';
    if (s === 'Refunded' || s === '2') return 'earnings.statusType.refunded';
    if (s === 'Voided'   || s === '3') return 'earnings.statusType.voided';
    return 'earnings.allStatuses';
  }

  getFilterLabel(status: InvoiceStatus | ''): string {
    if (status === '') return 'earnings.allStatuses';
    return this.getStatusLabel(status);
  }

  getStatusBadgeClass(status: any): string {
    const s = status?.toString();
    if (s === 'Paid'     || s === '1') return 'bg-success bg-opacity-10 text-success fw-bold px-3 py-2 rounded-pill';
    if (s === 'Unpaid'   || s === '0') return 'bg-warning bg-opacity-10 text-pending-brown fw-bold px-3 py-2 rounded-pill';
    if (s === 'Refunded' || s === '2') return 'bg-info bg-opacity-10 text-info fw-bold px-3 py-2 rounded-pill';
    if (s === 'Voided'   || s === '3') return 'bg-danger bg-opacity-10 text-danger fw-bold px-3 py-2 rounded-pill';
    return 'bg-secondary bg-opacity-10 text-secondary fw-bold px-3 py-2 rounded-pill';
  }

  viewInvoice(bookingId: number): void {
    window.open(`${environment.apiUrl}/api/invoices/booking/${bookingId}/pdf/view`, '_blank');
  }

  downloadPdf(bookingId: number): void {
    window.open(`${environment.apiUrl}/api/invoices/booking/${bookingId}/pdf`, '_blank');
  }
}
