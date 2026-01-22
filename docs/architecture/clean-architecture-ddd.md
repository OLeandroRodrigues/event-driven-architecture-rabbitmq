# Clean Architecture & DDD-Friendly Boundaries

## 1. Context

Event-driven systems tend to grow in complexity as services, message flows, retries, and failure scenarios
increase. Without clear boundaries, messaging concerns leak into business logic, and the codebase becomes
hard to test, evolve, and operate.

This repository is an architecture-focused reference implementation. It demonstrates how to integrate
RabbitMQ and production-grade messaging patterns **without compromising domain boundaries**.

The goal is **DDD-friendly, Clean Architecture-aligned design**, where:
- Business logic remains testable and independent
- Infrastructure concerns (RabbitMQ, persistence, serialization) are replaceable
- Messaging is treated as a delivery mechanism, not the domain itself

---

## 2. Problem

How can we design services so that:

- The domain model is not coupled to RabbitMQ concepts (exchanges, queues, ACK/NACK)
- Message serialization and transport concerns do not leak into application use cases
- Idempotency and delivery guarantees can be implemented without polluting domain code
- The system remains testable, maintainable, and evolvable as the number of events grows

The solution must support:
- Event-driven workflows
- At-least-once delivery
- Retry and DLQ
- Correlation and observability

---

## 3. Design Decision

This project follows **Clean Architecture principles** with **DDD-friendly boundaries**:

- The **Domain** layer contains business rules and domain events
- The **Application** layer contains use cases and orchestrates workflows
- The **Infrastructure** layer contains RabbitMQ integration and technical implementations
- The **Presentation/Host** layer wires everything together (DI, configuration, hosted services)

Messaging is an **infrastructure detail**. The domain does not reference RabbitMQ types.

---

## 4. Clean Architecture Layers (in this repo)

### 4.1 Domain (Core Business)
Responsibilities:
- Entities, Value Objects, Aggregates
- Domain Services
- Domain Events (facts that happened)

Rules:
- No dependencies on external frameworks
- No RabbitMQ references
- No serialization concerns

Examples of what belongs here:
- `Order`, `Payment`, `Invoice`
- `OrderCreated` (domain event definition)

---

### 4.2 Application (Use Cases)
Responsibilities:
- Use cases / command handlers
- Orchestration between domain and external services
- Transaction boundaries
- Publishing domain events via abstractions

Rules:
- Depends on Domain
- Depends on abstractions (ports), not implementations
- Contains coordination logic, not RabbitMQ details

Examples:
- `CreateOrderHandler`
- `AuthorizePaymentHandler`

---

### 4.3 Infrastructure (Adapters)
Responsibilities:
- RabbitMQ publisher/consumer implementations
- Serialization (JSON/Protobuf) and message envelopes
- Idempotency store implementation (Inbox)
- Persistence implementations
- External service clients

Rules:
- Implements application ports (interfaces)
- May depend on libraries (RabbitMQ.Client, database SDKs)
- Must not contain domain business rules

---

### 4.4 Host / Presentation (Composition Root)
Responsibilities:
- Dependency Injection wiring
- Configuration (queues, exchanges, retry policies)
- Hosted services / background workers
- Observability setup (logging, tracing)

Rules:
- This is where frameworks live
- This is the only layer that knows the full system shape

---

## 5. DDD-Friendly Messaging

### 5.1 Domain Events vs Integration Events

**Domain Events**
- Represent facts within a bounded context
- Often internal to the service
- Expressed in domain language
- May not be stable for external consumers

**Integration Events**
- Represent facts published for other services
- Treated as public contracts
- Versioned and stable
- Contain only what external consumers need

In practice, a common approach is:
- Raise a domain event inside the domain
- Map it to an integration event in the application layer (or a dedicated mapper)
- Publish the integration event via an infrastructure publisher

This avoids leaking transport and contract concerns into the domain.

---

### 5.2 Bounded Context Boundaries

Each service owns:
- Its domain model
- Its event contracts (integration events)
- Its data and invariants

Other services:
- Consume events
- Build their own models
- Must not share domain entities

This prevents coupling through shared “business classes”.

---

## 6. Where Messaging Fits

### 6.1 Ports and Adapters

The application layer defines ports such as:
- `IEventPublisher`
- `IMessageBus`
- `IInboxStore`

Infrastructure provides adapters:
- `RabbitMqEventPublisher`
- `RabbitMqConsumer`
- `SqlInboxStore` (example)

This enables:
- Unit testing application logic without RabbitMQ
- Replacing RabbitMQ without rewriting business logic
- Keeping domain code clean

---

### 6.2 Message Envelopes and Metadata

Message metadata belongs to the infrastructure boundary:
- `MessageId`
- `CorrelationId`
- `CausationId`
- Headers and delivery info

Domain events should not carry transport metadata.
Instead, infrastructure wraps payloads into an envelope.

---

## 7. Idempotency Without Polluting the Domain

Idempotency is required under at-least-once delivery but should not leak into domain logic.

Recommended approach:
- Perform idempotency checks in the consumer pipeline (infrastructure/application boundary)
- Only call application use cases if the message is not yet processed
- Mark as processed after successful completion

The domain remains unaware of retries and redeliveries.

See:
- [Idempotent Consumers (Inbox Pattern)](idempotent-consumers-inbox-pattern.md)

---

## 8. Failure Handling Boundaries

Failures should be handled at the appropriate layer:

- **Infrastructure**: connectivity, broker errors, serialization, ACK/NACK, retry routing
- **Application**: validation, orchestration, use-case errors
- **Domain**: invariant violations, domain rule enforcement

Avoid:
- throwing RabbitMQ exceptions from domain code
- mixing retry policies with domain rules

See:
- [Retry Policies & Dead Letter Queues (DLQ)](retry-and-dlq.md)
- [Message Acknowledgements](acknowledgements.md)

---

## 9. Trade-offs

### Advantages
- Domain remains clean, testable, and framework-independent
- Messaging remains replaceable and explicit
- Clear ownership boundaries between services
- Easier long-term evolution of the architecture

### Disadvantages
- More abstractions and boilerplate
- Requires disciplined layering and code review enforcement
- Additional mapping between domain and integration events

These trade-offs are accepted to preserve correctness and maintainability in distributed systems.

---

## 10. Common Anti-Patterns

### 10.1 RabbitMQ Types in Domain
Domain code referencing broker objects is a boundary violation.

### 10.2 Domain Entities Shared Across Services
Sharing entities couples bounded contexts and breaks autonomy.

### 10.3 “God Service” Handling All Events
Central consumers that process everything become bottlenecks and create coupling.

### 10.4 Transport Metadata in Domain Events
CorrelationId, delivery tags, retry counts should not pollute domain models.

---

## 11. References

- Robert C. Martin — Clean Architecture
- Eric Evans — Domain-Driven Design
- Vaughn Vernon — Implementing Domain-Driven Design
- Enterprise Integration Patterns — Hohpe & Woolf
- Martin Fowler — Bounded Context and Integration Events
