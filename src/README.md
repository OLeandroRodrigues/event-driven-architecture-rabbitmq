# Code Skeleton

This folder contains the initial **Clean Architecture** code skeleton:

- `Domain` — core business rules (no framework dependencies)
- `Application` — use cases and ports (interfaces)
- `Infrastructure` — adapters (RabbitMQ, persistence, observability)
- `Host` — composition root (DI, hosted services)

This commit is intentionally **structure-only**.
