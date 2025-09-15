#!/bin/bash

# Server Configuration
SERVER_IP="172.237.101.211"
SERVER_USER="root"
SERVER_PASSWORD="Braveheart1190!12"
DOMAIN="emailservice.pricesnap.co.uk"
PROJECT_NAME="EmailService"

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Main cleanup function
cleanup_server() {
    print_status "Starting server cleanup..."

    sshpass -p "$SERVER_PASSWORD" ssh -o StrictHostKeyChecking=no $SERVER_USER@$SERVER_IP << 'EOF'

echo "=== Stopping and disabling EmailService ==="
systemctl stop EmailService 2>/dev/null || echo "Service was not running"
systemctl disable EmailService 2>/dev/null || echo "Service was not enabled"

echo "=== Removing systemd service file ==="
rm -f /etc/systemd/system/EmailService.service
systemctl daemon-reload

echo "=== Killing any remaining processes on ports 5000-5001 ==="
for port in 5000 5001; do
    if lsof -ti:$port >/dev/null 2>&1; then
        echo "Killing processes on port $port..."
        lsof -ti:$port | xargs kill -9 2>/dev/null || true
        sleep 1
    fi
done

echo "=== Removing application directory ==="
rm -rf /var/www/EmailService

echo "=== Removing Nginx configuration ==="
rm -f /etc/nginx/sites-enabled/EmailService
rm -f /etc/nginx/sites-available/EmailService
rm -f /etc/nginx/sites-available/EmailService_temp

echo "=== Reloading Nginx ==="
systemctl reload nginx

echo "=== Checking for remaining processes ==="
ps aux | grep -i emailservice | grep -v grep || echo "No EmailService processes found"

echo "=== Checking ports ==="
for port in 5000 5001; do
    if lsof -i:$port >/dev/null 2>&1; then
        echo "Port $port is still in use:"
        lsof -i:$port
    else
        echo "Port $port is free"
    fi
done

echo "=== Cleanup completed ==="

EOF

    if [ $? -eq 0 ]; then
        print_status "✓ Server cleanup completed successfully"
    else
        print_error "✗ Server cleanup failed"
        exit 1
    fi
}

# Main function
main() {
    print_status "Server cleanup script for $PROJECT_NAME"
    print_status "Server: $SERVER_USER@$SERVER_IP"
    echo ""

    cleanup_server

    print_status "==================== Cleanup Summary ===================="
    print_status "✓ EmailService systemd service removed"
    print_status "✓ Application directory cleaned"
    print_status "✓ Nginx configuration removed"
    print_status "✓ Processes on ports 5000-5001 terminated"
    print_status "✓ Server ready for fresh deployment"
    print_status "========================================================"
}

# Run main function
main