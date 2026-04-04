// ─────────────────────────────────────────────────────────────────────────
// auth.service.ts — Authentication Service
//
// Wraps MSAL (Microsoft Authentication Library) for Azure AD B2C.
// Provides: login, logout, token retrieval, authentication state.
//
// NOTE: Azure AD B2C configuration is in environment.ts
// Full MSAL integration is completed in Month 1, Week 2.
// This is a STUB for now — just enough to not break the app.
// ─────────────────────────────────────────────────────────────────────────

import { Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';

export interface User {
  id: string;
  name: string;
  email: string;
  roles: string[];
}

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private readonly router = inject(Router);

  // Angular signals for reactive auth state (Angular 17+ feature)
  readonly currentUser = signal<User | null>(null);
  readonly isAuthenticated = signal<boolean>(false);

  private accessToken: string | null = null;

  constructor() {
    // For development: auto-login with mock user
    // TODO: Replace with MSAL in Week 2
    this.setMockUser();
  }

  /** Get the current JWT access token */
  getAccessToken(): string | null {
    return this.accessToken;
  }

  /** Initiate login flow (MSAL redirect) */
  async login(): Promise<void> {
    // TODO: Replace with MsalService.loginRedirect() in Week 2
    this.setMockUser();
    this.router.navigate(['/dashboard']);
  }

  /** Log out and clear session */
  async logout(): Promise<void> {
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
    this.accessToken = null;
    // TODO: Replace with MsalService.logoutRedirect() in Week 2
    this.router.navigate(['/login']);
  }

  /** Check if user has a specific role */
  hasRole(role: string): boolean {
    return this.currentUser()?.roles.includes(role) ?? false;
  }

  // ── Development helper — remove in production
  private setMockUser(): void {
    this.currentUser.set({
      id: 'dev-user-001',
      name: 'Dev User',
      email: 'dev@staffpro.com',
      roles: ['CompanyAdmin'],
    });
    this.isAuthenticated.set(true);
    this.accessToken = 'mock-jwt-token-for-development';
  }
}

// Needed for inject() outside constructor
import { inject } from '@angular/core';
