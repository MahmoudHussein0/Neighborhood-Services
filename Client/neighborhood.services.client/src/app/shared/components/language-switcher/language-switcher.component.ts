import { Component, inject, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { MyTranslateService } from '../../../core/services/my-translate.service';

@Component({
  selector: 'app-language-switcher',
  imports: [],
  template: `
    <button class="btn btn-sm btn-outline-secondary" type="button" (click)="toggle()">
      {{ currentLang() === 'en' ? 'AR' : 'EN' }}
    </button>
  `,
})
export class LanguageSwitcherComponent {
  private readonly myTranslate = inject(MyTranslateService);
  private readonly translate = inject(TranslateService);

  currentLang = signal(this.translate.getCurrentLang());

  toggle() {
    const next = this.currentLang() === 'en' ? 'ar' : 'en';
    this.myTranslate.ChangeLang(next);
    this.currentLang.set(this.translate.getCurrentLang());
  }
}
