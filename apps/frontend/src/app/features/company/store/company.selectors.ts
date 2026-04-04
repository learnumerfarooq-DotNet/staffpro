// ─────────────────────────────────────────────────────────────────────────
// company.selectors.ts — NgRx Selectors
//
// Selectors are functions that READ from the NgRx store.
// They are memoized — they only recompute when their input changes.
// ─────────────────────────────────────────────────────────────────────────

import { createFeatureSelector, createSelector } from '@ngrx/store';
import { CompanyState } from './company.reducer';

// ── Feature selector — selects the 'company' slice of the root state
const selectCompanyFeature = createFeatureSelector<CompanyState>('company');

// ── Derived selectors
export const selectCompany = createSelector(selectCompanyFeature, (state) => state.company);

export const selectIsLoading = createSelector(selectCompanyFeature, (state) => state.loading);

export const selectError = createSelector(selectCompanyFeature, (state) => state.error);

export const selectWizardStep = createSelector(selectCompanyFeature, (state) => state.wizardStep);

export const selectWizardDraft = createSelector(selectCompanyFeature, (state) => state.wizardDraft);

export const selectIsSetupComplete = createSelector(
  selectCompany,
  (company) => company?.isSetupComplete ?? false,
);
