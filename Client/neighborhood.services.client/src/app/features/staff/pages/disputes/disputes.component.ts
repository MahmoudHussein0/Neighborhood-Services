import {
  Component,
  OnInit,
  OnDestroy,
  ChangeDetectionStrategy,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Subject } from 'rxjs';
import {
  debounceTime,
  distinctUntilChanged,
  takeUntil
} from 'rxjs/operators';

import { DisputeService } from '../../services/disputes.service';
import { DisputeDto } from '../../models/staff-disput.model';
import { DisputeDetailsComponent } from './dispute-details.component';

@Component({
  selector: 'app-disputes',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DisputeDetailsComponent
  ],
  templateUrl: './disputes.component.html',
  styleUrl: './disputes.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DisputesComponent
implements OnInit, OnDestroy {

  disputes: DisputeDto[] = [];
  filteredDisputes: DisputeDto[] = [];

  selectedDisputeId: number | null = null;

  // Resolve modal state (replaces the native prompt()).
  resolveTargetId: number | null = null;
  resolutionText = '';
  resolveSubmitting = false;
  resolveError = '';

  loading = false;

  filters = {
    status: '',
    type: '',
    search: ''
  };

  private destroy$ = new Subject<void>();
  private search$ = new Subject<string>();

  constructor(
    private disputeService: DisputeService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {

    this.search$
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe(() => this.applySearch());

    this.loadDisputes();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadDisputes() {

    this.loading = true;
    this.cdr.markForCheck();

    this.disputeService
      .getDisputes()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.disputes = res ?? [];
          this.filteredDisputes = [...this.disputes];
          this.loading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.loading = false;
          this.cdr.markForCheck();
        }
      });
  }

  onStatusChange() {

    if (!this.filters.status) {
      this.loadDisputes();
      return;
    }

    this.loading = true;

    this.disputeService
      .getByStatus(this.filters.status)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.disputes = res ?? [];
          this.applySearch();
          this.loading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.loading = false;
          this.cdr.markForCheck();
        }
      });
  }

  onTypeChange() {

    if (!this.filters.type) {
      this.loadDisputes();
      return;
    }

    this.loading = true;

    this.disputeService
      .getByType(this.filters.type)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.disputes = res ?? [];
          this.applySearch();
          this.loading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.loading = false;
          this.cdr.markForCheck();
        }
      });
  }

  onSearchInput() {
    this.search$.next(this.filters.search);
  }

  applySearch() {

    const q =
      this.filters.search
        .trim()
        .toLowerCase();

    if (!q) {
      this.filteredDisputes = [...this.disputes];
      this.cdr.markForCheck();
      return;
    }

    this.filteredDisputes =
      this.disputes.filter(d =>
        d.reason?.toLowerCase().includes(q)
        || d.disputeType?.toLowerCase().includes(q)
      );

    this.cdr.markForCheck();
  }

  openDispute(id: number) {
    this.selectedDisputeId = id;
    this.cdr.markForCheck();
  }

  closeDispute() {
    this.selectedDisputeId = null;
    this.cdr.markForCheck();
  }
  startReview(id: number): void {

  const body = {
    id,
    status: 'UnderReview'
  };

  this.disputeService
    .updateDispute(id, body)
    .subscribe({
      next: () => {
        this.loadDisputes();
      }
    });
}
openResolve(id: number): void {
  this.resolveTargetId = id;
  this.resolutionText = '';
  this.resolveError = '';
  this.cdr.markForCheck();
}

cancelResolve(): void {
  this.resolveTargetId = null;
  this.cdr.markForCheck();
}

submitResolve(): void {

  const id = this.resolveTargetId;
  const resolution = this.resolutionText.trim();

  if (!id || !resolution) {
    this.resolveError = 'Please enter a resolution.';
    this.cdr.markForCheck();
    return;
  }

  this.resolveSubmitting = true;
  this.resolveError = '';
  this.cdr.markForCheck();

  const body = {
    id,
    status: 'Resolved',
    resolution,
    resolvedByStaffId: 1
  };

  this.disputeService
    .updateDispute(id, body)
    .subscribe({
      next: () => {
        this.resolveSubmitting = false;
        this.resolveTargetId = null;
        this.loadDisputes();
      },
      error: (err) => {
        this.resolveSubmitting = false;
        this.resolveError =
          err?.error?.detail ||
          err?.error?.message ||
          'Failed to resolve the dispute.';
        this.cdr.markForCheck();
      }
    });
}
}