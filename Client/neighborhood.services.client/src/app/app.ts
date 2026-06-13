import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ChatWidgetComponent } from './shared/components/chat-widget/chat-widget.component';
import { TranslateService } from '@ngx-translate/core';
import { MyTranslateService } from './core/services/my-translate.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ChatWidgetComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('neighborhood.services.client');

  private readonly myTranslateService = inject(MyTranslateService);
}
