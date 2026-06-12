import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { TechnicianEarningsService } from '../../services/technician-earnings.service';
import { Invoice, InvoiceStatus } from '../../models/earnings.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-technician-earnings',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './technician-earnings.component.html',
  styleUrls: ['./technician-earnings.component.css']
})
export class TechnicianEarningsComponent implements OnInit {
  invoices = signal<Invoice[]>([]);
  
  InvoiceStatus = InvoiceStatus;
  
  // Computed stats
  totalEarned = computed(() => {
    return this.invoices()
      .filter(i => i.status === InvoiceStatus.Paid)
      .reduce((sum, i) => sum + i.totalAmount, 0);
  });

  pendingAmount = computed(() => {
    return this.invoices()
      .filter(i => i.status === InvoiceStatus.Unpaid)
      .reduce((sum, i) => sum + i.totalAmount, 0);
  });

  constructor(
    private earningsService: TechnicianEarningsService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    // Hardcoded technicianId for demo purposes until auth context provides it
    const technicianId = 1;
    
    this.earningsService.getMyInvoices(technicianId).subscribe({
      next: (inv) => this.invoices.set(inv),
      error: () => this.toastr.error('Failed to load invoices')
    });
  }

  downloadPdf(bookingId: number): void {
    // Calling backend to get PDF bytes
    window.open(`${environment.apiUrl}/api/invoices/booking/${bookingId}/pdf`, '_blank');
  }

  getStatusBadgeClass(status: InvoiceStatus): string {
    switch(status) {
      case InvoiceStatus.Paid: return 'bg-success';
      case InvoiceStatus.Unpaid: return 'bg-warning text-dark';
      case InvoiceStatus.Refunded: return 'bg-info text-dark';
      case InvoiceStatus.Voided: return 'bg-danger';
      default: return 'bg-secondary';
    }
  }
}
