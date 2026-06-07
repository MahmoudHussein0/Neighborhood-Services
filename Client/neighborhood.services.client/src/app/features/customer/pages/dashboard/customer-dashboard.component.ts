import { Component } from '@angular/core';

@Component({
  selector: 'app-customer-dashboard',
  imports: [],
  template: `
    <h2 class="h4 fw-bold mb-3">Welcome back, Ahmed 👋</h2>
    <p class="text-muted">This is a placeholder overview page — just here to preview the layout.</p>

    <div class="row g-3">
      <div class="col-6 col-lg-3">
        <div class="card shadow-sm border-0 rounded-3">
          <div class="card-body">
            <div class="text-muted small">Active Bookings</div>
            <div class="h4 fw-bold mb-0">2</div>
          </div>
        </div>
      </div>
      <div class="col-6 col-lg-3">
        <div class="card shadow-sm border-0 rounded-3">
          <div class="card-body">
            <div class="text-muted small">Open Requests</div>
            <div class="h4 fw-bold mb-0">1</div>
          </div>
        </div>
      </div>
      <div class="col-6 col-lg-3">
        <div class="card shadow-sm border-0 rounded-3">
          <div class="card-body">
            <div class="text-muted small">Wallet</div>
            <div class="h4 fw-bold mb-0">EGP 1,200</div>
          </div>
        </div>
      </div>
      <div class="col-6 col-lg-3">
        <div class="card shadow-sm border-0 rounded-3">
          <div class="card-body">
            <div class="text-muted small">Avg Rating</div>
            <div class="h4 fw-bold mb-0">4.8</div>
          </div>
        </div>
      </div>
    </div>
  `,
})
export class CustomerDashboardComponent {}
