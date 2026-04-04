// ─────────────────────────────────────────────────────────────────────────
// company.actions.ts — NgRx Actions
//
// Actions describe EVENTS that happen in the app:
//   - User loads a company
//   - API returns company data
//   - API returns an error
//
// Action naming convention: "[Source] Event Description"
// ─────────────────────────────────────────────────────────────────────────

import { createAction, props } from '@ngrx/store';
import { CompanyDto, CreateCompanyDto } from '../models/company';

// ─────────────────────────────────────────────────────────────────────────
// LOAD COMPANY
// ─────────────────────────────────────────────────────────────────────────

export const loadCompany = createAction('[Company] Load Company', props<{ companyId: string }>());

export const loadCompanySuccess = createAction(
  '[Company API] Load Company Success',
  props<{ company: CompanyDto }>(),
);

export const loadCompanyFailure = createAction(
  '[Company API] Load Company Failure',
  props<{ error: string }>(),
);

// ─────────────────────────────────────────────────────────────────────────
// CREATE COMPANY
// ─────────────────────────────────────────────────────────────────────────

export const createCompany = createAction(
  '[Company Wizard] Create Company',
  props<{ payload: CreateCompanyDto }>(),
);

export const createCompanySuccess = createAction(
  '[Company API] Create Company Success',
  props<{ company: CompanyDto }>(),
);

export const createCompanyFailure = createAction(
  '[Company API] Create Company Failure',
  props<{ error: string }>(),
);

// ─────────────────────────────────────────────────────────────────────────
// WIZARD ACTIONS
// ─────────────────────────────────────────────────────────────────────────

export const setWizardStep = createAction('[Company Wizard] Set Step', props<{ step: number }>());

export const saveWizardDraft = createAction(
  '[Company Wizard] Save Draft',
  props<{ draft: Partial<CreateCompanyDto> }>(),
);

export const clearCompanyState = createAction('[Company] Clear State');
