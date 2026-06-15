import { inject, Injectable, RendererFactory2 } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root',
})
export class MyTranslateService {

  private readonly renderer = inject(RendererFactory2).createRenderer(null, null);

  constructor(private translateService: TranslateService) {
    // 1- Set Default Language 
    this.translateService.setDefaultLang('en');
    // 2- Get SavedLang from LocalStorage 
    const savedLang = localStorage.getItem('lang');
    // 3- Use SavedLang In LocalStorage
    if (savedLang)
      this.translateService.use(savedLang);
    // 4- changeDirection 
    this.ChangeDirection();
  }


  ChangeDirection() {
    const isArabic = localStorage.getItem('lang') === 'ar';

    if (isArabic) {
      this.renderer.setAttribute(document.documentElement, 'dir', 'rtl');
      this.renderer.setAttribute(document.documentElement, 'lang', 'ar');
    } else {
      this.renderer.setAttribute(document.documentElement, 'dir', 'ltr');
      this.renderer.setAttribute(document.documentElement, 'lang', 'en');
    }

    this.toggleRtlStylesheet(isArabic);
  }

  /** Loads Bootstrap's RTL stylesheet only for Arabic (lazy 'bootstrap-rtl' bundle). */
  private toggleRtlStylesheet(enable: boolean) {
    const id = 'bootstrap-rtl-css';
    const existing = document.getElementById(id);

    if (enable && !existing) {
      const link = this.renderer.createElement('link');
      this.renderer.setAttribute(link, 'id', id);
      this.renderer.setAttribute(link, 'rel', 'stylesheet');
      this.renderer.setAttribute(link, 'href', 'bootstrap-rtl.css');
      this.renderer.appendChild(document.head, link);
    } else if (!enable && existing) {
      existing.remove();
    }
  }


  ChangeLang(lang: string) {
    localStorage.setItem('lang', lang);
    var savedLang = localStorage.getItem('lang');
    if (savedLang)
      this.translateService.use(savedLang);
    this.ChangeDirection();
    window.location.reload();
  }



}
