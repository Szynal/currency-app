# currency-app

# CurrencyApp

**CurrencyApp** is a microservice-based system built with .NET 8 for managing currency wallets and executing transactions using exchange rates from the National Bank of Poland (NBP) Table B.

## 🧱 Architecture

The solution follows a modular microservices architecture, with domain-specific services and an API gateway.

### Microservices:

| Service                               | Description                                                       |
|---------------------------------------|-------------------------------------------------------------------|
| `InsERT.CurrencyApp.WalletService`    | Manages wallets and their balances                               |
| `InsERT.CurrencyApp.CurrencyService`  | Periodically fetches and stores exchange rates from NBP          |
| `InsERT.CurrencyApp.TransactionService` | Handles deposits, withdrawals, and currency conversions          |
| `InsERT.CurrencyApp.ReportingService` | Integrates with external systems to confirm transactions         |
| `InsERT.CurrencyApp.ApiGateway`       | API entry point (Ocelot or custom gateway)                       |

---

## 📌 Architecture Overview

You can find the current system architecture diagram on Miro:  
🔗 [View the Miro board](https://miro.com/welcomeonboard/RFNTVW50bFROSVVURjl5QTEwcVNOWEp6WUNxdGdoWUFVNE5yeU55ZlB4citHc0xGdDVuTFJZVUtHd1M0NVRLc3crRmd4NkJDWSszSGt4bzUwa0VyVHhDQXNpU25ROGdWZVJUL1R6bEFza2ttZU9NTjY0WldMODkwajhNcmRibUt3VHhHVHd5UWtSM1BidUtUYmxycDRnPT0hdjE=?share_link_id=348655454602)

---

## 🚀 Technologies

- .NET 8 (ASP.NET Core Web API)
- Docker & Docker Compose
- REST API with OpenAPI (Swagger)
- (Planned) EF Core + PostgreSQL, MediatR, Outbox Pattern
- (Optional) Azure Service Bus, Hangfire, Ocelot Gateway

---

## 🏁 Getting Started

### Prerequisites:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Run locally:

```bash
cd InsERT.CurrencyApp/src/InsERT.CurrencyApp.CurrencyService
dotnet run
```

---

# 💱 InsERT.CurrencyApp

**InsERT.CurrencyApp** is a modular microservice system for retrieving and exposing currency exchange rates from the National Bank of Poland (NBP – Table B). The project is designed following modern architectural principles such as **Domain-Driven Design (DDD)**, **CQRS**, **Dependency Injection**, and **containerized integration testing** with **Testcontainers**.

---

## 🧱 Projects

| Project                                   | Description                                                                 |
|-------------------------------------------|-----------------------------------------------------------------------------|
| `InsERT.CurrencyApp.CurrencyService`      | Microservice for fetching and storing currency exchange rates (NBP)        |
| `InsERT.CurrencyApp.CurrencyService.Tests`| Unit and integration tests (including PostgreSQL via Testcontainers)       |
| `InsERT.CurrencyApp.Abstractions`         | Shared contracts, DTOs, CQRS infrastructure and interfaces                 |

---

## 🔧 Required NuGet Packages

### 📦 `InsERT.CurrencyApp.CurrencyService`

| Package                                  | Purpose                                                                   |
|------------------------------------------|---------------------------------------------------------------------------|
| `Microsoft.Extensions.DependencyInjection` | Service registration with DI                                              |
| `Microsoft.Extensions.Hosting`           | Background services like `CurrencyRateFetcher`                            |
| `Microsoft.Extensions.Http.Polly`        | Resilient HTTP clients with retry policies                                |
| `Microsoft.AspNetCore.Mvc`               | Web API controllers and attributes                                        |
| `Microsoft.EntityFrameworkCore`          | Database access via EF Core                                               |
| `Npgsql.EntityFrameworkCore.PostgreSQL`  | PostgreSQL EF Core provider                                               |
| `Swashbuckle.AspNetCore`                 | Swagger/OpenAPI UI for API documentation                                  |

### 📦 `InsERT.CurrencyApp.CurrencyService.Tests`

| Package                          | Purpose                                                                   |
|----------------------------------|---------------------------------------------------------------------------|
| `xUnit`                          | Unit testing framework                                                    |
| `Moq`                            | Mocking interfaces like `IQueryDispatcher`                                |
| `Testcontainers.PostgreSql`      | Running test PostgreSQL database inside Docker containers                 |
| `Microsoft.EntityFrameworkCore.InMemory` | Lightweight EF Core provider for fast local logic testing                 |

---

## 🧰 Architectural Patterns

| Pattern / Practice                  | Applied In                                                                |
|-------------------------------------|----------------------------------------------------------------------------|
| **Domain-Driven Design (DDD)**      | Domain model (e.g., `ExchangeRate` entity), separation of concerns        |
| **CQRS**                            | Command/query separation with interfaces like `ICommandHandler`, `IQueryHandler` |
| **Dependency Injection (DI)**       | Constructor injection and modular service registration via `IServiceCollection` |
| **Testcontainers**                  | Integration testing with a real PostgreSQL container                       |
| **Options Pattern**                 | Strongly-typed configuration (e.g., `NbpClientSettings`)                   |
| **Hosted Services**                 | Background fetch jobs (e.g., `CurrencyRateFetcher`)                        |

---

## 🚀 Run the Service

```bash
cd InsERT.CurrencyApp/src/InsERT.CurrencyApp.CurrencyService
dotnet run
```

---

## 🧪 Run Tests

```bash
cd InsERT.CurrencyApp/src/InsERT.CurrencyApp.CurrencyService.Tests
dotnet test
```

---

## 📘 Sample Endpoints

- `GET /rates?date=2025-06-19&code=USD` — fetches exchange rates for a given day and currency code
- `GET /health/status` — application health status (returns 200/503)

---

## 🗺 Architecture Overview

System architecture is documented in Miro:

🔗 [View Architecture Diagram (Miro)](https://miro.com/welcomeonboard/RFNTVW50bFROSVVURjl5QTEwcVNOWEp6WUNxdGdoWUFVNE5yeU55ZlB4citHc0xGdDVuTFJZVUtHd1M0NVRLc3crRmd4NkJDWSszSGt4bzUwa0VyVHhDQXNpU25ROGdWZVJUL1R6bEFza2ttZU9NTjY0WldMODkwajhNcmRibUt3VHhHVHd5UWtSM1BidUtUYmxycDRnPT0hdjE=?share_link_id=348655454602)
