# currency-app

# CurrencyApp

**CurrencyApp** is a modular microservice-based system built with **.NET 8** for managing currency wallets and executing transactions using exchange rates from the National Bank of Poland (NBP, Table B).

---

## 🧱 Architecture

The solution follows a **domain-oriented microservices architecture** with bounded contexts and an API gateway.

### Microservices

| Service                                 | Responsibility                                                        |
|----------------------------------------|------------------------------------------------------------------------|
| `InsERT.CurrencyApp.WalletService`     | Manages wallets and their balances                                     |
| `InsERT.CurrencyApp.CurrencyService`   | Periodically fetches and stores exchange rates from NBP                |
| `InsERT.CurrencyApp.TransactionService`| Handles deposits, withdrawals, and currency conversions                |
| `InsERT.CurrencyApp.ReportingService`  | Integrates with external systems to confirm and report transactions    |
| `InsERT.CurrencyApp.ApiGateway`        | Central API entry point (planned: Ocelot or custom gateway)            |

---

## 📌 Architecture Overview

The current system architecture is available in Miro:  
🔗 [View Architecture Diagram (Miro)](https://miro.com/welcomeonboard/RFNTVW50bFROSVVURjl5QTEwcVNOWEp6WUNxdGdoWUFVNE5yeU55ZlB4citHc0xGdDVuTFJZVUtHd1M0NVRLc3crRmd4NkJDWSszSGt4bzUwa0VyVHhDQXNpU25ROGdWZVJUL1R6bEFza2ttZU9NTjY0WldMODkwajhNcmRibUt3VHhHVHd5UWtSM1BidUtUYmxycDRnPT0hdjE=?share_link_id=348655454602)

---

## 🚀 Technologies

- .NET 8 (ASP.NET Core Web API)
- Docker & Docker Compose
- EF Core + PostgreSQL
- CQRS (with MediatR or custom dispatcher)
- Polly for HTTP resilience
- REST API with Swagger
- Testcontainers for integration testing
- CI/CD with GitHub Actions (planned)

---

## 🏁 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Run the full solution with Docker:

```bash
docker compose up --build
```

---

## 🧪 Development Mode

All services currently run in `ASPNETCORE_ENVIRONMENT=Development` mode, which enables:

- Swagger UI
- Detailed error pages
- Dev diagnostics

---

## 📘 Swagger & API Testing

When services are running, Swagger UI is available at:

- [`CurrencyService`](http://localhost:5000/swagger/index.html)
- [`WalletService`](http://localhost:5001/swagger/index.html)
- (Other services coming soon)

---

# 💱 InsERT.CurrencyApp

**InsERT.CurrencyApp** is a domain-driven microservice ecosystem for retrieving and exposing currency exchange rates from NBP Table B, managing wallets, and performing secure transactions. The project emphasizes **Domain-Driven Design (DDD)**, **CQRS**, **Dependency Injection (DI)**, and **containerized testing** using **Testcontainers**.

---

## 🧱 Projects

| Project                                     | Description                                                                 |
|--------------------------------------------|-----------------------------------------------------------------------------|
| `InsERT.CurrencyApp.CurrencyService`       | Fetches and stores currency exchange rates                                 |
| `InsERT.CurrencyApp.WalletService`         | Stores wallet states and balances                                          |
| `InsERT.CurrencyApp.CurrencyService.Tests` | Unit and integration tests for currency logic                              |
| `InsERT.CurrencyApp.Abstractions`          | Shared CQRS infrastructure and DTO contracts                               |

---

## 🔧 Required NuGet Packages

### 📦 `InsERT.CurrencyApp.CurrencyService`

| Package                                    | Purpose                                                                  |
|--------------------------------------------|--------------------------------------------------------------------------|
| `Microsoft.EntityFrameworkCore`            | Database access                                                           |
| `Npgsql.EntityFrameworkCore.PostgreSQL`    | PostgreSQL EF Core provider                                               |
| `Microsoft.Extensions.Http.Polly`          | HTTP retry policies                                                       |
| `Swashbuckle.AspNetCore`                   | Swagger UI                                                                |

### 📦 `InsERT.CurrencyApp.CurrencyService.Tests`

| Package                          | Purpose                                                                   |
|----------------------------------|---------------------------------------------------------------------------|
| `xUnit`                          | Unit testing framework                                                    |
| `Moq`                            | Interface mocking                                                         |
| `Testcontainers.PostgreSql`      | Real PostgreSQL container testing                                         |
| `EF Core InMemory`               | Lightweight local EF Core testing                                         |

---

## 🧰 Architectural Patterns

| Pattern                     | Usage Example                                                              |
|----------------------------|-----------------------------------------------------------------------------|
| **DDD**                    | Entities: `Wallet`, `ExchangeRate`; strict boundaries per service           |
| **CQRS**                   | Queries vs Commands, with dedicated handlers and dispatchers                |
| **DI (Dependency Injection)**| Modular service registration via `IServiceCollection`                     |
| **Testcontainers**         | PostgreSQL containerized integration tests                                 |
| **Options Pattern**        | `AppSettings`, `NbpClientSettings`, strongly typed config                  |
| **Hosted Services**        | Background jobs like currency sync                                          |

---

## 🚀 Run CurrencyService Locally

```bash
cd src/InsERT.CurrencyApp.CurrencyService
dotnet run
```

---

## 🧪 Run Tests

```bash
cd src/InsERT.CurrencyApp.CurrencyService.Tests
dotnet test
```

---

## 📘 Sample API Endpoints

- `GET /rates?date=2025-06-19&code=USD` – exchange rates by date and code
- `GET /health/status` – system health (200 / 503)
- `GET /api/wallet/{userId}/balances` – retrieve wallet balances by user
- `POST /api/wallet/{userId}/wallets` – create a new wallet for a user

---

## 📌 Additional Notes

- Transaction confirmation logic is handled by `TransactionService` and `ReportingService` (WIP)
- The system is container-ready and supports Docker-based CI/CD pipelines

---
