# 📌 TaskFlow — Fully Microservices Architecture

## Overview

**TaskFlow** — это полностью микросервисная платформа для управления асинхронными бизнес-процессами с real-time уведомлениями.

Система построена строго по принципам **microservices architecture**:
- каждый сервис имеет **собственную базу данных**;
- отсутствует shared domain model;
- взаимодействие осуществляется **исключительно через события и сообщения**;
- сервисы могут разворачиваться, масштабироваться и обновляться независимо.

---

## Architectural Principles

TaskFlow следует следующим принципам:

1. **Database per Service** — у каждого сервиса своё хранилище
2. **Event-Driven Communication** — нет синхронных вызовов между сервисами
3. **Loose Coupling** — только контракты и события
4. **Autonomous Services** — каждый сервис владеет своей бизнес-логикой
5. **Eventual Consistency** — данные согласуются асинхронно

---

## Service Landscape

### 1️⃣ Task Management Service

**Responsibility:**
- создание задач
- управление жизненным циклом
- приоритеты, квоты, SLA
- API для клиентов

**Owns:**
- Task metadata
- User-task relationship

**Database:**
- PostgreSQL (task-management-db)

**Publishes Events:**
- TaskCreated
- TaskCancelled
- TaskPriorityChanged

---

### 2️⃣ Task Execution Service

**Responsibility:**
- выполнение задач
- retry и backoff
- контроль прогресса
- обработка ошибок

**Owns:**
- Execution state
- Retry state

**Database:**
- PostgreSQL (task-execution-db)

**Consumes Events:**
- TaskCreated
- TaskCancelled

**Publishes Events:**
- TaskStarted
- TaskProgressUpdated
- TaskCompleted
- TaskFailed

---

### 3️⃣ Notification Service

**Responsibility:**
- доставка real-time уведомлений
- хранение истории уведомлений

**Database:**
- PostgreSQL (notifications-db)

**Consumes Events:**
- TaskStarted
- TaskProgressUpdated
- TaskCompleted
- TaskFailed

---

### 4️⃣ Scheduler Service

**Responsibility:**
- отложенные задачи
- cron-like бизнес-правила
- time-based эскалация приоритетов

**Database:**
- Redis / PostgreSQL (scheduler-db)

**Publishes Events:**
- ScheduledTaskTriggered

---

## Communication Model

### Event Bus

Все сервисы взаимодействуют через **event bus**:

- Redis Streams / Kafka (логический уровень)
- at-least-once delivery
- idempotent consumers

❌ Запрещено:
- direct HTTP calls service-to-service
- shared libraries с domain моделями

---

## Event Contracts

### TaskCreated
```json
{
  "eventId": "uuid",
  "taskId": "uuid",
  "type": "Report",
  "priority": "High",
  "createdAt": "timestamp"
}
```

### TaskCompleted
```json
{
  "eventId": "uuid",
  "taskId": "uuid",
  "resultLocation": "string",
  "completedAt": "timestamp"
}
```

Контракты версионируются и обратно совместимы.

---

## Data Ownership

| Service | Owns Data |
|------|-----------|
| Task Management | Task metadata, SLA, priority |
| Task Execution | Execution state, progress |
| Notification | Delivery state |
| Scheduler | Time rules |

Сервисы **не читают чужие базы данных**.

---

## Consistency Model

Система использует **eventual consistency**:

- Task Management знает о результате через события
- временные расхождения допустимы
- бизнес-решения принимаются асинхронно

Для защиты:
- idempotency keys
- deduplication
- retry with backoff

---

## Deployment Model

Каждый сервис:
- имеет собственный Dockerfile
- разворачивается независимо
- масштабируется независимо

Пример:
```
services:
  task-management
  task-execution
  notification
  scheduler
```

---

## Observability

Каждый сервис предоставляет:
- health endpoint
- structured logs
- correlationId

Трейсинг осуществляется через eventId.

---

## Why This Is Fully Microservices

Система удовлетворяет всем условиям microservices architecture:

- нет shared state
- нет shared database
- нет shared domain
- только async communication
- независимый lifecycle сервисов

---

## When This Architecture Makes Sense

Данная архитектура оправдана, если:
- требуется высокая масштабируемость
- команды работают независимо
- система растёт по функциональности

Для pet-project это **осознанно сложный**, но показательный вариант.

---

End of fully microservices documentation.