import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { MyTranslateService } from '../../../core/services/my-translate.service';
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
  private readonly authService = inject(AuthService);

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
  }
}
