import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-customer-placeholder',
  imports: [],
  template: `
    <h2 class="h4 fw-bold mb-3">{{ title() }}</h2>
    <p class="text-muted mb-0">This customer page is ready for implementation.</p>
  `,
})
export class CustomerPlaceholderComponent {
  private readonly route = inject(ActivatedRoute);

  readonly title = computed(() => this.route.snapshot.data['title'] as string || 'Customer Page');
}
