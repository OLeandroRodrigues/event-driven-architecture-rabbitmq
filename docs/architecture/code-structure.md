# Code Structure & Architecture Mapping

## 1. Purpose

This document explains **how the codebase is structured** and how each part maps to the
architectural concepts described in this repository.

The goal is to make it explicit:
- Where each responsibility lives
- How Clean Architecture and DDD boundaries are enforced
- Where messaging concerns (RabbitMQ, retries, idempotency, observability) are implemented
- How future contributors should navigate and extend the codebase

This structure prioritizes:
- Clear separation of concerns
- Testability
- Replaceability of infrastructure
- Long-term maintainability

---

## 2. High-Level Structure

At a high level, the solution is organized by **architectural layers**, not by technical frameworks.

/src
├── Domain
├── Application
├── Infrastructure
├── Host


Each layer has a **single, well-defined responsibility** and strict dependency rules.

---

## 3. Dependency Rules (Very Important)

Dependencies must always point **inward**:

Host → Infrastructure → Application → Domain


Rules:
- Domain depends on nothing
- Application depends only on Domain
- Infrastructure depends on Application and Domain
- Host depends on everything (composition root)

Violating these rules breaks the architecture.

---

## 4. Domain Layer

### Location
/src/Domain


### Responsibilities
- Core business logic
- Domain entities and value objects
- Domain services
- Domain events (facts that happened)

### What must NOT exist here
- RabbitMQ references
- Message brokers
- Serialization logic
- Retry, ACK/NACK, DLQ concepts
- Framework dependencies

### Example contents
Domain/
├── Entities/
├── ValueObjects/
├── Services/
└── Events/


Domain events represent **business facts**, not integration contracts.

---

## 5. Application Layer

### Location
/src/Application


### Responsibilities
- Use cases / application services
- Orchestration of domain logic
- Transaction boundaries
- Publishing domain events via abstractions
- Defining ports (interfaces)

### Key characteristics
- Depends on Domain
- Defines interfaces for infrastructure concerns
- Contains no RabbitMQ-specific code

### Example contents
Application/
├── UseCases/
├── Commands/
├── Queries/
├── Interfaces/
│ ├── IEventPublisher.cs
│ ├── IMessageBus.cs
│ └── IInboxStore.cs
└── DTOs/


The application layer **coordinates**, but does not implement infrastructure.

---

## 6. Infrastructure Layer

### Location
/src/Infrastructure


### Responsibilities
- RabbitMQ producers and consumers
- Message serialization and envelopes
- Retry and DLQ configuration
- Idempotency store (Inbox Pattern)
- External system integrations
- Persistence implementations

### Key characteristics
- Implements application interfaces
- Contains all messaging details
- Can be replaced without changing business logic

### Example contents
Infrastructure/
├── Messaging/
│ ├── RabbitMq/
│ │ ├── RabbitMqPublisher.cs
│ │ ├── RabbitMqConsumer.cs
│ │ └── RabbitMqConfiguration.cs
│ └── Serialization/
├── Idempotency/
│ └── InboxStore.cs
├── Persistence/
└── Observability/


This is where:
- ACK / NACK logic lives
- Retry and DLQ routing is configured
- CorrelationId is injected and propagated

---

## 7. Host / Composition Root

### Location
/src/Host


### Responsibilities
- Dependency Injection configuration
- Application startup
- RabbitMQ connection setup
- Background workers / hosted services
- Configuration binding
- Logging and observability setup

### Key characteristics
- Knows the full system
- Contains framework glue code
- No business logic

### Example contents
Host/
├── Program.cs
├── DependencyInjection/
├── HostedServices/
└── Configuration/


This is the **only place** where:
- RabbitMQ libraries are wired
- Infrastructure implementations are bound to interfaces
- Environment-specific configuration exists

---

## 8. Messaging Flow Mapping

### Producer side
- Domain raises a domain event
- Application maps it to an integration event
- Application calls `IEventPublisher`
- Infrastructure publishes to RabbitMQ

### Consumer side
- Infrastructure receives message
- Infrastructure extracts metadata (MessageId, CorrelationId)
- Infrastructure checks Inbox Store
- Application use case is executed
- Infrastructure ACKs or routes to retry/DLQ

This flow ensures:
- Domain remains clean
- Messaging remains explicit
- Reliability patterns are enforced consistently

---

## 9. Idempotency Placement

Idempotency logic lives at the **boundary between Infrastructure and Application**.

- Inbox checks happen before invoking use cases
- Inbox updates happen after successful processing
- Domain logic is unaware of retries or duplicates

See:
- [ADR-003: Idempotency Strategy](../decisions/adr-003-idempotency-strategy.md)

---

## 10. Observability Placement

Observability concerns are centralized in Infrastructure and Host:

- CorrelationId generation
- Log enrichment
- Structured logging
- Metrics hooks

Domain and Application layers should only **accept context**, not manage observability.

See:
- [Correlation & Observability](correlation-and-observability.md)

---

## 11. Testing Strategy (Mapping)

- **Domain**: pure unit tests
- **Application**: use case tests with mocked ports
- **Infrastructure**: integration tests (RabbitMQ, database)
- **Host**: smoke tests / wiring tests

This structure enables fast feedback and confidence.

---

## 12. Why This Structure Matters

This code structure:
- Makes architectural decisions explicit
- Prevents accidental coupling
- Scales as the system grows
- Supports long-lived systems
- Reflects real-world production architectures

It is intentionally more structured than small demos,
because this repository serves as an **architecture reference**, not a toy example.

---

## 13. Related Documentation

- [Clean Architecture & DDD-Friendly Boundaries](clean-architecture-ddd.md)
- [ADR-002: Delivery Guarantees](../decisions/adr-002-delivery-guarantees.md)
- [ADR-003: Idempotency Strategy](../decisions/adr-003-idempotency-strategy.md)
- [ADR-004: Retry & DLQ Strategy](../decisions/adr-004-retry-and-dlq.md)

## 14. Test Structure Mapping

Tests mirror the architectural layers:

- Domain → Domain.UnitTests
- Application → Application.UnitTests
- Infrastructure → Infrastructure.IntegrationTests
- Host → Host.SmokeTests

This ensures test responsibility aligns with architectural boundaries.
