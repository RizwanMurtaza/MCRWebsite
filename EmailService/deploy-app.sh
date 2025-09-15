#!/bin/bash

# Server Configuration
SERVER_IP="172.237.101.211"
SERVER_USER="root"
SERVER_PASSWORD="Braveheart1190!12"
DOMAIN="emailservice.pricesnap.co.uk"
MYSQL_PASSWORD="rizwan321"
PROJECT_NAME="EmailService"
REMOTE_APP_PATH="/var/www/$PROJECT_NAME"
LOCAL_PROJECT_PATH="$(pwd)/$PROJECT_NAME"

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
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

print_step() {
    echo -e "${BLUE}[STEP]${NC} $1"
}

# Function to check command success
check_success() {
    if [ $? -eq 0 ]; then
        print_status "✓ $1"
    else
        print_error "✗ $2"
        exit 1
    fi
}

# Step 1: Build the application locally
build_application() {
    print_step "Step 1: Building application locally..."

    # Check if project directory exists
    if [ ! -d "$LOCAL_PROJECT_PATH" ]; then
        print_error "Project directory not found: $LOCAL_PROJECT_PATH"
        exit 1
    fi

    cd "$LOCAL_PROJECT_PATH"

    # Clean previous builds
    print_status "Cleaning previous builds..."
    rm -rf bin/Release/net9.0/linux-x64/publish/

    # Build and publish the application
    print_status "Building and publishing the application..."
    dotnet publish -c Release -r linux-x64 --self-contained false

    check_success "Application built successfully" "Failed to build application"

    # Check if publish directory exists
    if [ ! -d "bin/Release/net9.0/linux-x64/publish" ]; then
        print_error "Publish directory not found. Build may have failed."
        exit 1
    fi

    print_status "Build output location: $LOCAL_PROJECT_PATH/bin/Release/net9.0/linux-x64/publish"
}

# Step 2: Create deployment package
create_deployment_package() {
    print_step "Step 2: Creating deployment package..."

    cd "$LOCAL_PROJECT_PATH"

    # Create tar archive of the published application
    print_status "Creating deployment archive..."
    tar -czf $PROJECT_NAME.tar.gz -C bin/Release/net9.0/linux-x64/publish .

    check_success "Deployment package created" "Failed to create deployment package"

    print_status "Package size: $(ls -lh $PROJECT_NAME.tar.gz | awk '{print $5}')"
}

# Step 3: Transfer application to server
transfer_to_server() {
    print_step "Step 3: Transferring application to server..."

    # Install sshpass if not available
    if ! command -v sshpass &> /dev/null; then
        print_warning "sshpass not found. Installing..."
        if [[ "$OSTYPE" == "linux-gnu"* ]]; then
            sudo apt-get install -y sshpass
        elif [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
            print_warning "Please install sshpass manually for Windows"
        fi
    fi

    # Create remote directory
    print_status "Creating remote application directory..."
    sshpass -p "$SERVER_PASSWORD" ssh -o StrictHostKeyChecking=no $SERVER_USER@$SERVER_IP "mkdir -p $REMOTE_APP_PATH"

    # Transfer the archive
    print_status "Transferring application to server..."
    sshpass -p "$SERVER_PASSWORD" scp -o StrictHostKeyChecking=no "$LOCAL_PROJECT_PATH/$PROJECT_NAME.tar.gz" $SERVER_USER@$SERVER_IP:/tmp/

    check_success "Application transferred successfully" "Failed to transfer application"

    # Extract on server
    print_status "Extracting application on server..."
    sshpass -p "$SERVER_PASSWORD" ssh -o StrictHostKeyChecking=no $SERVER_USER@$SERVER_IP << EOF
        cd $REMOTE_APP_PATH
        tar -xzf /tmp/$PROJECT_NAME.tar.gz
        rm /tmp/$PROJECT_NAME.tar.gz
        chown -R www-data:www-data $REMOTE_APP_PATH
EOF

    check_success "Application extracted successfully" "Failed to extract application"
}

# Step 4: Setup and start systemd service
setup_systemd_service() {
    print_step "Step 4: Setting up systemd service..."

    print_status "Checking for port conflicts and creating systemd service..."

    sshpass -p "$SERVER_PASSWORD" ssh -o StrictHostKeyChecking=no $SERVER_USER@$SERVER_IP << 'EOF'
# Check what's using port 5000
if lsof -i:5000 >/dev/null 2>&1; then
    echo "Port 5000 is in use. Checking what's using it..."
    lsof -i:5000
    # Kill any existing service on port 5000
    systemctl stop EmailService 2>/dev/null || true
    sleep 2
fi

# Use port 5001 instead if 5000 is still in use
PORT=5000
if lsof -i:5000 >/dev/null 2>&1; then
    PORT=5001
    echo "Using port $PORT instead"
fi
cat > /etc/systemd/system/EmailService.service << SERVICE
[Unit]
Description=EmailService .NET Web API
After=network.target

[Service]
Type=notify
User=www-data
WorkingDirectory=/var/www/EmailService
ExecStart=/usr/bin/dotnet /var/www/EmailService/EmailService.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=EmailService
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:${PORT}
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
TimeoutStartSec=120

[Install]
WantedBy=multi-user.target
SERVICE

# Ensure MySQL is running first
systemctl start mysql
systemctl enable mysql

# Wait a moment for MySQL to fully start
sleep 3

# Test database connection
echo "Testing database connection..."
mysql -u root -p"rizwan321" -e "CREATE DATABASE IF NOT EXISTS EmailServiceDb;" 2>/dev/null || echo "Database already exists or MySQL not ready"

# Reload systemd and start service
systemctl daemon-reload
systemctl stop EmailService 2>/dev/null || true

# Wait a bit for the stop to complete
sleep 2

systemctl start EmailService
systemctl enable EmailService

# Check service status
if systemctl is-active --quiet EmailService; then
    echo "Service is running"
    systemctl status EmailService --no-pager | head -10
else
    echo "Service failed to start"
    journalctl -u EmailService -n 50 --no-pager
    exit 1
fi
EOF

    check_success "Service configured and started" "Failed to setup service"
}

# Step 5: Configure Nginx with HTTPS
configure_nginx_https() {
    print_step "Step 5: Configuring Nginx with HTTPS..."

    print_status "Setting up Nginx configuration with SSL..."

    # First get the port that the service is using
    APP_PORT=$(sshpass -p "$SERVER_PASSWORD" ssh -o StrictHostKeyChecking=no $SERVER_USER@$SERVER_IP "grep 'ASPNETCORE_URLS' /etc/systemd/system/EmailService.service | cut -d':' -f3")

    if [ -z "$APP_PORT" ]; then
        APP_PORT="5000"
    fi

    print_status "Application is configured to run on port $APP_PORT"

    sshpass -p "$SERVER_PASSWORD" ssh -o StrictHostKeyChecking=no $SERVER_USER@$SERVER_IP << EOF
# Create Nginx configuration
cat > /etc/nginx/sites-available/$PROJECT_NAME << NGINX
server {
    listen 80;
    listen [::]:80;
    server_name $DOMAIN;

    # Redirect all HTTP traffic to HTTPS
    return 301 https://\\\$server_name\\\$request_uri;
}

server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name $DOMAIN;

    # SSL Configuration - will be updated by Certbot
    ssl_certificate /etc/letsencrypt/live/$DOMAIN/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/$DOMAIN/privkey.pem;

    # SSL Security Settings
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_prefer_server_ciphers off;
    ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384;
    ssl_session_timeout 1d;
    ssl_session_cache shared:SSL:10m;
    ssl_session_tickets off;
    ssl_stapling on;
    ssl_stapling_verify on;

    # Security Headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;

    # Logging
    access_log /var/log/nginx/${PROJECT_NAME}_access.log;
    error_log /var/log/nginx/${PROJECT_NAME}_error.log;

    # Proxy Settings
    location / {
        proxy_pass http://localhost:$APP_PORT;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \\\$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \\\$host;
        proxy_cache_bypass \\\$http_upgrade;
        proxy_set_header X-Forwarded-For \\\$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \\\$scheme;
        proxy_set_header X-Real-IP \\\$remote_addr;

        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }
}
NGINX

# Enable the site
ln -sf /etc/nginx/sites-available/$PROJECT_NAME /etc/nginx/sites-enabled/

# Remove default site if exists
rm -f /etc/nginx/sites-enabled/default

# Test Nginx configuration
nginx -t

if [ \$? -eq 0 ]; then
    echo "Nginx configuration is valid"
else
    echo "Nginx configuration test failed"
    nginx -t
    exit 1
fi
EOF

    check_success "Nginx configured" "Failed to configure Nginx"
}

# Step 6: Obtain SSL certificate
setup_ssl_certificate() {
    print_step "Step 6: Setting up SSL certificate..."

    print_status "Checking and obtaining SSL certificate..."

    sshpass -p "$SERVER_PASSWORD" ssh -o StrictHostKeyChecking=no $SERVER_USER@$SERVER_IP << EOF
# Check if certificate already exists
if [ -d "/etc/letsencrypt/live/$DOMAIN" ]; then
    echo "SSL certificate already exists for $DOMAIN"
    # Reload Nginx with existing certificate
    systemctl reload nginx
else
    echo "Obtaining new SSL certificate..."

    # First, create a temporary Nginx config for cert verification
    cat > /etc/nginx/sites-available/${PROJECT_NAME}_temp << 'TEMPNGINX'
server {
    listen 80;
    listen [::]:80;
    server_name $DOMAIN;

    location /.well-known/acme-challenge/ {
        root /var/www/certbot;
    }

    location / {
        return 301 https://\\\$server_name\\\$request_uri;
    }
}
TEMPNGINX

    # Create certbot directory
    mkdir -p /var/www/certbot

    # Temporarily use the temp config
    ln -sf /etc/nginx/sites-available/${PROJECT_NAME}_temp /etc/nginx/sites-enabled/$PROJECT_NAME
    systemctl reload nginx

    # Obtain certificate
    certbot certonly --webroot -w /var/www/certbot -d $DOMAIN --non-interactive --agree-tos --email admin@$DOMAIN

    if [ \$? -eq 0 ]; then
        echo "SSL certificate obtained successfully"

        # Switch back to the main config
        ln -sf /etc/nginx/sites-available/$PROJECT_NAME /etc/nginx/sites-enabled/$PROJECT_NAME
        rm -f /etc/nginx/sites-available/${PROJECT_NAME}_temp

        # Reload Nginx with SSL
        systemctl reload nginx

        # Setup auto-renewal
        (crontab -l 2>/dev/null | grep -v certbot; echo "0 0,12 * * * /usr/bin/certbot renew --quiet --post-hook 'systemctl reload nginx'") | crontab -
        echo "SSL auto-renewal configured"
    else
        echo "Failed to obtain SSL certificate"
        echo "Please ensure your domain $DOMAIN points to $SERVER_IP"

        # Use HTTP only configuration for now
        cat > /etc/nginx/sites-available/$PROJECT_NAME << HTTPONLY
server {
    listen 80;
    listen [::]:80;
    server_name $DOMAIN;

    location / {
        proxy_pass http://localhost:$APP_PORT;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \\\$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \\\$host;
        proxy_cache_bypass \\\$http_upgrade;
        proxy_set_header X-Forwarded-For \\\$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \\\$scheme;
    }
}
HTTPONLY

        ln -sf /etc/nginx/sites-available/$PROJECT_NAME /etc/nginx/sites-enabled/
        systemctl reload nginx
        echo "Using HTTP configuration for now. Run 'certbot --nginx -d $DOMAIN' manually when domain is ready."
    fi
fi
EOF

    print_status "SSL setup completed"
}

# Step 7: Verify deployment
verify_deployment() {
    print_step "Step 7: Verifying deployment..."

    print_status "Checking service status..."
    sshpass -p "$SERVER_PASSWORD" ssh -o StrictHostKeyChecking=no $SERVER_USER@$SERVER_IP << EOF
echo "=== Service Status ==="
systemctl is-active EmailService && echo "✓ Service is active" || echo "✗ Service is not active"

echo -e "\n=== Nginx Status ==="
systemctl is-active nginx && echo "✓ Nginx is active" || echo "✗ Nginx is not active"

echo -e "\n=== Application Response ==="
PORT=\$(grep 'ASPNETCORE_URLS' /etc/systemd/system/EmailService.service | cut -d':' -f3)
curl -s -o /dev/null -w "HTTP Status: %{http_code}\n" http://localhost:\$PORT || echo "✗ Application not responding"

echo -e "\n=== SSL Certificate ==="
if [ -d "/etc/letsencrypt/live/$DOMAIN" ]; then
    echo "✓ SSL certificate exists"
    openssl x509 -in /etc/letsencrypt/live/$DOMAIN/cert.pem -noout -dates
else
    echo "⚠ SSL certificate not configured"
fi

echo -e "\n=== Recent Application Logs ==="
journalctl -u EmailService -n 10 --no-pager
EOF
}

# Clean up function
cleanup() {
    print_status "Cleaning up temporary files..."
    rm -f "$LOCAL_PROJECT_PATH/$PROJECT_NAME.tar.gz"
}

# Main deployment function
main() {
    print_status "Starting deployment process..."
    print_status "Server: $SERVER_USER@$SERVER_IP"
    print_status "Domain: $DOMAIN"
    print_status "Project: $PROJECT_NAME"
    echo ""

    # Check if we're in the correct directory
    if [ ! -f "$LOCAL_PROJECT_PATH/$PROJECT_NAME.csproj" ]; then
        print_error "Project file not found. Please run this script from the project root directory."
        exit 1
    fi

    # Execute deployment steps
    build_application
    create_deployment_package
    transfer_to_server
    setup_systemd_service
    configure_nginx_https
    setup_ssl_certificate
    verify_deployment
    cleanup

    print_status "==================== Deployment Summary ===================="
    print_status "✓ Application built and deployed to $REMOTE_APP_PATH"
    print_status "✓ Systemd service '$PROJECT_NAME' configured and started"
    print_status "✓ Nginx configured with HTTPS on port 443"
    print_status "✓ SSL certificate configured (if domain is pointing to server)"
    print_status ""
    print_status "Access your application at:"
    print_status "  HTTP:  http://$DOMAIN"
    print_status "  HTTPS: https://$DOMAIN"
    print_status ""
    print_status "Useful commands:"
    print_status "  Check service: systemctl status $PROJECT_NAME"
    print_status "  View logs: journalctl -u $PROJECT_NAME -f"
    print_status "  Restart service: systemctl restart $PROJECT_NAME"
    print_status "  Reload Nginx: systemctl reload nginx"
    print_status "==========================================================="

    print_status "Deployment completed successfully!"
}

# Trap errors and cleanup
trap cleanup EXIT

# Run main function
main