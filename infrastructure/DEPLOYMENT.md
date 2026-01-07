# TaskFlow Docker Deployment Guide

This guide explains how to deploy TaskFlow microservices on a Linux server using Docker Compose.

## Prerequisites

- Docker Engine 20.10 or higher
- Docker Compose 2.0 or higher
- At least 4GB RAM available
- Linux server (Ubuntu 20.04+ recommended)

## Quick Start (Development)

1. **Clone the repository** (or upload files to your server):
   ```bash
   git clone <repository-url>
   cd TaskFlow
   ```

2. **Make scripts executable**:
   ```bash
   chmod +x scripts/*.sh
   ```

3. **Start services**:
   ```bash
   cd infrastructure
   docker-compose up -d --build
   ```

4. **Check service status**:
   ```bash
   docker-compose ps
   docker-compose logs -f
   ```

Services will be available at:
- Task Management API: http://localhost:5000
- Notification API: http://localhost:5002
- RabbitMQ Management: http://localhost:15672 (guest/guest)

## Production Deployment

### 1. Create Environment File

```bash
cd infrastructure
cp .env.example .env
nano .env  # Edit with secure passwords
```

**Important**: Set strong passwords for all services in `.env` file:
- `POSTGRES_TASK_MGMT_PASSWORD`
- `POSTGRES_TASK_EXEC_PASSWORD`
- `RABBITMQ_PASSWORD`

### 2. Deploy Using Script

```bash
./scripts/deploy-prod.sh
```

Or manually:

```bash
cd infrastructure
docker-compose -f docker-compose.prod.yml up -d --build
```

### 3. Configure Reverse Proxy (Nginx)

Example Nginx configuration (`/etc/nginx/sites-available/taskflow`):

```nginx
server {
    listen 80;
    server_name your-domain.com;

    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    location /hubs {
        proxy_pass http://localhost:5002;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    location /files/reports {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
    }
}
```

Enable and restart Nginx:
```bash
sudo ln -s /etc/nginx/sites-available/taskflow /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

### 4. Configure SSL with Let's Encrypt

```bash
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d your-domain.com
```

## Service Management

### View Logs

```bash
# All services
docker-compose -f docker-compose.prod.yml logs -f

# Specific service
docker-compose -f docker-compose.prod.yml logs -f task-management-api
```

### Restart Services

```bash
docker-compose -f docker-compose.prod.yml restart [service-name]
```

### Stop Services

```bash
./scripts/stop.sh
# Or
docker-compose -f docker-compose.prod.yml down
```

### Update Services

1. Pull latest code:
   ```bash
   git pull
   ```

2. Rebuild and restart:
   ```bash
   cd infrastructure
   docker-compose -f docker-compose.prod.yml up -d --build
   ```

### Backup Database

```bash
# Task Management DB
docker exec taskflow-postgres-task-management pg_dump -U postgres task_management_db > backup_task_mgmt_$(date +%Y%m%d).sql

# Task Execution DB
docker exec taskflow-postgres-task-execution pg_dump -U postgres task_execution_db > backup_task_exec_$(date +%Y%m%d).sql
```

### Restore Database

```bash
# Task Management DB
docker exec -i taskflow-postgres-task-management psql -U postgres task_management_db < backup_task_mgmt_YYYYMMDD.sql

# Task Execution DB
docker exec -i taskflow-postgres-task-execution psql -U postgres task_execution_db < backup_task_exec_YYYYMMDD.sql
```

## Architecture

### Services

1. **Task Management API** (Port 5000)
   - REST API for task management
   - Serves static files (reports) from shared volume

2. **Task Execution Worker**
   - Background service for task processing
   - Generates reports and saves to shared volume

3. **Notification API** (Port 5002)
   - SignalR hub for real-time notifications

4. **PostgreSQL** (2 instances)
   - Task Management DB (Port 5432)
   - Task Execution DB (Port 5433)

5. **RabbitMQ**
   - Message broker (Port 5672)
   - Management UI (Port 15672)

### Volumes

- `postgres_task_management_data`: Task Management database
- `postgres_task_execution_data`: Task Execution database
- `rabbitmq_data`: RabbitMQ data
- `task_execution_reports`: Shared volume for generated reports

### Network

All services communicate through `taskflow-network` bridge network.

## Troubleshooting

### Services Won't Start

1. Check logs:
   ```bash
   docker-compose -f docker-compose.prod.yml logs
   ```

2. Check disk space:
   ```bash
   df -h
   docker system df
   ```

3. Check if ports are in use:
   ```bash
   sudo netstat -tlnp | grep -E '5000|5002|5432|5672'
   ```

### Database Connection Issues

1. Verify database is healthy:
   ```bash
   docker-compose -f docker-compose.prod.yml ps
   ```

2. Check database logs:
   ```bash
   docker-compose -f docker-compose.prod.yml logs postgres-task-management
   ```

### Report Files Not Accessible

1. Check volume mount:
   ```bash
   docker exec taskflow-task-execution-worker ls -la /app/wwwroot/files/reports
   ```

2. Check permissions:
   ```bash
   docker exec taskflow-task-execution-worker chmod -R 755 /app/wwwroot/files/reports
   ```

## Security Considerations

1. **Change default passwords** in `.env` file
2. **Don't expose** database ports externally (they're internal only in prod)
3. **Use firewall** to restrict access:
   ```bash
   sudo ufw allow 80/tcp
   sudo ufw allow 443/tcp
   sudo ufw enable
   ```
4. **Regular backups** of databases
5. **Keep Docker** and images updated:
   ```bash
   sudo apt update && sudo apt upgrade docker.io docker-compose
   ```

## Monitoring

### Health Checks

All services have health checks configured. Check status:
```bash
docker-compose -f docker-compose.prod.yml ps
```

### Resource Usage

```bash
docker stats
```

### Service Health Endpoints

- Task Management API: http://localhost:5000/health
- Notification API: http://localhost:5002/health

## Support

For issues or questions, check logs first:
```bash
docker-compose -f docker-compose.prod.yml logs --tail=100
```

