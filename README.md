# Hotel Management System

A microservices-based hotel management system built with .NET 8, implementing DDD patterns, CQRS, and the Saga pattern for distributed transactions.

## Architecture Overview

The system is composed of multiple microservices, each responsible for a specific business domain:

- **API Gateway**: YARP-based gateway that routes requests to appropriate services
- **Identity Service**: Handles authentication, authorization, and user management
- **Hotel Inventory Service**: Manages hotels, room types, and amenities
- **Availability & Pricing Service**: Handles room availability and dynamic pricing
- **Reservation Service**: Manages bookings and reservation workflow
- **Guest Service**: Handles guest profiles and preferences
- **Payment Service**: Processes payments and manages payment lifecycle
- **Billing Service**: Generates invoices and handles financial transactions
- **Check-In/Out Service**: Manages guest arrivals and departures
- **Housekeeping Service**: Tracks room status and cleaning schedules
- **Maintenance Service**: Handles maintenance requests and asset management
- **Notifications Service**: Sends emails, SMS, and other notifications
- **Loyalty Service**: Manages loyalty program points and rewards
- **Reporting Service**: Generates business analytics and reports
- **Search Service**: Provides full-text search capabilities

## Technologies

- **Backend**: .NET 8 (ASP.NET Core)
- **Data Access**: Dapper with Stored Procedures (PostgreSQL/SQL Server)
- **Message Bus**: Internal HTTP-based orchestration
- **Authentication**: JWT with refresh tokens
- **Monitoring**: OpenTelemetry, Jaeger, Seq
- **Containerization**: Docker, Kubernetes
- **CI/CD**: GitHub Actions

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker Desktop
- Visual Studio 2022 or VS Code

### Setup

1. Clone the repository:
   \`\`\`bash
   git clone <repository-url>
   cd hotel-management
   \`\`\`

2. Start infrastructure services:
   \`\`\`bash
   cd docker
   docker-compose up -d
   \`\`\`

3. Run database migrations:
   \`\`\`bash
   # For PostgreSQL (default)
   export DB_PROVIDER=postgres
   dotnet run --project tools/DbMigrator

   # For SQL Server
   export DB_PROVIDER=mssql
   dotnet run --project tools/DbMigrator
   \`\`\`

4. Start the services:
   \`\`\`bash
   docker-compose up -d
   \`\`\`

### Development

- API Gateway runs on http://localhost:5000
- Seq dashboard: http://localhost:5341
- Jaeger UI: http://localhost:16686

## Project Structure

\`\`\`
src/
  BuildingBlocks/           # Shared libraries
    Common/                 # Common utilities and base classes
    EventBus/              # Event bus abstractions
    Security/              # Authentication and authorization
    Resilience/           # Polly policies
    Observability/        # Logging and tracing

  Services/                # Microservices
    ApiGateway/           # YARP-based API Gateway
    Identity/             # User authentication and authorization
    HotelInventory/       # Hotel and room management
    AvailabilityPricing/  # Availability and pricing
    Reservation/          # Booking management
    Guest/                # Guest profiles
    Payment/              # Payment processing
    Billing/             # Invoice generation
    CheckInOut/          # Check-in/out management
    Housekeeping/        # Room status and cleaning
    Maintenance/         # Maintenance requests
    Notifications/       # Communication service
    Loyalty/             # Loyalty program
    Reporting/           # Analytics and reporting
    Search/              # Search functionality

tests/                    # Test projects
docker/                  # Docker compose files
k8s/                    # Kubernetes manifests
docs/                   # Documentation
tools/                  # Utilities and scripts
\`\`\`

## Testing

Run tests:
\`\`\`bash
dotnet test
\`\`\`

## Contributing

1. Create a feature branch
2. Make your changes
3. Submit a pull request

## License

[MIT License](LICENSE)
