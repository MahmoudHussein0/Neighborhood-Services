import { Injectable, inject, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';

/**
 * Shared dashboard-layout state. Lets the topbar hamburger (in dashboard-shell)
 * and the projected role sidebars coordinate the mobile off-canvas drawer.
 * Desktop is unaffected — the `mobile-open` class only does anything under the
 * responsive breakpoint in styles.css.
 */
@Injectable({ providedIn: 'root' })
export class LayoutService {
  private readonly router = inject(Router);

  /** Whether the mobile drawer is currently open. */
  readonly mobileOpen = signal(false);

  constructor() {
    // Auto-close the drawer after navigating, so tapping a link dismisses it.
    this.router.events
      .pipe(filter(e => e instanceof NavigationEnd))
      .subscribe(() => this.mobileOpen.set(false));
  }

  toggleMobile() {
    this.mobileOpen.update(v => !v);
  }

  closeMobile() {
    this.mobileOpen.set(false);
  }
}
