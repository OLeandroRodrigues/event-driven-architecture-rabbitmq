# Message Acknowledgements

## 1. Context

In message-based systems, acknowledgements define **when a message is considered successfully processed**.
They are a fundamental mechanism for enforcing delivery guarantees and for coordinating retries, redeliveries,
and failure handling.

In RabbitMQ-based architectures, acknowledgements are the primary tool that determines whether a message:
- Is removed from the queue
- Is redelivered
- Is routed to retry or Dead Letter Queues (DLQ)

This project treats acknowledgements as a **first-class architectural concern**.

---

## 2. Problem

How can the system correctly determine message processing outcomes when:

- Consumers may fail during processing
- Network issues may prevent acknowledgements from reaching the broker
- Message processing involves side effects
- Message loss must be avoided

At the same time, the solution must:
- Make success and failure explicit
- Support retries and DLQ strategies
- Work consistently with at-least-once delivery

---

## 3. Design Decision

This project uses **manual message acknowledgements**.

Acknowledgements are sent **explicitly by the consumer** after processing completes.
Automatic acknowledgements are intentionally avoided.

The rules are simple and strict:
- A message is **ACKed only after successful processing**
- A message is **not ACKed** if processing fails
- Failure paths rely on redelivery, retry, or DLQ mechanisms

This ensures that delivery semantics remain **explicit and observable**.

---

## 4. Acknowledgement Types

### 4.1 ACK (Positive Acknowledgement)

An **ACK** indicates that:
- The message was processed successfully
- All required side effects have been applied
- The message can be safely removed from the queue

ACK must always be the **last step** in a successful processing pipeline.

---

### 4.2 NACK (Negative Acknowledgement)

A **NACK** indicates that:
- The message was not processed successfully
- The broker must decide the next action

Depending on configuration, a NACK may:
- Requeue the message for retry
- Route the message to a retry queue
- Dead-letter the message to a DLQ

---

### 4.3 Reject

Reject is conceptually similar to NACK and is typically applied to a single message.
For architectural purposes, reject is treated as a form of negative acknowledgement
with explicit requeue or dead-letter semantics.

---

## 5. Processing Flow

A typical consumer processing flow:

1. Message is delivered to the consumer
2. Message is validated and deserialized
3. Idempotency check is performed
4. Business logic is executed
5. Side effects are applied
6. Message is ACKed

If any step fails:
- The message is not ACKed
- Failure handling mechanisms take over

---

## 6. Failure Scenarios

### 6.1 Consumer crashes before ACK
- The broker does not receive an ACK
- The message remains unacknowledged
- The message is redelivered

### 6.2 Consumer crashes after processing but before ACK
- The broker assumes processing did not complete
- The message is redelivered
- Duplicate delivery occurs

### 6.3 Network failure during ACK
- ACK may not reach the broker
- The message is redelivered
- Idempotency prevents duplicate side effects

These scenarios are expected under at-least-once delivery.

---

## 7. Relationship with Delivery Guarantees

Acknowledgements are the mechanism that enforces **at-least-once delivery semantics**.

- Late ACK → possible duplicates, no loss
- Early ACK → possible message loss

This project prioritizes **correctness over convenience** by ACKing messages only after
processing completes successfully.

See:
- [Delivery Guarantees (At-Least-Once)](delivery-guarantees-at-least-once.md)

---

## 8. Relationship with Idempotency

Because acknowledgements may fail or be delayed:
- Message redelivery is expected
- Consumers must be idempotent
- Side effects must not be duplicated

Idempotency complements acknowledgements by ensuring correctness under redelivery.

See:
- [Idempotent Consumers (Inbox Pattern)](idempotent-consumers-inbox-pattern.md)

---

## 9. Trade-offs

### Advantages
- Explicit and observable processing outcomes
- No silent message loss
- Works naturally with retries and DLQ
- Clear failure semantics

### Disadvantages
- Increased responsibility in consumer logic
- Requires strict processing order discipline
- Duplicate deliveries must be handled

These trade-offs are accepted to preserve reliability and transparency.

---

## 10. Alternatives Considered

### Automatic Acknowledgements (Auto-ACK)
Messages are acknowledged immediately upon delivery.

Rejected because:
- Consumer crashes cause silent message loss
- Failures become invisible
- Delivery guarantees are weakened

---

## 11. References

- RabbitMQ Documentation – Consumer Acknowledgements
- Enterprise Integration Patterns – Messaging Reliability
- Martin Kleppmann – *Designing Data-Intensive Applications*
