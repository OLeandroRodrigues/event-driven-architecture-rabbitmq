# Producers & Consumers

## 1. Context

In Event-Driven Architecture (EDA), **producers** and **consumers** are the fundamental building blocks.
They enable asynchronous communication by exchanging events through a message broker, allowing services
to remain loosely coupled and independently deployable.

This project adopts a **clear separation of responsibilities** between producers and consumers to
avoid tight coupling and to reflect real-world, production-grade systems.

---

## 2. Problem

How can services communicate asynchronously while ensuring that:
- Producers are not aware of who consumes their events
- Consumers are not tightly coupled to producers
- Failures in one service do not cascade to others
- The system remains scalable and evolvable over time

At the same time, the model must:
- Support multiple consumers per event
- Handle failures and retries gracefully
- Preserve domain boundaries

---

## 3. Design Decision

This project follows the **Producer–Consumer pattern** as defined in Event-Driven Architecture:

- **Producers** emit events representing facts that already occurred
- **Consumers** subscribe to events and react to them independently
- Communication happens exclusively via the message broker
- Producers never invoke consumers directly

This design enforces **temporal and spatial decoupling** between services.

---

## 4. Producer Responsibilities

A producer is responsible for:

- Emitting **domain events** (e.g., `OrderCreated`, `PaymentAuthorized`)
- Publishing events after a successful domain state change
- Including required metadata in the event envelope:
  - `eventId`
  - `correlationId`
  - `occurredAtUtc`
  - event type and version
- Remaining unaware of:
  - how many consumers exist
  - which services will consume the event
  - how consumers process the event

Producers must not assume that an event will be processed immediately or only once.

---

## 5. Consumer Responsibilities

A consumer is responsible for:

- Subscribing to one or more event types
- Processing messages asynchronously
- Handling failures explicitly
- Implementing **idempotency**
- Sending message acknowledgements (ACK / NACK)

Consumers must assume:
- Messages may be delivered more than once
- Processing may fail and be retried
- Ordering is not guaranteed unless explicitly enforced

---

## 6. Message Ownership and Boundaries

In this architecture:
- Producers **own the event schema**
- Consumers **adapt to events**, not the other way around
- Events represent **facts**, not commands

This prevents:
- Breaking changes across services
- Tight coupling through shared business logic
- Implicit dependencies between teams or bounded contexts

Versioning strategies can be applied when event schemas evolve.

---

## 7. Message Flow

A typical producer–consumer interaction:

1. A producer completes a domain operation
2. The producer publishes a domain event to an exchange
3. The broker routes the message to one or more queues
4. Consumers receive the message independently
5. Each consumer processes the message and acknowledges it
6. Failures trigger retries or DLQ routing

The producer does not wait for consumers to complete.

---

## 8. Failure Scenarios

### 8.1 Producer failure after publishing
- Event may already be in the broker
- Consumers may process the event
- System remains consistent due to event semantics

### 8.2 Consumer failure during processing
- Message is redelivered
- Idempotency prevents duplicate side effects

### 8.3 One consumer fails, others succeed
- Each consumer is isolated
- Failures do not affect other consumers
- Partial progress is expected and acceptable

---

## 9. Relationship with Delivery Guarantees

Producer–consumer interaction is governed by **at-least-once delivery semantics**.

This means:
- Producers guarantee event emission
- Consumers guarantee safe processing
- Duplicate delivery is expected

See:
- [Delivery Guarantees (At-Least-Once)](delivery-guarantees-at-least-once.md)

---

## 10. Trade-offs

### Advantages
- Strong service decoupling
- Horizontal scalability
- Independent evolution of services
- Clear failure isolation

### Disadvantages
- Increased system complexity
- Eventual consistency
- Harder end-to-end debugging without observability

These trade-offs are accepted to enable scalable distributed systems.

---

## 11. References

- Enterprise Integration Patterns – Publisher-Subscriber
- Martin Fowler – Event-Driven Architecture
- RabbitMQ Documentation – Messaging Patterns
