# ADR-002: Delivery Guarantees

| Field        | Value |
|-------------|-------|
| Status      | Accepted |
| Version     | 1.0 |
| Date        | 2026-01-11 |
| Decision ID | ADR-002 |
| Scope       | Messaging / Delivery Semantics |

---

## 1. Context

In event-driven and distributed systems, message delivery is subject to failures such as:
- Network interruptions
- Broker restarts
- Consumer crashes
- Timeouts during message processing

Because failures are unavoidable, the system must explicitly define **delivery guarantees**, making message behavior predictable and observable under failure conditions.

---

## 2. Problem

Which delivery guarantee should be adopted to ensure:
- Reliable message processing
- No silent message loss
- Clear operational semantics
- Realistic production behavior

While also balancing:
- System complexity
- Consumer responsibilities
- Failure handling transparency

---

## 3. Delivery Models Considered

### 3.1 At-Most-Once Delivery ❌ Rejected

**Definition:**  
A message is delivered **zero or one time**. If a failure occurs, the message may be lost.

**Characteristics:**
- No message redelivery
- No duplicates
- Message loss is possible

**Pros:**
- Simple consumer implementation
- No idempotency required

**Cons:**
- Silent message loss
- Unacceptable for business-critical events
- Failures are hard to detect

**Decision:**  
❌ Rejected due to the risk of message loss and lack of reliability guarantees.

---

### 3.2 At-Least-Once Delivery ✅ Accepted

**Definition:**  
A message is delivered **one or more times**. Redelivery may occur under failure scenarios.

**Characteristics:**
- No silent message loss
- Duplicate delivery is possible
- Explicit acknowledgment semantics

**Pros:**
- Strong reliability guarantees
- Widely adopted in production systems
- Clear and observable failure behavior
- Compatible with RabbitMQ delivery semantics

**Cons:**
- Consumers must handle duplicate messages
- Requires idempotent processing

**Decision:**  
✅ **Accepted**.

This project adopts **at-least-once delivery** combined with:
- Manual ACK / NACK
- Retry and Dead Letter Queue (DLQ) strategies
- Idempotent consumers (Inbox Pattern)

This approach favors **correctness and transparency over convenience**.

---

### 3.3 Exactly-Once Delivery ❌ Rejected

**Definition:**  
A message is delivered **exactly one time**, even in the presence of failures.

**Characteristics:**
- No duplicates
- No message loss (in theory)
- Strong coordination between broker and consumer

**Pros:**
- Simplifies consumer logic
- Appears attractive conceptually

**Cons:**
- Extremely difficult to guarantee in distributed systems
- Often relies on hidden broker or framework magic
- Performance and operational complexity
- Rarely truly “exactly once” in practice

**Decision:**  
❌ Rejected in favor of explicit and observable delivery semantics.

---

## 4. Final Decision

This project **explicitly adopts At-Least-Once delivery semantics**.

The following models were evaluated:
- At-Most-Once → ❌ Rejected
- At-Least-Once → ✅ Accepted
- Exactly-Once → ❌ Rejected

The decision prioritizes:
- Reliability over convenience
- Transparency over abstraction
- Explicit failure handling over hidden guarantees

---

## 5. Consequences

As a result of this decision:
- Consumers must be idempotent
- Duplicate message delivery is expected and handled
- Message acknowledgements are explicit (ACK / NACK)
- Retry and DLQ mechanisms are first-class concerns

These consequences are addressed through dedicated architectural patterns and documentation.

---

## 6. Related Documentation

> 🚧 **Work in progress**  
> The documents listed below are part of the planned architecture documentation and are being written incrementally.

- [Delivery Guarantees (At-Least-Once)](../architecture/delivery-guarantees-at-least-once.md)
- [Idempotent Consumers (Inbox Pattern)](../architecture/idempotent-consumers-inbox-pattern.md)
- [Message Acknowledgements (ACK / NACK)](../architecture/message-acknowledgements-ack-nack.md)

---

## 7. References

- RabbitMQ Documentation – Reliability and Acknowledgements
- Martin Kleppmann – *Designing Data-Intensive Applications*
- Enterprise Integration Patterns