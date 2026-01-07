# 🐳 TaskFlow Docker Deployment

Complete Docker setup for TaskFlow microservices on Linux servers.

## 📋 Quick Start

### 1. Prepare Environment

```bash
cd infrastructure
cp .env.example .env
nano .env  # Set secure passwords
```

### 2. Deploy

**Development:**
```bash
./scripts/deploy.sh
```

**Production:**
```bash
./scripts/deploy-prod.sh
```

## 📁 Files Structure

```
TaskFlow/
├── infrastructure/
│   ├── docker-compose.yml          # Development setup
│   ├── docker-compose.prod.yml     # Production setup
│   ├── .env.example                # Environment template
│   └── DEPLOYMENT.md               # Detailed deployment guide
├── services/
│   ├── task-management/src/TaskManagement.Api/Dockerfile
│   ├── task-execution/src/TaskExecution.Worker/Dockerfile
│   └── notification/src/Notification.Api/Dockerfile
└── scripts/
    ├── deploy.sh                   # Development deployment
    ├── deploy-prod.sh              # Production deployment
    └── stop.sh                     # Stop all services
```

## 🚀 Services

- **Task Management API** - Port 5000
- **Notification API** - Port 5002
- **RabbitMQ Management** - Port 15672
- **PostgreSQL** - Internal network only (prod)

## 📚 Documentation

See [infrastructure/DEPLOYMENT.md](infrastructure/DEPLOYMENT.md) for:
- Detailed setup instructions
- Production configuration
- Nginx reverse proxy setup
- SSL/HTTPS configuration
- Backup and restore procedures
- Troubleshooting guide

## 🔧 Linux Server Setup

1. Install Docker:
   ```bash
   curl -fsSL https://get.docker.com -o get-docker.sh
   sudo sh get-docker.sh
   ```

2. Install Docker Compose:
   ```bash
   sudo apt install docker-compose-plugin
   ```

3. Make scripts executable:
   ```bash
   chmod +x scripts/*.sh
   ```

4. Deploy:
   ```bash
   ./scripts/deploy-prod.sh
   ```

## 🔐 Security

- ✅ All services use environment variables for secrets
- ✅ Databases not exposed externally in production
- ✅ Health checks configured
- ✅ Resource limits set
- ✅ Automatic restarts on failure

## 📊 Monitoring

Check service health:
```bash
docker-compose -f infrastructure/docker-compose.prod.yml ps
```

View logs:
```bash
docker-compose -f infrastructure/docker-compose.prod.yml logs -f
```

