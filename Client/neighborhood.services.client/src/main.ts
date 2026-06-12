import { bootstrapApplication } from '@angular/platform-browser';
import { registerLocaleData } from '@angular/common';
import localeAr from '@angular/common/locales/ar';
import { appConfig } from './app/app.config';
import { App } from './app/app';

// Make Arabic number/date formatting available to DatePipe/CurrencyPipe
registerLocaleData(localeAr);

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
