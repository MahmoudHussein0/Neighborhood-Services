import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { SafeAuthUser } from '../../models/auth.models';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-external-callback',
  imports: [RouterLink],
  template: `
    <section class="auth-page">
      <div class="container py-5">
        <div class="row justify-content-center">
          <div class="col-12 col-md-8 col-lg-5">
            <div class="auth-panel bg-white border shadow-sm text-center">
              @if (error()) {
                <h1 class="h4 fw-bold mb-2">Google sign-in failed</h1>
                <p class="text-muted mb-4">{{ error() }}</p>
                <a class="btn btn-primary" routerLink="/auth/login">Back to sign in</a>
              } @else {
                <span class="spinner-border text-primary mb-3" aria-hidden="true"></span>
                <h1 class="h4 fw-bold mb-2">Signing you in</h1>
                <p class="text-muted mb-0">Please wait while we finish Google sign-in.</p>
              }
            </div>
          </div>
        </div>
      </div>
    </section>
  `,
  styles: `
    .auth-page {
      background-color: #f9fafb;
      min-height: 100vh;
    }

    .auth-panel {
      border-radius: 0.5rem;
      padding: 2rem;
    }
  `,
})
export class ExternalCallbackComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    const params = this.route.snapshot.queryParamMap;
    const user: SafeAuthUser = {
      userId: params.get('userId') ?? '',
      fullName: params.get('fullName') ?? '',
      email: params.get('email') ?? '',
      photo: params.get('photo') ?? '',
      role: params.get('role') ?? '',
      expiresAt: params.get('expiresAt') ?? '',
    };

    if (!user.userId || !user.email || !user.role || !user.expiresAt) {
      this.error.set('Google did not return enough account information.');
      return;
    }

    this.authService.completeExternalLogin(user);
    this.router.navigateByUrl(this.authService.getRedirectUrlForRole(user.role));
  }
}
