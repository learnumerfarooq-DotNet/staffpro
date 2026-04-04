// ─────────────────────────────────────────────────────────────────────────
// api.service.ts — Base HTTP Service
//
// A reusable wrapper around Angular's HttpClient.
// All feature services extend or use this for API calls.
// ─────────────────────────────────────────────────────────────────────────

import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
}

@Injectable({
  providedIn: 'root',
})
export class Api {
  private readonly http = inject(HttpClient);

  // Base URLs for each microservice
  private readonly baseUrls = {
    company: `${environment.apiGateway}/api/v1/companies`,
    client: `${environment.apiGateway}/api/v1/clients`,
    employee: `${environment.apiGateway}/api/v1/employees`,
    candidate: `${environment.apiGateway}/api/v1/candidates`,
    jobs: `${environment.apiGateway}/api/v1/jobs`,
    careers: `${environment.apiGateway}/api/v1/careers`,
    dashboard: `${environment.apiGateway}/api/v1/dashboard`,
  };

  /** GET request */
  get<T>(
    serviceKey: keyof typeof this.baseUrls,
    path = '',
    params?: Record<string, string>,
  ): Observable<T> {
    const url = `${this.baseUrls[serviceKey]}${path}`;
    const httpParams = params ? new HttpParams({ fromObject: params }) : undefined;

    return this.http.get<T>(url, { params: httpParams }).pipe(catchError(this.handleError));
  }

  /** POST request */
  post<T>(serviceKey: keyof typeof this.baseUrls, body: unknown, path = ''): Observable<T> {
    const url = `${this.baseUrls[serviceKey]}${path}`;
    return this.http.post<T>(url, body).pipe(catchError(this.handleError));
  }

  /** PUT request */
  put<T>(serviceKey: keyof typeof this.baseUrls, id: string, body: unknown): Observable<T> {
    const url = `${this.baseUrls[serviceKey]}/${id}`;
    return this.http.put<T>(url, body).pipe(catchError(this.handleError));
  }

  /** DELETE request */
  delete<T>(serviceKey: keyof typeof this.baseUrls, id: string): Observable<T> {
    const url = `${this.baseUrls[serviceKey]}/${id}`;
    return this.http.delete<T>(url).pipe(catchError(this.handleError));
  }

  /** Global error handler */
  private handleError(error: unknown): Observable<never> {
    console.error('API Error:', error);
    return throwError(() => error);
  }
}
