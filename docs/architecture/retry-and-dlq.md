# Retry Policies & Dead Letter Queues (DLQ)

## 1. Context

In event-driven systems, **failures are expected**.
Transient errors, network issues, unavailable dependencies, or temporary resource exhaustion
can all cause message processing to fail.

A robust architecture must define **explicit retry and failure-handling strategies** to ensure:
- Messages are not silently lost
- Transient failures are retried safely
- Permanent failures are isolated and observable

This project documents and adopts **retry mechanisms combined with Dead Letter Queues (DLQ)**.

---

## 2. Problem

How can the system handle message processing failures when:

- A consumer temporarily fails to process a message
- A dependency (database, external API) is unavailable
- A message is malformed or semantically invalid
- Retrying indefinitely would cause infinite loops

The solution must:
- Differentiate transient from permanent failures
- Avoid blocking healthy consumers
- Preserve delivery guarantees
- Provide visibility into failed messages

---

## 3. Design Decision

This project uses **explicit retry strategies combined with Dead Letter Queues (DLQ)**.

The design follows these principles:
- **Retries are finite**
- **Retry behavior is explicit and observable**
- **Poison messages are isolated**
- **Consumers remain responsive**

Retry and DLQ handling are implemented using RabbitMQ features such as:
- Manual ACK / NACK
- Message TTL (Time-To-Live)
- Dead Letter Exchanges (DLX)

---

## 4. Retry Strategy

### 4.1 Retry for Transient Failures

Transient failures include:
- Temporary network issues
- Short-lived database outages
- Rate limits or timeouts

For these cases:
- The consumer NACKs the message
- The message is requeued or routed to a retry queue
- Processing is attempted again after a delay

Retries are limited to avoid infinite loops.

---

### 4.2 Retry with Delay

Retries are typically implemented using:
- A **retry queue** with a TTL
- A **Dead Letter Exchange (DLX)** pointing back to the original queue

Flow:
1. Message fails processing
2. Message is routed to a retry queue
3. Message waits for the TTL to expire
4. Message is dead-lettered back to the main queue
5. Consumer retries processing

This creates a controlled retry delay without blocking consumers.

---

## 5. Dead Letter Queue (DLQ)

### 5.1 Purpose of DLQ

A **Dead Letter Queue** is used to isolate messages that:
- Have exceeded the maximum number of retries
- Cannot be processed due to permanent errors
- Require manual inspection or intervention

Messages in a DLQ are **not retried automatically**.

---

### 5.2 When Messages Go to DLQ

A message is routed to the DLQ when:
- Retry count exceeds a predefined threshold
- Message validation fails permanently
- Business rules explicitly reject the message

This prevents poison messages from repeatedly failing and consuming resources.

---

## 6. Message Flow Example

A typical retry + DLQ flow:

1. Consumer receives a message
2. Processing fails
3. Consumer NACKs the message
4. Message is routed to a retry queue
5. Message is retried after a delay
6. Retry count increases
7. After max retries:
   - Message is routed to DLQ
   - Processing stops

This flow is explicit and observable.

---

## 7. Failure Scenarios

### 7.1 Transient failure
- Message is retried
- Processing eventually succeeds
- Message is ACKed

### 7.2 Permanent failure
- Message fails consistently
- Retry limit is reached
- Message is sent to DLQ

### 7.3 Poison message
- Message content is invalid
- Retrying does not help
- Message is isolated in DLQ

---

## 8. Relationship with Delivery Guarantees

Retry and DLQ mechanisms operate under **at-least-once delivery semantics**.

This means:
- Messages may be delivered multiple times
- Idempotent consumers are required
- ACK/NACK ordering is critical

See:
- [Delivery Guarantees (At-Least-Once)](delivery-guarantees-at-least-once.md)

---

## 9. Relationship with Idempotency

Because retries imply redelivery:
- Duplicate messages are expected
- Consumers must be idempotent
- Side effects must not be duplicated

The Inbox Pattern ensures safe retries.

See:
- [Idempotent Consumers (Inbox Pattern)](idempotent-consumers-inbox-pattern.md)

---

## 10. Trade-offs

### Advantages
- Strong reliability and fault tolerance
- Clear separation between transient and permanent failures
- Prevents infinite retry loops
- Improves observability and debuggability

### Disadvantages
- Additional queues and configuration
- Increased operational complexity
- Requires careful retry policy design

These trade-offs are accepted to ensure system correctness and resilience.

---

## 11. Alternatives Considered

### Immediate Infinite Retries
- Simple to implement
- Causes resource exhaustion
- Hides permanent failures

Rejected due to operational risk.

---

## 12. References

- RabbitMQ Documentation – Dead Letter Exchanges
- Enterprise Integration Patterns – Retry and DLQ
- Martin Kleppmann – *Designing Data-Intensive Applications*
