# event-driven-architecture-rabbitmq

**A practical reference implementation of Event-Driven Architecture (EDA) using RabbitMQ and .NET.**

This repository is **not a generic messaging library**.  
It is an **architecture-focused example** that demonstrates how to design, structure, and operate **event-driven systems in production-like environments**.

---

## Goals of this repository

- Demonstrate **Event-Driven Architecture (EDA)** patterns in real-world scenarios
- Show how to **decouple services** using asynchronous messaging
- Provide **production-grade patterns**, not toy examples

---

## Architectural Concepts Covered

- Event Producers & Consumers
- Exchange / Queue / Binding strategies
- **Idempotent Consumers (Inbox Pattern)**
- Retry policies & **Dead Letter Queues (DLQ)**
- Message acknowledgements (**ACK / NACK**)
- CorrelationId & observability-friendly messaging
- Clean Architecture / DDD-friendly boundaries

---

## Architecture & Design

This repository places a strong emphasis on **explicit architectural design**.

Key architectural and design aspects are documented, including:
- High-level system architecture and service boundaries
- Code structure and layer responsibilities
- Messaging flow and interaction patterns
- Design trade-offs and architectural decisions

See:
- [Architecture Overview](docs/architecture/architecture-overview.md)
- [Code Structure & Architecture Mapping](docs/architecture/code-structure.md)
- [Architectural Decision Records (ADR)](docs/decisions)

## Project Scope

This repository focuses on **how services interact**, not on business complexity.

### You will find:
- A **messaging abstraction layer**
- One or more **sample services**
- RabbitMQ infrastructure configuration
- Failure scenarios and **safe reprocessing strategies**

### What you won’t find:
- A generic RabbitMQ SDK
- Hidden magic or opinionated frameworks
- Business-heavy domains

---

## Who is this for?

- Software Engineers learning **distributed systems**
- Developers preparing for **senior / staff interviews**
- Architects looking for **realistic EDA examples**
- Engineers migrating from **synchronous to asynchronous systems**

---

## Architecture Documentation

All architectural decisions, trade-offs, and messaging patterns are **fully documented**.

## Start Here

If this is your first time exploring this repository, start here:

**[START HERE – How to navigate this repository](docs/START-HERE.md)**

This guide explains the project goals, architecture-first approach, and the recommended reading order.

---

## How to use this repo

This repository currently focuses on **architecture and documentation**.

Executable code and runnable examples will be added incrementally.  
Once available, the project will be runnable **locally via Docker Compose**.

For now, use this repository as an **architecture reference**.

---

## Tech Stack

- .NET 8
- RabbitMQ
- Docker & Docker Compose
- C#
- Clean Architecture principles

---

## ⚠️ Important Note

This project intentionally prioritizes **clarity and architecture decisions** over API minimalism.

> Think of it as an **architecture playbook**, not a framework.