# Delivery Guarantees (At-Least-Once)

## 1. Context

In distributed and event-driven systems, message delivery is inherently unreliable.
Failures such as network interruptions, broker restarts, consumer crashes, or timeouts
may occur at any point during message processing.

Because of these uncertainties, messaging systems must define **explicit delivery guarantees**
that describe how messages behave in the presence of failures.

This project adopts the **at-least-once delivery guarantee**, which is the most common
and realistic model used in production-grade event-driven architectures.

---

## 2. Problem

How can the system ensure reliable message processing when:

- Messages may be delivered more than once
- Consumers may crash before acknowledging messages
- Network failures may occur after processing but before acknowledgement
- Message loss must be avoided at all costs

At the same time, the solution must:
- Make delivery semantics explicit
- Avoid hidden or implicit guarantees
- Reflect real-world production behavior

---

## 3. Design Decision

This project explicitly adopts **at-least-once delivery semantics**.

Under this model:
- Every message is delivered **one or more times**
- Duplicate delivery is possible
- Message loss is avoided as long as queues and exchanges are durable

To safely operate under at-least-once delivery, the system relies on:
- **Manual message acknowledgements (ACK / NACK)**
- **Retry and Dead Letter Queue (DLQ) strategies**
- **Idempotent consumers (Inbox Pattern)**

Exactly-once delivery is **not assumed**.

---

## 4. At-Least-Once Semantics Explained

At-least-once delivery guarantees that:
- A message will not be silently dropped
- If a failure occurs, the broker may redeliver the message
- Consumers must be prepared to handle duplicates

This model favors **reliability over convenience** and makes failure behavior
explicit and observable.

---

## 5. Message Flow

A simplified message lifecycle under at-least-once delivery:

1. A producer publishes an event to a RabbitMQ exchange
2. The message is persisted in a durable queue
3. A consumer receives the message
4. The consumer processes the message
5. If processing succeeds → the consumer sends an **ACK**
6. If processing fails or the consumer crashes:
   - The message is redelivered
   - Or routed to a retry queue
   - Eventually sent to a Dead Letter Queue (DLQ)

This behavior is intentional and visible by design.

---

## 6. Failure Scenarios

### 6.1 Consumer crash before processing
- Message remains unacknowledged
- Broker redelivers the message
- Processing resumes safely

### 6.2 Consumer crash after processing but before ACK
- Broker assumes processing did not complete
- Message is redelivered
- Duplicate delivery occurs

### 6.3 Network failure during acknowledgement
- ACK may not reach the broker
- Message is redelivered
- Consumer receives the same message again

These scenarios are expected under at-least-once delivery.

---

## 7. Relationship with Idempotency

At-least-once delivery **requires idempotent consumers**.

While at-least-once guarantees that messages are delivered,
it does **not** guarantee that messages are processed only once.

This project enforces idempotency using the **Inbox Pattern**:
- Each message carries a unique `eventId`
- Consumers record processed message IDs
- Duplicate deliveries are safely ignored

> At-least-once guarantees delivery.  
> Idempotency guarantees correct side effects.

See:
- [Idempotent Consumers (Inbox Pattern)](idempotent-consumers-inbox-pattern.md)

---

## 8. Trade-offs

### Advantages
- Strong reliability guarantees
- No silent message loss
- Explicit and observable failure behavior
- Widely supported by message brokers

### Disadvantages
- Duplicate message delivery is possible
- Consumers must implement idempotency
- Slightly increased consumer complexity

These trade-offs are accepted in exchange for correctness and reliability.

---

## 9. Alternatives Considered

### At-Most-Once Delivery
- Messages are delivered zero or one time
- Message loss is possible
- Simpler consumer logic

Rejected due to unacceptable data loss risk.

### Exactly-Once Delivery
- Appears attractive conceptually
- Extremely difficult to guarantee in distributed systems
- Often relies on hidden coordination or broker-level magic

Rejected in favor of explicit and realistic delivery semantics.

---

## 10. References

- RabbitMQ Documentation – Reliability and Acknowledgements
- Martin Kleppmann – *Designing Data-Intensive Applications*
- Enterprise Integration Patterns
