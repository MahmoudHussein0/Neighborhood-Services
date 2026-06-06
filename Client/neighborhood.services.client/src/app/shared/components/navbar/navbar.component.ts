import { Component, inject } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { MyTranslateService } from '../../../core/services/my-translate.service';

@Component({
  selector: 'app-navbar',
  imports: [TranslatePipe],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {

  currentLang: string;

  constructor() {
    this.currentLang = this.translateService.getCurrentLang();
  }
  private readonly myTranslateService = inject(MyTranslateService)
  private readonly translateService = inject(TranslateService);





  ChangeLang(lang: string) {
    this.myTranslateService.ChangeLang(lang);
    this.currentLang = this.translateService.getCurrentLang();
  }




}
