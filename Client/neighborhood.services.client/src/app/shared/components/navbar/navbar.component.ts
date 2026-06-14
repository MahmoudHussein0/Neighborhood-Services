import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { MyTranslateService } from '../../../core/services/my-translate.service';
import { LangService } from '../../../core/services/lang.service';

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
}
