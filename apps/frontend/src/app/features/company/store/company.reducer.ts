import { createReducer, on } from '@ngrx/store';
import * as CompanyActions from './company.actions';

export interface CompanyState {
  companies: any[];
  loading: boolean;
  error: any;
}

export const initialState: CompanyState = {
  companies: [],
  loading: false,
  error: null,
};

export const companyReducer = createReducer(
  initialState,
  on(CompanyActions.loadCompanies, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(CompanyActions.loadCompaniesSuccess, (state, { companies }) => ({
    ...state,
    companies,
    loading: false,
  })),
  on(CompanyActions.loadCompaniesFailure, (state, { error }) => ({
    ...state,
    error,
    loading: false,
  })),
);
