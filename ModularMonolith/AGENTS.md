# AGENTS.md

## Project Overview

**Facware Modular Monolith** — .NET 10 modular monolith with Vertical Slice Architecture.
Target: PostgreSQL + EF Core + Dapper. Initial module: Identity/Auth.

The architecture spec lives in `Facware.ModularMonolith.ARCHITECTURE.md` (1895 lines). Read it for all architectural decisions, layering rules, and implementation phases.

## Current State

This is an **early-stage scaffold**. The solution has only one project:
- `src/Facware.ModularMonolith.Api/` — ASP.NET Core Web API (net10.0)
- `test/` — empty directory (no test projects yet)
- No modules, no tests, no CI, no Docker yet implemented

The API currently has a template weather forecast endpoint only. Identity module is planned but not built.

## Solution & Build

- **Solution file**: `Facware.ModularMonolith.slnx` (not `.sln` — uses the new .NET 10 solution format)
- **Build**: `dotnet build src/Facware.ModularMonolith.Api/Facware.ModularMonolith.Api.csproj`
- **Run**: `dotnet run --project src/Facware.ModularMonolith.Api/Facware.ModularMonolith.Api.csproj`
- **Default ports**: `http://localhost:5225` (HTTP), `https://localhost:7102` (HTTPS)
- **Launch settings**: Defined in `src/Facware.ModularMonolith.Api/Properties/launchSettings.json`
- **OpenAPI**: Available at `/openapi` when `ASPNETCORE_ENVIRONMENT=Development`

## Architecture Structure (Planned)

```
src/
├── Facware.ModularMonolith.Api/           # Entry point only
├── Facware.ModularMonolith.SharedKernel/  # Shared domain/app abstractions
└── Modules/Identity/
    ├── Facware.ModularMonolith.Identity.Domain/
    ├── Facware.ModularMonolith.Identity.Application/
    ├── Facware.ModularMonolith.Identity.Persistence/
    └── Facware.ModularMonolith.Identity.Contracts/
```

Key rules from the architecture spec:
- Each module owns its `DbContext` — no shared DbContext across modules
- Domain has **no** infrastructure dependencies (no EF Core, Dapper, ASP.NET Core, etc.)
- Cross-module communication via contracts only — never access another module's Persistence directly
- Use **vertical slices** for application use cases (e.g., `Authentication/Login/` contains command, handler, validator, response, endpoint together)
- `Program.cs` stays small — register modules via `services.AddIdentityModule(configuration)`
- Use `Directory.Packages.props` and `Directory.Build.props` for centralized package version management

## Database & Migrations

- **Database**: PostgreSQL (not yet configured)
- **Migrations**: `dotnet ef migrations add InitialIdentity --project Facware.ModularMonolith.Identity.Persistence --startup-project Facware.ModularMonolith.Api`
- Migrations live in `Modules/Identity/Persistence/Migrations/`
- Use PostgreSQL schemas: `identity.users`, `identity.roles`, etc.
- Use lowercase table/column names

## Testing

- Test projects planned but not yet created
- Architecture tests enforce module boundaries (Domain → no infrastructure, no circular deps)
- Integration tests use Testcontainers for PostgreSQL where practical
- No test infrastructure exists yet — `test/` is empty

## Key Commands (When Implemented)

```bash
# Build the solution
dotnet build Facware.ModularMonolith.slnx

# Add a migration
dotnet ef migrations add <Name> --project Facware.ModularMonolith.Identity.Persistence --startup-project Facware.ModularMonolith.Api

# Apply migrations
dotnet ef database update --project Facware.ModularMonolith.Identity.Persistence --startup-project Facware.ModularMonolith.Api

# Run the API
dotnet run --project src/Facware.ModularMonolith.Api/Facware.ModularMonolith.Api.csproj
```

## Important Notes

- **No `&&` or `||` in PowerShell** on this machine — use semicolons or separate commands
- The `.slnx` solution format is new — some older `dotnet` tooling may not support it
- `Facware.ModularMonolith.ARCHITECTURE.md` is the source of truth for all architectural decisions
- Implementation follows phased approach (Phase 1–10) defined in the architecture doc
- Do not add packages not listed in `Directory.Packages.props` when it exists
- Never commit secrets, connection strings, or JWT keys
