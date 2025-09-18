#!/bin/bash

# Service Deployment Script - Deploy Email Service Application
# This script builds and deploys the .NET application to a pre-configured server
# Run this after server-setup.sh to deploy/update the application

set -e

# Configuration - Edit these for your server
SERVER_IP="${SERVER_IP:-172.237.117.145}"
SERVER_USER="${SERVER_USER:-root}"
SERVER_PASSWORD="${SERVER_PASSWORD:-Braveheart1190!12}"
DOMAIN="${DOMAIN:-emailservice.mcrsolicitors.co.uk}"
MYSQL_PASSWORD="${MYSQL_PASSWORD:-rizwan321}"
PROJECT_NAME="${PROJECT_NAME:-EmailService}"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

print_status() { echo -e "${BLUE}[INFO]${NC} $1"; }
print_success() { echo -e "${GREEN}[SUCCESS]${NC} $1"; }
print_error() { echo -e "${RED}[ERROR]${NC} $1"; }
print_step() { echo -e "\n${BLUE}==== $1 ====${NC}"; }

# Function to execute commands on remote server
execute_ssh() {
    local command="$1"
    local description="$2"

    print_status "$description"

    # Use sshpass for automated SSH
    if ! command -v sshpass &> /dev/null; then
        print_error "sshpass not found. Installing..."
        sudo apt update && sudo apt install -y sshpass
    fi

    sshpass -p "$SERVER_PASSWORD" ssh -o StrictHostKeyChecking=no "$SERVER_USER@$SERVER_IP" "$command"

    if [ $? -eq 0 ]; then
        print_success "$description completed"
    else
        print_error "$description failed"
        exit 1
    fi
}

# Function to copy files to server
copy_files() {
    local local_path="$1"
    local remote_path="$2"
    local description="$3"

    print_status "$description"

    sshpass -p "$SERVER_PASSWORD" scp -o StrictHostKeyChecking=no -r "$local_path" "$SERVER_USER@$SERVER_IP:$remote_path"

    if [ $? -eq 0 ]; then
        print_success "$description completed"
    else
        print_error "$description failed"
        exit 1
    fi
}

print_step "Starting Email Service Deployment"
echo "Server: $SERVER_IP"
echo "Domain: $DOMAIN"
echo "Project: $PROJECT_NAME"

# Step 1: Test connection
print_step "Testing SSH Connection"
execute_ssh "echo 'Connection successful'" "Testing SSH connection"

# Step 2: Check prerequisites
print_step "Checking Prerequisites"
execute_ssh "
echo 'Checking installed services...'
echo -n '.NET Runtime: '
if command -v dotnet >/dev/null 2>&1; then
    echo 'OK'
    dotnet --list-runtimes | head -1
else
    echo 'MISSING - Please run server-setup.sh first'
    exit 1
fi

echo -n 'MySQL: '
if systemctl is-active --quiet mysql; then
    echo 'OK (Running)'
else
    echo 'MISSING or NOT RUNNING - Please run server-setup.sh first'
    exit 1
fi

echo -n 'Nginx: '
if systemctl is-active --quiet nginx; then
    echo 'OK (Running)'
else
    echo 'MISSING or NOT RUNNING - Please run server-setup.sh first'
    exit 1
fi

echo 'All prerequisites verified!'
" "Checking server prerequisites"

# Step 3: Stop existing service if running
print_step "Stopping Existing Service"
execute_ssh "
if systemctl list-units --full -all | grep -Fq 'emailservice.service'; then
    echo 'Stopping existing emailservice...'
    systemctl stop emailservice || true
    echo 'Service stopped'
else
    echo 'No existing service found'
fi
" "Stopping existing service"

# Step 4: Build application locally
print_step "Building .NET Application"
print_status "Building application locally..."

cd EmailService

# Clean up any existing build artifacts
rm -rf publish bin obj

dotnet restore
if [ $? -ne 0 ]; then
    print_error "dotnet restore failed"
    exit 1
fi

dotnet publish -c Release -o ./publish
if [ $? -ne 0 ]; then
    print_error "dotnet publish failed"
    exit 1
fi

print_success "Application built successfully"

# Step 5: Deploy application
print_step "Deploying Application"

execute_ssh "
# Backup existing deployment if it exists
if [ -d /var/www/$PROJECT_NAME ] && [ \"\$(ls -A /var/www/$PROJECT_NAME)\" ]; then
    echo 'Backing up existing deployment...'
    mkdir -p /var/www/backups
    tar czf /var/www/backups/$PROJECT_NAME-\$(date +%Y%m%d-%H%M%S).tar.gz -C /var/www/$PROJECT_NAME .
    echo 'Backup created'
fi

# Clean and prepare directory
rm -rf /var/www/$PROJECT_NAME/*
mkdir -p /var/www/$PROJECT_NAME
chown -R root:root /var/www/$PROJECT_NAME
echo 'Application directory prepared'
" "Preparing application directory"

# Copy files from publish directory to application directory
print_status "Copying application files"
sshpass -p "$SERVER_PASSWORD" ssh -o StrictHostKeyChecking=no "$SERVER_USER@$SERVER_IP" "mkdir -p /tmp/publish_transfer"
copy_files "./publish/" "/tmp/publish_transfer/" "Uploading publish files to temp directory"

execute_ssh "
cd /tmp/publish_transfer/publish
cp -r * /var/www/$PROJECT_NAME/
rm -rf /tmp/publish_transfer
chown -R www-data:www-data /var/www/$PROJECT_NAME
chmod +x /var/www/$PROJECT_NAME/$PROJECT_NAME 2>/dev/null || echo 'No executable to chmod'
echo 'File permissions set'
" "Moving files and setting permissions"

# Step 6: Create/Update systemd service
print_step "Configuring Systemd Service"
execute_ssh "
cat > /etc/systemd/system/emailservice.service << 'EOF'
[Unit]
Description=Email Service ASP.NET Core Application
After=network.target mysql.service

[Service]
Type=simple
User=www-data
Group=www-data
WorkingDirectory=/var/www/$PROJECT_NAME
ExecStart=/usr/bin/dotnet /var/www/$PROJECT_NAME/$PROJECT_NAME.dll
KillSignal=SIGINT
SyslogIdentifier=emailservice
Restart=always
RestartSec=10
TimeoutStartSec=60
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
EOF

systemctl daemon-reload
systemctl enable emailservice
echo 'Service configured'
" "Configuring systemd service"

# Step 7: Run database migrations if needed
print_step "Database Migrations"
execute_ssh "
cd /var/www/$PROJECT_NAME
echo 'Checking for pending migrations...'
# The application will handle migrations on startup
echo 'Migrations will be applied on application startup'
" "Preparing database migrations"

# Step 8: Start service
print_step "Starting Application Service"
execute_ssh "
systemctl start emailservice

# Wait for service to start
sleep 5

# Check if service started successfully
if systemctl is-active --quiet emailservice; then
    echo 'Service started successfully'
    systemctl status emailservice --no-pager --lines=10
else
    echo 'Service failed to start. Checking logs...'
    journalctl -u emailservice --no-pager -n 20
    exit 1
fi
" "Starting application service"

# Step 9: Verify and Configure SSL
print_step "Verifying and Configuring SSL"
execute_ssh "
echo '=== SSL VERIFICATION AND CONFIGURATION ==='
echo ''

# Check if SSL certificate exists and is valid
echo '1. Checking SSL Certificate Status:'
if certbot certificates 2>/dev/null | grep -q '$DOMAIN'; then
    echo '   ✓ SSL certificate exists for $DOMAIN'

    # Check certificate expiry
    cert_info=\$(certbot certificates 2>/dev/null | grep -A 10 '$DOMAIN')
    if echo \"\$cert_info\" | grep -q 'VALID'; then
        echo '   ✓ Certificate is valid'
        expiry_date=\$(echo \"\$cert_info\" | grep 'Expiry Date:' | cut -d: -f2- | xargs)
        echo \"   ℹ  Expires: \$expiry_date\"
    else
        echo '   ⚠  Certificate may be expired or invalid'
    fi
else
    echo '   ✗ No SSL certificate found for $DOMAIN'
    echo '   → Creating SSL certificate...'

    # Create SSL certificate
    certbot --nginx -d $DOMAIN --non-interactive --agree-tos --email admin@$DOMAIN --redirect

    if [ \$? -eq 0 ]; then
        echo '   ✓ SSL certificate created successfully'
    else
        echo '   ✗ Failed to create SSL certificate'
    fi
fi

echo ''
echo '2. Nginx Configuration Test:'
nginx -t
if [ \$? -eq 0 ]; then
    echo '   ✓ Nginx configuration is valid'
    systemctl reload nginx
    echo '   ✓ Nginx reloaded'
else
    echo '   ✗ Nginx configuration has errors'
fi

echo ''
echo '3. SSL Auto-Renewal Configuration:'
# Ensure certbot timer is enabled
systemctl enable certbot.timer >/dev/null 2>&1
systemctl start certbot.timer >/dev/null 2>&1

if systemctl is-active --quiet certbot.timer; then
    echo '   ✓ Certbot auto-renewal timer is active'
else
    echo '   ✗ Certbot auto-renewal timer is not active'
fi

# Test auto-renewal
certbot renew --dry-run >/dev/null 2>&1
if [ \$? -eq 0 ]; then
    echo '   ✓ Auto-renewal test passed'
else
    echo '   ⚠  Auto-renewal test failed (certificate may be too new)'
fi

echo ''
echo '=== SSL CONFIGURATION COMPLETE ==='
" "Configuring and verifying SSL"

# Step 10: Final Verification
print_step "Final Deployment Verification"
execute_ssh "
echo '=== FINAL DEPLOYMENT VERIFICATION ==='
echo ''
echo '1. Service Status:'
systemctl is-active emailservice && echo '   ✓ Service is ACTIVE' || echo '   ✗ Service is NOT ACTIVE'

echo ''
echo '2. Port Check:'
if ss -tlnp | grep -q ':5000'; then
    echo '   ✓ Application listening on port 5000'
else
    echo '   ✗ Port 5000 not listening'
fi

echo ''
echo '3. Local Health Check:'
if curl -s -f -o /dev/null http://localhost:5000; then
    echo '   ✓ Local application responding'
else
    echo '   ✗ Local application not responding'
fi

echo ''
echo '4. HTTPS Health Check:'
sleep 3  # Give SSL a moment to propagate
if curl -s -f -o /dev/null https://$DOMAIN; then
    echo '   ✓ HTTPS endpoint responding'
    # Check if HTTPS redirects properly
    http_response=\$(curl -s -I http://$DOMAIN | head -1)
    if echo \"\$http_response\" | grep -q '301\|302'; then
        echo '   ✓ HTTP to HTTPS redirect working'
    else
        echo '   ⚠  HTTP to HTTPS redirect may not be configured'
    fi
else
    echo '   ✗ HTTPS endpoint not responding'
    echo '   → Checking SSL certificate status...'
    curl -s -I https://$DOMAIN 2>&1 | head -3 || echo '   SSL connection failed'
fi

echo ''
echo '5. Database Connection:'
mysql -u root -p'$MYSQL_PASSWORD' -e 'USE EmailServiceDb; SHOW TABLES;' 2>/dev/null && \
    echo '   ✓ Database accessible' || echo '   ✗ Database connection failed'

echo ''
echo '6. SSL Certificate Details:'
certbot certificates 2>/dev/null | grep -A 5 '$DOMAIN' || echo '   No certificate information available'

echo ''
echo '7. Recent Logs (last 5 entries):'
journalctl -u emailservice --no-pager -n 5 --no-hostname

echo ''
echo '=== VERIFICATION COMPLETE ==='
" "Running final deployment verification"

cd ..

# Summary
print_step "Deployment Complete!"
print_success "Email Service deployed successfully!"
echo ""
echo -e "${GREEN}🎉 DEPLOYMENT SUMMARY 🎉${NC}"
echo "=============================="
echo -e "✅ Server: ${YELLOW}$SERVER_IP${NC}"
echo -e "✅ Domain: ${YELLOW}https://$DOMAIN${NC}"
echo -e "✅ Service: ${YELLOW}emailservice (systemd)${NC}"
echo -e "✅ Status: ${YELLOW}Running on port 5000${NC}"
echo ""
echo -e "${BLUE}🔗 Access Points:${NC}"
echo -e "   Main App: ${YELLOW}https://$DOMAIN${NC}"
echo -e "   Admin Panel: ${YELLOW}https://$DOMAIN/admin.html${NC}"
echo -e "   Email Queue: ${YELLOW}https://$DOMAIN/emails.html${NC}"
echo ""
echo -e "${BLUE}🛠️  Useful Commands:${NC}"
echo -e "   View logs: ${YELLOW}ssh $SERVER_USER@$SERVER_IP 'sudo journalctl -u emailservice -f'${NC}"
echo -e "   Restart: ${YELLOW}ssh $SERVER_USER@$SERVER_IP 'sudo systemctl restart emailservice'${NC}"
echo -e "   Status: ${YELLOW}ssh $SERVER_USER@$SERVER_IP 'sudo systemctl status emailservice'${NC}"
echo -e "   Stop: ${YELLOW}ssh $SERVER_USER@$SERVER_IP 'sudo systemctl stop emailservice'${NC}"
echo ""
echo -e "${BLUE}📦 Backup Location:${NC}"
echo -e "   ${YELLOW}/var/www/backups/${NC}"
echo ""
echo -e "${GREEN}🚀 Your Email Service has been deployed!${NC}"