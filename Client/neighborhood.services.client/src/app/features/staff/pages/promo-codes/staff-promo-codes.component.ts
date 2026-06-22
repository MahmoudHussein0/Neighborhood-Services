import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { StaffPromoCodeService } from '../../services/staff-promo-code.service';
import { PromoCode } from '../../models/promo-code.model';

@Component({
  selector: 'app-staff-promo-codes',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './staff-promo-codes.component.html',
  styleUrls: ['./staff-promo-codes.component.css']
})
export class StaffPromoCodesComponent implements OnInit {
  promoCodes = signal<PromoCode[]>([]);
  
  // Create Form State
  newCode = signal<string>('');
  newDiscount = signal<number>(10);
  newMaxUses = signal<number>(100);
  newExpiresAt = signal<string>('');

  // Earliest selectable expiry — today (yyyy-MM-dd for the native date input's min attr).
  readonly today = new Date().toISOString().split('T')[0];

  constructor(
    private promoCodeService: StaffPromoCodeService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    // Default expiry to 30 days from now
    const nextMonth = new Date();
    nextMonth.setMonth(nextMonth.getMonth() + 1);
    this.newExpiresAt.set(nextMonth.toISOString().split('T')[0]);
    
    this.loadPromoCodes();
  }

  loadPromoCodes(): void {
    this.promoCodeService.getAll().subscribe({
      next: (codes) => this.promoCodes.set(codes),
      error: () => this.toastr.error('Failed to load promo codes')
    });
  }

  createPromoCode(): void {
    if (!this.newCode()) {
      this.toastr.warning('Please enter a promo code');
      return;
    }

    if (!this.newExpiresAt() || this.newExpiresAt() < this.today) {
      this.toastr.warning('Expiry date must be today or later');
      return;
    }

    this.promoCodeService.create(
      this.newCode().toUpperCase(),
      this.newDiscount(),
      this.newMaxUses(),
      new Date(this.newExpiresAt()).toISOString()
    ).subscribe({
      next: () => {
        this.toastr.success('Promo code created');
        this.loadPromoCodes();
        this.newCode.set('');
      },
      error: () => this.toastr.error('Failed to create promo code')
    });
  }

  deletePromoCode(id: number): void {
    if (confirm('Are you sure you want to delete this promo code?')) {
      this.promoCodeService.delete(id).subscribe({
        next: () => {
          this.toastr.success('Promo code deleted');
          this.loadPromoCodes();
        },
        error: () => this.toastr.error('Failed to delete promo code')
      });
    }
  }
}
