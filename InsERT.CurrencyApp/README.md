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

