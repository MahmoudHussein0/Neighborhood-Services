import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';

import { ModerationService, FlaggedServiceRequest } from '../../services/moderation.service';
import { PagedResult } from '../../../../core/models/paged-result.model';

@Component({
  selector: 'app-flagged-requests',
  imports: [DatePipe, CurrencyPipe, TranslatePipe],
  templateUrl: './flagged-requests.component.html',
})
export class FlaggedRequestsComponent implements OnInit {
  private readonly service = inject(ModerationService);
  private readonly toastr = inject(ToastrService);

  readonly pageSize = 5;

  loading = signal(false);
  result = signal<PagedResult<FlaggedServiceRequest> | null>(null);
  page = signal(1);
  acting = signal<number | null>(null); // id currently being approved/rejected

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading.set(true);
    this.service.getFlagged(this.page(), this.pageSize).subscribe({
      next: (r) => {
        this.result.set(r);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  goToPage(p: number) {
    this.page.set(p);
    this.load();
  }

  approve(sr: FlaggedServiceRequest) {
    this.acting.set(sr.id);
    this.service.approve(sr.id).subscribe({
      next: () => {
        this.toastr.success('Request approved and published.');
        this.afterAction();
      },
      error: () => {
        this.toastr.error('Could not approve the request.');
        this.acting.set(null);
      },
    });
  }

  reject(sr: FlaggedServiceRequest) {
    this.acting.set(sr.id);
    this.service.reject(sr.id).subscribe({
      next: () => {
        this.toastr.success('Request rejected.');
        this.afterAction();
      },
      error: () => {
        this.toastr.error('Could not reject the request.');
        this.acting.set(null);
      },
    });
  }

  // After a decision, the item leaves the queue. If we just emptied the page, step back one.
  private afterAction() {
    this.acting.set(null);
    const res = this.result();
    if (res && res.items.length === 1 && this.page() > 1) {
      this.page.set(this.page() - 1);
    }
    this.load();
  }
}
