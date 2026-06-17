import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router, UrlTree } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AuthService } from '../../features/auth/services/auth.service';

/**
 * Blocks a staff route unless the user holds the permission named in its route data
 * (`data: { permission: 'ManageCategories' }`). Routes without a `permission` are allowed.
 *
 * Awaits `ensurePermissions()` so a hard refresh / deep-link decides only after GET /me
 * has resolved (otherwise the check would race the fetch and wrongly reject everyone).
 * Lacking the permission redirects to the staff overview, which every staffer can see.
 */
export const permissionGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
): Observable<boolean | UrlTree> => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const required = route.data?.['permission'] as string | undefined;

  return auth.ensurePermissions().pipe(
    map(() => {
      if (!required || auth.hasPermission(required)) {
        return true;
      }
      return router.createUrlTree(['/staff']);
    }),
  );
};
