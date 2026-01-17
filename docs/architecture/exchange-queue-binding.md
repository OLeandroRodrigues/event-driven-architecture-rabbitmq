# Exchange / Queue / Binding Strategies

## 1. Context

In RabbitMQ, message routing is explicitly defined through **exchanges**, **queues**, and **bindings**.
These concepts determine **how messages flow** from producers to consumers and are central to
the scalability, flexibility, and reliability of an Event-Driven Architecture.

This project intentionally makes exchange, queue, and binding strategies **explicit and visible**
to avoid hidden routing behavior and to reflect real-world production systems.

---

## 2. Problem

How can events be routed so that:

- Producers remain unaware of consumers
- Multiple consumers can react to the same event
- Routing rules are explicit and easy to reason about
- The system can evolve without breaking existing consumers

At the same time, the solution must:
- Support different consumption patterns
- Avoid tight coupling through shared queues
- Be observable and debuggable

---

## 3. Core RabbitMQ Concepts

### 3.1 Exchange

An **exchange** receives messages from producers and routes them to queues.
Exchanges never store messages — they only decide **where messages should go**.

Common exchange types:
- **Direct**
Routes messages to queues whose binding key exactly matches the message routing key.
Useful when messages must be delivered to specific consumers based on a precise identifier.
Typical use case: point-to-point routing.
- **Fanout**
Routes messages to all bound queues, ignoring routing keys entirely.
Best suited for broadcast scenarios, where every consumer should receive the same event.
Typical use case: system-wide notifications or cache invalidation events.
- **Topic**
Routes messages based on pattern matching between routing keys and binding keys.
Supports wildcards (*, #) and provides the most flexible routing strategy.
Typical use case: event-driven systems with multiple consumers reacting to subsets of events.
- **Headers**
Routes messages based on message headers instead of routing keys.
Provides flexible routing but introduces higher complexity and is less commonly used.
Typical use case: advanced routing scenarios requiring metadata-based filtering.
---

### 3.2 Queue

A **queue** stores messages until they are consumed.
Queues are the unit of:
- Message persistence
- Load balancing
- Delivery guarantees

Consumers always read from queues, never directly from exchanges.

---

### 3.3 Binding

A **binding** defines the relationship between an exchange and a queue.
It specifies:
- Which queue should receive messages
- Under which routing conditions (e.g., routing keys)

Bindings make routing rules **explicit and declarative**.

---

## 4. Design Decision

This project primarily uses **Topic Exchanges**.

Topic exchanges allow messages to be routed based on **routing key patterns**, providing:
- Fine-grained control
- Clear intent
- Flexibility as the system grows

Routing keys follow a semantic, domain-oriented structure, such as:

payment.authorized  
order.created  
invoice.generated

---

## 5. Exchange Strategy

### 5.1 One Exchange per Domain or Bounded Context

A common pattern used in this project is:
- One exchange per domain or bounded context

Example:
payments.exchange  
orders.exchange

This avoids:
- Overloaded global exchanges
- Cross-domain coupling
- Ambiguous routing rules

---

## 6. Queue Strategy

### 6.1 One Queue per Consumer Group

Each consumer (or consumer group) has its **own queue**.

This ensures:
- Independent scaling
- Independent failure handling
- No competition between unrelated consumers

Example:
payments.authorized.billing.queue  
payments.authorized.notifications.queue

---

## 7. Binding Strategy

### 7.1 Explicit Routing Keys

Queues are bound to exchanges using explicit routing keys or patterns.

Examples:
payment.authorized  
payment.*  
order.created

This allows:
- Multiple consumers to subscribe to the same event
- New consumers to be added without changing producers
- Safe evolution of routing rules

---

## 8. Message Flow Example

A typical routing flow:

1. Producer publishes an event with routing key `payment.authorized`
2. The message is sent to a topic exchange
3. The exchange evaluates bindings
4. Matching queues receive a copy of the message
5. Consumers read from their own queues

Producers do not know which queues or consumers exist.

---

## 9. Failure and Isolation Scenarios

### 9.1 One consumer is slow or failing
- Its queue grows independently
- Other consumers are not affected

### 9.2 One queue is misconfigured
- Only that consumer is impacted
- Routing to other queues continues normally

This isolation is a key advantage of explicit queue-per-consumer design.

---

## 10. Trade-offs

### Advantages
- Clear and explicit routing
- Strong service decoupling
- Independent scaling and failure isolation
- Easy onboarding of new consumers

### Disadvantages
- More queues and bindings to manage
- Requires naming conventions and discipline
- Slightly higher operational overhead

These trade-offs are accepted to preserve clarity and evolvability.

---

## 11. Relationship with Other Concepts

Exchange, queue, and binding strategies work together with:
- At-least-once delivery guarantees
- Idempotent consumers
- ACK / NACK handling
- Retry and DLQ mechanisms

See:
- Producers & Consumers
- Delivery Guarantees (At-Least-Once)
- Message Acknowledgements (ACK / NACK)

---

## 12. References

- RabbitMQ Documentation – Exchanges, Queues, and Bindings
- Enterprise Integration Patterns
- Martin Fowler – Event-Driven Architecture
