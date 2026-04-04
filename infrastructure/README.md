# StaffPro Local Infrastructure

## Quick Start

```bash
# Navigate to the project root first
cd C:\Projects\staffpro

# Start all services (runs in background)
docker-compose -f infrastructure/docker-compose.yml up -d

# Stop all services (data is preserved)
docker-compose -f infrastructure/docker-compose.yml down

# ⚠️ DANGER: Stop and DELETE all data (fresh start)
docker-compose -f infrastructure/docker-compose.yml down -v
```

## Services

| Service         | Container              | Port                    | Credentials             |
| --------------- | ---------------------- | ----------------------- | ----------------------- |
| SQL Server 2022 | staffpro-sqlserver     | 1433                    | sa / StaffPro@Dev2024!  |
| MongoDB 7       | staffpro-mongodb       | 27017                   | admin / MongoDB@Dev2024 |
| RabbitMQ 3      | staffpro-rabbitmq      | 5672 (AMQP), 15672 (UI) | guest / guest           |
| Elasticsearch 8 | staffpro-elasticsearch | 9200                    | No auth (dev only)      |

## Connection Strings for Your .NET Apps

### SQL Server (CompanyService, ClientService, EmployeeService)

Server=localhost,1433;Database=StaffPro_Company;User Id=sa;Password=StaffPro@Dev2024!;TrustServerCertificate=True

### MongoDB (CandidateService)

mongodb://admin:MongoDB@Dev2024@localhost:27017/staffpro_candidates?authSource=admin

### RabbitMQ (All services)

amqp://guest:guest@localhost:5672/staffpro

### Elasticsearch (JobsService)

http://localhost:9200

## Web UIs (open in browser)

- **RabbitMQ Dashboard**: http://localhost:15672 — login: guest / guest
- **Kibana (Elasticsearch UI)**: Run `docker-compose --profile tools up -d` first → http://localhost:5601
