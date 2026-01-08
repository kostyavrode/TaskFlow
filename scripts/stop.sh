#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
INFRA_DIR="$PROJECT_ROOT/infrastructure"

cd "$INFRA_DIR"

echo "Stopping TaskFlow services..."

if [ -f docker-compose.prod.yml ] && docker-compose -f docker-compose.prod.yml ps -q | grep -q .; then
    echo "Stopping production services..."
    docker-compose -f docker-compose.prod.yml down
elif docker-compose ps -q | grep -q .; then
    echo "Stopping development services..."
    docker-compose down
else
    echo "No running services found."
fi

echo "Done!"


