# Sales Commission Control

![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)
[![CI](https://github.com/MateusCizeski/dotnet-sales-commission-control/actions/workflows/ci.yml/badge.svg)](https://github.com/MateusCizeski/dotnet-sales-commission-control/actions/workflows/ci.yml)

Sellers issue invoices; every invoice generates a commission based on the seller's rate. Admins approve
invoices and pay commissions; sellers log in to follow their own numbers.

- **API**: ASP.NET Core 10, Clean Architecture, DDD aggregate, CQRS-style read side with [ApiBase](https://github.com/MateusCizeski/dotnet-api-base)
- **Front**: Razor Pages that talk to the API over HTTP only
- **Database**: SQL Server (EF Core 10, migrations)
- **Security**: JWT with roles, deny-by-default authorization, ownership filter, ProblemDetails, login rate limiting

---

## Business rules

| Rule | Where |
|---|---|
| Commission = invoice total × seller rate (0–15%), rounded to 2 decimals | `Commission.Calculate` |
| An invoice always has exactly one commission, saved in the same transaction | `Invoice` (aggregate root) |
| Changing seller or total of a pending invoice recalculates the commission with the new seller's rate | `Invoice.Update` |
| Inactive sellers cannot receive invoices | `Invoice` |
| Sellers with commissions cannot be deactivated or deleted | `SellerService` |
| CPF and email are unique; CPF/CNPJ check digits are validated | `Seller`, `SellerService`, validators |
| A Seller user only sees their own invoices, commissions and dashboard | `InvoiceQueries.DefaultFilter`, `DashboardQueries` |

CPF and CNPJ are Brazilian tax IDs (individual and company); the names were kept because they are domain terms.

### Status flow

```
Invoice:     Pending ──approve──▶ Approved
                │                    │
                └──cancel──▶ Cancelled ◀──cancel── (only while the commission is not paid)

Commission:  Pending ──pay──▶ Paid
                └──cancel──▶ Cancelled          (cancelling the invoice cancels its pending commission)
```

Every forbidden transition throws a `DomainException` → HTTP 400 with the reason.

---

## Architecture

```
Front (Razor Pages) ──HTTP/JSON──▶ Api ──▶ Application ──▶ Domain
                                     │          ▲              ▲
                                     └──▶ Infra ┴──────────────┘
```

| Project | Responsibility | Depends on |
|---|---|---|
| `Domain` | Entities with their rules (`Seller`, `Invoice` + `Commission`, `User`), repository interfaces | `ApiBase.Domain` (base entity only) |
| `Application` | Use cases (commands), ports (`ICurrentUser`, `IInvoiceNumberGenerator`, `ITokenService`...), views and the dashboard query contract | Domain |
| `Infra` | EF Core, repositories, SQL Server sequence, password hashing, read side (ApiBase), dashboard aggregation | Application, Domain |
| `Api` | Controllers, JWT, authorization, ProblemDetails, OpenAPI — the composition root | Application, Infra |
| `Front` | UI; its own models of the API contract | nothing from the backend |

**Dependency rule**: `Application` does not reference EF Core or Infra. Infra implements the Application
interfaces, and `Api` is the only place where everything is wired together.

### Commands vs queries

- **Commands** (create, update, approve, cancel, pay) go through `Application/Services` → domain methods →
  one `CommitAsync` per use case.
- **Queries** (lists and details) use ApiBase: the query string (`filter`, `sort`, `page`, `limit`, `fields`)
  becomes SQL and the entities are projected into views (`InvoiceView`, `CommissionView`, `SellerView`).
  Only view properties can be returned, filtered or sorted, and `limit` is capped at 100.
- The **dashboard** is a dedicated query (`IDashboardQueries`) computed with `COUNT`/`SUM`/`GROUP BY` in the database.

Both sides use the same `IUnitOfWork` (from ApiBase) over the same scoped `DbContext`.

---

## API

Every endpoint requires a token except the login. Errors use [ProblemDetails](https://www.rfc-editor.org/rfc/rfc9457).

| Method | Route | Role |
|---|---|---|
| POST | `/api/auth/login` | anonymous (5 attempts/min per IP) |
| GET | `/api/sellers`, `/api/sellers/{id}`, `/api/sellers/options` | Admin |
| POST / PUT / DELETE | `/api/sellers`, `/api/sellers/{id}`, `/{id}/activate`, `/{id}/deactivate` | Admin |
| GET | `/api/invoices`, `/api/invoices/{id}` | Admin, Seller (own only) |
| POST / PUT | `/api/invoices`, `/api/invoices/{id}`, `/{id}/approve`, `/{id}/cancel` | Admin |
| GET | `/api/commissions`, `/api/commissions/{id}` | Admin, Seller (own only) |
| POST | `/api/commissions/{id}/pay`, `/{id}/cancel` | Admin |
| GET | `/api/dashboard/summary?startDate&endDate&sellerId&status` | Admin, Seller (own only) |

List example:

```
GET /api/invoices?filter=[{"property":"Status","value":"Approved"},{"property":"Seller.FullName","operator":"contains","value":"ana"}]
                &sort=[{"property":"IssueDate","direction":"desc"}]&page=1&limit=20&fields=Id,Number,TotalAmount
```

`Api/Api.http` has ready-to-run requests. In Development, Swagger UI is at `/swagger`.

---

## Running

### Before ApiBase 3.0.0 is published

The projects reference `ApiBase.* 3.0.0`. Until it is on nuget.org, pack it locally and add the folder as a source:

```bash
# in dotnet-api-base
dotnet build -c Release
for p in ApiBase.Domain ApiBase.Infra ApiBase.Repository ApiBase.Application ApiBase.Controller; do dotnet pack $p/$p.csproj -c Release --no-build -o ../local-nuget; done

# in this repository
dotnet restore Portal.slnx -s https://api.nuget.org/v3/index.json -s ../local-nuget
```

After the real 3.0.0 is published, clear the local copy from the cache (`dotnet nuget locals global-packages --list`, delete the `apibase.*` 3.0.0 folders) so the published version is used.

### Local (SQL Server Express / LocalDB)

```bash
cd Api
dotnet user-secrets set "Jwt:Secret" "<random value, 32+ characters>"
dotnet user-secrets set "Seed:AdminEmail" "admin@example.com"
dotnet user-secrets set "Seed:AdminPassword" "<your local password>"
dotnet user-secrets set "Seed:SellerEmail" "seller@example.com"
dotnet user-secrets set "Seed:SellerPassword" "<your local password>"
dotnet run                                   # http://localhost:5001/swagger
dotnet run --project ../Front                # http://localhost:7000
```

The connection string is in `Api/appsettings.json` (`Server=localhost\SQLEXPRESS`); override it with
`ConnectionStrings__DefaultConnection` if needed. In Development the API applies the migrations and creates
the users on startup.

### Docker

```bash
cp .env.example .env    # then edit the values
docker compose up --build
```

API on http://localhost:5001, Front on http://localhost:7000. The API waits for the SQL Server health check.

### Tests

```bash
dotnet test Tests/Tests.Unit          # domain rules and use cases (Moq)
dotnet test Tests/Tests.Integration   # the whole API over HTTP (WebApplicationFactory + SQLite in memory)
```

The integration tests replace only the database provider and the SQL Server sequence; routing, authentication,
authorization, validation, the exception handler and EF Core run for real.

---

## Design decisions

- **Invoice as aggregate root** — the commission is created and recalculated by the invoice, so a single commit saves both. Before, a failure between two commits could leave an invoice without commission.
- **Invoice number from a SQL Server `SEQUENCE`** — atomic with many API instances; `MAX()+1` would duplicate numbers under concurrency. Gaps are accepted.
- **Ownership from the token, never from the request** — `DefaultFilter` uses the seller id of the JWT, so `?sellerId=` of another seller returns nothing (no IDOR).
- **`TimeProvider` instead of `DateTime.UtcNow`** — domain methods receive the date; tests use a fixed clock.
- **Migrations at startup only in Development** — in production the schema change belongs to the pipeline (migration bundle or script): several instances would race, and the application user should not have DDL rights.
- **No refresh token** — the access token lasts 60 minutes and the Front cookie expires with it. A real product would add refresh tokens or an identity provider.

## Third-party licenses

- **EPPlus 6** (Excel export): PolyForm Noncommercial — fine for a portfolio, not for commercial use.
- **QuestPDF** (PDF export): Community license, free below the revenue threshold in its terms.

Check the current terms before any commercial use.
