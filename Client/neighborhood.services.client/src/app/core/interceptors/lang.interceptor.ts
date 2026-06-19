import { HttpInterceptorFn } from '@angular/common/http';

export const langInterceptor: HttpInterceptorFn = (req, next) => {


  const currentLang = localStorage.getItem("lang") || "en";
  const endPointsNeedLang: string[] = [
    '/api/Categories',
    '/api/ProblemTypes',
    '/api/TechnicianPricing',
    '/api/TechnitianCategory'
  ];


  const isNeedLang: boolean = endPointsNeedLang.some(endPoint => req.url.includes(endPoint));

  if (isNeedLang) {
    req = req.clone({ params: req.params.set('lang', currentLang) });
  }


  return next(req);
};
