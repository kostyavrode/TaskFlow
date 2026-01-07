# 🚀 Быстрый запуск TaskFlow на Windows

## Проблема с политикой выполнения PowerShell?

Если вы видите ошибку "выполнение сценариев отключено", используйте один из вариантов ниже:

## ✅ Вариант 1: Разрешить выполнение скриптов (Рекомендуется)

**Откройте PowerShell от имени администратора** и выполните:

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

Затем запустите:
```powershell
.\scripts\docker-start.ps1
```

## ✅ Вариант 2: Запуск без изменения политики

Выполните команды напрямую в PowerShell:

```powershell
cd infrastructure
docker-compose up -d --build
```

Или используйте скрипт с обходом:
```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\docker-start.ps1
```

## ✅ Вариант 3: Использовать готовые команды

### Запуск:
```powershell
cd infrastructure
docker-compose up -d --build
```

### Остановка:
```powershell
cd infrastructure
docker-compose down
```

### Просмотр логов:
```powershell
cd infrastructure
docker-compose logs -f
```

### Статус сервисов:
```powershell
cd infrastructure
docker-compose ps
```

## 📊 После запуска

Откройте в браузере:
- **Swagger UI**: http://localhost:5000/swagger
- **Web UI**: Откройте файл `web/index.html`
- **RabbitMQ**: http://localhost:15672 (guest/guest)

