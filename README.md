# StaffPro HR Management System

A modern, enterprise-grade HR management system built with a microservices architecture.

## 🏗️ Tech Stack

| Layer         | Technology                                  |
| ------------- | ------------------------------------------- |
| Frontend      | Angular 21 + NgRx + Angular Material        |
| Backend       | .NET 10 (C# 14) — 8 Microservices           |
| Primary DB    | SQL Server 2022 (via Entity Framework Core) |
| Document DB   | MongoDB 7                                   |
| Message Queue | RabbitMQ 3                                  |
| Search        | Elasticsearch 8                             |
| Auth          | Azure AD B2C / MSAL                         |
| DevOps        | Docker, GitHub Actions, Azure               |

## 📦 Project Structure

staffpro/
├── apps/
│ ├── frontend/ ← Angular 21 SPA
│ └── services/
│ ├── CompanyService/ ← Company management
│ ├── ClientService/ ← Client management
│ ├── EmployeeService/ ← Employee management
│ ├── CandidateService/ ← Recruitment
│ ├── JobsService/ ← Job listings
│ ├── CareersService/ ← Career paths
│ ├── DashboardService/ ← Analytics
│ └── NotificationService ← Alerts & emails
├── libs/
│ ├── shared-contracts/ ← Shared DTOs & events
│ └── api-clients/ ← Auto-generated API clients
└── infrastructure/
└── docker-compose.yml ← Local dev environment

## 🚀 Quick Start

### Prerequisites

- .NET 10 SDK
- Node.js 20 LTS
- Docker Desktop
- Git

### Run Locally

```bash
# 1. Clone the repo
git clone https://github.com/yourname/staffpro.git
cd staffpro

# 2. Start databases
docker-compose -f infrastructure/docker-compose.yml up -d

# 3. Install Node dependencies
npm install

# 4. Run the frontend
npm start
# → Opens http://localhost:4200

# 5. Run a specific service
cd apps/services/CompanyService/CompanyService.API
dotnet run
# → API available at http://localhost:5001
```

## 📅 Development Roadmap

| Month   | Focus                                      |
| ------- | ------------------------------------------ |
| Month 1 | Foundation, Auth, Company Setup            |
| Month 2 | Clients & Employees                        |
| Month 3 | Recruitment (Jobs, Candidates, Careers)    |
| Month 4 | Dashboard, Notifications, Azure Deployment |

## 🤝 Contributing

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Commit changes: `git commit -m "feat: add your feature"`
3. Push: `git push origin feature/your-feature`
4. Open a Pull Request

## 📜 License

MIT License — see [LICENSE](LICENSE) for details.
