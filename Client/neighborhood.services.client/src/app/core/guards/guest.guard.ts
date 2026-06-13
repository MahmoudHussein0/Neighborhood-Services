import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { AuthService } from '../../features/auth/services/auth.service';

/**
 * Blocks the login/register pages for already-authenticated users, sending them
 * to their role's dashboard instead. The inverse of authGuard.
 */
export const guestGuard: CanActivateFn = (): boolean | UrlTree => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    const role = authService.currentUser()?.role ?? '';
    return router.createUrlTree([authService.getRedirectUrlForRole(role)]);
  }

  return true;
};
