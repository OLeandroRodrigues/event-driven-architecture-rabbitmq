# ADR-001: Why RabbitMQ

| Field  | Value |
|--------|-------|
| Status | Proposed |
| Date   | 2026-01-10 |

---

## 1. Context
This project aims to demonstrate a **production-grade Event-Driven Architecture (EDA)** using .NET.
A message broker is required to enable **asynchronous communication**, **service decoupling**, and **reliable message delivery** between independent services.

---

## 2. Problem
Which messaging technology should be used to support:
- Asynchronous event-driven communication
- Reliable delivery with retry and failure handling
- Clear operational semantics for learning and demonstration purposes
- A realistic setup commonly found in production systems

---

## 3. Design Decision
**RabbitMQ** was selected as the message broker for this project.

The decision prioritizes:
- Explicit control over exchanges, queues, bindings, ACK/NACK
- Support for classic messaging patterns (work queues, pub/sub, routing)
- Operational transparency over implicit magic
- Suitability for demonstrating **at-least-once delivery** and **idempotent consumers**

---

## 4. Message Flow
At a high level:

1. Producers publish domain events to a RabbitMQ exchange.
2. Exchanges route messages to queues using routing keys.
3. Consumers receive messages and process them.
4. Messages are acknowledged (ACK) upon successful processing.
5. Failures trigger retries or routing to a Dead Letter Queue (DLQ).

This flow is intentionally explicit to make delivery semantics visible.

---

## 5. Trade-offs

### Pros
- Mature and widely adopted message broker
- Fine-grained control over delivery semantics
- Simple operational model for learning purposes
- Strong ecosystem and documentation

### Cons
- Requires explicit idempotency handling
- Less suitable for high-throughput event streaming compared to Kafka
- Operational overhead compared to managed cloud-native alternatives

---

## 6. Failure Scenarios
- Consumer crashes after message delivery → message redelivery occurs
- Handler throws an exception → message is retried or sent to DLQ
- Network interruptions → at-least-once delivery guarantees still apply

Idempotent consumers mitigate duplicate side effects.

---

## 7. Alternatives Considered

### Apache Kafka
- Strong for event streaming and replay
- Higher operational and conceptual complexity
- Less explicit control over per-message acknowledgements

### Cloud-native brokers (e.g., Azure Service Bus, AWS SQS)
- Fully managed
- Abstract away low-level messaging semantics
- Less suitable for educational and architecture-focused goals

---

## 8. Consequences
- The system embraces **at-least-once delivery**
- Idempotency becomes a first-class concern
- Consumers are responsible for handling retries and duplicates
- Messaging infrastructure remains explicit and observable

---

## 9. References
- RabbitMQ Documentation
- Martin Fowler – Event-Driven Architecture
- Enterprise Integration Patterns