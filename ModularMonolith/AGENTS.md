# AGENTS.md

## Project Overview

**Facware Modular Monolith** — .NET 10 modular monolith with Vertical Slice Architecture.
Target: PostgreSQL + EF Core + Dapper. Initial module: Identity/Auth (not yet built).

The architecture spec lives in `Facware.ModularMonolith.ARCHITECTURE.md` (1895 lines). Read it for all architectural decisions, layering rules, and implementation phases.

## Current State

Base scaffold complete. The solution has four projects:

```
src/
├── Facware.ModularMonolith.Api/           # ASP.NET Core Web API (net10.0) — entry point
└── Facware.ModularMonolith.SharedKernel/  # Domain/ + Application/ abstractions only (§41: keep minimal)
tests/
├── Facware.ModularMonolith.Api.IntegrationTests/   # xUnit + WebApplicationFactory
└── Facware.ModularMonolith.ArchitectureTests/     # NetArchTest.Rules + project-reference checks
```

API currently provides: health checks (`/health/live`, `/health/ready`), `/openapi/v1.json` (Dev), Serilog, ProblemDetails + `IExceptionHandler`. No Identity module, no PostgreSQL yet.

## Repo Layout & Architecture

- SharedKernel contains ONLY `Entity`, `AggregateRoot`, `ValueObject`, `IDomainEvent`, `DomainEvent`, `Result`, `Error` (§41). Do not add business code there.
- Vertical slices: each application use case is a folder (`Authentication/Login/` with command, handler, validator, response, endpoint together) — no global `Controllers/`/`Services/`/`DTOs/` dumping grounds.
- Modules (future): each owns Domain + Application + Persistence + Contracts. Domain must have **no** infrastructure dependencies (no EF Core, Dapper, ASP.NET Core).
- `Program.cs` stays small — module registration via `services.AddIdentityModule(configuration)`.
- Package versions go directly in each `.csproj` (no `Directory.Packages.props` / `Directory.Build.props` — decided by the team).

## Build / Run / Test — IMPORTANT (WSL environment)

**CRITICAL: Tests must run from inside WSL. `WebApplicationFactory` hangs indefinitely when `dotnet test` runs from Windows over the `\\wsl.localhost\...\` UNC path.** Use `wsl -- bash -lc "..."` for all test/build commands, so paths resolve to `/home/chino/...`.

```bash
# Build the whole solution (from Windows shell, works over UNC too)
dotnet build Facware.ModularMonolith.slnx

# Run ALL tests (MUST be from inside WSL)
wsl -- bash -lc "cd ~/projects/architecture/ModularMonolith && dotnet test Facware.ModularMonolith.slnx"

# Run one test project in WSL
wsl -- bash -lc "cd ~/projects/architecture/ModularMonolith && dotnet test tests/Facware.ModularMonolith.Api.IntegrationTests/Facware.ModularMonolith.Api.IntegrationTests.csproj"

# Filter a single test in WSL
wsl -- bash -lc "cd ~/projects/architecture/ModularMonolith && dotnet test tests/Facware.ModularMonolith.Api.IntegrationTests/Facware.ModularMonolith.Api.IntegrationTests.csproj --filter 'FullyQualifiedName~HealthChecks'"

# Run the API (Windows, normal)
dotnet run --project src/Facware.ModularMonolith.Api/Facware.ModularMonolith.Api.csproj
```

```bash
http://localhost:5225/openapi/v1.json

http://localhost:5225/health/live

http://localhost:5225/swagger/index.html
```


- OpenAPI: `/openapi/v1.json` when `ASPNETCORE_ENVIRONMENT=Development` (default via launch profile). Swagger UI at `/swagger` (Dev only), pointed at the built-in OpenAPI doc via `UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", ...))` — positioned before `UseExceptionHandler()` and `UseHttpsRedirection()`. Title/version come from the `Swagger` section in `appsettings.json`, applied via an `AddOpenApi` document transformer in `Program.cs`. `launchBrowser` is on with `launchUrl: "swagger"`.
- The UI is bound to the built-in `Microsoft.AspNetCore.OpenApi` doc, so `AddSwaggerGen`/`UseSwagger` are **not** used (avoids the `Microsoft.OpenApi.Models` namespace change in Microsoft.OpenApi 2.x).
- Ports: HTTP `5225`, HTTPS `7102` (`Properties/launchSettings.json`).
- `dotnet ef` (10.0.2) is installed globally; watch for the mixed SDK: Windows has 10.0.3xx, WSL has 10.0.1xx.

## Build-artifact gotcha

`bin/` and `obj/` are shared between the Windows and WSL toolchains (same filesystem). If switching which side runs a build and you hit odd failures (e.g. `RZ3600: Invalid value '10.0' for RazorLangVersion`), clean artifacts first:

```bash
wsl -- bash -lc "cd ~/projects/architecture/ModularMonolith && find src tests -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +"
```

## Architecture Tests

- `tests/Facware.ModularMonolith.ArchitectureTests/` enforces: Domain types have no infra dependencies (NetArchTest); project-reference boundaries are parsed from the `.csproj` files (assembly-ref checks are unreliable because the C# compiler drops unused references).
- `ProjectReferences.cs` normalizes MSBuild `\` paths — keep that helper when adding module reference rules.

## Database & Migrations (when Identity is built)

- PostgreSQL, per-module `DbContext`, lowercase tables in `identity.*` schema.
- Migrations: `dotnet ef migrations add <Name> --project Facware.ModularMonolith.Identity.Persistence --startup-project Facware.ModularMonolith.Api`

## Important Notes

- **No `&&` or `||` in Windows PowerShell** — use `;` or separate commands (inside `wsl ... bash -lc "..."` strings, `&&` is fine).
- `.slnx` is the new .NET 10 solution format — some older tooling doesn't support it; add projects with `dotnet sln ... add`.
- `Facware.ModularMonolith.ARCHITECTURE.md` is the source of truth for architectural decisions; implementation follows Phases 1–10.
- Never commit secrets, connection strings, or JWT keys.