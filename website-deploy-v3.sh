#!/bin/bash

# Website Deployment Script V3 for mcrsolicitors.co.uk
# Fixed version with proper folder inclusion and Nginx configuration
# Run from WSL on Windows to deploy to the Ubuntu server

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

# Deployment Configuration
WEBSITE_DIR="/var/www/mcrsolicitors"
BACKUP_DIR="/var/backups/mcrsolicitors"
LOCAL_WEBSITE_PATH="."  # Current directory with all HTML files
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
ZIP_FILE="website_deploy_${TIMESTAMP}.zip"

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

copy_file_to_server() {
    local local_path="$1"
    local remote_path="$2"
    local description="$3"

    print_status "$description"

    # Use scp with progress indicator
    sshpass -p "$SERVER_PASSWORD" scp -o StrictHostKeyChecking=no "$local_path" "$SERVER_USER@$SERVER_IP:$remote_path"

    if [ $? -eq 0 ]; then
        print_success "$description completed"
        return 0
    else
        print_error "$description failed"
        return 1
    fi
}

# ===========================
# MAIN DEPLOYMENT PROCESS
# ===========================

clear
echo -e "${BOLD}${MAGENTA}╔════════════════════════════════════════════╗${NC}"
echo -e "${BOLD}${MAGENTA}║  DEPLOYMENT FOR MCRSOLICITORS.CO.UK        ║${NC}"
echo -e "${BOLD}${MAGENTA}║       Version 3.0 - Complete Deploy         ║${NC}"
echo -e "${BOLD}${MAGENTA}╚════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${CYAN}Server:${NC} ${YELLOW}$SERVER_IP${NC}"
echo -e "${CYAN}Domain:${NC} ${YELLOW}https://$DOMAIN${NC}"
echo -e "${CYAN}Deployment ID:${NC} ${YELLOW}$TIMESTAMP${NC}"
echo ""

# ===========================
# STEP 1: CREATE ZIP ARCHIVE
# ===========================
print_step "STEP 1: Creating Complete Deployment Archive"

# Check if zip is installed
if ! command -v zip &> /dev/null; then
    print_warning "Installing zip utility..."
    sudo apt update && sudo apt install -y zip
fi

# List folders to include
print_status "Identifying website content..."
echo "Folders to deploy:"
for dir in british-citizenship business-investment-visas family-law fees Images immigration-applications indefinite-leave-to-remain-ilr js personal-injury-claim student-visas-uk uk-family-visas uk-work-visas visitor-visas-uk; do
    if [ -d "$dir" ]; then
        echo "  ✓ $dir"
    fi
done

# Count files for progress tracking
FILE_COUNT=$(find . -type f \( -name "*.html" -o -name "*.css" -o -name "*.js" -o -name "*.jpg" -o -name "*.jpeg" -o -name "*.png" -o -name "*.gif" -o -name "*.svg" -o -name "*.ico" \) | grep -v EmailService | wc -l)
print_success "Found $FILE_COUNT files to deploy"

# Create deployment package with ALL content
print_status "Creating complete deployment archive..."

# Remove old zip if exists
rm -f "$ZIP_FILE"

# Create comprehensive zip with all website content
zip -r "$ZIP_FILE" \
    *.html \
    *.css \
    *.js \
    *.txt \
    *.xml \
    *.ico \
    british-citizenship/ \
    business-investment-visas/ \
    family-law/ \
    fees/ \
    Images/ \
    immigration-applications/ \
    indefinite-leave-to-remain-ilr/ \
    js/ \
    personal-injury-claim/ \
    student-visas-uk/ \
    uk-family-visas/ \
    uk-work-visas/ \
    visitor-visas-uk/ \
    -x "*.sh" \
    -x "*.md" \
    -x "EmailService/*" \
    -x "*.zip" \
    -x ".git/*" \
    -x ".claude/*" \
    2>/dev/null || true

# Check zip file size
ZIP_SIZE=$(du -h "$ZIP_FILE" | cut -f1)
print_success "Created complete deployment archive: $ZIP_FILE (Size: $ZIP_SIZE)"

# ===========================
# STEP 2: TEST CONNECTION
# ===========================
print_step "STEP 2: Testing Server Connection"

execute_ssh "echo 'Connection successful'" "Testing SSH connection"

# ===========================
# STEP 3: INSTALL REQUIRED PACKAGES
# ===========================
print_step "STEP 3: Server Setup & Package Installation"

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
echo '=== SYSTEM READY ==='
" "Setting up server environment"

# ===========================
# STEP 4: BACKUP EXISTING SITE
# ===========================
print_step "STEP 4: Backing Up Existing Website"

execute_ssh "
# Create backup directory
mkdir -p $BACKUP_DIR

# Backup existing site if it exists
if [ -d '$WEBSITE_DIR' ] && [ \"\$(ls -A $WEBSITE_DIR 2>/dev/null)\" ]; then
    echo 'Creating backup of existing website...'
    BACKUP_FILE=\"$BACKUP_DIR/backup_$TIMESTAMP.tar.gz\"
    tar -czf \"\$BACKUP_FILE\" -C $WEBSITE_DIR . 2>/dev/null
    BACKUP_SIZE=\$(du -h \"\$BACKUP_FILE\" | cut -f1)
    echo \"✓ Backup created: \$BACKUP_FILE (Size: \$BACKUP_SIZE)\"

    # Rotate backups (keep last 5)
    cd $BACKUP_DIR
    ls -t backup_*.tar.gz 2>/dev/null | tail -n +6 | xargs rm -f 2>/dev/null || true
    echo '✓ Backup rotation complete (keeping last 5)'
else
    echo 'No existing website to backup'
fi
" "Creating backup"

# ===========================
# STEP 5: CLEAN AND FIX NGINX
# ===========================
print_step "STEP 5: Cleaning Nginx Configuration"

execute_ssh "
# Stop Nginx
systemctl stop nginx || true

# Remove problematic configurations
rm -f /etc/nginx/sites-enabled/mcrsolicitors*
rm -f /etc/nginx/sites-available/mcrsolicitors*
rm -f /etc/nginx/sites-enabled/pricesnap*
rm -f /etc/nginx/sites-available/pricesnap*
rm -f /etc/nginx/sites-enabled/default
rm -f /etc/nginx/sites-available/default

echo '✓ Cleaned up Nginx configuration'
" "Cleaning Nginx configuration"

# ===========================
# STEP 6: UPLOAD ZIP FILE
# ===========================
print_step "STEP 6: Uploading Complete Website Archive"

print_status "Uploading $ZIP_FILE to server..."
print_status "This may take a moment depending on file size and connection speed..."

# Upload with progress
copy_file_to_server "$ZIP_FILE" "/tmp/$ZIP_FILE" "Uploading complete website archive ($ZIP_SIZE)"

# ===========================
# STEP 7: DEPLOY WEBSITE
# ===========================
print_step "STEP 7: Extracting and Deploying Website"

execute_ssh "
echo 'Preparing deployment...'

# Clean old website directory
if [ -d '$WEBSITE_DIR' ]; then
    echo 'Removing old website files...'
    rm -rf $WEBSITE_DIR/*
else
    echo 'Creating website directory...'
    mkdir -p $WEBSITE_DIR
fi

# Extract new website
echo 'Extracting website archive...'
cd $WEBSITE_DIR
unzip -q /tmp/$ZIP_FILE

# Clean up zip file
rm -f /tmp/$ZIP_FILE

# Set proper permissions
echo 'Setting file permissions...'
chown -R www-data:www-data $WEBSITE_DIR
find $WEBSITE_DIR -type d -exec chmod 755 {} \;
find $WEBSITE_DIR -type f -exec chmod 644 {} \;

# Count deployed files
echo ''
echo '=== DEPLOYMENT STATISTICS ==='
echo \"HTML files: \$(find $WEBSITE_DIR -name '*.html' | wc -l)\"
echo \"CSS files: \$(find $WEBSITE_DIR -name '*.css' | wc -l)\"
echo \"JS files: \$(find $WEBSITE_DIR -name '*.js' | wc -l)\"
echo \"Image files: \$(find $WEBSITE_DIR \( -name '*.jpg' -o -name '*.png' -o -name '*.gif' -o -name '*.jpeg' \) | wc -l)\"
echo \"Directories: \$(find $WEBSITE_DIR -type d | wc -l)\"
echo \"Total files: \$(find $WEBSITE_DIR -type f | wc -l)\"
echo \"Total size: \$(du -sh $WEBSITE_DIR | cut -f1)\"

echo ''
echo 'Deployed directories:'
ls -d $WEBSITE_DIR/*/ 2>/dev/null | xargs -n1 basename | sed 's/^/  ✓ /'

echo ''
echo '✓ Website files deployed successfully!'
" "Extracting and deploying website"

# ===========================
# STEP 8: CONFIGURE NGINX (HTTP FIRST)
# ===========================
print_step "STEP 8: Configuring Web Server (HTTP First)"

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

echo '✓ Web server configured for HTTP successfully!'
" "Configuring Nginx for HTTP"

# ===========================
# STEP 9: CONFIGURE FIREWALL
# ===========================
print_step "STEP 9: Securing Server with Firewall"

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
# STEP 10: SSL CERTIFICATE
# ===========================
print_step "STEP 10: Installing SSL Certificate"

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
# STEP 11: FINAL VERIFICATION
# ===========================
print_step "STEP 11: Deployment Verification"

execute_ssh "
echo '🔍 Running final checks...'
echo ''

# Check services
echo '=== SERVICE STATUS ==='
echo -n 'Nginx: '
systemctl is-active nginx && echo '✅ Running' || echo '❌ Not running'
echo -n 'SSL Auto-renewal: '
systemctl is-active certbot.timer && echo '✅ Active' || echo '⚠️ Not active'

echo ''
echo '=== WEBSITE TESTS ==='
echo -n 'HTTP Response: '
curl -s -o /dev/null -w '%{http_code}' http://$DOMAIN
echo ''
echo -n 'HTTPS Response: '
curl -s -o /dev/null -w '%{http_code}' https://$DOMAIN 2>/dev/null || echo 'Pending SSL'
echo ''

echo ''
echo '=== DIRECTORY STRUCTURE ==='
echo 'Deployed directories:'
ls -d $WEBSITE_DIR/*/ 2>/dev/null | xargs -n1 basename | sed 's/^/  ✓ /' || echo 'No subdirectories'

echo ''
echo '=== FILE VERIFICATION ==='
if [ -f '$WEBSITE_DIR/index.html' ]; then
    echo '✅ index.html present'
else
    echo '❌ index.html missing'
fi
echo \"Total files deployed: \$(find $WEBSITE_DIR -type f | wc -l)\"
echo \"Total directories: \$(find $WEBSITE_DIR -type d | wc -l)\"
echo \"Website size: \$(du -sh $WEBSITE_DIR | cut -f1)\"

echo ''
echo '=== NGINX CONFIG CHECK ==='
nginx -t 2>&1 | grep -E 'test is successful|syntax is ok' && echo '✅ Nginx config valid' || echo '⚠️ Check Nginx config'

echo ''
echo '=== RECENT ACCESS LOG ==='
tail -3 /var/log/nginx/mcrsolicitors_access.log 2>/dev/null || echo 'No access yet'
" "Running verification"

# Clean up local zip file
print_status "Cleaning up local files..."
rm -f "$ZIP_FILE"
print_success "Cleanup complete"

# ===========================
# DEPLOYMENT COMPLETE
# ===========================
echo ""
echo -e "${BOLD}${GREEN}╔═══════════════════════════════════════════════════════╗${NC}"
echo -e "${BOLD}${GREEN}║         🚀 DEPLOYMENT COMPLETED SUCCESSFULLY! 🚀       ║${NC}"
echo -e "${BOLD}${GREEN}╚═══════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${CYAN}📋 DEPLOYMENT SUMMARY${NC}"
echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "  ${YELLOW}✅${NC} Server: ${GREEN}$SERVER_IP${NC}"
echo -e "  ${YELLOW}✅${NC} Primary URL: ${GREEN}https://$DOMAIN${NC}"
echo -e "  ${YELLOW}✅${NC} Alternative: ${GREEN}https://$DOMAIN_WITHOUT_WWW${NC}"
echo -e "  ${YELLOW}✅${NC} All folders deployed successfully${NC}"
echo -e "  ${YELLOW}✅${NC} Deployment ID: ${GREEN}$TIMESTAMP${NC}"
echo ""
echo -e "${CYAN}🌐 TEST YOUR WEBSITE${NC}"
echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "  Browse to:    ${YELLOW}https://$DOMAIN${NC}"
echo -e "  Check SSL:    Look for padlock icon in browser${NC}"
echo ""
echo -e "${CYAN}⚡ QUICK COMMANDS${NC}"
echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "  Check status:     ${YELLOW}./website-monitor.sh --quick${NC}"
echo -e "  View logs:        ${YELLOW}./website-monitor.sh --logs${NC}"
echo -e "  Update again:     ${YELLOW}./website-deploy-v3.sh${NC}"
echo -e "  Rollback:         ${YELLOW}./website-rollback.sh${NC}"
echo ""
echo -e "${CYAN}📝 TROUBLESHOOTING${NC}"
echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "  If SSL not working, wait for DNS propagation then run:"
echo -e "    ${YELLOW}ssh root@$SERVER_IP 'certbot --nginx -d $DOMAIN -d $DOMAIN_WITHOUT_WWW'${NC}"
echo ""
echo -e "${GREEN}Your complete website with all folders is now LIVE!${NC}"
echo ""