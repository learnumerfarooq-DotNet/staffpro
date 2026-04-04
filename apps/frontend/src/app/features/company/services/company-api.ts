import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CompanyDto, CreateCompanyDto } from '../models/company';

const API_BASE = '/api/companies';

@Injectable({
  providedIn: 'root',
})
export class CompanyApi {
  private readonly http = inject(HttpClient);

  /**
   * Get a company by ID
   */
  getById(companyId: string): Observable<CompanyDto> {
    return this.http.get<CompanyDto>(`${API_BASE}/${companyId}`);
  }

  /**
   * Create a new company
   */
  create(payload: CreateCompanyDto): Observable<CompanyDto> {
    return this.http.post<CompanyDto>(API_BASE, payload);
  }

  /**
   * Update an existing company
   */
  update(companyId: string, payload: Partial<CreateCompanyDto>): Observable<CompanyDto> {
    return this.http.put<CompanyDto>(`${API_BASE}/${companyId}`, payload);
  }

  /**
   * Delete a company
   */
  delete(companyId: string): Observable<void> {
    return this.http.delete<void>(`${API_BASE}/${companyId}`);
  }

  /**
   * Get all companies
   */
  getAll(): Observable<CompanyDto[]> {
    return this.http.get<CompanyDto[]>(API_BASE);
  }
}
