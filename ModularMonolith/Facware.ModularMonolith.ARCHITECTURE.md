# Facware Modular Monolith --- Architecture Specification

**Project:** `ModularMonolith\Facware.ModularMonolith.Api`\
**Target:** .NET 10\
**Architecture:** Modular Monolith + Vertical Slice +
Domain/Application/Persistence layers\
**Initial Module:** Identity / Authentication / Authorization\
**Database:** PostgreSQL\
**ORM/Data Access:** EF Core + Dapper where justified

------------------------------------------------------------------------

## 1. Purpose

This document defines the architectural and implementation standards for
`Facware.ModularMonolith.Api`.

The project is intentionally starting as a **modular monolith**, not as
a microservice system. The architecture must provide strong module
boundaries so that individual modules can evolve independently and, if
necessary, be extracted into services later.

The initial implementation focuses on:

-   Identity
-   Authentication
-   Authorization
-   Users
-   Roles
-   Permissions
-   JWT access tokens
-   Refresh tokens
-   PostgreSQL persistence
-   EF Core
-   Dapper where read/query performance or SQL control justifies it
-   Vertical Slice Architecture
-   Domain events
-   Module contracts
-   Validation
-   Error handling
-   Security
-   Observability
-   Caching where appropriate
-   Testing
-   CI/CD
-   Database migrations
-   API documentation

------------------------------------------------------------------------

# 2. Architectural Principles

## 2.1 Modular Monolith

The application is one deployable process and one primary API, but
consists of independently owned business modules.

``` text
Facware.ModularMonolith.Api
│
├── Identity Module
├── Future Module A
├── Future Module B
└── Shared Infrastructure
```

Modules must communicate through explicit contracts rather than directly
accessing another module's internal implementation.

### Rules

1.  A module owns its domain.
2.  A module owns its persistence model.
3.  A module must not directly access another module's DbContext.
4.  A module must not query another module's tables directly.
5.  Cross-module communication uses contracts, commands, queries, or
    events.
6.  Infrastructure details remain internal to a module.
7.  Shared code must remain minimal.
8.  Circular module dependencies are forbidden.
9.  Domain code must not depend on infrastructure.
10. Architecture tests must enforce these rules.

------------------------------------------------------------------------

# 3. Initial Solution Structure

Recommended solution:

``` text
ModularMonolith/
│
├── Facware.ModularMonolith.sln
│
├── src/
│   │
│   ├── Facware.ModularMonolith.Api/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── Middleware/
│   │   ├── OpenApi/
│   │   └── DependencyInjection/
│   │
│   ├── Facware.ModularMonolith.SharedKernel/
│   │   ├── Domain/
│   │   │   ├── Entity.cs
│   │   │   ├── AggregateRoot.cs
│   │   │   ├── ValueObject.cs
│   │   │   ├── IDomainEvent.cs
│   │   │   └── DomainEvent.cs
│   │   ├── Application/
│   │   │   ├── Result.cs
│   │   │   └── Error.cs
│   │   └── Infrastructure/
│   │
│   └── Modules/
│       │
│       └── Identity/
│           │
│           ├── Facware.ModularMonolith.Identity.Domain/
│           │   ├── Entities/
│           │   ├── ValueObjects/
│           │   ├── Events/
│           │   ├── Exceptions/
│           │   ├── Services/
│           │   └── Contracts/
│           │
│           ├── Facware.ModularMonolith.Identity.Application/
│           │   ├── Authentication/
│           │   │   ├── Login/
│           │   │   ├── RefreshToken/
│           │   │   └── RevokeToken/
│           │   ├── Users/
│           │   │   ├── CreateUser/
│           │   │   ├── GetUser/
│           │   │   ├── UpdateUser/
│           │   │   └── DeleteUser/
│           │   ├── Roles/
│           │   ├── Permissions/
│           │   ├── Services/
│           │   ├── Interfaces/
│           │   └── DependencyInjection.cs
│           │
│           ├── Facware.ModularMonolith.Identity.Persistence/
│           │   ├── IdentityDbContext.cs
│           │   ├── Configurations/
│           │   ├── Repositories/
│           │   ├── Queries/
│           │   ├── Migrations/
│           │   ├── Dapper/
│           │   └── DependencyInjection.cs
│           │
│           └── Facware.ModularMonolith.Identity.Contracts/
│               ├── Users/
│               ├── Authentication/
│               ├── Authorization/
│               └── Events/
│
└── tests/
    │
    ├── Facware.ModularMonolith.Identity.UnitTests/
    ├── Facware.ModularMonolith.Identity.IntegrationTests/
    ├── Facware.ModularMonolith.Api.IntegrationTests/
    └── Facware.ModularMonolith.ArchitectureTests/
```

------------------------------------------------------------------------

# 4. Layer Responsibilities

## 4.1 Domain

The Domain layer contains business rules and domain behavior.

Allowed:

-   Entities
-   Aggregate roots
-   Value objects
-   Domain events
-   Domain exceptions
-   Domain services
-   Business invariants

Not allowed:

-   EF Core
-   Dapper
-   ASP.NET Core
-   HTTP
-   JWT implementation
-   PostgreSQL
-   Redis
-   Serilog
-   Configuration
-   External services

Dependency direction:

``` text
Domain
  ↓
No infrastructure dependencies
```

------------------------------------------------------------------------

## 4.2 Application

The Application layer implements use cases.

Responsibilities:

-   Commands
-   Queries
-   Handlers
-   DTOs
-   Validation
-   Authorization requirements
-   Application services
-   Transaction coordination
-   Domain event orchestration
-   Module interfaces

Application should depend on Domain and abstractions, not concrete
infrastructure implementations.

``` text
Application
    ↓
Domain

Application
    ↓
Interfaces / Contracts
```

------------------------------------------------------------------------

## 4.3 Persistence

Persistence contains implementation details for storing and retrieving
module data.

Responsibilities:

-   EF Core DbContext
-   EF Core entity configurations
-   Repositories
-   Dapper queries
-   Database migrations
-   Transactions
-   Persistence interceptors
-   Outbox persistence
-   PostgreSQL-specific configuration

``` text
Persistence
    ↓
EF Core
Dapper
PostgreSQL
```

Persistence can depend on Application and Domain.

------------------------------------------------------------------------

# 5. Vertical Slice Architecture

Use vertical slices for application use cases.

Example:

``` text
Application/
└── Authentication/
    └── Login/
        ├── LoginCommand.cs
        ├── LoginHandler.cs
        ├── LoginValidator.cs
        ├── LoginResponse.cs
        └── LoginEndpoint.cs
```

A feature should contain the code required to implement that use case
rather than spreading it across global folders.

Avoid:

``` text
Controllers/
Services/
Validators/
DTOs/
Repositories/
```

when those folders become global dumping grounds.

Prefer:

``` text
Authentication/
├── Login/
├── RefreshToken/
└── RevokeToken/
```

------------------------------------------------------------------------

# 6. Identity Module

The first module is Identity.

Core responsibilities:

``` text
Identity
├── Users
├── Credentials
├── Roles
├── Permissions
├── Authentication
├── Authorization
├── Access Tokens
├── Refresh Tokens
└── Security/Audit
```

The module owns all identity-related persistence.

Suggested aggregates:

``` text
User
Role
Permission
RefreshToken
```

Do not expose persistence entities directly through the API.

------------------------------------------------------------------------

# 7. Authentication

Initial authentication mechanism:

``` text
JWT Access Token
+
Refresh Token
```

Login flow:

``` text
POST /api/v1/auth/login
        │
        ▼
Validate credentials
        │
        ▼
Load user
        │
        ▼
Validate account status
        │
        ▼
Resolve roles/permissions
        │
        ▼
Generate access token
        │
        ▼
Generate refresh token
        │
        ▼
Persist refresh token
        │
        ▼
Return authentication response
```

Example response concept:

``` json
{
  "accessToken": "...",
  "expiresAt": "2026-01-01T12:00:00Z",
  "refreshToken": "...",
  "refreshTokenExpiresAt": "2026-02-01T12:00:00Z"
}
```

Access tokens should be short-lived.

Refresh tokens should:

-   Be cryptographically random
-   Be stored hashed
-   Have expiration
-   Support revocation
-   Support rotation
-   Detect token reuse where practical

Never store raw refresh tokens in PostgreSQL.

------------------------------------------------------------------------

# 8. Authorization

Use policy/permission-based authorization.

Recommended model:

``` text
User
 └── Roles
      └── Permissions
```

Example:

``` text
Users.Read
Users.Create
Users.Update
Users.Delete

Roles.Read
Roles.Create
Roles.Update
Roles.Delete
```

Prefer policies/permissions over hard-coded role checks throughout
application code.

Avoid:

``` csharp
if (user.Role == "Admin")
```

Prefer:

``` text
Permission: Users.Create
```

Authorization must be enforced server-side.

------------------------------------------------------------------------

# 9. API Design

Base route:

``` text
/api/v1
```

Identity endpoints:

``` text
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/revoke

POST /api/v1/users
GET  /api/v1/users/{id}
GET  /api/v1/users
PUT  /api/v1/users/{id}
DELETE /api/v1/users/{id}
```

Use consistent HTTP semantics.

``` text
200 OK
201 Created
204 No Content
400 Bad Request
401 Unauthorized
403 Forbidden
404 Not Found
409 Conflict
422 Unprocessable Entity
429 Too Many Requests
500 Internal Server Error
```

Use `ProblemDetails` for errors.

------------------------------------------------------------------------

# 10. API Best Practices

The API should support:

-   Versioning
-   OpenAPI
-   ProblemDetails
-   Validation
-   Pagination
-   Filtering
-   Sorting
-   CancellationToken
-   Consistent response models
-   Idempotency where required
-   Rate limiting
-   Correlation/trace IDs

Never expose internal exceptions or stack traces to clients in
production.

------------------------------------------------------------------------

# 11. Validation

Use FluentValidation or an equivalent validation abstraction.

Validation occurs at multiple levels.

### API/Application validation

Examples:

``` text
Required fields
Email format
String length
Request structure
Pagination limits
```

### Domain validation

Examples:

``` text
User cannot be activated without required state
Order cannot be paid twice
Account cannot transition from Deleted to Active
```

Application validation must not replace domain invariants.

------------------------------------------------------------------------

# 12. Domain Events

Domain events represent business facts.

Example:

``` text
UserRegisteredDomainEvent
UserActivatedDomainEvent
UserDisabledDomainEvent
PasswordChangedDomainEvent
```

Example flow:

``` text
CreateUser
    │
    ▼
User aggregate
    │
    ▼
UserRegisteredDomainEvent
    │
    ├── Audit handler
    ├── Notification handler
    └── Other module handlers
```

Domain events should be in-process initially.

Avoid introducing a distributed message broker until there is an actual
requirement.

------------------------------------------------------------------------

# 13. Module Contracts

Modules communicate through explicit contracts.

Example:

``` text
Identity.Contracts
├── Users
│   ├── UserDto.cs
│   └── IUserModule.cs
├── Authentication
│   └── AuthenticationResult.cs
└── Events
    └── UserRegisteredIntegrationEvent.cs
```

Other modules may reference:

``` text
Identity.Contracts
```

but must not reference:

``` text
Identity.Persistence
Identity.Infrastructure
Identity.Domain
```

unless there is an explicitly justified architectural exception.

------------------------------------------------------------------------

# 14. EF Core Strategy

Each module should own a DbContext.

Identity:

``` text
IdentityDbContext
```

Future:

``` text
OrdersDbContext
BillingDbContext
CustomersDbContext
```

Avoid a single application-wide DbContext containing every module.

Entity configurations:

``` text
Persistence/
└── Configurations/
    ├── UserConfiguration.cs
    ├── RoleConfiguration.cs
    ├── PermissionConfiguration.cs
    └── RefreshTokenConfiguration.cs
```

Use PostgreSQL schemas where appropriate:

``` text
identity.users
identity.roles
identity.permissions
identity.refresh_tokens
```

------------------------------------------------------------------------

# 15. EF Core + Dapper

EF Core is the default persistence technology.

Use EF Core for:

-   Writes
-   Aggregate persistence
-   Transactions
-   Change tracking
-   Migrations

Use Dapper selectively for:

-   Complex read models
-   Reporting queries
-   Performance-sensitive queries
-   SQL requiring precise control
-   Large projections where EF-generated SQL is insufficient

Do not use Dapper merely because it is faster in theory.

Measure first.

------------------------------------------------------------------------

# 16. Database Migrations

Use EF Core migrations.

Example:

``` bash
dotnet ef migrations add InitialIdentity \
  --project Facware.ModularMonolith.Identity.Persistence \
  --startup-project Facware.ModularMonolith.Api
```

Apply migrations in controlled environments.

Production deployment should use an explicit migration/deployment
process rather than relying on developers manually executing commands.

Migration ownership remains with the module:

``` text
Identity.Persistence/
└── Migrations/
```

------------------------------------------------------------------------

# 17. PostgreSQL

PostgreSQL is the primary database.

Recommended considerations:

-   UUID/Guid identifiers
-   UTC timestamps
-   Proper indexes
-   Unique constraints
-   Foreign keys within module boundaries
-   Optimistic concurrency where required
-   PostgreSQL-specific types only when justified
-   JSONB only when relational modeling is inappropriate

Use lowercase PostgreSQL naming conventions consistently.

Example:

``` text
identity.users
identity.refresh_tokens
```

------------------------------------------------------------------------

# 18. Transactions

Transaction boundaries should normally align with a single application
command.

Example:

``` text
Command
  │
  ├── Domain changes
  ├── Persistence changes
  ├── Domain events
  └── Outbox record
        │
        ▼
     Commit
```

Do not create distributed transactions between modules.

If a workflow spans modules, use events and eventual consistency.

------------------------------------------------------------------------

# 19. Outbox Pattern

The architecture should be ready for an Outbox Pattern even if the
initial implementation uses only in-process domain events.

Concept:

``` text
Database Transaction
├── Business Data
└── Outbox Message
          │
          ▼
Background Processor
          │
          ▼
Event Handler / Broker
```

This prevents the failure scenario:

``` text
Database committed
+
Message publishing failed
```

Outbox messages should support:

``` text
Id
Type
Payload
OccurredAt
ProcessedAt
RetryCount
Error
```

------------------------------------------------------------------------

# 20. Caching

Caching is optional and must be driven by actual access patterns.

Potential candidates:

-   Permission lookups
-   Public configuration
-   Reference data
-   Expensive read models

Abstraction:

``` text
ICacheService
```

Potential implementations:

``` text
MemoryCache
Redis
```

Do not cache authentication/security decisions indefinitely.

Cache invalidation must be explicit.

------------------------------------------------------------------------

# 21. Error Handling

Use a centralized exception handling pipeline.

Concept:

``` text
Request
  │
  ▼
Endpoint
  │
  ▼
Application
  │
  ├── Validation error
  ├── Domain error
  ├── Not found
  ├── Conflict
  └── Unexpected error
       │
       ▼
Global Exception Handler
       │
       ▼
ProblemDetails
```

Define application/domain errors consistently.

Never expose:

-   Stack traces
-   SQL exceptions
-   Connection strings
-   Internal implementation details
-   Sensitive authentication information

------------------------------------------------------------------------

# 22. Security

Security requirements include:

### Authentication

-   Strong password hashing
-   Password policy
-   JWT signing key protection
-   Refresh token rotation
-   Refresh token hashing
-   Token expiration
-   Token revocation

### Authorization

-   Policy-based authorization
-   Permission checks
-   Least privilege

### API

-   HTTPS
-   CORS restrictions
-   Rate limiting
-   Request size limits
-   Input validation
-   Secure headers where applicable

### Secrets

Never commit:

``` text
JWT signing keys
Database passwords
API keys
OAuth secrets
Production credentials
```

Use environment variables or a secret manager.

------------------------------------------------------------------------

# 23. Password Security

Use a proven password hashing implementation.

Do not implement cryptographic password hashing manually.

Passwords must never be:

-   Logged
-   Returned in API responses
-   Stored in plaintext
-   Stored using reversible encryption

Password reset and account recovery should use short-lived, single-use
tokens.

------------------------------------------------------------------------

# 24. Configuration

Use strongly typed options.

Example:

``` text
Configuration/
├── DatabaseOptions.cs
├── JwtOptions.cs
├── OpenTelemetryOptions.cs
└── RateLimitOptions.cs
```

Configuration should be environment-specific.

``` text
appsettings.json
appsettings.Development.json
environment variables
secret provider
```

Validate critical configuration at startup.

The application should fail fast when mandatory production configuration
is missing.

------------------------------------------------------------------------

# 25. Dependency Injection

Each module exposes a single registration entry point.

Example:

``` csharp
services.AddIdentityModule(configuration);
```

Internally:

``` text
AddIdentityModule
├── AddIdentityApplication
├── AddIdentityPersistence
├── AddIdentityAuthentication
└── AddIdentityAuthorization
```

`Program.cs` should remain small.

Avoid registering hundreds of services directly in `Program.cs`.

------------------------------------------------------------------------

# 26. Observability

Observability should be built in from the beginning.

Cover:

``` text
Logs
Metrics
Traces
Health Checks
```

Recommended stack:

``` text
Serilog
OpenTelemetry
ASP.NET Core Health Checks
```

Use structured logging.

Example properties:

``` text
TraceId
SpanId
CorrelationId
UserId
Module
Operation
Duration
StatusCode
```

Never log:

``` text
Passwords
Access tokens
Refresh tokens
Authorization headers
Sensitive personal data
```

------------------------------------------------------------------------

# 27. Distributed Tracing

Use OpenTelemetry-compatible tracing.

Request:

``` text
HTTP
 │
 ▼
Application Handler
 │
 ▼
EF Core
 │
 ▼
PostgreSQL
```

All operations should retain trace context.

This becomes especially valuable if the modular monolith is later
decomposed.

------------------------------------------------------------------------

# 28. Health Checks

Expose health endpoints.

Example:

``` text
/health
/health/ready
/health/live
```

Checks may include:

``` text
Application
PostgreSQL
Redis, if enabled
External dependencies
```

Separate liveness from readiness.

------------------------------------------------------------------------

# 29. API Documentation

OpenAPI documentation is required.

Document:

-   Endpoints
-   Request models
-   Response models
-   Authentication
-   Authorization requirements
-   Error responses
-   Validation errors
-   Examples

Swagger UI may be enabled for development environments.

Production exposure should be an explicit security decision.

------------------------------------------------------------------------

# 30. Testing Strategy

Testing is divided into:

``` text
Unit Tests
Integration Tests
API Tests
Architecture Tests
```

## Unit Tests

Focus on:

-   Domain behavior
-   Business rules
-   Application handlers
-   Validators

## Integration Tests

Test:

-   PostgreSQL
-   EF Core
-   Repositories
-   Transactions
-   Authentication
-   Authorization
-   Module integration

Prefer Testcontainers for realistic infrastructure tests where
practical.

## API Tests

Test complete HTTP behavior:

``` text
Request
→ Authentication
→ Authorization
→ Validation
→ Handler
→ Database
→ Response
```

## Architecture Tests

Verify:

``` text
Domain → no infrastructure
Application → no persistence implementation dependency
Module A → cannot access Module B persistence
No circular dependencies
Contracts remain accessible
```

------------------------------------------------------------------------

# 31. CI/CD

CI pipeline:

``` text
Checkout
  ↓
Restore
  ↓
Build
  ↓
Unit Tests
  ↓
Integration Tests
  ↓
Architecture Tests
  ↓
API Tests
  ↓
Code Quality
  ↓
Security Scan
  ↓
Docker Build
```

CD pipeline:

``` text
Build Artifact
      ↓
Container Image
      ↓
Database Migration
      ↓
Deploy
      ↓
Health Check
      ↓
Smoke Test
```

Use GitHub Actions initially unless the deployment platform requires
another CI/CD provider.

------------------------------------------------------------------------

# 32. Docker

Provide:

``` text
Dockerfile
docker-compose.yml
```

Development infrastructure may include:

``` text
API
PostgreSQL
Redis
Mailpit
OpenTelemetry Collector
Grafana
```

Only enable infrastructure that the current project actually uses.

------------------------------------------------------------------------

# 33. Dependency Management

Use centralized package management.

Recommended:

``` text
Directory.Packages.props
Directory.Build.props
```

This allows package versions to be controlled centrally.

Keep package count intentionally small.

Avoid adding libraries for patterns that can be implemented cleanly
using built-in .NET capabilities.

------------------------------------------------------------------------

# 34. Logging

Serilog should provide structured application logging.

Recommended sinks should be environment-specific.

Development:

``` text
Console
```

Production may use:

``` text
Console → log collector
```

or another centralized sink.

Logging levels:

``` text
Trace
Debug
Information
Warning
Error
Critical
```

Do not use `Information` for high-frequency diagnostic events that will
generate excessive production volume.

------------------------------------------------------------------------

# 35. Resilience

For external dependencies use modern .NET resilience mechanisms.

Potential policies:

``` text
Timeout
Retry
Circuit Breaker
Rate Limiting
Fallback
```

Retries must only be used for operations that are safe to retry.

Do not blindly retry non-idempotent commands.

------------------------------------------------------------------------

# 36. API Rate Limiting

Authentication endpoints should receive stricter rate limiting.

Especially:

``` text
/login
/refresh
/password-reset
```

Rate limiting should reduce:

-   Credential stuffing
-   Brute-force attempts
-   Abuse
-   Accidental request storms

------------------------------------------------------------------------

# 37. Audit Logging

Identity operations should support auditing.

Potential events:

``` text
UserCreated
UserDisabled
UserEnabled
LoginSucceeded
LoginFailed
PasswordChanged
RefreshTokenRevoked
RoleAssigned
PermissionChanged
```

Audit records should include:

``` text
Actor
Action
Timestamp
Target
Result
TraceId
```

Do not store credentials or tokens in audit logs.

------------------------------------------------------------------------

# 38. Naming Conventions

Projects:

``` text
Facware.ModularMonolith.Api
Facware.ModularMonolith.SharedKernel

Facware.ModularMonolith.Identity.Domain
Facware.ModularMonolith.Identity.Application
Facware.ModularMonolith.Identity.Persistence
Facware.ModularMonolith.Identity.Contracts
```

Types:

``` text
PascalCase
```

Private fields:

``` text
_camelCase
```

Async methods:

``` text
Async suffix
```

Cancellation:

``` text
CancellationToken cancellationToken
```

------------------------------------------------------------------------

# 39. Cancellation

Every I/O-bound application operation should support cancellation.

Example:

``` csharp
Task<Result<UserResponse>> Handle(
    CreateUserCommand command,
    CancellationToken cancellationToken);
```

Pass the token through:

``` text
Endpoint
 → Handler
 → Repository
 → EF Core/Dapper
```

------------------------------------------------------------------------

# 40. Performance Guidelines

Performance should be measured rather than assumed.

General rules:

-   Use async I/O
-   Use pagination
-   Avoid N+1 queries
-   Project only required columns
-   Use `AsNoTracking()` for read-only EF queries
-   Use Dapper for justified complex/read-heavy queries
-   Add indexes based on query patterns
-   Avoid unnecessary object allocation
-   Avoid premature caching

------------------------------------------------------------------------

# 41. Shared Kernel Rules

The Shared Kernel must remain small.

Allowed examples:

``` text
Entity
AggregateRoot
ValueObject
DomainEvent
Result
Error
StronglyTypedId
```

Do not place business-specific functionality in SharedKernel.

Bad:

``` text
SharedKernel/
    UserService.cs
    OrderService.cs
    CustomerService.cs
```

Good:

``` text
SharedKernel/
    Domain/
    Application/
```

------------------------------------------------------------------------

# 42. Dependency Rules

Target dependency graph:

``` text
                    API
                     │
          ┌──────────┴──────────┐
          ▼                     ▼
   Identity.Presentation   Other Modules
          │
          ▼
   Identity.Application
          │
          ▼
   Identity.Domain
          ▲
          │
   Identity.Persistence
```

More explicitly:

``` text
Domain
  ← Application
  ← Persistence

Contracts
  ← API
  ← Other Modules

Persistence
  → Domain
  → Application

API
  → Application/Presentation
```

The exact project references should be validated by architecture tests.

------------------------------------------------------------------------

# 43. Initial Identity Data Model

Initial conceptual model:

``` text
users
-----
id
email
normalized_email
password_hash
first_name
last_name
is_active
is_locked
created_at
updated_at
last_login_at
concurrency_token

roles
-----
id
name
normalized_name
description

permissions
-----------
id
name
description

user_roles
----------
user_id
role_id

role_permissions
----------------
role_id
permission_id

refresh_tokens
--------------
id
user_id
token_hash
expires_at
created_at
revoked_at
replaced_by_token_id
created_by_ip
revoked_by_ip
```

Use unique constraints for normalized email, role names, permission
names, and relationship keys.

------------------------------------------------------------------------

# 44. Initial Authentication Use Cases

Required slices:

``` text
Authentication/
├── Login/
├── RefreshToken/
├── RevokeToken/
└── Logout/
```

Users:

``` text
Users/
├── CreateUser/
├── GetUser/
├── GetUsers/
├── UpdateUser/
├── DisableUser/
├── EnableUser/
└── DeleteUser/
```

Roles:

``` text
Roles/
├── CreateRole/
├── GetRole/
├── GetRoles/
├── UpdateRole/
├── DeleteRole/
├── AssignRole/
└── RemoveRole/
```

Permissions:

``` text
Permissions/
├── GetPermissions/
├── AssignPermission/
└── RemovePermission/
```

Do not implement every feature immediately. The structure defines the
intended direction.

------------------------------------------------------------------------

# 45. Recommended Implementation Order

## Phase 1 --- Solution Foundation

-   Create solution
-   Create API project
-   Create SharedKernel
-   Create Identity projects
-   Configure project references
-   Configure centralized package versions
-   Configure nullable reference types
-   Configure analyzers
-   Add architecture tests

## Phase 2 --- PostgreSQL

-   Add Npgsql
-   Add EF Core
-   Create IdentityDbContext
-   Configure PostgreSQL
-   Create entity configurations
-   Create first migration
-   Add development database

## Phase 3 --- Identity Domain

Implement:

-   User
-   Role
-   Permission
-   RefreshToken
-   Value objects
-   Domain errors
-   Domain events

## Phase 4 --- Authentication

Implement:

-   Password hashing
-   Login
-   JWT generation
-   Refresh tokens
-   Token rotation
-   Token revocation

## Phase 5 --- Authorization

Implement:

-   Roles
-   Permissions
-   Policies
-   Permission authorization handler

## Phase 6 --- API

Implement:

-   Authentication endpoints
-   User endpoints
-   Role endpoints
-   ProblemDetails
-   Validation
-   OpenAPI
-   API versioning strategy

## Phase 7 --- Infrastructure

Implement:

-   Serilog
-   OpenTelemetry
-   Health checks
-   Global exception handling
-   Rate limiting
-   Configuration validation

## Phase 8 --- Domain Events / Outbox

Implement:

-   Domain event dispatcher
-   Outbox table
-   Outbox processor
-   Retry mechanism
-   Idempotency

## Phase 9 --- Testing

Implement:

-   Domain unit tests
-   Handler tests
-   Persistence integration tests
-   API integration tests
-   Architecture tests

## Phase 10 --- CI/CD

Implement:

-   Build pipeline
-   Test pipeline
-   Security scanning
-   Docker image
-   Migration process
-   Deployment
-   Health checks
-   Smoke tests

------------------------------------------------------------------------

# 46. Definition of Done for the Initial Template

The initial template is considered complete when:

-   [ ] Solution builds with .NET 10
-   [ ] Identity is isolated as a module
-   [ ] Identity has Domain/Application/Persistence/Contracts
-   [ ] Vertical Slice structure is established
-   [ ] PostgreSQL is configured
-   [ ] EF Core migrations work
-   [ ] Users can be created
-   [ ] Users can authenticate
-   [ ] JWT access tokens work
-   [ ] Refresh tokens work
-   [ ] Refresh token rotation works
-   [ ] Token revocation works
-   [ ] Roles work
-   [ ] Permissions work
-   [ ] Policy authorization works
-   [ ] Validation is centralized
-   [ ] ProblemDetails is implemented
-   [ ] Global exception handling exists
-   [ ] Domain events exist
-   [ ] Module contracts exist
-   [ ] Architecture tests enforce module boundaries
-   [ ] Unit tests exist
-   [ ] Integration tests exist
-   [ ] API tests exist
-   [ ] OpenAPI documentation exists
-   [ ] Structured logging exists
-   [ ] OpenTelemetry is configured
-   [ ] Health checks exist
-   [ ] Rate limiting exists
-   [ ] Security configuration is externalized
-   [ ] Docker build works
-   [ ] CI pipeline works
-   [ ] Database migration process works

------------------------------------------------------------------------

# 47. Long-Term Evolution

The architecture should allow future modules such as:

``` text
Identity
Customers
Products
Orders
Billing
Payments
Notifications
Files
Audit
Reporting
```

The desired evolution is:

``` text
                    Modular Monolith
                           │
       ┌───────────────────┼───────────────────┐
       ▼                   ▼                   ▼
    Identity             Orders             Billing
       │                   │                   │
       └───────────────────┼───────────────────┘
                           │
                     PostgreSQL
                           │
                       Outbox
```

If a module eventually requires independent scaling or deployment:

``` text
Modular Monolith
       │
       ├── Identity
       ├── Customers
       └── Orders
               │
               ▼
        Extract Orders
               │
               ▼
          Orders Service
```

This extraction should be possible because the module already has:

-   Its own domain
-   Its own persistence
-   Its own application layer
-   Its own contracts
-   Explicit module communication
-   Domain/integration events
-   Independent tests

------------------------------------------------------------------------

# 48. Architectural Goal

The primary goal is **not maximum abstraction**.

The goal is:

``` text
Simple enough to develop quickly
+
Strict enough to prevent architectural decay
+
Modular enough to scale the codebase
+
Observable enough to operate in production
+
Testable enough to change safely
+
Decoupled enough to extract services later
```

The project should remain a **modular monolith until there is a concrete
reason to introduce distributed systems complexity**.
