# InsERT.CurrencyApp

**CurrencyApp** is a modular, microservice-based platform built with **.NET 8** for managing multi-currency wallets, executing secure transactions, and retrieving exchange rates from the **NBP Table B**.

---

## 🧱 Architecture

This solution follows a **domain-driven microservices architecture** with separate bounded contexts and clearly defined responsibilities.

### Microservices

| Service                                 | Responsibility                                                       |
| --------------------------------------- | -------------------------------------------------------------------- |
| `InsERT.CurrencyApp.WalletService`      | Manages wallets, balances, and applies transactions                  |
| `InsERT.CurrencyApp.CurrencyService`    | Fetches and stores exchange rates from NBP (Table B)                 |
| `InsERT.CurrencyApp.TransactionService` | Coordinates deposits, withdrawals, and currency conversions          |
| `InsERT.CurrencyApp.ReportingService`   | Handles reporting and integration with external systems (planned)    |
| `InsERT.CurrencyApp.ApiGateway`         | API Gateway for routing requests (planned: Ocelot or custom gateway) |

---

## 📌 Architecture Diagram

View the full architecture in Miro:[🔗 Architecture Diagram (Miro)](https://miro.com/welcomeonboard/RFNTVW50bFROSVVURjl5QTEwcVNOWEp6WUNxdGdoWUFVNE5yeU55ZlB4citHc0xGdDVuTFJZVUtHd1M0NVRLc3crRmd4NkJDWSszSGt4bzUwa0VyVHhDQXNpU25ROGdWZVJUL1R6bEFza2ttZU9NTjY0WldMODkwajhNcmRibUt3VHhHVHd5UWtSM1BidUtUYmxycDRnPT0hdjE=?share_link_id=348655454602)

---

## 🚀 Technologies Used

- .NET 8 (ASP.NET Core Web API)
- Docker + Docker Compose
- EF Core + PostgreSQL
- CQRS (custom dispatcher)
- REST API + Swagger UI
- Testcontainers for integration testing
- FluentValidation + Middleware
- Polly for resilient HTTP clients

---

## 💪 Projects Overview

| Project                  | Description                                                              |
| ------------------------ | ------------------------------------------------------------------------ |
| `CurrencyService`        | Retrieves and stores exchange rates from NBP Table B                     |
| `WalletService`          | Manages wallet creation, balances, and transaction application           |
| `TransactionService`     | Orchestrates deposit/withdraw/convert flows using CQRS                   |
| `ReportingService` (WIP) | Responsible for reporting and audit integration                          |
| `CurrencyService.Tests`  | Unit + integration tests with PostgreSQL Testcontainers                  |
| `Abstractions`           | Shared contracts (DTOs, interfaces, CQRS abstractions, validation, etc.) |

---

## 🚜 Run the Solution

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Start all services

```bash
docker compose up --build
```

---

## 📚 Development Mode

All services run in `ASPNETCORE_ENVIRONMENT=Development`, which enables:

- Swagger UI
- Detailed error pages
- Console logging

---

## 🔍 API Documentation (Swagger)

When running locally:

| Service              | Swagger URL                                                                          |
| -------------------- | ------------------------------------------------------------------------------------ |
| `CurrencyService`    | [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html) |
| `WalletService`      | [http://localhost:5001/swagger/index.html](http://localhost:5001/swagger/index.html) |
| `TransactionService` | [http://localhost:5002/swagger/index.html](http://localhost:5002/swagger/index.html) |

---

## 📃 Sample API Endpoints

- `GET /nbp/table-b/rates?date=2025-06-19&code=USD` – get exchange rate
- `GET /nbp/table-b/codes` – get list of supported currency codes
- `POST /api/transaction/deposit` – deposit funds into a wallet
- `POST /api/transaction/withdraw` – withdraw funds
- `POST /api/transaction/convert` – convert currency (e.g. USD → EUR)

---

## 🎓 Key Patterns & Concepts

| Pattern              | Usage Example                                       |
| -------------------- | --------------------------------------------------- |
| **DDD**              | Aggregates: `Wallet`, `Transaction`, `ExchangeRate` |
| **CQRS**             | Split between Commands/Queries with dispatchers     |
| **DI**               | `IServiceCollection` configuration per module       |
| **FluentValidation** | Validation per request DTO with middleware support  |
| **Testcontainers**   | Real PostgreSQL instance for integration tests      |
| **Hosted Services**  | Background sync of exchange rates                   |

---

## 🚀 Run CurrencyService Standalone

```bash
cd src/InsERT.CurrencyApp.CurrencyService
DOTNET_ENVIRONMENT=Development dotnet run
```

## 📊 Run Tests

```bash
cd src/InsERT.CurrencyApp.CurrencyService.Tests
dotnet test
```

---

## 📎 Configuration Notes

All URLs for dependent services are configured via `appsettings.json`:

```json
"Services": {
  "CurrencyServiceBaseUrl": "http://currency-service:5000",
  "WalletServiceBaseUrl": "http://wallet-service:5001"
}
```

---

