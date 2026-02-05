# START HERE

Welcome   
This document is the **recommended starting point** for anyone exploring this repository.

It explains **what this project is**, **who it is for**, and **how to navigate the documentation and codebase**.

---

## What Is This Repository?

This repository is a **reference implementation of Event-Driven Architecture (EDA)** using **RabbitMQ** and **.NET**.

It is **not** a generic messaging library or framework.

Instead, it focuses on:
- Architectural clarity
- Explicit design decisions
- Production-grade messaging patterns
- Real-world trade-offs in distributed systems

Think of it as an **architecture playbook with runnable examples**.

---

## Who Is This For?

This repository is especially useful for:

- Software Engineers learning **event-driven and distributed systems**
- Engineers preparing for **senior / staff interviews**
- Architects looking for **clear EDA reference material**
- Developers migrating from **synchronous to asynchronous** architectures

If you are looking for a plug-and-play RabbitMQ SDK, this repository is **not** the right place.

---

## How This Repository Is Organized

The repository follows a **docs-first approach**.

Architecture decisions are documented **before** code is introduced, and the codebase is expected to
reflect those decisions explicitly.

At a high level, you will find:

- High-level architecture documentation
- Core messaging concepts
- Architectural Decision Records (ADR)
- Code structure documentation
- (Soon) A runnable codebase aligned with the docs

---

## Recommended Reading Order

To get the most value, follow this order:

1. **Architecture Overview**  
   Understand the system at a high level and how components interact.  
   → `docs/architecture/architecture-overview.md`

2. **Code Structure & Architecture Mapping**  
   See how architectural decisions map to code structure and layers.  
   → `docs/architecture/code-structure.md`

3. **Core Messaging Concepts**  
   Dive into producers, consumers, retries, DLQ, idempotency, and observability.  
   → `docs/README.md` (Core Concepts section)

4. **Architectural Decision Records (ADR)**  
   Understand *why* key decisions were made and which alternatives were rejected.  
   → `docs/decisions/`

5. **Code Examples** (when available)  
   Explore how the documented architecture is implemented in code.

---

## Project Status

This repository currently emphasizes **architecture and documentation**.

- Core concepts and ADRs are complete
- Code structure is defined
- Executable code will be added incrementally

Once available, the project will be runnable locally using **Docker Compose**.

Until then, treat this repository as an **architecture reference**, not a finished product.

---

## Design Philosophy

This project intentionally prioritizes:
- Explicit boundaries
- Clear responsibilities
- Reliability over convenience
- Understandable trade-offs

The goal is not minimal code, but **understandable systems**.

---

## Where to Go Next

If this is your first visit:
- Start with the **Architecture Overview**.

If you are interested in design decisions:
- Read the **ADR documents**.

If you want to see how architecture maps to code:
- Jump to **Code Structure & Architecture Mapping**.

---

Enjoy exploring the architecture 
