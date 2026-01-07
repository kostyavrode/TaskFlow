# 🚀 TaskFlow - Microservices Task Management System

Полностью микросервисная платформа для управления асинхронными бизнес-процессами с real-time уведомлениями.

## 📐 Архитектура

Система построена по принципам **microservices architecture**:
- Database per Service
- Event-Driven Communication
- Loose Coupling
- Autonomous Services
- Eventual Consistency

## 🏗️ Текущее состояние

**Реализован Этап 1: Task Management Service** (полностью)

### Структура проекта

```
TaskFlow/
├── services/
│   └── task-management/
│       ├── src/
│       │   ├── TaskManagement.Domain/        # Entities, Value Objects, Events
│       │   ├── TaskManagement.Application/   # Commands, Queries, Handlers (CQRS)
│       │   ├── TaskManagement.Infrastructure/ # EF Core, MassTransit, Repositories
│       │   └── TaskManagement.Api/           # REST API Controllers
├── infrastructure/
│   └── docker-compose.yml                     # PostgreSQL + RabbitMQ
├── scripts/
│   └── local-dev.ps1                          # Скрипт запуска окружения
└── docs/                                      # Архитектурная документация
```

## 🛠️ Технологический стек

- **.NET 8** - основной фреймворк
- **Entity Framework Core 8** - ORM для PostgreSQL
- **MediatR** - CQRS pattern
- **FluentValidation** - валидация
- **MassTransit** - event bus (RabbitMQ)
- **PostgreSQL 16** - база данных
- **RabbitMQ 3.13** - message broker

## 🎯 Паттерны и практики

### Clean Architecture
- Domain Layer (бизнес-логика)
- Application Layer (use cases, CQRS)
- Infrastructure Layer (persistence, event bus)
- API Layer (HTTP endpoints)

### Domain-Driven Design
- Rich Domain Model
- Value Objects (Priority, TaskType)
- Domain Events
- Repository pattern

### CQRS (Command Query Responsibility Segregation)
- Commands для изменения состояния
- Queries для чтения данных
- MediatR для orchestration

### Dependency Injection
- Constructor injection
- Interface-based dependencies
- Extension methods для регистрации слоев

## 🚀 Быстрый старт

### Требования

- .NET 8 SDK
- Docker Desktop
- PowerShell (для Windows)

### Запуск

1. **Клонировать репозиторий**
```bash
git clone <repository-url>
cd TaskFlow
```

2. **Запустить инфраструктуру**
```powershell
.\scripts\local-dev.ps1
```

Этот скрипт:
- Запустит PostgreSQL и RabbitMQ в Docker
- Применит миграции базы данных
- Покажет инструкции для запуска API

3. **Запустить Task Management Service**
```bash
cd services/task-management/src/TaskManagement.Api
dotnet run
```

API будет доступен на `http://localhost:5000`

Swagger UI: `http://localhost:5000/swagger`

### Доступ к инфраструктуре

- **PostgreSQL**: `localhost:5432`
  - Database: `task_management_db`
  - User: `postgres`
  - Password: `postgres`

- **RabbitMQ Management**: `http://localhost:15672`
  - User: `guest`
  - Password: `guest`

## 📡 API Endpoints

### Tasks Controller

**Создать задачу**
```http
POST /api/tasks
Content-Type: application/json

{
  "userId": "user123",
  "taskType": "Report",
  "priority": "High",
  "payload": "{\"reportId\": 42}",
  "scheduledAt": null
}
```

**Получить задачу**
```http
GET /api/tasks/{taskId}?userId=user123
```

**Получить все задачи пользователя**
```http
GET /api/tasks/user/{userId}
```

**Отменить задачу**
```http
POST /api/tasks/{taskId}/cancel
Content-Type: application/json

{
  "userId": "user123"
}
```

## 🔍 Domain Model

### TaskEntity
- **Id**: Guid
- **UserId**: string
- **Type**: TaskType (Report, Email, DataProcessing, Notification, Backup)
- **Priority**: Priority (Low, Medium, High, Critical)
- **Status**: TaskStatus (Created, Pending, Cancelled)
- **Payload**: string (JSON)
- **ScheduledAt**: DateTime?

### Domain Events
- **TaskCreatedEvent** - публикуется при создании задачи
- **TaskCancelledEvent** - при отмене
- **TaskPriorityChangedEvent** - при изменении приоритета

## 🎓 Объяснение кода

### Dependency Injection

Каждый слой регистрирует свои зависимости через extension methods:

**Application Layer:**
```csharp
services.AddApplication();  // Регистрирует MediatR handlers и validators
```

**Infrastructure Layer:**
```csharp
services.AddInfrastructure(configuration);  // DbContext, Repositories, MassTransit
```

### CQRS Flow

1. **HTTP Request** → Controller
2. **Controller** создает Command/Query
3. **MediatR** находит соответствующий Handler
4. **Handler** вызывает Domain methods
5. **Domain** выполняет бизнес-логику
6. **Repository** сохраняет изменения
7. **EventPublisher** отправляет события в RabbitMQ
8. **Response** возвращается клиенту

### Event-Driven Communication

```csharp
// Task Management Service публикует событие
await _eventPublisher.PublishAsync(new TaskCreatedEvent(...));

// Task Execution Service (будет в Этапе 3) подпишется на это событие
// и начнет выполнение задачи
```

## 📋 Следующие этапы

- [ ] Этап 2: Event Bus Infrastructure (расширенная настройка)
- [ ] Этап 3: Task Execution Service (worker для выполнения задач)
- [ ] Этап 4: Notification Service (real-time уведомления)
- [ ] Этап 5: Scheduler Service (отложенные задачи)

## 📚 Документация

См. папку `/docs` для детальной архитектурной документации:
- `task_flow_fully_microservices_architecture_documentation.md`
- `task_flow_recommended_implementation_plan_project_structure.md`

## 🧪 Тестирование

### Ручное тестирование через Swagger

1. Запустить сервис: `dotnet run`
2. Открыть `http://localhost:5000/swagger`
3. Выполнить запросы через Swagger UI

### Проверка событий в RabbitMQ

1. Открыть `http://localhost:15672`
2. Войти (guest/guest)
3. Перейти в Queues
4. Проверить что события публикуются

## 📝 Лицензия

MIT


