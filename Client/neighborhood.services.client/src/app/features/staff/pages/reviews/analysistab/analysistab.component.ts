import {
  Component, OnInit, inject, signal, computed, AfterViewInit, ElementRef, ViewChild
} from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { ReviewsService } from '../../../services/Reviews.service';
import { ReviewDto, StaffRatingSummary } from '../../../models/Review.model';

// Chart.js — install: npm install chart.js
import { Chart, registerables } from 'chart.js';
Chart.register(...registerables);

@Component({
  selector: 'app-analysistab',
  imports: [CommonModule, DecimalPipe],
  templateUrl: './analysistab.component.html',
  styleUrl: './analysistab.component.css',
})

export class AnalysisTabComponent implements OnInit, AfterViewInit {
  @ViewChild('trendChart') trendRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('sentimentChart') sentimentRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('staffChart') staffRef!: ElementRef<HTMLCanvasElement>;

  private svc = inject(ReviewsService);

  reviews = signal<ReviewDto[]>([]);
  loading = signal(true);
  private chartsReady = false;
  private dataReady = false;

  // ── Computed analytics ──────────────────────────────────────────────────

  avgRating = computed(() => {
    const r = this.reviews();
    if (!r.length) return 0;
    return r.reduce((s, x) => s + x.rating, 0) / r.length;
  });

  positivePercent = computed(() => {
    const r = this.reviews();
    if (!r.length) return 0;
    const pos = r.filter(x => x.rating >= 4).length;
    return Math.round((pos / r.length) * 100);
  });

  staffSummaries = computed<StaffRatingSummary[]>(() => {
    const map = new Map<string, { sum: number; count: number }>();
    this.reviews().forEach(r => {
      const cur = map.get(r.revieweeId) ?? { sum: 0, count: 0 };
      map.set(r.revieweeId, { sum: cur.sum + r.rating, count: cur.count + 1 });
    });
    return Array.from(map.entries())
      .map(([id, v]) => ({
        staffId: id,
        staffName: id,
        averageRating: v.sum / v.count,
        totalReviews: v.count,
      }))
      .sort((a, b) => b.averageRating - a.averageRating);
  });

  topPerformers = computed(() => this.staffSummaries().filter(s => s.averageRating >= 4).slice(0, 5));
  redFlags = computed(() => this.staffSummaries().filter(s => s.averageRating < 2.5));
  redFlagCount = computed(() => this.redFlags().length);
  staffBarHeight = computed(() => Math.max(200, this.staffSummaries().length * 40 + 80));

  // ── Lifecycle ────────────────────────────────────────────────────────────

  ngOnInit() {
    this.svc.getAll().subscribe({
      next: data => {
        this.reviews.set(data);
        this.loading.set(false);
        this.dataReady = true;
        if (this.chartsReady) this.buildCharts();
      },
      error: () => this.loading.set(false)
    });
  }

  ngAfterViewInit() {
    this.chartsReady = true;
    if (this.dataReady) this.buildCharts();
  }

  // ── Chart builders ───────────────────────────────────────────────────────

  private buildCharts() {
    setTimeout(() => {
      this.buildTrendChart();
      this.buildSentimentChart();
      this.buildStaffChart();
    }, 0);
  }

  private buildTrendChart() {
    const monthly = this.monthlyAvg();
    new Chart(this.trendRef.nativeElement, {
      type: 'line',
      data: {
        labels: monthly.map(m => m.label),
        datasets: [{
          label: 'Avg rating',
          data: monthly.map(m => +m.avg.toFixed(2)),
          borderColor: '#0d6efd',
          backgroundColor: 'rgba(13,110,253,0.08)',
          fill: true,
          tension: 0.4,
          pointRadius: 4,
          borderWidth: 2
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
          y: { min: 1, max: 5, ticks: { font: { size: 11 } } },
          x: { ticks: { font: { size: 11 } }, grid: { display: false } }
        }
      }
    });
  }

  private buildSentimentChart() {
    const r = this.reviews();
    const pos = r.filter(x => x.rating >= 4).length;
    const neu = r.filter(x => x.rating === 3).length;
    const neg = r.filter(x => x.rating <= 2).length;

    new Chart(this.sentimentRef.nativeElement, {
      type: 'doughnut',
      data: {
        labels: ['Positive (4-5★)', 'Neutral (3★)', 'Negative (1-2★)'],
        datasets: [{
          data: [pos, neu, neg],
          backgroundColor: ['#198754', '#6c757d', '#dc3545'],
          borderWidth: 0,
          hoverOffset: 4
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '65%',
        plugins: {
          legend: {
            position: 'bottom',
            labels: { font: { size: 11 }, boxWidth: 10 }
          }
        }
      }
    });
  }

  private buildStaffChart() {
    const staff = this.staffSummaries();
    const colors = staff.map(s =>
      s.averageRating >= 4 ? '#198754' :
      s.averageRating >= 2.5 ? '#ffc107' : '#dc3545'
    );

    new Chart(this.staffRef.nativeElement, {
      type: 'bar',
      data: {
        labels: staff.map(s => s.staffId.slice(0, 10) + '…'),
        datasets: [{
          label: 'Avg rating',
          data: staff.map(s => +s.averageRating.toFixed(2)),
          backgroundColor: colors,
          borderWidth: 0,
          borderRadius: 4
        }]
      },
      options: {
        indexAxis: 'y',
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
          x: { min: 0, max: 5, ticks: { font: { size: 11 } } },
          y: { ticks: { font: { size: 11 }, autoSkip: false } }
        }
      }
    });
  }

  // ── Helpers ──────────────────────────────────────────────────────────────

  private monthlyAvg(): { label: string; avg: number }[] {
    const map = new Map<string, number[]>();
    this.reviews().forEach(r => {
      const d = new Date(r.createdAt);
      const key = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}`;
      const cur = map.get(key) ?? [];
      cur.push(r.rating);
      map.set(key, cur);
    });
    return Array.from(map.entries())
      .sort(([a], [b]) => a.localeCompare(b))
      .slice(-6)
      .map(([key, ratings]) => ({
        label: new Date(key + '-01').toLocaleString('default', { month: 'short', year: '2-digit' }),
        avg: ratings.reduce((s, v) => s + v, 0) / ratings.length
      }));
  }

  initials(id: string): string {
    return id.slice(0, 2).toUpperCase();
  }
}

