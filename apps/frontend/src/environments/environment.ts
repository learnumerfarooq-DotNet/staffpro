// ─────────────────────────────────────────────────────────────────────────
// environment.ts — Development Environment Configuration
//
// Contains API URLs, Azure AD config, and feature flags for DEVELOPMENT.
// NEVER commit real secrets here. Use environment variables in production.
// ─────────────────────────────────────────────────────────────────────────

export const environment = {
  production: false,

  // ── API Gateway URL (direct to services in development)
  apiGateway: 'http://localhost:5001',

  // ── Individual service URLs (for development without gateway)
  services: {
    company: 'http://localhost:5001',
    client: 'http://localhost:5002',
    employee: 'http://localhost:5003',
    candidate: 'http://localhost:5004',
    jobs: 'http://localhost:5005',
    careers: 'http://localhost:5006',
    dashboard: 'http://localhost:5007',
    notification: 'http://localhost:5008',
  },

  // ── Azure Active Directory B2C (filled in Week 2)
  azure: {
    clientId: 'YOUR_AZURE_AD_CLIENT_ID',
    tenantId: 'YOUR_AZURE_TENANT_ID',
    redirectUri: 'http://localhost:4200',
    scopes: ['openid', 'profile', 'email'],
    authority: 'https://login.microsoftonline.com/YOUR_TENANT_ID',
  },

  // ── Feature flags
  features: {
    enableDevTools: true,
    enableMockData: true, // Use mock data before backend is ready
    enableDebugLogs: true,
  },
};
