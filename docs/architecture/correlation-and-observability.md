# Correlation & Observability

## 1. Context

In distributed and event-driven systems, a single business operation often spans
multiple services, queues, and asynchronous message exchanges.

Without proper observability, it becomes extremely difficult to:
- Trace a request end-to-end
- Debug failures across service boundaries
- Understand system behavior under load
- Correlate logs, metrics, and events

This project treats **correlation and observability as first-class architectural concerns**,
not as afterthoughts.

---

## 2. Problem

How can the system remain observable when:

- Requests cross multiple services asynchronously
- Messages are processed out of order
- Retries and redeliveries occur
- Failures happen far from their original cause

The solution must:
- Preserve context across asynchronous boundaries
- Allow engineers to trace a single operation end-to-end
- Work reliably under at-least-once delivery semantics
- Scale with system complexity

---

## 3. Design Decision

This project adopts **explicit correlation identifiers combined with structured observability**.

The key principles are:
- Every operation has a **CorrelationId**
- Correlation context is propagated across messages
- Logs, metrics, and traces reference the same identifiers
- Observability is consistent across producers and consumers

This enables deterministic tracing even in fully asynchronous flows.

---

## 4. Correlation Identifiers

### 4.1 CorrelationId

A **CorrelationId** uniquely identifies a logical operation or business flow.

Characteristics:
- Generated at the system boundary (API, ingress)
- Immutable
- Propagated across all messages and services
- Included in logs and metrics

Typical formats:
- UUID
- ULID
- Trace-compatible identifiers

---

### 4.2 CausationId

A **CausationId** identifies the immediate cause of an event.

Relationship:
- CorrelationId → tracks the full workflow
- CausationId → tracks the direct parent operation

This distinction helps reconstruct causal chains in complex event flows.

---

## 5. Context Propagation

### 5.1 Message Metadata

Correlation information is propagated using message metadata or headers, not payloads.

Common headers:
- `correlation_id`
- `causation_id`
- `message_id`
- `trace_id` (if applicable)

This keeps domain events clean and transport concerns isolated.

---

### 5.2 Propagation Rules

- Producers must attach correlation metadata to every message
- Consumers must read and propagate the same metadata
- New messages must preserve the original CorrelationId
- New causation relationships generate a new CausationId

Failure to propagate context breaks traceability.

---

## 6. Observability Pillars

### 6.1 Logs

Logs must be:
- Structured (JSON or key-value)
- Machine-readable
- Correlation-aware

Minimum log fields:
- CorrelationId
- MessageId
- Service name
- Operation name
- Timestamp

This enables efficient querying and aggregation.

---

### 6.2 Metrics

Metrics provide high-level system visibility.

Examples:
- Messages processed per second
- Retry counts
- DLQ message counts
- Processing latency

Metrics should be:
- Tagged with service identifiers
- Aggregated independently of CorrelationId
- Used to detect trends and anomalies

---

### 6.3 Traces

Distributed tracing provides end-to-end visibility.

Traces help answer:
- Where did time get spent?
- Where did failures occur?
- Which services were involved?

Correlation identifiers can be integrated with tracing systems
to bridge synchronous and asynchronous workflows.

---

## 7. Message Flow Example

A correlated message flow:

1. API request generates CorrelationId
2. Producer publishes an event with CorrelationId
3. Consumer processes the message and logs with CorrelationId
4. Consumer emits a new event preserving CorrelationId
5. Downstream services continue propagation

All logs and traces can be queried using the same identifier.

---

## 8. Failure Scenarios

### 8.1 Message redelivery
- Same CorrelationId
- Same MessageId
- New delivery attempt

Logs must clearly indicate retries without breaking correlation.

---

### 8.2 Partial failures
- Some services succeed
- Others fail or retry

Correlation allows engineers to understand the full impact
of a single operation.

---

### 8.3 DLQ investigation
- Messages in DLQ retain CorrelationId
- Engineers can trace the full execution history
- Root cause analysis becomes deterministic

---

## 9. Relationship with Delivery Guarantees

Under **at-least-once delivery**:
- Duplicate messages are expected
- Retries and redeliveries occur

Correlation ensures that:
- Duplicates are identifiable
- Retries are observable
- Message lifecycles are traceable

See:
- [Delivery Guarantees (At-Least-Once)](delivery-guarantees-at-least-once.md)

---

## 10. Relationship with Idempotency

Correlation and idempotency complement each other:
- Correlation explains *what happened*
- Idempotency ensures *it happens safely*

Together they enable:
- Safe retries
- Clear debugging
- Reliable operations

See:
- [Idempotent Consumers (Inbox Pattern)](idempotent-consumers-inbox-pattern.md)

---

## 11. Trade-offs

### Advantages
- End-to-end traceability
- Faster debugging and incident response
- Better system understanding
- Strong operational confidence

### Disadvantages
- Additional metadata in messages
- Slight increase in implementation complexity
- Requires discipline across all services

These trade-offs are accepted to preserve system observability at scale.

---

## 12. Alternatives Considered

### Implicit Correlation
Relying on timestamps or log ordering.

Rejected because:
- Breaks under concurrency
- Fails in asynchronous systems
- Does not scale with retries and DLQ

---

## 13. References

- OpenTelemetry Documentation
- Distributed Tracing in Event-Driven Systems
- Martin Fowler – Observability and Correlation
