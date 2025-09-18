#!/bin/bash

# Server Setup Script for mcrsolicitors.co.uk
# This script handles initial server configuration, package installation, and SSL setup
# Run this script once when setting up a new server

set -e

# ===========================
# SERVER CONFIGURATION
# ===========================
SERVER_IP="172.237.117.145"
SERVER_USER="root"
SERVER_PASSWORD="Braveheart1190!12"
DOMAIN="www.mcrsolicitors.co.uk"
DOMAIN_WITHOUT_WWW="mcrsolicitors.co.uk"
ADMIN_EMAIL="admin@mcrsolicitors.co.uk"
WEBSITE_DIR="/var/www/mcrsolicitors"

# ===========================
# COLOR OUTPUT FUNCTIONS
# ===========================
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
MAGENTA='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m'
BOLD='\033[1m'

print_status() { echo -e "${BLUE}[INFO]${NC} $1"; }
print_success() { echo -e "${GREEN}[SUCCESS]${NC} $1"; }
print_error() { echo -e "${RED}[ERROR]${NC} $1"; }
print_warning() { echo -e "${YELLOW}[WARNING]${NC} $1"; }
print_step() {
    echo -e "\n${CYAN}┌─────────────────────────────────────────┐${NC}"
    echo -e "${CYAN}│${NC} ${MAGENTA}$1${NC}"
    echo -e "${CYAN}└─────────────────────────────────────────┘${NC}"
}

# ===========================
# SSH EXECUTION FUNCTIONS
# ===========================
execute_ssh() {
    local command="$1"
    local description="$2"

    print_status "$description"

    # Install sshpass if not available
    if ! command -v sshpass &> /dev/null; then
        print_warning "sshpass not found. Installing..."
        sudo apt update && sudo apt install -y sshpass
    fi

    sshpass -p "$SERVER_PASSWORD" ssh -o StrictHostKeyChecking=no "$SERVER_USER@$SERVER_IP" "$command"

    if [ $? -eq 0 ]; then
        print_success "$description completed"
        return 0
    else
        print_error "$description failed"
        return 1
    fi
}

# ===========================
# MAIN SERVER SETUP PROCESS
# ===========================

clear
echo -e "${BOLD}${MAGENTA}╔════════════════════════════════════════════╗${NC}"
echo -e "${BOLD}${MAGENTA}║     SERVER SETUP FOR MCRSOLICITORS.CO.UK    ║${NC}"
echo -e "${BOLD}${MAGENTA}║         Initial Server Configuration         ║${NC}"
echo -e "${BOLD}${MAGENTA}╚════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${CYAN}Server:${NC} ${YELLOW}$SERVER_IP${NC}"
echo -e "${CYAN}Domain:${NC} ${YELLOW}https://$DOMAIN${NC}"
echo ""

# ===========================
# STEP 1: TEST CONNECTION
# ===========================
print_step "STEP 1: Testing Server Connection"

execute_ssh "echo 'Connection successful'" "Testing SSH connection"

# ===========================
# STEP 2: INSTALL REQUIRED PACKAGES
# ===========================
print_step "STEP 2: Installing Required Server Packages"

execute_ssh "
export DEBIAN_FRONTEND=noninteractive

# Update package lists
echo 'Updating package lists...'
apt update -y >/dev/null 2>&1

# Install required packages if not present
PACKAGES_TO_INSTALL=''

# Check Nginx
if ! which nginx >/dev/null 2>&1; then
    PACKAGES_TO_INSTALL=\"\$PACKAGES_TO_INSTALL nginx\"
    echo '📦 Nginx will be installed'
else
    echo '✓ Nginx already installed'
fi

# Check Certbot
if ! which certbot >/dev/null 2>&1; then
    PACKAGES_TO_INSTALL=\"\$PACKAGES_TO_INSTALL certbot python3-certbot-nginx\"
    echo '📦 Certbot will be installed'
else
    echo '✓ Certbot already installed'
fi

# Check UFW
if ! which ufw >/dev/null 2>&1; then
    PACKAGES_TO_INSTALL=\"\$PACKAGES_TO_INSTALL ufw\"
    echo '📦 UFW will be installed'
else
    echo '✓ UFW already installed'
fi

# Check unzip
if ! which unzip >/dev/null 2>&1; then
    PACKAGES_TO_INSTALL=\"\$PACKAGES_TO_INSTALL unzip\"
    echo '📦 Unzip will be installed'
else
    echo '✓ Unzip already installed'
fi

# Install if needed
if [ ! -z \"\$PACKAGES_TO_INSTALL\" ]; then
    echo ''
    echo 'Installing required packages...'
    apt install -y \$PACKAGES_TO_INSTALL >/dev/null 2>&1
    echo 'Package installation complete!'
fi

# Enable services
systemctl enable nginx >/dev/null 2>&1 || true

echo ''
echo '=== SYSTEM PACKAGES READY ==='
" "Setting up server packages"

# ===========================
# STEP 3: CLEAN NGINX CONFIGURATION
# ===========================
print_step "STEP 3: Cleaning Nginx Configuration"

execute_ssh "
# Stop Nginx
systemctl stop nginx || true

# Remove any existing conflicting configurations
rm -f /etc/nginx/sites-enabled/mcrsolicitors*
rm -f /etc/nginx/sites-available/mcrsolicitors*
rm -f /etc/nginx/sites-enabled/pricesnap*
rm -f /etc/nginx/sites-available/pricesnap*
rm -f /etc/nginx/sites-enabled/default
rm -f /etc/nginx/sites-available/default

echo '✓ Cleaned up Nginx configuration'
" "Cleaning Nginx configuration"

# ===========================
# STEP 4: CREATE WEBSITE DIRECTORY
# ===========================
print_step "STEP 4: Creating Website Directory Structure"

execute_ssh "
# Create website directory if it doesn't exist
if [ ! -d '$WEBSITE_DIR' ]; then
    mkdir -p $WEBSITE_DIR
    echo '✓ Created website directory: $WEBSITE_DIR'
else
    echo '✓ Website directory already exists: $WEBSITE_DIR'
fi

# Set proper ownership
chown -R www-data:www-data $WEBSITE_DIR
chmod 755 $WEBSITE_DIR

# Create backup directory
mkdir -p /var/backups/mcrsolicitors
echo '✓ Created backup directory: /var/backups/mcrsolicitors'

echo ''
echo '=== DIRECTORY STRUCTURE READY ==='
" "Creating directory structure"

# ===========================
# STEP 5: CONFIGURE NGINX (HTTP)
# ===========================
print_step "STEP 5: Configuring Nginx for HTTP"

execute_ssh "
# Create initial HTTP-only configuration
cat > /etc/nginx/sites-available/mcrsolicitors << 'EOF'
server {
    listen 80;
    listen [::]:80;

    server_name $DOMAIN $DOMAIN_WITHOUT_WWW;

    root $WEBSITE_DIR;
    index index.html index.htm;

    # Security headers
    add_header X-Frame-Options \"SAMEORIGIN\" always;
    add_header X-XSS-Protection \"1; mode=block\" always;
    add_header X-Content-Type-Options \"nosniff\" always;
    add_header Referrer-Policy \"no-referrer-when-downgrade\" always;

    # Enable gzip compression
    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_types text/plain text/css text/xml application/javascript application/json application/xml+rss application/rss+xml application/atom+xml image/svg+xml font/opentype application/vnd.ms-fontobject image/x-icon;

    # Cache static assets
    location ~* \.(jpg|jpeg|png|gif|ico|css|js|svg|woff|woff2|ttf|eot)$ {
        expires 30d;
        add_header Cache-Control \"public, immutable\";
        access_log off;
    }

    # Main location
    location / {
        try_files \$uri \$uri/ /index.html;
    }

    # Deny access to hidden files
    location ~ /\. {
        deny all;
        access_log off;
        log_not_found off;
    }

    # Custom error pages
    error_page 404 /404.html;
    location = /404.html {
        internal;
    }

    # Logging
    access_log /var/log/nginx/mcrsolicitors_access.log;
    error_log /var/log/nginx/mcrsolicitors_error.log;
}
EOF

# Enable the site
ln -sf /etc/nginx/sites-available/mcrsolicitors /etc/nginx/sites-enabled/

# Test configuration
nginx -t

# Start Nginx
systemctl start nginx
systemctl reload nginx

echo '✓ Nginx configured for HTTP successfully!'
" "Configuring Nginx"

# ===========================
# STEP 6: CONFIGURE FIREWALL
# ===========================
print_step "STEP 6: Configuring Firewall"

execute_ssh "
# Configure UFW
ufw --force enable >/dev/null 2>&1
ufw allow ssh >/dev/null 2>&1
ufw allow 'Nginx Full' >/dev/null 2>&1
ufw allow 80/tcp >/dev/null 2>&1
ufw allow 443/tcp >/dev/null 2>&1

echo '✓ Firewall configured'
echo ''
echo 'Open ports:'
ufw status | grep ALLOW
" "Configuring firewall"

# ===========================
# STEP 7: SETUP SSL CERTIFICATE
# ===========================
print_step "STEP 7: Setting Up SSL Certificate"

print_warning "Attempting SSL certificate installation..."
print_warning "Note: Domain must be pointing to $SERVER_IP"

execute_ssh "
# Get SSL certificate (this will modify nginx config automatically)
certbot --nginx \
    -d $DOMAIN \
    -d $DOMAIN_WITHOUT_WWW \
    --non-interactive \
    --agree-tos \
    --email $ADMIN_EMAIL \
    --redirect \
    --keep-until-expiring \
    2>&1 | tee /tmp/certbot_log.txt

if grep -q 'Successfully' /tmp/certbot_log.txt || grep -q 'Congratulations' /tmp/certbot_log.txt || grep -q 'Certificate not yet due' /tmp/certbot_log.txt; then
    echo ''
    echo '✅ SSL certificate configured successfully!'

    # Enable auto-renewal
    systemctl enable certbot.timer >/dev/null 2>&1
    systemctl start certbot.timer >/dev/null 2>&1

    echo '✓ Auto-renewal enabled'
else
    echo ''
    echo '⚠️ SSL installation may need manual intervention'
    echo 'Check the logs above for details'
    echo ''
    echo 'To manually install SSL later, run:'
    echo \"  ssh root@$SERVER_IP 'certbot --nginx -d $DOMAIN -d $DOMAIN_WITHOUT_WWW'\"
fi

# Reload Nginx with final configuration
systemctl reload nginx
" "Setting up SSL certificate" || true

# ===========================
# STEP 8: VERIFY SETUP
# ===========================
print_step "STEP 8: Server Setup Verification"

execute_ssh "
echo '🔍 Running server setup verification...'
echo ''

# Check services
echo '=== SERVICE STATUS ==='
echo -n 'Nginx: '
systemctl is-active nginx && echo '✅ Running' || echo '❌ Not running'
echo -n 'SSL Auto-renewal: '
systemctl is-active certbot.timer && echo '✅ Active' || echo '⚠️ Not active'
echo -n 'UFW Firewall: '
ufw status | grep -q active && echo '✅ Active' || echo '⚠️ Not active'

echo ''
echo '=== INSTALLED PACKAGES ==='
echo -n 'Nginx version: '
nginx -v 2>&1 | grep -oP 'nginx/\K[\d.]+' || echo 'Not installed'
echo -n 'Certbot version: '
certbot --version 2>&1 | grep -oP 'certbot \K[\d.]+' || echo 'Not installed'

echo ''
echo '=== DIRECTORY STRUCTURE ==='
echo \"Website directory: $WEBSITE_DIR\"
ls -la $WEBSITE_DIR | head -5

echo ''
echo '=== NGINX CONFIG CHECK ==='
nginx -t 2>&1 | grep -E 'test is successful|syntax is ok' && echo '✅ Nginx config valid' || echo '⚠️ Check Nginx config'

echo ''
echo '=== SSL CERTIFICATE STATUS ==='
if [ -d /etc/letsencrypt/live/$DOMAIN_WITHOUT_WWW ]; then
    echo '✅ SSL certificates found'
    certbot certificates 2>/dev/null | grep -E 'Domains|Expiry' | head -4
else
    echo '⚠️ No SSL certificates installed yet'
fi
" "Verifying server setup"

# ===========================
# SETUP COMPLETE
# ===========================
echo ""
echo -e "${BOLD}${GREEN}╔═══════════════════════════════════════════════════════╗${NC}"
echo -e "${BOLD}${GREEN}║       🚀 SERVER SETUP COMPLETED SUCCESSFULLY! 🚀       ║${NC}"
echo -e "${BOLD}${GREEN}╚═══════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${CYAN}📋 SERVER CONFIGURATION SUMMARY${NC}"
echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "  ${YELLOW}✅${NC} Server: ${GREEN}$SERVER_IP${NC}"
echo -e "  ${YELLOW}✅${NC} Nginx: Installed and configured${NC}"
echo -e "  ${YELLOW}✅${NC} SSL/Certbot: Installed${NC}"
echo -e "  ${YELLOW}✅${NC} Firewall: Configured${NC}"
echo -e "  ${YELLOW}✅${NC} Website directory: ${GREEN}$WEBSITE_DIR${NC}"
echo ""
echo -e "${CYAN}📝 NEXT STEPS${NC}"
echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "  1. Run website deployment: ${YELLOW}./website-deploy.sh${NC}"
echo -e "  2. Verify SSL certificate is working${NC}"
echo -e "  3. Test website at ${YELLOW}https://$DOMAIN${NC}"
echo ""
echo -e "${CYAN}⚡ TROUBLESHOOTING${NC}"
echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "  If SSL not working, ensure DNS is pointing to server, then:"
echo -e "    ${YELLOW}ssh root@$SERVER_IP 'certbot --nginx -d $DOMAIN -d $DOMAIN_WITHOUT_WWW'${NC}"
echo ""
echo -e "${GREEN}Server is ready for website deployment!${NC}"
echo ""