# event-driven-architecture-rabbitmq

**A practical reference implementation of Event-Driven Architecture (EDA) using RabbitMQ and .NET.**

This repository is **not a generic messaging library**.  
It is an **architecture-focused example** that demonstrates how to design, structure, and operate **event-driven systems in production-like environments**.

---

## 🎯 Goals of this repository

- Demonstrate **Event-Driven Architecture (EDA)** patterns in real-world scenarios  
- Show how to **decouple services** using asynchronous messaging  
- Provide **production-grade patterns**, not toy examples  

---

## 🧠 Architectural Concepts Covered

- Event Producers & Consumers  
- Exchange / Queue / Binding strategies  
- **Idempotent Consumers (Inbox Pattern)**  
- Retry policies & **Dead Letter Queues (DLQ)**  
- Message acknowledgements (**ACK / NACK**)  
- CorrelationId & observability-friendly messaging  
- Clean Architecture / DDD-friendly boundaries  

---

## 🏗 Project Scope

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

## 🛠 Tech Stack

- .NET 8  
- RabbitMQ  
- Docker & Docker Compose  
- C#  
- Clean Architecture principles  

---

## 🧩 Who is this for?

- Software Engineers learning **distributed systems**
- Developers preparing for **senior / staff interviews**
- Architects looking for **realistic EDA examples**
- Engineers migrating from **synchronous to asynchronous systems**

---

## 📚 How to use this repo

- Clone and run locally using **Docker Compose**
- Read the **Architecture Wiki** for design decisions
- Use it as a **reference**, not as a dependency

---

## ⚠️ Important Note

This project intentionally prioritizes **clarity and architecture decisions** over API minimalism.

> Think of it as an **architecture playbook**, not a framework.
