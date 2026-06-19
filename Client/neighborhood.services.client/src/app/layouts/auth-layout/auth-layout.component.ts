import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-auth-layout',
  imports: [RouterOutlet],
  template: `<router-outlet />`,
  styles: [`
    :host {
      display: block;
      width: 100%;
      min-height: 100vh;
    }
    :host ::ng-deep router-outlet + * {
      display: block;
      width: 100%;
    }
  `],
})
export class AuthLayoutComponent {}
