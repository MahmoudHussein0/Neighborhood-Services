import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { AuthService } from '../../features/auth/services/auth.service';

export const roleGuard: CanActivateFn = (route): boolean | UrlTree => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const allowedRoles = route.data['roles'] as string[] | undefined;

  if (!allowedRoles?.length) {
    return true;
  }

  if (authService.hasAnyRole(allowedRoles)) {
    return true;
  }

  const currentUser = authService.currentUser();

  if (currentUser) {
    return router.createUrlTree([authService.getRedirectUrlForRole(currentUser.role)]);
  }

  return router.createUrlTree(['/auth/login']);
};
