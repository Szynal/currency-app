# InsERT.CurrencyApp

**CurrencyApp** is a modular, microservice-based platform built with **.NET 8** for managing multi-currency wallets, executing secure transactions, and retrieving exchange rates from the **NBP Table B**.


## 1.  W jaki sposób (z użyciem jakich usług) aplikacja mogłaby być wdrożona w platformę Azure:

**Odpowiedz** 
Aplikacja została zbudowana w architekturze mikroserwisowej i już teraz działa w środowisku kontenerowym, co znacznie ułatwia wdrożenie jej do platformy Azure. Korzystam z niezależnych usług (m.in. wallet-service, transaction-service, currency-service) oraz trzech instancji baz danych PostgreSQL (dla każdego mikroserwisu osobno). Całość uruchamiana jest za pomocą kontenerów Docker.

Dodatkowo, system przetwarza transakcje w tle, korzystając z background service, który loguje liczbę oczekujących transakcji (np. Found {Count} pending transactions.), co pozwala na pełną kontrolę nad obciążeniem i przetwarzaniem danych.

W kontekście platformy Azure, aplikację można wdrożyć z wykorzystaniem następujących usług:

1. Compute (uruchamianie aplikacji)
Azure Kubernetes Service (AKS) – naturalny wybór przy gotowej architekturze kontenerowej. Umożliwia zarządzanie skalowalnością, wdrażaniem i siecią pomiędzy mikroserwisami.

2. Bazy danych i przechowywanie
Azure Database for PostgreSQL – Flexible Server – jako bezpieczne i skalowalne źródło danych dla każdego mikroserwisu.

3. Integracja i komunikacja
Azure Service Bus – do asynchronicznej komunikacji między mikroserwisami oraz integracji z systemami zewnętrznymi (np. potwierdzenia transakcji).
Azure Logic Apps – do reakcji na zdarzenia, np. przetworzenie potwierdzenia zewnętrznego dla transakcji oczekujących na akceptację.

4. Monitorowanie i niezawodność
Azure Application Insights – do analizy działania usług, diagnostyki błędów i monitorowania opóźnień np. w przetwarzaniu transakcji.
Azure Monitor + Log Analytics – do zbierania danych operacyjnych z całej aplikacji (logi, metryki, alerty).

5. CI/CD
Azure DevOps Pipelines lub GitHub Actions – do zautomatyzowanego procesu budowania, testowania i wdrażania kontenerów do środowiska produkcyjnego.

6. Zarządzanie konfiguracją i bezpieczeństwem
Azure Key Vault – do bezpiecznego przechowywania poufnych danych, takich jak connection stringi i tokeny API.

Azure API Management – do zarządzania dostępem do API, dokumentacją, throttlingiem i wersjonowaniem.

Podsumowując – aplikacja jest gotowa do wdrożenia w chmurze, a przejście do Azure (szczególnie AKS) wymaga głównie skonfigurowania infrastruktury jako kodu oraz automatyzacji procesu deploymentu.

## 2 . W jaki sposób zapewnić wydajne działanie aplikacji jeśli transakcji wykonywanych będzie bardzo dużo.


**Odpowiedz** 
1. Asynchroniczna komunikacja między TransactionService i WalletService (kolejki) oraz Batching i paginacja 
Zamiast przetwarzać wszystkie oczekujące transakcje naraz:
Pobieramy w partiach (np. Take(n)): np n =100/
.OrderBy(t => t.CreatedAt) .Take(limit)
Zapisujemy zmiany po każdej partii (SaveChangesAsync).

2. Lockowanie i zapobieganie podwójnemu przetwarzaniu (must have)
Możemy mieć wiele instancji aplikacji które,  mogą próbować przetwarzać te same dane.
Rozwiązania:
FOR UPDATE SKIP LOCKED w PostgreSQL (lub równoważne w MSSQL)
Kolumna IsProcessing albo LockedAt
Zmienny Status (np. Pending → Processing → Done) z datami (To już zaimplementowałem z InMemoryEventBus) 
 3. Kolejkowanie zamiast pollingu 
Obecnie dałem Polling co 10–30s a on nie skaluje się dobrze.Celem jests mieć tzw. event-driven:
Azure Service Bus
Worker przetwarza tylko to, co przyjdzie
 4. Oddzielanie zapisu i przetwarzania (Command Queue / Outbox pattern)
Każda transakcja zapisywana do DB → trafia do tabeli Outbox
Worker czyta z Outbox, publikuje komunikat, zmienia status
Gwarantuje atomiczność, odporność na awarie, retry
5. Optymalizacja bazy danych (proste a skuteczne)
Doadnie Indeksów na:
Status
CreatedAt
WalletId
Enum jako smallint
...
6. Ograniczamy zbytnie logowanie i monitoring
Logi per transakcja. do
dodatkowe Logi per batch? Tak, np. "10 przetworzono, 2 odrzucone" (do przemyślenia)
7. Skalowalność pozioma
Chcemy umożliwć:
uruchamianie wielu instancji workerów
każda instancja przetwarza inny zestaw transakcji (Da nam ogromny boost)
8. Retry i backoff z Polly
Retry np. 3 razy z 2^n sekundą opóźnienia. Potem ubijamy po x czasu
Dla transakcji odrzuconych można mieć osobny stan i obsługę ręczną (Dead Letter Queue)

 Przykład  flow (Event-Driven, Outbox)
Klient wysyła transakcję → trafia do bazy (status: Pending)
Zapisana do Outbox
Worker odczytuje z Outboxa, publikuje TransactionCreated do Kafka
Consumer wykonuje ApplyTransactionCommand
Wynik (Accepted/Rejected) aktualizuje DB + Outbox

## 3.Transakcje muszą być raportowane do systemów zewnętrznych - transakcja w "naszym" systemie nie jest "zatwierdzona" póki system zewnętrzny jej nie zaakceptuje. Jak można by przerobić aplikację by to zapewnić.  
**Odpowiedz** 
Na obecnym etapie aplikacja obsługuje podstawową logikę transakcji, w tym zapis do bazy, integrację z zewnętrznym systemem (WalletServiceClient) oraz asynchroniczne przetwarzanie transakcji przez usługę backgroundową. Transakcje posiadają statusy, które są aktualizowane po przetworzeniu, a całość oparta jest na CQRS z użyciem Unit of Work i retry policy (Polly).

![image](https://github.com/user-attachments/assets/949c9e52-4334-4d0d-8868-bd6317937d63)

Obecna implementacja zakłada, że transakcja zostaje zaakceptowana w momencie pomyślnego wywołania WalletServiceClient, co nie spełnia wymagania, według którego transakcja powinna być zatwierdzona dopiero po potwierdzeniu przez system zewnętrzny.

Aby to poprawić, należy:
 - Zrezygnować z bezpośredniego ustawiania statusu "Accepted" w handlerze. Zamiast tego transakcja powinna pozostawać w stanie "Pending" do momentu otrzymania odpowiedzi z systemu zewnętrznego.
  - Dodać osobny endpoint API, który pozwoli systemowi zewnętrznemu potwierdzić lub odrzucić transakcję. Na podstawie tej informacji status transakcji będzie aktualizowany.
 - Opcjonalnie) Warto rozważyć zastosowanie wzorca Outbox do bezpiecznego i spójnego przesyłania informacji o transakcjach do systemu zewnętrznego.
 - Zapewnić idempotencję potwierdzeń – ponowne przesłanie tej samej odpowiedzi nie powinno zmieniać stanu transakcji, jeśli została już zatwierdzona lub odrzucona.
 - Dodać logikę retry i ewentualne oznaczanie transakcji jako niepowodzone po przekroczeniu limitu prób.

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

