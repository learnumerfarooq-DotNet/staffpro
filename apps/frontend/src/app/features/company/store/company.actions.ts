import { createAction, props } from '@ngrx/store';

export const loadCompanies = createAction('[Company] Load Companies');
export const loadCompaniesSuccess = createAction(
  '[Company] Load Companies Success',
  props<{ companies: any[] }>(),
);
export const loadCompaniesFailure = createAction(
  '[Company] Load Companies Failure',
  props<{ error: any }>(),
);
