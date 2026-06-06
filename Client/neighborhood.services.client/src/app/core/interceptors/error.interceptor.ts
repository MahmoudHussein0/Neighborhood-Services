import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toastr = inject(ToastrService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      switch (error.status) {
        case 401:
          router.navigate(['/auth/login']);
          break;
        case 403:
          toastr.error('You do not have permission to perform this action.');
          break;
        case 500:
          toastr.error('Server error, please try again later.');
          break;
        default:
          toastr.error(error.error?.message || 'Something went wrong.');
          break;
      }
      return throwError(() => error);
    })
  );
};
