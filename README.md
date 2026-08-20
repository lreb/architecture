# Architecture Patterns

## Layered architecture

**Best** for: business apps, CRUD systems, enterprise apps
**Idea**: presentation, business logic, data access are separated

## Monolithic architecture

**Best** for: small to medium products, fast initial development
**Idea**: everything is in one deployable application

## Microservices architecture

**Best** for: large systems, independent teams, high scalability needs
**Idea**: split into small services that deploy independently

## Client-server architecture

**Best** for: web apps, mobile apps, networked applications
**Idea**: client requests data/services from a server

## Event-driven architecture

**Best** for: real-time systems, asynchronous workflows, integrations
**Idea**: components react to events instead of direct calls

## Hexagonal architecture

**Best** for: maintainable systems, testability, long-lived apps
**Idea**: keep core business logic isolated from external systems

## Clean architecture

**Best** for: complex business rules, large codebases
**Idea**: dependency points inward toward the domain

## Service-oriented architecture (SOA)

**Best** for: enterprise integration, older large organizations
**Idea**: services communicate over a network, often with shared infrastructure

## Serverless architecture

**Best** for: event-based apps, low-ops systems, bursty workloads
**Idea**: use managed functions/services without managing servers

## Pipe-and-filter architecture

**Best** for: data processing, ETL, compilers, batch workflows
**Idea**: data passes through a series of processing steps

# How to choose

Use:

**Monolith** / layered for simpler products and fast delivery
**Clean** / hexagonal when maintainability and testing matter
**Microservices** only when the system is large enough to justify complexity
**Event-driven** when async workflows and decoupling matter
**Serverless** when you want low ops and variable traffic

# Rule of thumb

Start simple.
Use modular monolith first in many cases.
Move to microservices only if you have clear scaling, team, or deployment reasons.