// ─────────────────────────────────────────────────────────────────────────
// app.routes.ts — Application Routes
//
// Defines all navigation paths in the application.
// Uses LAZY LOADING — each feature module loads only when navigated to.
// This makes the initial app load much faster.
// ─────────────────────────────────────────────────────────────────────────

import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  // ── Default redirect
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full',
  },

  // ── Auth routes (no guard needed)
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login').then((m) => m.Login),
    title: 'Sign In — StaffPro',
  },

  // ── Protected routes (require authentication)
  {
    path: '',
    loadComponent: () => import('./layout/shell/shell').then((m) => m.Shell),
    canActivate: [authGuard],
    children: [
      // Dashboard
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard/dashboard').then((m) => m.Dashboard),
        title: 'Dashboard — StaffPro',
      },

      // Company Setup Wizard
      {
        path: 'company/setup',
        loadComponent: () => import('./features/company/wizard/wizard').then((m) => m.Wizard),
        title: 'Company Setup — StaffPro',
      },

      // Clients (Month 2)
      {
        path: 'clients',
        loadComponent: () =>
          import('./features/client/client-list/client-list').then((m) => m.ClientList),
        title: 'Clients — StaffPro',
      },

      // Employees (Month 2)
      {
        path: 'employees',
        loadComponent: () =>
          import('./features/employees/employee-list/employee-list').then((m) => m.EmployeeList),
        title: 'Employees — StaffPro',
      },

      // Jobs (Month 3)
      {
        path: 'jobs',
        loadComponent: () => import('./features/jobs/job-list/job-list').then((m) => m.JobList),
        title: 'Jobs — StaffPro',
      },

      // Candidates (Month 3)
      {
        path: 'candidates',
        loadComponent: () =>
          import('./features/candidates/candidate-list/candidate-list').then(
            (m) => m.CandidateList,
          ),
        title: 'Candidates — StaffPro',
      },
    ],
  },

  // ── Fallback
  {
    path: '**',
    redirectTo: 'dashboard',
  },
];
