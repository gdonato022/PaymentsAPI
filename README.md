# Plooto.Payments.API - Clean Architecture Web API

Plooto.Payments.API is a web API designed using the Clean Architecture principles. This API facilitates the management of payments and bills.

## Architecture Overview

The API is structured into the following layers:

1. **Domain Layer**: Contains entities representing the core business logic, interfaces for repositories to access data, and interfaces for domain events.
2. **Application Layer**: Implements use cases through commands and queries, allowing manipulation of domain entities.
3. **Infrastructure Layer**: Handles data storage using SQL Server and provides concrete implementations for repositories and domain events.
4. **Web API Layer**: Exposes endpoints through controllers, implements exception filters for error handling, and interacts with the application layer for use cases.
5. **Unit Tests Layer**: Contains unit tests for application use cases, ensuring the reliability and correctness of the codebase.

## Getting Started

To run the Plooto.Payments.API locally, ensure you have Docker Desktop installed. You can set up an instance of SQL Server by running the following command:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=2b4GhHErgA8Cv8gFt4rK" -p 1434:1433 --name sqlserver2022 -d mcr.microsoft.com/mssql/server:2022-latest
```

## Database Configuration

If Entity Framework CLI (EF CLI) is not installed, run this command to install it:
```bash
dotnet tool install --global dotnet-ef
```

Navigate to the directory of the API project and execute the following commands:
```bash
dotnet ef migrations add InitialCreate --verbose --project "{path of Plooto.Payments.Infrastructure.csproj}" --startup-project "{path of Plooto.Payments.API\Plooto.Payments.API.csproj}"
dotnet ef database update --verbose --project "{path of Plooto.Payments.Infrastructure.csproj}" --startup-project "{path of Plooto.Payments.API\Plooto.Payments.API.csproj}"
```
