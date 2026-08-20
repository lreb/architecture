Given your previous plan, I would structure it as a Modular Monolith Reference Platform rather than a single business application.

1. Main goal

The project should demonstrate the common patterns you are likely to need in future APIs:

.NET 10
Modular Monolith
Vertical Slice Architecture
Clean Architecture principles
Domain-Driven Design
CQRS
Authentication / Authorization
Identity
Domain Events
Integration Events
Outbox Pattern
Background Workers
External API integration
Resilience
Caching
Validation
Auditing
Observability
Health Checks
API versioning
Testing

The important part is that each feature should demonstrate a real architectural case, rather than simply adding technologies.

2. Suggested reference project

I would use something generic such as:

ModularMonolith.Reference

with a fictional business domain such as:

Platform
├── Identity
├── Accounts
├── Catalog
├── Orders
├── Payments
├── Notifications
└── Audit

The domain itself isn't the important part. It gives you enough relationships to demonstrate architecture.

For example:

Customer
   │
   ├── Accounts
   │
   └── Orders
          │
          ├── Payment
          │
          └── Notification
3. The identity cases are particularly important

I would explicitly implement three identity scenarios.

Case A — One Identity

The simplest application:

API
 │
 └── Identity
       ├── Users
       ├── Roles
       └── Claims

Example:

User
 ├── Admin
 ├── Manager
 └── User

Use this to demonstrate:

ASP.NET Core Identity
JWT authentication
Role-based authorization
Claim-based authorization
Password policies
Refresh tokens
Account lockout
Email confirmation
User management

This becomes your default reference configuration.

4. Case B — Multiple identities in the same application

This is much more interesting.

For example:

                    API
                     │
          ┌──────────┴──────────┐
          │                     │
      Customer Identity     Admin Identity
          │                     │
     Customer users        Internal users

Example:

Customer
 ├── customer1
 └── customer2


Internal
 ├── administrator
 ├── support
 └── operator

Then demonstrate different authentication schemes:

Bearer Customer
Bearer Internal

For example:

Authorization: Bearer <customer-token>

versus:

Authorization: Bearer <internal-token>

And policies such as:

CustomerOnly
InternalOnly
AdministratorOnly
SupportOnly

This is extremely useful as a reference because future applications sometimes need:

customers + employees
external users + internal users
users + service accounts
B2C + B2B identities
5. Case C — External Identity Provider

Your third example should demonstrate an external IdP.

For example:

                 ┌─────────────────┐
                 │ External IdP    │
                 │                 │
                 │ Entra ID        │
                 │ Keycloak        │
                 │ Auth0           │
                 └────────┬────────┘
                          │
                          │ JWT
                          ▼
                    ┌───────────┐
                    │ .NET API  │
                    └───────────┘

Your application should demonstrate:

External authentication
        ↓
JWT validation
        ↓
Claims mapping
        ↓
Application authorization

This gives you a reference for enterprise environments where authentication is handled outside the application.

6. Recommended module structure

I would not create one giant Application, Domain, and Infrastructure project.

Instead:

src/
│
├── BuildingBlocks/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   ├── Messaging/
│   ├── Observability/
│   └── Web/
│
├── Modules/
│   │
│   ├── Identity/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Presentation/
│   │
│   ├── Accounts/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Presentation/
│   │
│   ├── Catalog/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Presentation/
│   │
│   ├── Orders/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Presentation/
│   │
│   ├── Payments/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Presentation/
│   │
│   ├── Notifications/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Presentation/
│   │
│   └── Audit/
│       ├── Domain/
│       ├── Application/
│       ├── Infrastructure/
│       └── Presentation/
│
└── Api/
    └── Host/

The critical architectural rule:

Module A
   │
   X  ────────> Module B database

should never happen.

Instead:

Module A
   │
   ├── Public contract
   │
   ├── Command/query
   │
   └── Integration event
             │
             ▼
        Module B
7. Give every module a specific architectural purpose

I would intentionally make each module demonstrate something different.

Module	Purpose
Identity	Authentication / authorization
Accounts	CRUD + Vertical Slice
Catalog	Queries + caching
Orders	Complex domain rules
Payments	External integration
Notifications	Async processing
Audit	Cross-cutting concerns

This makes the project useful as a cookbook.

8. Your Order module should be the "complex" example

For example:

CreateOrder
     │
     ▼
Validate customer
     │
     ▼
Validate products
     │
     ▼
Calculate totals
     │
     ▼
Create Order
     │
     ▼
OrderCreated
     │
     ├───────────────► Notification
     │
     ├───────────────► Payment
     │
     └───────────────► Audit

This demonstrates:

aggregate
entity
value object
domain service
domain event
integration event
transaction boundary
eventual consistency
9. Explicitly demonstrate Domain Events vs Integration Events

This is something I strongly recommend putting in the reference project.

Domain event

Internal to the module:

Order
  ↓
OrderCreatedDomainEvent
  ↓
Update internal state
Integration event

Cross-module:

Order
  ↓
OrderCreatedIntegrationEvent
  ↓
Outbox
  ↓
Worker
  ↓
Notifications

This distinction is extremely valuable when using the repository as a future reference.

10. Outbox example

Your reference should have an actual implementation:

HTTP Request
     │
     ▼
Order Transaction
     │
     ├── Order
     │
     └── OutboxMessage
             │
             ▼
        Commit transaction
             │
             ▼
       Background Worker
             │
             ▼
       Publish Event
             │
       ┌─────┴─────┐
       ▼           ▼
 Notification   Audit

And demonstrate:

Failed publishing
Retry
Exponential backoff
Dead-letter state
Idempotency
11. External API integration

The Payments module can demonstrate:

Orders
   │
   ▼
Payment Service
   │
   ├── Timeout
   ├── Retry
   ├── Circuit Breaker
   └── Fallback

Use HttpClientFactory and .NET resilience capabilities.

The example should intentionally include failure scenarios so you can use it later as a reference.

12. Common API features

Your template should also have examples for:

HTTP
GET
POST
PUT
PATCH
DELETE
API behavior
Pagination
Filtering
Sorting
Searching
Validation
ProblemDetails
Error handling
Idempotency
Concurrency
ETags
API versioning
Security
JWT
Roles
Claims
Policies
Scopes
API keys
CORS
Rate limiting
HTTPS
Security headers
Data
EF Core
SQL Server
Migrations
Transactions
Optimistic concurrency
Indexes
Specifications
AsNoTracking
Query projections
13. Observability

Make this a first-class module/cross-cutting concern.

Application
    │
    ├── Logs
    ├── Metrics
    └── Traces
           │
           ▼
       OpenTelemetry

Include:

Correlation ID
Trace ID
Request logging
Structured logging
Exception logging
Business events
Performance metrics
Health checks
Readiness
Liveness

And preferably expose:

/health
/health/live
/health/ready
14. Testing should also be part of the reference

I'd include four levels:

Tests/
│
├── Unit
│
├── Integration
│
├── Architecture
│
└── EndToEnd
Unit
Order.CalculateTotal()
Order.Cancel()
Order.AddItem()
Integration
API
 ↓
Database
Architecture

For example:

Domain
X Infrastructure


Domain
X ASP.NET Core


Orders
X Accounts.Infrastructure
E2E
POST /orders
      ↓
Database
      ↓
Outbox
      ↓
Worker
      ↓
Notification
15. Architecture tests are particularly valuable

Since this is intended as a future template, I would make architecture rules executable.

For example:

Domain projects cannot reference Infrastructure.


Modules cannot reference another module's Infrastructure.


Modules cannot directly access another module's DbContext.


Presentation cannot access another module's database.


Cross-module communication must use contracts.

Then if somebody accidentally violates the architecture later, the build fails.

16. Recommended feature matrix

The repository could have a document like this:

Feature	Example
CRUD	Accounts
Vertical Slice	Accounts
Simple CQRS	Catalog
Complex CQRS	Orders
Aggregate	Orders
Value Object	Money / Address
Domain Event	Order
Integration Event	Order → Notification
Outbox	Orders
Background Worker	Notifications
External API	Payments
Retry	Payments
Circuit Breaker	Payments
Cache	Catalog
Authentication	Identity
Authorization	Identity
Multiple Identities	Identity
External IdP	Identity
Audit	Audit
Logging	Platform
Metrics	Platform
Tracing	Platform
Health Checks	Platform
Rate Limiting	API
API Versioning	API
Pagination	Catalog
Filtering	Catalog
Idempotency	Orders
Optimistic Concurrency	Orders
Integration Tests	All
Architecture Tests	All

That matrix becomes your index for future projects.

17. One important recommendation

Don't make the template overly abstract.

I would avoid starting with things like:

IRepository<T>
IUnitOfWork
IGenericService<T>
GenericController<T>
GenericCrudHandler<T>

unless a concrete use case demonstrates why you need them.

Instead:

CreateOrderCommand
CreateOrderHandler
Order
OrderRepository
OrderDbContext

and then use the same pattern in another module where appropriate.

The purpose of the project should be:

"Show me how to solve a problem."

not:

"Show me how many abstractions I can put into .NET."

18. I would organize the repository as a learning/reference system

Something like:

ModularMonolith.Reference/
│
├── docs/
│   ├── architecture/
│   ├── security/
│   ├── persistence/
│   ├── messaging/
│   ├── observability/
│   └── testing/
│
├── src/
│   ├── Api/
│   ├── BuildingBlocks/
│   └── Modules/
│
├── tests/
│
├── samples/
│   ├── single-identity/
│   ├── multiple-identities/
│   └── external-identity/
│
├── docker/
│
├── scripts/
│
├── README.md
├── ARCHITECTURE.md
├── DEVELOPMENT.md
├── SECURITY.md
└── TECH_STACK.md

And the README could have:

Scenario
   ↓
What problem does it solve?
   ↓
Where is the implementation?
   ↓
What pattern is demonstrated?
   ↓
What alternatives exist?

That makes it much more valuable than a normal sample project.

19. Suggested progression

I would build it in phases:

Phase 1
.NET 10
Modular structure
Vertical Slice
CRUD
EF Core
SQL Server

↓

Phase 2
Identity
JWT
Roles
Claims
Policies
Multiple identity schemes

↓

Phase 3
Domain model
Aggregates
Value Objects
Domain Events

↓

Phase 4
Integration Events
Outbox
Background Worker
Idempotency

↓

Phase 5
External APIs
HttpClient
Resilience
Retry
Circuit Breaker

↓

Phase 6
Caching
Pagination
Filtering
Concurrency
Rate Limiting

↓

Phase 7
OpenTelemetry
Logging
Metrics
Tracing
Health Checks
Audit

↓

Phase 8
Unit Tests
Integration Tests
Architecture Tests
E2E

↓

Phase 9
Docker
CI/CD
Production configuration
Security hardening
The key idea

I would treat this repository as your personal .NET 10 architectural laboratory:

                         Modular Monolith
                               │
          ┌────────────────────┼────────────────────┐
          │                    │                    │
       Identity             Modules             Platform
          │                    │                    │
    ┌─────┼─────┐       ┌──────┼──────┐      ┌─────┼─────┐
    │     │     │       │      │      │      │     │     │
   One  Multi  External CRUD  DDD   Events  Logs Metrics Trace
    │     │     │       │      │      │      │     │     │
    └─────┴─────┘       └──────┴──────┘      └─────┴─────┘
                               │
                       Reference Cases

This aligns very well with the eight POCs you previously identified: CRUD/Vertical Slice, security, order workflow, module communication, events, outbox/worker, external APIs/resilience, and audit/observability.

If you build those deliberately as independent reference scenarios, you'll end up with something you can consult years later when starting a new .NET system, rather than just another demo application.