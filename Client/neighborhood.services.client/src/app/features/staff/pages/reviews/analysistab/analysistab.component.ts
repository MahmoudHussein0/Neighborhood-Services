import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ReviewAnalysisService } from '../../../services/Review-analysis.service';
import { ReviewAnalysisDto } from '../../../models/ReviewAnalysis.model';


@Component({
  selector: 'app-analysistab',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './analysistab.component.html',
  styleUrl: './analysistab.component.css'
})
export class AnalysisTabComponent implements OnInit {

  private svc = inject(ReviewAnalysisService);
  private translate = inject(TranslateService);

  analyses = signal<ReviewAnalysisDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);

    this.svc.getAll().subscribe({
      next: (data) => {
        this.analyses.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set(this.translate.instant('reviewsAnalysis.loadFail'));
        this.loading.set(false);
      }
    });
  }

  totalReviews = computed(() => this.analyses().length);

  positiveCount = computed(() =>
    this.analyses().filter(x => x.sentiment === 'Positive').length
  );

  neutralCount = computed(() =>
    this.analyses().filter(x => x.sentiment === 'Neutral').length
  );

  negativeCount = computed(() =>
    this.analyses().filter(x => x.sentiment === 'Negative').length
  );

  flaggedCount = computed(() =>
    this.analyses().filter(x => x.isFlagged).length
  );

  averageQuality = computed(() => {
    const data = this.analyses();

    if (!data.length) return 0;

    return (
      data.reduce((sum, item) => sum + item.qualityScore, 0) /
      data.length
    ).toFixed(2);
  });

  flaggedReviews = computed(() =>
    this.analyses().filter(x => x.isFlagged)
  );
}