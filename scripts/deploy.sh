#!/bin/bash

set -e

echo "======================================"
echo "TaskFlow Docker Deployment Script"
echo "======================================"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
INFRA_DIR="$PROJECT_ROOT/infrastructure"

cd "$INFRA_DIR"

if [ ! -f .env ]; then
    echo "⚠️  .env file not found. Creating from .env.example..."
    cp .env.example .env
    echo "⚠️  Please edit .env file and set secure passwords before continuing!"
    echo "Press Enter to continue after editing .env file..."
    read
fi

echo ""
echo "Building and starting services..."
docker-compose -f docker-compose.yml up -d --build

echo ""
echo "Waiting for services to be healthy..."
sleep 10

echo ""
echo "Checking service status..."
docker-compose ps

echo ""
echo "======================================"
echo "Deployment completed!"
echo "======================================"
echo ""
echo "Services:"
echo "  - Task Management API: http://localhost:5000"
echo "  - Notification API: http://localhost:5002"
echo "  - RabbitMQ Management: http://localhost:15672"
echo ""
echo "To view logs:"
echo "  docker-compose logs -f [service-name]"
echo ""
echo "To stop services:"
echo "  docker-compose down"
echo ""

