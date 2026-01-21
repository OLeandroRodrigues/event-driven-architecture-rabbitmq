## Architecture Documentation

### 1. Core Concepts

| #   | Document | Status |
|----:|---------|--------|
| 1.1 | [Producers & Consumers](architecture/producer-consumer.md) | ✅ Completed |
| 1.2 | [Exchange / Queue / Binding](architecture/exchange-queue-binding.md) | ✅ Completed |
| 1.3 | [Idempotent Consumers (Inbox Pattern)](architecture/idempotent-consumers-inbox-pattern.md) | ✅ Completed |
| 1.4 | [Retry & Dead Letter Queues (DLQ)](architecture/retry-and-dlq.md) | ✅ Completed |
| 1.5 | [Message Acknowledgements (ACK / NACK)](architecture/acknowledgements.md) | ✅ Completed |
| 1.6 | [Correlation & Observability](architecture/correlation-and-observability.md) | ✅ Completed |
| 1.7 | [Clean Architecture & DDD-Friendly Boundaries](architecture/clean-architecture-ddd.md) | ⏳ Planned |

---

### 2. Architectural Decisions (ADR)

| #   | Decision | Status |
|----:|---------|--------|
| ADR-001 | [Why RabbitMQ](decisions/adr-001-why-rabbitmq.md) | ✅ Completed |
| ADR-002 | [Delivery Guarantees](decisions/adr-002-delivery-guarantees.md) | ✅ Completed |

## Documentation Status Legend

| Status | Meaning |
|------|--------|
| ❌ Not Started | Document has been planned but not written yet |
| ⏳ Planned | Document structure defined, content pending |
| 📝 Draft | Initial version written, subject to changes |
| ✅ Completed | Content is complete and stable |
| ⚠️ Deprecated | Document no longer reflects current architecture |
