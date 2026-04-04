// TypeScript interfaces matching the backend DTOs
export interface CompanyDto {
  id: string;
  name: string;
  tradeName: string;
  industry: string;
  size: string;
  headOfficeAddress: AddressDto;
  contactEmail: string;
  contactPhone: string;
  website?: string;
  taxNumber: string;
  isSetupComplete: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface AddressDto {
  street: string;
  apartmentSuite?: string;
  city: string;
  state: string;
  postalCode: string;
  country: string;
}

export interface CreateCompanyDto {
  name: string;
  tradeName: string;
  industry: string;
  size: string;
  headOfficeAddress: AddressDto;
  contactEmail: string;
  contactPhone: string;
  taxNumber: string;
  website?: string;
}
