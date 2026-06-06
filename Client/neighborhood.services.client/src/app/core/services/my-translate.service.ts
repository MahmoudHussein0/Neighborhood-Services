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
    if (localStorage.getItem('lang') == 'en') {
      this.renderer.setAttribute(document.documentElement, 'dir', 'ltr')
      this.renderer.setAttribute(document.documentElement, 'lang', 'en')



    }

    else if (localStorage.getItem('lang') == 'ar') {
      this.renderer.setAttribute(document.documentElement, 'dir', 'rtl')
      this.renderer.setAttribute(document.documentElement, 'lang', 'ar')
    }
  }


  ChangeLang(lang: string) {
    localStorage.setItem('lang', lang);
    var savedLang = localStorage.getItem('lang');
    if (savedLang)
      this.translateService.use(savedLang);
    this.ChangeDirection();


  }



}
