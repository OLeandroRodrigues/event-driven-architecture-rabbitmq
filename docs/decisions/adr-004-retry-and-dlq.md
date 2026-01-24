# ADR-004: Retry & Dead Letter Queue (DLQ) Strategy

| Field        | Value |
|-------------|-------|
| Status      | Proposed |
| Date        | 2026-01-23 |
| Decision ID | ADR-004 |
| Scope       | Messaging / Reliability |

---

## 1. Context

In Event-Driven Architectures using message brokers such as RabbitMQ, **failures are inevitable**.
Consumers may fail due to:

- Temporary infrastructure issues (network, broker, database)
- Transient downstream dependencies
- Application bugs
- Invalid or unexpected message payloads

Under **At-Least-Once delivery**, failures result in message redelivery.
Without a clear retry and failure-handling strategy, systems risk:

- Infinite retry loops
- Message storms
- Resource exhaustion
- Silent message loss
- Undiagnosable failures

This ADR defines how **retries and irrecoverable failures** are handled in this project.

---

## 2. Problem

How can the system handle message processing failures in a way that:

- Preserves reliability under At-Least-Once delivery
- Differentiates between transient and permanent failures
- Avoids infinite retries and consumer overload
- Enables safe inspection and reprocessing of failed messages
- Maintains observability and traceability

The solution must integrate cleanly with:
- ACK / NACK semantics
- Idempotent consumers
- Correlation and observability
- Clean Architecture boundaries

---

## 3. Design Decision

This project adopts an explicit **Retry + Dead Letter Queue (DLQ)** strategy using RabbitMQ primitives.

The core principles are:

1. **Retries are expected and controlled**
2. **Transient failures trigger retries**
3. **Permanent failures are routed to a DLQ**
4. **No message retries indefinitely**
5. **All failed messages remain observable**

Retries and DLQs are implemented as **infrastructure concerns**, not domain logic.

---

## 4. Retry Strategy

### 4.1 When to Retry

Retries are used for **transient failures**, such as:
- Temporary database unavailability
- Network timeouts
- Downstream service errors
- Resource contention

These failures are assumed to be **recoverable given time**.

---

### 4.2 Retry Mechanism

The retry mechanism typically involves:

- Rejecting (`NACK`) the message
- Routing it to a retry queue
- Applying a delay (via TTL or delayed exchange)
- Re-enqueuing the message to the original queue

Each retry attempt:
- Preserves `MessageId`
- Preserves `CorrelationId`
- Increments a retry counter (header)

---

### 4.3 Retry Limits

Retries are **bounded**.

After a configured number of attempts:
- The message is no longer retried
- It is routed to a Dead Letter Queue (DLQ)

This prevents infinite retry loops.

---

## 5. Dead Letter Queue (DLQ)

### 5.1 Purpose of the DLQ

A DLQ is a **quarantine queue** for messages that cannot be processed successfully.

Typical causes:
- Invalid payload
- Schema incompatibility
- Violated business invariants
- Non-recoverable application errors

The DLQ ensures:
- No message is silently dropped
- Failures can be inspected and analyzed
- Operators retain full visibility

---

### 5.2 Message Integrity in DLQ

Messages sent to the DLQ must retain:
- Original payload
- `MessageId`
- `CorrelationId`
- Retry count
- Failure reason (if available)

This enables deterministic root-cause analysis.

---

## 6. Message Flow Overview

1. Consumer receives a message
2. Processing fails
3. Failure classified as transient or permanent
4. Transient failure:
   - Message is retried (via retry queue)
5. Retry limit exceeded:
   - Message is routed to DLQ
6. Permanent failure:
   - Message is routed directly to DLQ

Throughout the flow:
- CorrelationId is preserved
- Idempotency prevents duplicate side effects

---

## 7. Relationship with Idempotency

Retries and redeliveries naturally produce **duplicate deliveries**.

This strategy relies on:
- **Idempotent Consumers (Inbox Pattern)** to prevent duplicate side effects
- Stable MessageId across retries

Retries are safe because **side effects are protected by idempotency**.

See:
- [ADR-003: Idempotency Strategy](adr-003-idempotency-strategy.md)

---

## 8. Relationship with ACK / NACK

Retry behavior is tightly coupled to acknowledgements:

- Successful processing → `ACK`
- Transient failure → `NACK` (with requeue or dead-lettering)
- Permanent failure → `NACK` routed to DLQ

Consumers must never:
- `ACK` failed messages
- Swallow exceptions silently

See:
- [Message Acknowledgements](../architecture/acknowledgements.md)

---

## 9. Observability and Operations

Retry and DLQ metrics are critical operational signals.

Recommended metrics:
- Retry count per queue
- DLQ message rate
- Processing latency
- Failure rate per consumer

Logs must include:
- CorrelationId
- MessageId
- Retry attempt
- Failure classification

This enables fast detection and diagnosis of systemic issues.

---

## 10. Trade-offs

### Advantages
- Predictable failure handling
- No message loss
- Clear operational visibility
- Safe retries under At-Least-Once delivery
- Compatible with Clean Architecture boundaries

### Disadvantages
- Additional queues and configuration complexity
- Requires operational discipline to monitor DLQs
- Requires clear failure classification logic

These trade-offs are accepted for reliability and debuggability.

---

## 11. Alternatives Considered

### 11.1 Infinite Retries
**Rejected.**
Leads to resource exhaustion and hides permanent failures.

---

### 11.2 Dropping Messages on Failure
**Rejected.**
Violates reliability and makes failures invisible.

---

### 11.3 Exactly-Once Processing
**Rejected.**
Introduces excessive complexity and tight coupling between broker and storage.

---

### 11.4 Handling Retries in Domain Logic
**Rejected.**
Mixes infrastructure concerns with business rules and violates Clean Architecture.

---

## 12. Consequences

By adopting this Retry & DLQ strategy:

- Failures are explicit and observable
- Retries are bounded and safe
- Operators can inspect and reprocess failed messages
- The system behaves predictably under failure
- Messaging reliability aligns with production-grade expectations

This decision completes the reliability model alongside
At-Least-Once delivery and Idempotent Consumers.
