import {
  Component,
  Input,
  OnChanges,
  SimpleChanges
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { DisputeService } from '../../services/disputes.service';

@Component({
  selector: 'app-dispute-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dispute-details.component.html',
  styleUrls: ['./dispute-details.component.css']
})
export class DisputeDetailsComponent implements OnChanges {

  @Input() disputeId!: number;

  dispute: any;
  loading = false;
  error = '';

  constructor(
    private disputeService: DisputeService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {

    if (changes['disputeId']?.currentValue) {
      this.loadDispute();
    }
  }

  loadDispute(): void {

    this.loading = true;
    this.error = '';

    this.disputeService
      .getById(this.disputeId)
      .subscribe({
        next: (res: any) => {
          this.dispute = res;
          this.loading = false;
        },
        error: () => {
          this.error = 'Failed to load dispute details';
          this.loading = false;
        }
      });
  }
}