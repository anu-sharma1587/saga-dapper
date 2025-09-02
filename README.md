
# Hotel Management Microservices Solution

This solution is a modern, production-ready hotel management system built with .NET 8, Dapper, and PostgreSQL. It features a robust microservices architecture, distributed transactions via Saga orchestration, and advanced API Gateway resilience.

## Solution Highlights

- **Microservices**: Each business domain is implemented as an independent ASP.NET Core service.
- **Data Access**: All services use Dapper with PostgreSQL stored procedures. The shared `DataAccess` library provides `IDbConnectionFactory` and `IDataRepository` abstractions.
- **No EF Core**: Entity Framework Core has been fully removed. All data operations are via Dapper and stored procedures.
- **Saga Orchestration**: Distributed transactions are managed using the Saga pattern, with compensation logic for reliability.
- **API Gateway**: Built with YARP, featuring rate limiting (Microsoft.AspNetCore.RateLimiting) and circuit breaker (Polly) for resilience.
- **Authentication**: JWT-based authentication and refresh tokens.
- **Monitoring**: OpenTelemetry, Jaeger, and Seq for tracing and logging.
- **Containerization**: Docker and Kubernetes support for deployment.

## Microservices List

- ApiGateway
- Identity
- HotelInventory
- AvailabilityPricing
- Reservation
- Guest
- Payment
- Billing
- CheckInOut
- Housekeeping
- Maintenance
- Notifications
- Loyalty
- Reporting
- Search

## Technologies Used

- .NET 8 (ASP.NET Core)
- Dapper (ORM)
- PostgreSQL
- YARP (API Gateway)
- Polly (Circuit Breaker)
- Microsoft.AspNetCore.RateLimiting
- JWT Authentication
- OpenTelemetry, Jaeger, Seq
- Docker, Kubernetes

## Getting Started

### Prerequisites

- .NET 8 SDK (required)
- Docker Desktop
- Visual Studio 2022 or VS Code

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd hotel-management
   ```

2. **Start infrastructure services**
   ```bash
   cd docker
   docker-compose up -d
   ```

3. **Run database migrations**
   ```bash
   export DB_PROVIDER=postgres
   dotnet run --project tools/DbMigrator
   ```

4. **Start all microservices**
   ```bash
   docker-compose up -d
   ```

### Development Endpoints

- API Gateway: http://localhost:5000
- Seq dashboard: http://localhost:5341
- Jaeger UI: http://localhost:16686

## Project Structure

```
src/
  BuildingBlocks/
    Common/           # Utilities and base classes
    EventBus/         # Event bus abstractions
    Security/         # AuthN/AuthZ
    Resilience/       # Polly policies
    Observability/    # Logging and tracing
    DataAccess/       # Dapper, connection factory, repository

  Services/
    ApiGateway/
    Identity/
    HotelInventory/
    AvailabilityPricing/
    Reservation/
    Guest/
    Payment/
    Billing/
    CheckInOut/
    Housekeeping/
    Maintenance/
    Notifications/
    Loyalty/
    Reporting/
    Search/

tests/                # Test projects
docker/               # Docker compose files
k8s/                  # Kubernetes manifests
docs/                 # Documentation
tools/                # Utilities and scripts
```

## Testing

Run all tests:
```bash
dotnet test
```

## Troubleshooting

- **.NET SDK Version**: Ensure you have .NET 8 SDK installed. Run `dotnet --version` to check.
- **Build Errors**: If you see errors about missing types (e.g., `SqlParameter`, `SqlDbType`), ensure all code uses Dapper and PostgreSQL types only. Remove any SQL Server/EF Core remnants.
- **API Gateway Issues**: Confirm YARP, Polly, and rate limiting are configured in `Program.cs` and `appsettings.json`.

## Contributing

1. Create a feature branch
2. Make your changes
3. Submit a pull request

## License

[MIT License](LICENSE)
