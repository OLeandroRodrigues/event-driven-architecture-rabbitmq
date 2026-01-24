# ADR-003: Idempotency Strategy (Inbox Pattern)

| Field        | Value |
|-------------|-------|
| Status      | Proposed |
| Date        | 2026-01-23 |
| Decision ID | ADR-003 |
| Scope       | Messaging / Idempotency |

---

## 1. Context

This repository demonstrates **production-grade messaging patterns** for Event-Driven Architecture (EDA)
using RabbitMQ and .NET.

Under RabbitMQ (and most brokers), reliable delivery is typically implemented as **At-Least-Once**:
messages may be delivered **more than once** due to retries, consumer crashes, network issues, or ACK failures.

This makes **idempotency** mandatory for consumers that perform side effects such as:
- Writing to a database
- Calling external services
- Publishing follow-up messages
- Triggering business workflows

Without an explicit idempotency strategy, duplicate message deliveries can cause:
- Duplicate records
- Double-charges
- Repeated notifications
- Inconsistent downstream state

This ADR defines the idempotency approach used in this project.

---

## 2. Problem

How can we ensure that message processing is **safe under duplicate deliveries** while keeping:

- Domain logic clean (no RabbitMQ concepts in the domain)
- Consumer behavior deterministic and observable
- Retry and DLQ strategies compatible with At-Least-Once delivery
- The solution easy to reason about and test

The system must guarantee:

- A message is processed **0 or 1 time** in terms of side effects
- Duplicate deliveries do not repeat side effects
- Failures during processing do not lead to partial or inconsistent outcomes

---

## 3. Design Decision

This project adopts **Idempotent Consumers using the Inbox Pattern**.

### Core rules

1. Every consumed message must have a stable **MessageId** (globally unique).
2. The consumer stores MessageId in an **Inbox Store** (durable) to track processing.
3. If a message with the same MessageId is received again:
   - The consumer **does not reapply side effects**
   - The consumer **ACKs the message** (or treats it as successfully processed)
4. The Inbox Store is updated only after processing completes successfully.

### What this achieves

- Duplicate deliveries become safe
- Retries and redeliveries are compatible with correctness
- Consumers can remain stateless while idempotency is enforced via storage

This is the primary idempotency mechanism in this repository.

---

## 4. Message Flow

### 4.1 Consumer processing pipeline (high level)

1. Receive message
2. Extract `MessageId` and `CorrelationId`
3. Check Inbox Store:
   - If **already processed** → ACK and stop
4. Execute handler / business logic
5. Commit side effects
6. Mark message as processed in Inbox Store
7. ACK

### 4.2 What happens on duplicates

- The same MessageId arrives again (redelivery/retry)
- Inbox detects it as processed
- Consumer skips side effects
- Consumer ACKs

This makes duplicates “cheap” and safe.

---

## 5. Trade-offs

### Advantages

- Works naturally with **At-Least-Once** delivery
- Simple operational model: duplicates are expected and handled deterministically
- Keeps domain clean: idempotency handled at consumer boundary
- Compatible with Retry/DLQ strategies
- Observable: duplicates can be counted and logged

### Disadvantages

- Requires a durable store (database) for Inbox
- Introduces extra read/write per message (overhead)
- Requires careful definition of MessageId semantics
- Requires retention strategy (cleanup/TTL) for Inbox records

These trade-offs are accepted for correctness and production reliability.

---

## 6. Failure Scenarios

### 6.1 Consumer crash before ACK
- Message will be redelivered
- Inbox record may or may not exist depending on crash timing
- If record exists → duplicate is skipped
- If record does not exist → message is processed again (expected under At-Least-Once)
  - Side effects remain safe if the Inbox is updated only after commit

### 6.2 Crash after side effects but before marking Inbox
- Message redelivered
- Without Inbox marking, side effects could repeat
- Mitigation:
  - Prefer committing side effects and inbox marking in a single transactional boundary where possible
  - If not possible, ensure the domain operation itself is idempotent (e.g., natural keys, UPSERT)

### 6.3 Poison message
- Repeated failures lead to retry exhaustion
- Message ends in DLQ with CorrelationId/MessageId preserved
- Inbox prevents duplicates from compounding side effects during retries

---

## 7. References / Related Documentation

- [Delivery Guarantees (At-Least-Once)](../architecture/delivery-guarantees-at-least-once.md)
- [Idempotent Consumers (Inbox Pattern)](../architecture/idempotent-consumers-inbox-pattern.md)
- [Retry Policies & Dead Letter Queues (DLQ)](../architecture/retry-and-dlq.md)
- [Message Acknowledgements](../architecture/acknowledgements.md)
- [Correlation & Observability](../architecture/correlation-and-observability.md)

---

## 8. Alternatives Considered

### 8.1 No explicit idempotency
**Rejected.**
Duplicates would cause repeated side effects and inconsistent state.

### 8.2 Producer-side deduplication only
**Rejected.**
Duplicates can be introduced by the broker/consumer lifecycle (redelivery), not only producers.

### 8.3 Exactly-once delivery guarantees
**Rejected.**
Exactly-once semantics across distributed systems are complex and typically require tighter coupling
between broker, storage, and processing. This repository intentionally adopts At-Least-Once + Idempotency
for practical, production-grade design.

### 8.4 Outbox Pattern (instead of Inbox)
**Not selected as the primary strategy** for consumer idempotency.

Outbox is valuable for **reliable publishing** of events from a database transaction.
Inbox is the direct fit for **consumer-side deduplication** under At-Least-Once delivery.

This repository may document Outbox separately as an optional complementary pattern.

---

## 9. Consequences

By adopting the Inbox Pattern:

- Consumers must treat duplicates as normal
- Every message must carry a stable MessageId
- The system gains safe retries and redelivery behavior
- Operational tooling should track duplicate rate and inbox growth
- The architecture remains DDD-friendly and cleanly layered

This decision enables production-grade correctness while keeping messaging boundaries explicit.
