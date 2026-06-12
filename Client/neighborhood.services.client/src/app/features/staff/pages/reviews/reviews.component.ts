
import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReviewsTabComponent } from './reviewstab/reviewstab.component';
import { AnalysisTabComponent } from './analysistab/analysistab.component';
type Tab = 'reviews' | 'analysis';

@Component({
  selector: 'app-reviews',
  imports: [CommonModule, ReviewsTabComponent, AnalysisTabComponent],
  templateUrl: './reviews.component.html',
  styleUrl: './reviews.component.css',
})

export class ReviewsComponent {

  activeTab = signal<Tab>('reviews');
  setTab(t: Tab) { this.activeTab.set(t); }

}
