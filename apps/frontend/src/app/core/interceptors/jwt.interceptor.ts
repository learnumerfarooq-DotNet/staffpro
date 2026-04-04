// ─────────────────────────────────────────────────────────────────────────
// jwt.interceptor.ts — HTTP Interceptor
//
// Automatically attaches the JWT access token to EVERY outgoing HTTP request.
// This means you don't need to manually add "Authorization" headers
// in every API call — the interceptor does it globally.
// ─────────────────────────────────────────────────────────────────────────

import { HttpInterceptorFn, HttpRequest, HttpHandlerFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Auth } from '../auth/auth';

export const jwtInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
) => {
  const authService = inject(Auth);
  const token = authService.getAccessToken();

  // Skip if no token or if calling an external URL (not our APIs)
  if (!token || !req.url.includes('/api/')) {
    return next(req);
  }

  // Clone the request and add the Authorization header
  const authenticatedReq = req.clone({
    headers: req.headers.set('Authorization', `Bearer ${token}`),
  });

  return next(authenticatedReq);
};
