import { ActionReducerMap } from '@ngrx/store';
import { companyReducer, CompanyState } from '../features/company/store/company.reducer';

export interface AppState {
  company: CompanyState;
}

export const reducers: ActionReducerMap<AppState> = {
  company: companyReducer,
};
