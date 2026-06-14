import { LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeAr from '@angular/common/locales/ar';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter, withHashLocation, withInMemoryScrolling, withViewTransitions } from '@angular/router';
import { provideToastr } from 'ngx-toastr';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { langInterceptor } from './core/interceptors/lang.interceptor';

registerLocaleData(localeAr)

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withInMemoryScrolling({ scrollPositionRestoration: 'top' }), withViewTransitions(), withHashLocation()),
    provideHttpClient(withFetch(), withInterceptors([errorInterceptor, authInterceptor, langInterceptor])),
    provideAnimations(),
    provideToastr(),
    // Locale for DatePipe/CurrencyPipe, taken from the saved language (applies on load/refresh)
    { provide: LOCALE_ID, useFactory: () => localStorage.getItem('lang') ?? 'en' },
    provideTranslateService({
      fallbackLang: 'en',
      loader: provideTranslateHttpLoader({
        prefix: './i18n/',
        suffix: '.json'
      })
    }),



  ]
};
