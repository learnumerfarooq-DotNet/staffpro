// ─────────────────────────────────────────────────────────────────────────
// app.config.ts — Application Configuration
//
// In Angular 21 (standalone), this file replaces the old AppModule.
// It registers all global providers: routing, HTTP, state, auth.
// ─────────────────────────────────────────────────────────────────────────

import { ApplicationConfig, isDevMode } from '@angular/core';
import { provideRouter, withComponentInputBinding, withViewTransitions } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';

import { routes } from './app.routes';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';
import { reducers } from './store';
import { CompanyEffects } from './features/company/store/company.effects';

export const appConfig: ApplicationConfig = {
  providers: [
    // ── Routing
    provideRouter(routes, withComponentInputBinding(), withViewTransitions()),

    // ── HTTP Client with JWT interceptor
    provideHttpClient(withFetch(), withInterceptors([jwtInterceptor])),

    // ── Angular Material Animations
    provideAnimationsAsync(),

    // ── PrimeNG Theme (replaces old /resources/ CSS imports)
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: false, // disable automatic dark mode switching
        },
      },
    }),

    // ── NgRx Store (state management)
    provideStore(reducers),

    // ── NgRx Effects (side effects like API calls)
    provideEffects([CompanyEffects]),

    // ── NgRx DevTools (Redux DevTools browser extension)
    provideStoreDevtools({
      maxAge: 25,
      logOnly: !isDevMode(),
      autoPause: true,
      trace: false,
      traceLimit: 75,
    }),
  ],
};
