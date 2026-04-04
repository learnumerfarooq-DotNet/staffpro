// ─────────────────────────────────────────────────────────────────────────
// company.effects.ts — NgRx Effects
//
// Effects handle SIDE EFFECTS — things that happen outside the reducer:
//   - API calls
//   - Navigation after success
//   - LocalStorage writes
//
// Pattern: Listen for an action → do async work → dispatch success/failure
// ─────────────────────────────────────────────────────────────────────────

import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { map, catchError, switchMap, tap } from 'rxjs/operators';
import * as CompanyActions from './company.actions';
import { CompanyApi } from '../services/company-api';

@Injectable()
export class CompanyEffects {
  private readonly actions$ = inject(Actions);
  private readonly companyApi = inject(CompanyApi);
  private readonly router = inject(Router);

  // ── Load Company Effect
  loadCompany$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CompanyActions.loadCompany),
      switchMap(({ companyId }) =>
        this.companyApi.getById(companyId).pipe(
          map((company) => CompanyActions.loadCompanySuccess({ company })),
          catchError((error) => of(CompanyActions.loadCompanyFailure({ error: error.message }))),
        ),
      ),
    ),
  );

  // ── Create Company Effect
  createCompany$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CompanyActions.createCompany),
      switchMap(({ payload }) =>
        this.companyApi.create(payload).pipe(
          map((company) => CompanyActions.createCompanySuccess({ company })),
          catchError((error) => of(CompanyActions.createCompanyFailure({ error: error.message }))),
        ),
      ),
    ),
  );

  // ── Navigate to dashboard after company creation
  createCompanySuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(CompanyActions.createCompanySuccess),
        tap(() => this.router.navigate(['/dashboard'])),
      ),
    { dispatch: false },
  );
}
