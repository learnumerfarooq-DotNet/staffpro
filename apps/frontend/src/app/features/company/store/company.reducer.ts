// ─────────────────────────────────────────────────────────────────────────
// company.reducer.ts — NgRx Reducer
//
// The reducer is a PURE FUNCTION that takes:
//   - current state
//   - action (event)
// And returns the NEW state. It NEVER mutates the original state.
// ─────────────────────────────────────────────────────────────────────────

import { createReducer, on } from '@ngrx/store';
import * as CompanyActions from './company.actions';
import { CompanyDto, CreateCompanyDto } from '../models/company';

// ── State Shape
export interface CompanyState {
  company: CompanyDto | null; // Currently loaded company
  wizardStep: number; // Current wizard step (1-5)
  wizardDraft: Partial<CreateCompanyDto>; // Draft data saved between steps
  loading: boolean; // Is API call in progress?
  error: string | null; // Last error message
}

// ── Initial State
const initialState: CompanyState = {
  company: null,
  wizardStep: 1,
  wizardDraft: {},
  loading: false,
  error: null,
};

// ── Reducer
export const companyReducer = createReducer(
  initialState,

  // Load Company
  on(CompanyActions.loadCompany, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),

  on(CompanyActions.loadCompanySuccess, (state, { company }) => ({
    ...state,
    company,
    loading: false,
    error: null,
  })),

  on(CompanyActions.loadCompanyFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Create Company
  on(CompanyActions.createCompany, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),

  on(CompanyActions.createCompanySuccess, (state, { company }) => ({
    ...state,
    company,
    loading: false,
    error: null,
    wizardStep: 1,
    wizardDraft: {},
  })),

  on(CompanyActions.createCompanyFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Wizard
  on(CompanyActions.setWizardStep, (state, { step }) => ({
    ...state,
    wizardStep: step,
  })),

  on(CompanyActions.saveWizardDraft, (state, { draft }) => ({
    ...state,
    wizardDraft: { ...state.wizardDraft, ...draft },
  })),

  on(CompanyActions.clearCompanyState, () => initialState),
);
