#!/bin/bash

# Email Service Deployment Script for Ubuntu Servers via SSH
# Run from WSL on Windows to deploy to any Ubuntu server

set -e

# Configuration - Edit these for different servers
SERVER_IP="172.237.101.211"
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
print_step "Installing Required Services"
execute_ssh "
export DEBIAN_FRONTEND=noninteractive
apt update -y
apt install -y mysql-server nginx curl ufw

# Install .NET 9 runtime if not already installed
if ! command -v dotnet &> /dev/null; then
    wget -q https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O /tmp/packages-microsoft-prod.deb
    dpkg -i /tmp/packages-microsoft-prod.deb
    rm /tmp/packages-microsoft-prod.deb
    apt update
    apt install -y aspnetcore-runtime-9.0
fi

# Install Certbot
apt install -y certbot python3-certbot-nginx

echo 'All services installed successfully'
" "Installing MySQL, Nginx, .NET 9, and Certbot"

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
chown -R root:root /var/www/$PROJECT_NAME
echo 'Application directory prepared'
" "Preparing application directory"

copy_files "./publish/" "/var/www/$PROJECT_NAME/" "Copying application files"

execute_ssh "
chown -R www-data:www-data /var/www/$PROJECT_NAME
echo 'File permissions set'
" "Setting file permissions"

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

# Create initial HTTP-only configuration for certificate validation
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
# Get SSL certificate (webroot method to avoid auto-configuration)
certbot certonly --webroot -w /var/www/html -d $DOMAIN --non-interactive --agree-tos --email admin@$DOMAIN

# Now create the full SSL configuration
cat > /etc/nginx/sites-available/$DOMAIN << 'EOF'
# HTTP server - redirect to HTTPS
server {
    listen 80;
    listen [::]:80;
    server_name $DOMAIN;

    # Redirect all HTTP traffic to HTTPS
    return 301 https://\$server_name\$request_uri;
}

# HTTPS server
server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name $DOMAIN;

    # SSL certificate paths
    ssl_certificate /etc/letsencrypt/live/$DOMAIN/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/$DOMAIN/privkey.pem;

    # Modern SSL configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-CHACHA20-POLY1305:ECDHE-RSA-CHACHA20-POLY1305:DHE-RSA-AES128-GCM-SHA256:DHE-RSA-AES256-GCM-SHA384;
    ssl_prefer_server_ciphers off;

    # SSL session settings
    ssl_session_timeout 1d;
    ssl_session_cache shared:MozSSL:10m;
    ssl_session_tickets off;

    # OCSP stapling
    ssl_stapling on;
    ssl_stapling_verify on;
    ssl_trusted_certificate /etc/letsencrypt/live/$DOMAIN/chain.pem;

    # Security headers
    add_header Strict-Transport-Security \"max-age=63072000; includeSubDomains; preload\" always;
    add_header X-Frame-Options \"SAMEORIGIN\" always;
    add_header X-Content-Type-Options \"nosniff\" always;
    add_header X-XSS-Protection \"1; mode=block\" always;
    add_header Referrer-Policy \"no-referrer-when-downgrade\" always;

    # Proxy settings
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_set_header X-Forwarded-Host \$host;
        proxy_cache_bypass \$http_upgrade;

        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;

        # Buffer settings
        proxy_buffering off;
        proxy_buffer_size 4k;
        proxy_buffers 8 4k;
        proxy_busy_buffers_size 8k;
    }
}
EOF

# Test and reload Nginx with new SSL configuration
nginx -t
systemctl reload nginx

# Enable auto-renewal
systemctl enable certbot.timer
systemctl start certbot.timer

echo 'SSL certificate installed and HTTPS configured with enhanced security'
" "Installing Let's Encrypt SSL certificate and configuring HTTPS"

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
echo 'Service Status:'
systemctl is-active emailservice || echo 'Service not running'

echo 'Nginx Status:'
systemctl is-active nginx || echo 'Nginx not running'

echo 'MySQL Status:'
systemctl is-active mysql || echo 'MySQL not running'

echo 'Port Check:'
netstat -tlnp | grep ':5000' || echo 'Port 5000 not listening'
netstat -tlnp | grep ':443' || echo 'Port 443 not listening'

echo 'SSL Certificate:'
certbot certificates | grep -A 3 'Certificate Name:' || echo 'No certificates found'

echo 'Application Test:'
curl -s -I http://localhost:5000 | head -1 || echo 'Local app test failed'

echo 'HTTPS Test:'
curl -s -I https://$DOMAIN | head -1 || echo 'HTTPS test failed'

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