# Idempotent Consumers (Inbox Pattern)

## 1. Context

In event-driven architectures with **at-least-once delivery guarantees**, message redelivery is expected.
Failures such as consumer crashes, network interruptions, or timeouts can cause the same message to be delivered more than once.

Without additional safeguards, duplicate message delivery may lead to:
- Duplicate database writes
- Inconsistent state
- Repeated side effects (e.g., sending emails, charging payments)

To safely operate under these conditions, consumers must be **idempotent**.

---

## 2. Problem

How can consumers safely process messages when:
- The same event may be delivered multiple times
- The broker cannot guarantee exactly-once delivery
- Side effects must be applied **only once**

At the same time, the solution must:
- Be observable and explicit
- Work reliably under retries and redeliveries
- Avoid hidden broker-level guarantees

---

## 3. Design Decision

This project implements **idempotent consumers using the Inbox Pattern**.

Each consumer maintains an **Inbox (or ProcessedMessages) store** that records which messages have already been processed.
Before executing any side effects, the consumer checks whether the message was processed previously.

If the message is already present in the Inbox:
- The message is acknowledged (ACK)
- No further processing occurs

If the message is not present:
- The consumer processes the message
- The message is recorded as processed
- The message is acknowledged

This guarantees **process-once side effects**, even when messages are delivered multiple times.

---

## 4. Inbox Pattern Overview

### 4.1 Core Idea

The Inbox Pattern relies on:
- A **unique message identifier** (`eventId`)
- A **durable storage** for processed message IDs
- An **atomic check-and-write** operation

The Inbox acts as a **deduplication barrier** at the consumer boundary.

---

### 4.2 Typical Inbox Schema

A typical Inbox table may contain:

- `MessageId` (Primary Key) → `eventId`
- `Consumer` (optional)
- `ProcessedAtUtc`
- `Status` (Processed / Failed)
- `PayloadHash` (optional)
- `ExpiresAtUtc` (optional, for cleanup)

A unique constraint on `MessageId` enforces idempotency.

---

## 5. Message Flow

A simplified consumer flow using the Inbox Pattern:

1. Consumer receives a message from the queue
2. Extract `eventId` from the message envelope
3. Attempt to record `eventId` in the Inbox
   - If the insert fails (duplicate) → message already processed
4. If duplicate:
   - ACK message
   - Exit handler
5. If new:
   - Execute business logic
   - Apply side effects
   - Mark message as processed
   - ACK message

This flow ensures that side effects are applied **at most once**.

---

## 6. Failure Scenarios

### 6.1 Consumer crash before processing
- Message is redelivered
- Inbox does not contain `eventId`
- Processing resumes safely

### 6.2 Consumer crash after side effects but before ACK
- Message is redelivered
- Inbox already contains `eventId`
- Side effects are not duplicated

### 6.3 Handler throws an exception
- Message is NACKed or retried
- Inbox entry may remain in a failed or pending state
- Retry logic determines next steps

---

## 7. Relationship with Delivery Guarantees

The Inbox Pattern complements **at-least-once delivery**.

- At-least-once ensures messages are not lost
- Inbox Pattern ensures duplicates do not cause harm

> At-least-once guarantees delivery.  
> Idempotency guarantees correctness.

See:
- [Delivery Guarantees (At-Least-Once)](delivery-guarantees-at-least-once.md)

---

## 8. Trade-offs

### Advantages
- Strong protection against duplicate side effects
- Explicit and observable behavior
- Works with any message broker
- Simple conceptual model

### Disadvantages
- Requires additional storage
- Adds complexity to consumer logic
- Cleanup of Inbox entries must be managed

These trade-offs are acceptable in exchange for reliability and correctness.

---

## 9. Alternatives Considered

### Broker-Level Exactly-Once Semantics
- Often complex and opaque
- Hard to reason about under failures
- Tightly coupled to specific technologies

Rejected in favor of explicit consumer-side guarantees.

### Stateless Consumers Without Idempotency
- Simpler implementation
- Unsafe under retries and redelivery

Rejected due to correctness risks.

---

## 10. References

- Enterprise Integration Patterns – Idempotent Consumer
- Martin Kleppmann – *Designing Data-Intensive Applications*
- RabbitMQ Documentation – Reliability Patterns