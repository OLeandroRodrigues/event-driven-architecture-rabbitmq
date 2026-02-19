# Tests Guide

This repository follows an **architecture-first** approach, and the test suite mirrors the same structure.

The goal of this directory is to make it clear:
- **What each test project validates**
- **Which dependencies are allowed**
- **How to run tests locally and in CI**
- **Where to add new tests as the codebase grows**

---

## Test Projects (Overview)

| Project | Type | Purpose |
|---|---|---|
| `Domain.UnitTests` | Unit | Validate pure domain rules (entities, value objects, invariants) |
| `Application.UnitTests` | Unit | Validate use cases and orchestration using mocks for ports |
| `Infrastructure.IntegrationTests` | Integration | Validate messaging/persistence against real dependencies (RabbitMQ, DB) |
| `Host.SmokeTests` | Smoke | Validate DI wiring and app startup (composition root) |

> If a test needs RabbitMQ, Docker, databases, file system, or network access, it is **not** a unit test.

---

## 1) Domain.UnitTests

### What it tests
- Entities and invariants
- Value objects behavior
- Domain services (pure logic)
- Domain events (payload correctness)

### What it must NOT use
- RabbitMQ client libraries
- Databases / file system / network calls
- Dependency Injection or configuration binding
- Time-based logic without abstractions

### Typical patterns
- Arrange → Act → Assert
- Small, deterministic tests

---

## 2) Application.UnitTests

### What it tests
- Use cases and handlers
- Validation and orchestration logic
- Correct calls to ports (interfaces), e.g.:
  - `IEventPublisher`
  - `IMessageBus`
  - `IInboxStore`

### What it should use
- Mocks / fakes for infrastructure ports
- Fluent assertions for readability (optional)
- Cancellation tokens where applicable

### What it must NOT do
- Connect to RabbitMQ
- Perform real I/O

---

## 3) Infrastructure.IntegrationTests

### What it tests
- RabbitMQ publish/consume behavior (real broker)
- Serialization and message envelope
- Retry routing and DLQ behavior
- Idempotency store persistence semantics
- CorrelationId propagation through messaging

### Dependencies
These tests typically require:
- Docker (RabbitMQ container)
- Optional persistence container (Postgres/SQL Server/etc.)

### Expected characteristics
- Slower than unit tests
- More environment sensitive
- Provide high confidence for reliability patterns

---

## 4) Host.SmokeTests

### What it tests
- Host startup and dependency injection wiring
- Service registration correctness
- Required configuration sanity checks
- Hosted services can be constructed

### What it avoids
- Full end-to-end runtime behavior
- Long-running background loops

---

## How to Run Tests

### Run all tests
```bash
dotnet test
```

### Run unit tests only
You can filter by naming convention:

```bash
dotnet test --filter "FullyQualifiedName~UnitTests"
```

### Run integration tests only
```bash
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

> Integration tests may require Docker. If Docker is not available, these tests should be skipped or run in CI.

---

## Naming & Structure Conventions

### Test file naming
- `XyzTests.cs`

### Test method naming
Use a clear pattern:
- `Method_WhenCondition_ShouldExpectedOutcome`
- or `Given_When_Then`

### Folder structure inside a test project
Mirror the layer being tested:

```
Domain.UnitTests/
  Entities/
  ValueObjects/
  Services/
  Events/
```

---

## When to Add a New Test

- If you change domain rules → add/update **Domain.UnitTests**
- If you change a use case flow → add/update **Application.UnitTests**
- If you change RabbitMQ configuration, retries, DLQ, idempotency persistence → add/update **Infrastructure.IntegrationTests**
- If you change service registration / startup → add/update **Host.SmokeTests**

---

## Fast Feedback Rule

A good healthy test suite should provide:
- **Fast feedback** via unit tests (most tests)
- **High confidence** via integration tests (smaller set)
- **Wiring safety** via smoke tests (very small set)

---

## Related Documentation

- [Architecture Overview](../docs/architecture/architecture-overview.md)
- [Code Structure & Architecture Mapping](../docs/architecture/code-structure.md)
- [Idempotent Consumers (Inbox Pattern)](../docs/architecture/idempotent-consumers-inbox-pattern.md)
- [Retry & DLQ](../docs/architecture/retry-and-dlq.md)
- [ADR-003: Idempotency Strategy](../docs/decisions/adr-003-idempotency-strategy.md)
- [ADR-004: Retry & DLQ Strategy](../docs/decisions/adr-004-retry-and-dlq.md)
