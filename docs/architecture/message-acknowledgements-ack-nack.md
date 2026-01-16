# Message Acknowledgements (ACK / NACK)

## 1. Context

In RabbitMQ, message acknowledgements determine **when a message is considered successfully processed**.
Acknowledgements are a core part of reliable messaging and directly impact:
- Delivery guarantees (at-least-once vs at-most-once behavior)
- Retry behavior and redelivery
- Duplicate processing scenarios
- Failure handling transparency

This project uses **manual acknowledgements** to keep delivery semantics explicit and production-like.

---

## 2. Problem

How can consumers safely process messages when:
- Failures may happen during processing
- Consumers may crash before completing work
- Network issues may prevent acknowledgements from reaching the broker
- Messages must not be silently lost

The solution must:
- Make success/failure outcomes explicit
- Support retries and DLQ routing
- Work correctly with at-least-once delivery

---

## 3. Design Decision

This project uses **manual ACK / NACK** handling.

- **ACK** is sent only after the consumer finishes processing successfully.
- **NACK** (or reject) is sent when processing fails, allowing:
  - redelivery (requeue), or
  - routing to retry/DLQ via broker configuration.

Auto-ack is avoided because it can cause silent message loss when consumers fail mid-processing.

---

## 4. ACK / NACK Semantics

### 4.1 ACK
**ACK** tells the broker:
- "This message has been processed successfully."
- The message can be removed from the queue.

### 4.2 NACK (Negative Acknowledgement)
**NACK** tells the broker:
- "This message was not processed successfully."

Depending on configuration, the broker may:
- **Requeue** the message (redeliver later), or
- **Dead-letter** the message (send to DLQ), or
- Discard it (not recommended in most cases)

### 4.3 Reject
Reject is similar to NACK but typically applies to a single message (implementation detail).
Conceptually, for this documentation, reject behaves like a NACK with a requeue decision.

---

## 5. Message Flow

A simplified consumer lifecycle:

1. Consumer receives message from the queue
2. Consumer validates/deserializes the message
3. Consumer performs idempotency check (Inbox Pattern)
4. Consumer executes handler logic
5. If success → **ACK**
6. If failure → **NACK**
   - message may be retried or routed to DLQ

ACK is the final step of a successful processing path.

---

## 6. Failure Scenarios

### 6.1 Consumer crashes before ACK
- The broker does not receive an ACK
- The message remains unacknowledged
- The broker redelivers the message
- **Duplicate delivery is expected** under at-least-once delivery

### 6.2 Consumer processes successfully but ACK is not delivered (network failure)
- Broker assumes the message was not processed
- Message is redelivered
- Idempotency prevents duplicate side effects

### 6.3 Handler throws an exception
- Consumer sends NACK (or the message remains unacknowledged if the consumer crashes)
- Retry policy applies
- Message may eventually be routed to DLQ

---

## 7. Relationship with Delivery Guarantees

ACK/NACK are the mechanism that makes **at-least-once** delivery practical:

- If ACK is sent only after success → duplicates may occur, but loss is avoided
- If ACK is sent too early (or auto-ack) → loss becomes possible

See:
- [Delivery Guarantees (At-Least-Once)](delivery-guarantees-at-least-once.md)

---

## 8. Relationship with Idempotency

With at-least-once delivery, redelivery is expected when ACK is not completed.
Therefore, consumers must be idempotent.

A typical safe processing pipeline is:

1. Receive message
2. Extract `eventId`
3. Inbox check (deduplication)
4. Execute side effects
5. ACK

See:
- [Idempotent Consumers (Inbox Pattern)](idempotent-consumers-inbox-pattern.md)

---

## 9. Trade-offs

### Advantages of manual ACK/NACK
- Explicit and observable delivery semantics
- Avoids silent message loss
- Works naturally with retries and DLQ
- Supports production-grade failure handling

### Disadvantages
- More responsibility in consumer code
- Requires careful ordering (ACK must be last)
- Duplicate deliveries must be expected and handled

These trade-offs are accepted to preserve correctness and reliability.

---

## 10. Alternatives Considered

### Auto-Acknowledgement (Auto-ACK)
Under auto-ack, messages are considered processed as soon as they are delivered.
If a consumer fails mid-processing, the message is lost.

Rejected because it can cause silent message loss and hides failure behavior.

---

## 11. References

- RabbitMQ Documentation – Consumer Acknowledgements
- Enterprise Integration Patterns – Messaging Reliability
- Martin Kleppmann – *Designing Data-Intensive Applications*
