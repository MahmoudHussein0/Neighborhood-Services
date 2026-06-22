import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { filter, map } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

import { ChatWidgetComponent } from './shared/components/chat-widget/chat-widget.component';
import { CustomerSupportWidgetComponent } from './features/public/components/customer-support-widget.component/customer-support-widget.component';

import { MyTranslateService } from './core/services/my-translate.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    ChatWidgetComponent,
    CustomerSupportWidgetComponent
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('neighborhood.services.client');

  private readonly myTranslateService = inject(MyTranslateService);
  private readonly router = inject(Router);

  private readonly url = toSignal(
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      map(e => e.urlAfterRedirects)
    ),
    { initialValue: this.router.url }
  );

  readonly showSupportWidget = computed(() =>
    !this.url().startsWith('/staff')
  );
}