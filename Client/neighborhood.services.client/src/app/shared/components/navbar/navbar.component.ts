import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { MyTranslateService } from '../../../core/services/my-translate.service';
import { LangService } from '../../../core/services/lang.service';
import { AuthService } from '../../../features/auth/services/auth.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive, TranslatePipe],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {

  private readonly myTranslateService = inject(MyTranslateService);
  private readonly translateService = inject(TranslateService);
  private readonly langService = inject(LangService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly isLoggingOut = signal(false);

  /** Reactive logged-in user — drives the Login ↔ Dashboard button. */
  readonly user = this.authService.currentUser;

  /** Where the Dashboard button points, based on the user's role. */
  readonly dashboardLink = computed(() => {
    const u = this.user();
    return u ? this.authService.getRedirectUrlForRole(u.role) : '/auth/login';
  });

  currentLang: string;
  menuOpen = signal(false);

  constructor() {
    this.currentLang = this.translateService.getCurrentLang();
  }

  toggleMenu() {
    this.menuOpen.update(v => !v);
  }

  ChangeLang(lang: string) {
    this.myTranslateService.ChangeLang(lang);
    this.currentLang = this.translateService.getCurrentLang();
    this.langService.setLanguage(lang);
  }

  logout() {
    if (this.isLoggingOut()) {
      return;
    }

    this.isLoggingOut.set(true);
    this.menuOpen.set(false);

    this.authService.logout().subscribe({
      next: () => {
        this.isLoggingOut.set(false);
        this.router.navigate(['/']);
      },
      error: () => {
        this.isLoggingOut.set(false);
        this.router.navigate(['/']);
      },
    });
  }
}
