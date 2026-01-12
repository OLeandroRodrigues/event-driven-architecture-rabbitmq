# Architecture Overview

## 1. Purpose

This repository is a **reference implementation of Event-Driven Architecture (EDA)** using **RabbitMQ** and **.NET**.
Its goal is to demonstrate **production-grade messaging patterns** (idempotency, retries, DLQ, ACK/NACK, observability) while enforcing **clean architectural boundaries** (Clean Architecture with DDD-friendly design).

This is **not** a generic messaging SDK.  
It is an **architecture playbook** with runnable, production-like examples.

---

## 2. High-Level Architecture

The system is composed of **independent services** communicating asynchronously through domain events.

### 2.1 Main Building Blocks
- **Producer services**: publish **domain events** (business facts that occurred)
- **RabbitMQ**: message broker (exchanges, queues, bindings)
- **Consumer services**: subscribe to events and execute message handlers
- **Idempotency store (Inbox Pattern)**: prevents duplicate side effects
- **Observability**: structured logging and CorrelationId propagation

---