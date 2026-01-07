#!/bin/bash

set -e

echo "======================================"
echo "TaskFlow Production Deployment Script"
echo "======================================"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
INFRA_DIR="$PROJECT_ROOT/infrastructure"

cd "$INFRA_DIR"

if [ ! -f .env ]; then
    echo "❌ .env file not found!"
    echo "Please create .env file from .env.example and set secure passwords."
    exit 1
fi

echo ""
echo "Validating environment variables..."
source .env

if [ -z "$POSTGRES_TASK_MGMT_PASSWORD" ] || [ -z "$POSTGRES_TASK_EXEC_PASSWORD" ] || [ -z "$RABBITMQ_PASSWORD" ]; then
    echo "❌ Missing required environment variables in .env file!"
    exit 1
fi

echo "✓ Environment variables validated"

echo ""
echo "Building production images..."
docker-compose -f docker-compose.prod.yml build --no-cache

echo ""
echo "Starting production services..."
docker-compose -f docker-compose.prod.yml up -d

echo ""
echo "Waiting for services to be healthy..."
sleep 15

echo ""
echo "Checking service status..."
docker-compose -f docker-compose.prod.yml ps

echo ""
echo "Service health checks:"
docker-compose -f docker-compose.prod.yml ps --format "table {{.Name}}\t{{.Status}}\t{{.Ports}}"

echo ""
echo "======================================"
echo "Production deployment completed!"
echo "======================================"
echo ""
echo "Services are running without exposed ports (internal network only)."
echo ""
echo "To view logs:"
echo "  docker-compose -f docker-compose.prod.yml logs -f [service-name]"
echo ""
echo "To stop services:"
echo "  docker-compose -f docker-compose.prod.yml down"
echo ""
echo "To update services:"
echo "  ./scripts/deploy-prod.sh"
echo ""

