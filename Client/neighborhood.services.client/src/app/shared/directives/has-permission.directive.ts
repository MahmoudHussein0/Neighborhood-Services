import { Directive, Input, TemplateRef, ViewContainerRef, effect, inject, signal } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';

/**
 * Structural directive that renders its element only when the signed-in user holds the
 * given permission (or `FullAccess`). UX-only — the backend `[HasPermission]` attributes
 * are the real guard; this just hides things the user would get a 403 on anyway.
 *
 * Reacts to the permissions signal, so elements appear/disappear automatically once
 * GET /me resolves or the user logs out.
 *
 *   <a *appHasPermission="'ManageUsers'" routerLink="/staff/users">Users</a>
 */
@Directive({
  selector: '[appHasPermission]',
  standalone: true,
})
export class HasPermissionDirective {
  private readonly auth = inject(AuthService);
  private readonly templateRef = inject(TemplateRef<unknown>);
  private readonly viewContainer = inject(ViewContainerRef);

  private readonly required = signal<string>('');
  private hasView = false;

  @Input({ required: true }) set appHasPermission(permission: string) {
    this.required.set(permission);
  }

  constructor() {
    effect(() => {
      const allowed = this.auth.hasPermission(this.required());
      if (allowed && !this.hasView) {
        this.viewContainer.createEmbeddedView(this.templateRef);
        this.hasView = true;
      } else if (!allowed && this.hasView) {
        this.viewContainer.clear();
        this.hasView = false;
      }
    });
  }
}
