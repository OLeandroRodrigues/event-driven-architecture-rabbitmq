## Architecture Documentation

### 1. Core Concepts

| #   | Document | Status |
|----:|---------|--------|
| 1.1 | [Producers & Consumers](architecture/producer-consumer.md) | ⏳ Planned |
| 1.2 | [Exchange / Queue / Binding](architecture/exchange-queue-binding.md) | ⏳ Planned |
| 1.3 | [Idempotent Consumers (Inbox Pattern)](architecture/idempotency-inbox.md) | ⏳ Planned |
| 1.4 | [Retry & Dead Letter Queues (DLQ)](architecture/retry-and-dlq.md) | ⏳ Planned |
| 1.5 | [Message Acknowledgements (ACK / NACK)](architecture/acknowledgements.md) | ⏳ Planned |
| 1.6 | [Correlation & Observability](architecture/correlation-and-observability.md) | ⏳ Planned |
| 1.7 | [Clean Architecture & DDD-Friendly Boundaries](architecture/clean-architecture-ddd.md) | ⏳ Planned |

---

### 2. Architectural Decisions (ADR)

| #   | Decision | Status |
|----:|---------|--------|
| ADR-001 | [Why RabbitMQ](decisions/adr-001-why-rabbitmq.md) | ✅ Completed |
| ADR-002 | [Delivery Guarantees (At-Least-Once)](decisions/adr-002-at-least-once.md) | ⏳ Planned |
| ADR-003 | [Idempotency Strategy](decisions/adr-003-idempotency-strategy.md) | ⏳ Planned |