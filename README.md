# 🚀 TaskFlow - Microservices Task Management System

Полностью микросервисная платформа для управления асинхронными бизнес-процессами с **real-time уведомлениями**.

## 📐 Архитектура

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────────┐
│    Web UI       │────▶│  Task Management │────▶│  Task Execution     │
│  (SignalR)      │     │      API :5000   │     │     Worker          │
└────────┬────────┘     └──────────────────┘     └─────────────────────┘
         │                     │                          │
         │                     │ TaskCreatedEvent         │ TaskStarted/Progress/
         │                     ▼                          │ Completed Events
         │               ┌──────────┐                     │
         │               │ RabbitMQ │◀────────────────────┘
         │               └──────────┘
         │                     │
         │                     ▼
         │            ┌─────────────────┐
         └───────────▶│  Notification   │
           SignalR    │  Service :5002  │
                      └─────────────────┘
```

## 🏗️ Реализованные этапы

| Этап | Сервис | Описание |
|------|--------|----------|
| ✅ 1 | Task Management API | REST API для создания/управления задачами |
| ✅ 2 | Shared Infrastructure | Контракты событий, EventBus конфигурация |
| ✅ 3 | Task Execution Worker | Выполнение задач с прогрессом и retry |
| ✅ 4 | Notification Service | Real-time уведомления через SignalR |

## 📁 Структура проекта

```
TaskFlow/
├── services/
│   ├── task-management/          # REST API (порт 5000)
│   ├── task-execution/           # Worker Service
│   └── notification/             # SignalR Hub (порт 5002)
├── shared/
│   ├── TaskFlow.Contracts/       # Интерфейсы событий
│   └── TaskFlow.Infrastructure/  # EventBus, Idempotency
├── infrastructure/
│   └── docker-compose.yml        # PostgreSQL x2 + RabbitMQ
├── web/
│   └── index.html                # Web UI с SignalR
└── scripts/
    └── local-dev.ps1
```

## 🚀 Быстрый старт

### 1. Запустить инфраструктуру
```powershell
docker-compose -f infrastructure/docker-compose.yml up -d
```

### 2. Применить миграции
```powershell
# Task Management
dotnet ef database update --project services/task-management/src/TaskManagement.Infrastructure --startup-project services/task-management/src/TaskManagement.Api

# Task Execution
dotnet ef database update --project services/task-execution/src/TaskExecution.Infrastructure --startup-project services/task-execution/src/TaskExecution.Worker
```

### 3. Запустить сервисы (3 терминала)

**Terminal 1 - Task Management API:**
```bash
cd services/task-management/src/TaskManagement.Api
dotnet run
```

**Terminal 2 - Task Execution Worker:**
```bash
cd services/task-execution/src/TaskExecution.Worker
dotnet run
```

**Terminal 3 - Notification Service:**
```bash
cd services/notification/src/Notification.Api
dotnet run
```

### 4. Открыть Web UI
Открой `web/index.html` в браузере

## 🔌 Порты

| Сервис | Порт |
|--------|------|
| Task Management API | 5000 |
| Notification Service (SignalR) | 5002 |
| PostgreSQL (Task Management) | 5432 |
| PostgreSQL (Task Execution) | 5433 |
| RabbitMQ | 5672 |
| RabbitMQ Management | 15672 |

## 🔄 Поток событий

1. **Создание задачи** → Task Management API
2. **TaskCreatedEvent** → RabbitMQ
3. **Task Execution Worker** получает событие
4. **TaskStartedEvent** → Notification Service → SignalR → Web UI
5. **TaskProgressUpdatedEvent** (10%, 30%, 60%, 80%) → Web UI обновляется
6. **TaskCompletedEvent** → Web UI показывает "Completed"

## 📡 SignalR Events

```javascript
connection.on("TaskCreated", notification => { ... });
connection.on("TaskStarted", notification => { ... });
connection.on("TaskProgress", notification => { ... });
connection.on("TaskCompleted", notification => { ... });
connection.on("TaskFailed", notification => { ... });
connection.on("TaskCancelled", notification => { ... });
```

## 🧪 Тестирование

1. Открой `web/index.html`
2. Проверь статусы: **API Connected** и **SignalR Connected**
3. Создай задачу типа "Report"
4. Наблюдай прогресс-бар в реальном времени!

## 🛠️ Технологический стек

- **.NET 8**
- **ASP.NET Core** (Web API, SignalR)
- **Entity Framework Core 8** + PostgreSQL
- **MassTransit 8.2** + RabbitMQ
- **MediatR** (CQRS)
- **FluentValidation**

## 📋 Следующие этапы

- [ ] Этап 5: Scheduler Service (отложенные задачи)
- [ ] Этап 6: Observability (Serilog, OpenTelemetry)
- [ ] Этап 7: Docker Compose для всех сервисов
- [ ] Этап 8: API Gateway (YARP)

## 📝 Лицензия

MIT
