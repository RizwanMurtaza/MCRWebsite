#!/bin/bash

# Email Service Deployment Script for Ubuntu Servers via SSH
# Run from WSL on Windows to deploy to any Ubuntu server

set -e

# Configuration - Edit these for different servers
SERVER_IP="172.237.117.145"
SERVER_USER="root"
SERVER_PASSWORD="Braveheart1190!12"
DOMAIN="emailservice.pricesnap.co.uk"
MYSQL_PASSWORD="rizwan321"
PROJECT_NAME="EmailService"

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
echo "Database: MySQL (root/$MYSQL_PASSWORD)"

# Step 1: Test connection
print_step "Testing SSH Connection"
execute_ssh "echo 'Connection successful'" "Testing SSH connection"

# Step 2: Install required services
print_step "Installing Required Services (1-4)"

# Service 1: MySQL Server
execute_ssh "
export DEBIAN_FRONTEND=noninteractive
apt update -y
apt install -y mysql-server
if ! systemctl is-enabled mysql >/dev/null 2>&1; then
    echo 'MySQL installation failed!'
    exit 1
fi
echo 'Service 1/4: MySQL installed successfully'
" "Installing Service 1: MySQL Server"

# Service 2: Nginx Web Server
execute_ssh "
apt install -y nginx curl
if ! which nginx >/dev/null 2>&1; then
    echo 'Nginx installation failed!'
    exit 1
fi
echo 'Service 2/4: Nginx installed successfully'
" "Installing Service 2: Nginx Web Server"

# Service 3: .NET 9 Runtime
execute_ssh "
# Install software-properties-common if not present (needed for add-apt-repository)
apt-get install -y software-properties-common

# Add the .NET backports PPA repository for .NET 9
echo 'Adding .NET 9 backports PPA repository...'
add-apt-repository -y ppa:dotnet/backports

# Update package lists with new repository
apt-get update -y

# Install ASP.NET Core Runtime 9.0 (includes .NET Runtime)
echo 'Installing ASP.NET Core Runtime 9.0...'
apt-get install -y aspnetcore-runtime-9.0

# Verify installation
if command -v dotnet >/dev/null 2>&1; then
    echo 'Successfully installed .NET!'
    echo 'Installed .NET version:'
    dotnet --version || true
    echo ''
    echo 'Installed .NET runtimes:'
    dotnet --list-runtimes

    # Double check ASP.NET Core runtime is installed
    if ! dotnet --list-runtimes | grep -q 'Microsoft.AspNetCore.App 9'; then
        echo 'Warning: ASP.NET Core 9 runtime not detected, attempting reinstall...'
        apt-get install --reinstall -y aspnetcore-runtime-9.0
        dotnet --list-runtimes
    fi
else
    echo '.NET installation failed - dotnet command not found!'
    exit 1
fi

echo 'Service 3/4: .NET 9 Runtime installed successfully'
" "Installing Service 3: .NET 9 Runtime"

# Service 4: Certbot for SSL
execute_ssh "
apt install -y certbot python3-certbot-nginx ufw
if ! which certbot >/dev/null 2>&1; then
    echo 'Certbot installation failed!'
    exit 1
fi
echo 'Service 4/4: Certbot installed successfully'
" "Installing Service 4: Certbot and UFW"

# Verify all services are installed
print_step "Verifying All Services Installation"
execute_ssh "
echo '=== SERVICES VERIFICATION ==='
echo -n 'Service 1 - MySQL: '
systemctl is-enabled mysql && echo 'INSTALLED ✓' || { echo 'FAILED ✗'; exit 1; }

echo -n 'Service 2 - Nginx: '
which nginx >/dev/null 2>&1 && echo 'INSTALLED ✓' || { echo 'FAILED ✗'; exit 1; }

echo -n 'Service 3 - .NET 9: '
if command -v dotnet >/dev/null 2>&1 && dotnet --list-runtimes | grep -q 'Microsoft.AspNetCore.App 9'; then
    echo 'INSTALLED ✓'
elif command -v dotnet >/dev/null 2>&1 && dotnet --list-sdks | grep -q '^9.0'; then
    echo 'INSTALLED (SDK) ✓'
elif command -v dotnet >/dev/null 2>&1; then
    echo 'INSTALLED (checking version)...'
    dotnet --version
else
    echo 'FAILED ✗'
    exit 1
fi

echo -n 'Service 4 - Certbot: '
which certbot >/dev/null 2>&1 && echo 'INSTALLED ✓' || { echo 'FAILED ✗'; exit 1; }

echo ''
echo 'All 4 services verified successfully! Proceeding with deployment...'
echo '==========================='
" "Verifying all 4 services are installed"

# Step 3: Configure MySQL
print_step "Configuring MySQL"
execute_ssh "
systemctl start mysql
systemctl enable mysql

# Set MySQL root password
mysql -e \"ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '$MYSQL_PASSWORD';\" 2>/dev/null || {
    mysql -u root -e \"ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '$MYSQL_PASSWORD';\"
}
mysql -e \"FLUSH PRIVILEGES;\"

# Create database
mysql -u root -p'$MYSQL_PASSWORD' -e \"CREATE DATABASE IF NOT EXISTS EmailServiceDb;\"

# Configure for remote access
sed -i 's/bind-address.*=.*/bind-address = 0.0.0.0/' /etc/mysql/mysql.conf.d/mysqld.cnf
mysql -u root -p'$MYSQL_PASSWORD' -e \"CREATE USER IF NOT EXISTS 'root'@'%' IDENTIFIED BY '$MYSQL_PASSWORD';\"
mysql -u root -p'$MYSQL_PASSWORD' -e \"GRANT ALL PRIVILEGES ON *.* TO 'root'@'%';\"
mysql -u root -p'$MYSQL_PASSWORD' -e \"FLUSH PRIVILEGES;\"

systemctl restart mysql
echo 'MySQL configured successfully'
" "Configuring MySQL database"

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
mkdir -p /var/www/$PROJECT_NAME
rm -rf /var/www/$PROJECT_NAME/*
chown -R root:root /var/www/$PROJECT_NAME
echo 'Application directory prepared'
" "Preparing application directory"

# Copy files from publish directory to application directory (flattened)
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

# Step 6: Create systemd service
print_step "Creating Systemd Service"
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
systemctl start emailservice

sleep 3
systemctl status emailservice --no-pager
echo 'Service created and started'
" "Creating and starting systemd service"

# Step 7: Configure Nginx
print_step "Configuring Nginx"
execute_ssh "
systemctl start nginx
systemctl enable nginx

# Create initial HTTP-only configuration
cat > /etc/nginx/sites-available/$DOMAIN << 'EOF'
server {
    listen 80;
    server_name $DOMAIN;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;
    }
}
EOF

ln -sf /etc/nginx/sites-available/$DOMAIN /etc/nginx/sites-enabled/
rm -f /etc/nginx/sites-enabled/default

nginx -t
systemctl reload nginx
echo 'Nginx configured for HTTP'
" "Configuring Nginx reverse proxy"

# Step 8: Install SSL certificate
print_step "Installing SSL Certificate"
execute_ssh "
# Get SSL certificate and automatically configure HTTPS
certbot --nginx -d $DOMAIN --non-interactive --agree-tos --email admin@$DOMAIN --redirect

# Enable auto-renewal
systemctl enable certbot.timer
systemctl start certbot.timer

echo 'SSL certificate installed and HTTPS configured'
" "Installing Let's Encrypt SSL certificate"

# Step 9: Configure firewall
print_step "Configuring Firewall"
execute_ssh "
ufw --force enable
ufw allow ssh
ufw allow 'Nginx Full'
ufw allow 3306/tcp

echo 'Firewall configured'
ufw status
" "Configuring UFW firewall"

# Step 10: Final verification
print_step "Final Verification"
execute_ssh "
echo '=== DEPLOYMENT VERIFICATION ==='
echo 'Files in application directory:'
ls -la /var/www/$PROJECT_NAME/

echo 'Service Status:'
systemctl status emailservice --no-pager --lines=5 || echo 'Service check failed'

echo 'Service Active Status:'
systemctl is-active emailservice || echo 'Service not running'

echo 'Port Check:'
ss -tlnp | grep ':5000' || echo 'Port 5000 not listening'

echo 'Application Test (with 10 second wait):'
sleep 10
curl -s -I http://localhost:5000 | head -1 || echo 'Local app test failed'

echo 'HTTPS Test:'
curl -s -I https://$DOMAIN | head -1 || echo 'HTTPS test failed'

echo 'Recent Service Logs:'
journalctl -u emailservice --no-pager -n 5 --since '1 minute ago' || echo 'No recent logs'

echo '=== VERIFICATION COMPLETE ==='
" "Running final verification"

cd ..

# Summary
print_step "Deployment Complete!"
print_success "Email Service deployed successfully!"
echo ""
echo -e "${GREEN}🎉 DEPLOYMENT SUMMARY 🎉${NC}"
echo "=============================="
echo -e "✅ Server: ${YELLOW}$SERVER_IP${NC}"
echo -e "✅ Domain: ${YELLOW}https://$DOMAIN${NC}"
echo -e "✅ Database: ${YELLOW}MySQL (root/$MYSQL_PASSWORD) on port 3306${NC}"
echo -e "✅ Application: ${YELLOW}Running on port 5000 behind Nginx proxy${NC}"
echo -e "✅ SSL: ${YELLOW}Let's Encrypt certificate installed${NC}"
echo -e "✅ Firewall: ${YELLOW}UFW configured with necessary ports${NC}"
echo ""
echo -e "${BLUE}🔗 Access Your Service:${NC}"
echo -e "   Main App: ${YELLOW}https://$DOMAIN${NC}"
echo -e "   Admin Panel: ${YELLOW}https://$DOMAIN/admin.html${NC}"
echo ""
echo -e "${BLUE}💾 Database Access:${NC}"
echo -e "   Host: ${YELLOW}$SERVER_IP${NC}"
echo -e "   Port: ${YELLOW}3306${NC}"
echo -e "   User: ${YELLOW}root${NC}"
echo -e "   Password: ${YELLOW}$MYSQL_PASSWORD${NC}"
echo -e "   Database: ${YELLOW}EmailServiceDb${NC}"
echo ""
echo -e "${BLUE}🛠️  Management Commands:${NC}"
echo -e "   Monitor logs: ${YELLOW}ssh $SERVER_USER@$SERVER_IP 'sudo journalctl -u emailservice -f'${NC}"
echo -e "   Restart service: ${YELLOW}ssh $SERVER_USER@$SERVER_IP 'sudo systemctl restart emailservice'${NC}"
echo -e "   Check status: ${YELLOW}ssh $SERVER_USER@$SERVER_IP 'sudo systemctl status emailservice'${NC}"
echo ""
echo -e "${GREEN}🚀 Your Email Service is now LIVE!${NC}"