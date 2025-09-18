#!/bin/bash

# Website Deployment Script for mcrsolicitors.co.uk
# This script handles website file deployment only
# Assumes server is already configured with server-setup.sh

set -e

# ===========================
# SERVER CONFIGURATION
# ===========================
SERVER_IP="172.237.117.145"
SERVER_USER="root"
SERVER_PASSWORD="Braveheart1190!12"
DOMAIN="www.mcrsolicitors.co.uk"
DOMAIN_WITHOUT_WWW="mcrsolicitors.co.uk"

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
echo -e "${BOLD}${MAGENTA}║   WEBSITE DEPLOYMENT FOR MCRSOLICITORS     ║${NC}"
echo -e "${BOLD}${MAGENTA}║          Website Files Update Only          ║${NC}"
echo -e "${BOLD}${MAGENTA}╚════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${CYAN}Server:${NC} ${YELLOW}$SERVER_IP${NC}"
echo -e "${CYAN}Domain:${NC} ${YELLOW}https://$DOMAIN${NC}"
echo -e "${CYAN}Deployment ID:${NC} ${YELLOW}$TIMESTAMP${NC}"
echo ""

# ===========================
# STEP 1: TEST CONNECTION
# ===========================
print_step "STEP 1: Testing Server Connection"

execute_ssh "echo 'Connection successful'" "Testing SSH connection"

# ===========================
# STEP 2: CREATE ZIP ARCHIVE
# ===========================
print_step "STEP 2: Creating Website Archive"

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
print_status "Creating deployment archive..."

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
    sitemap.xml \
    robots.txt \
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
print_success "Created deployment archive: $ZIP_FILE (Size: $ZIP_SIZE)"

# ===========================
# STEP 3: BACKUP EXISTING SITE
# ===========================
print_step "STEP 3: Backing Up Current Website"

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
# STEP 4: UPLOAD ZIP FILE
# ===========================
print_step "STEP 4: Uploading Website Archive"

print_status "Uploading $ZIP_FILE to server..."
print_status "This may take a moment depending on file size and connection speed..."

# Upload with progress
copy_file_to_server "$ZIP_FILE" "/tmp/$ZIP_FILE" "Uploading website archive ($ZIP_SIZE)"

# ===========================
# STEP 5: DEPLOY WEBSITE
# ===========================
print_step "STEP 5: Deploying Website Files"

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
" "Deploying website files"

# ===========================
# STEP 6: RESTART NGINX
# ===========================
print_step "STEP 6: Restarting Web Server"

execute_ssh "
# Test Nginx configuration
nginx -t

# Reload Nginx to ensure new files are served
systemctl reload nginx

echo '✓ Web server reloaded successfully!'
" "Restarting web server"

# ===========================
# STEP 7: SSL CERTIFICATE CHECK & RENEWAL
# ===========================
print_step "STEP 7: Checking and Setting Up SSL Certificate"

print_warning "Checking SSL certificate status and applying if needed..."
print_warning "Note: Domain must be pointing to $SERVER_IP"

execute_ssh "
# Check if SSL certificate already exists and is valid
if [ -d /etc/letsencrypt/live/$DOMAIN_WITHOUT_WWW ]; then
    echo 'SSL certificate directory found. Checking validity...'

    # Check certificate expiry
    CERT_EXPIRY=\$(openssl x509 -enddate -noout -in /etc/letsencrypt/live/$DOMAIN_WITHOUT_WWW/cert.pem 2>/dev/null | cut -d= -f2)
    if [ ! -z \"\$CERT_EXPIRY\" ]; then
        EXPIRY_EPOCH=\$(date -d \"\$CERT_EXPIRY\" +%s 2>/dev/null)
        CURRENT_EPOCH=\$(date +%s)
        DAYS_LEFT=\$(( (\$EXPIRY_EPOCH - \$CURRENT_EPOCH) / 86400 ))

        if [ \$DAYS_LEFT -gt 30 ]; then
            echo \"✅ SSL certificate is valid for \$DAYS_LEFT more days\"
            echo 'Skipping SSL setup - certificate is current'
        else
            echo \"⚠️ SSL certificate expires in \$DAYS_LEFT days - renewing...\"
            certbot renew --nginx --quiet
        fi
    else
        echo 'Certificate file issue - attempting fresh SSL setup...'
    fi
else
    echo 'No SSL certificate found - installing new certificate...'
fi

# Get or renew SSL certificate
certbot --nginx \
    -d $DOMAIN \
    -d $DOMAIN_WITHOUT_WWW \
    --non-interactive \
    --agree-tos \
    --email admin@mcrsolicitors.co.uk \
    --redirect \
    --keep-until-expiring \
    2>&1 | tee /tmp/certbot_log.txt

if grep -q 'Successfully' /tmp/certbot_log.txt || grep -q 'Congratulations' /tmp/certbot_log.txt || grep -q 'Certificate not yet due' /tmp/certbot_log.txt; then
    echo ''
    echo '✅ SSL certificate configured successfully!'

    # Enable auto-renewal if not already enabled
    systemctl enable certbot.timer >/dev/null 2>&1
    systemctl start certbot.timer >/dev/null 2>&1

    echo '✓ Auto-renewal enabled'

    # Test auto-renewal
    echo 'Testing auto-renewal setup...'
    certbot renew --dry-run --quiet && echo '✓ Auto-renewal test passed' || echo '⚠️ Auto-renewal test failed'
else
    echo ''
    echo '⚠️ SSL installation may need manual intervention'
    echo 'Check the logs above for details'
    echo ''
    echo 'To manually install SSL later, run:'
    echo \"  ssh root@$SERVER_IP 'certbot --nginx -d $DOMAIN -d $DOMAIN_WITHOUT_WWW'\"
fi

# Final reload with SSL configuration
systemctl reload nginx

# Verify SSL is working
echo ''
echo 'Testing SSL configuration...'
if curl -s -I https://$DOMAIN | grep -q 'HTTP/'; then
    echo '✅ HTTPS is responding correctly'
else
    echo '⚠️ HTTPS may not be configured correctly'
fi
" "Setting up SSL certificate" || true

# ===========================
# STEP 8: VERIFY DEPLOYMENT
# ===========================
print_step "STEP 8: Deployment Verification"

execute_ssh "
echo '🔍 Running deployment verification...'
echo ''

# Check services
echo '=== SERVICE STATUS ==='
echo -n 'Nginx: '
systemctl is-active nginx && echo '✅ Running' || echo '❌ Not running'

echo ''
echo '=== WEBSITE TESTS ==='
echo -n 'HTTP Response: '
curl -s -o /dev/null -w '%{http_code}' http://$DOMAIN
echo ''
echo -n 'HTTPS Response: '
curl -s -o /dev/null -w '%{http_code}' https://$DOMAIN 2>/dev/null || echo 'SSL not ready'
echo ''

echo '=== SSL CERTIFICATE STATUS ==='
if [ -d /etc/letsencrypt/live/$DOMAIN_WITHOUT_WWW ]; then
    echo '✅ SSL certificates found'
    CERT_EXPIRY=\$(openssl x509 -enddate -noout -in /etc/letsencrypt/live/$DOMAIN_WITHOUT_WWW/cert.pem 2>/dev/null | cut -d= -f2)
    if [ ! -z \"\$CERT_EXPIRY\" ]; then
        EXPIRY_EPOCH=\$(date -d \"\$CERT_EXPIRY\" +%s 2>/dev/null)
        CURRENT_EPOCH=\$(date +%s)
        DAYS_LEFT=\$(( (\$EXPIRY_EPOCH - \$CURRENT_EPOCH) / 86400 ))
        echo \"Certificate expires in \$DAYS_LEFT days\"
    fi
    echo -n 'Auto-renewal: '
    systemctl is-active certbot.timer && echo '✅ Active' || echo '⚠️ Not active'
else
    echo '⚠️ No SSL certificates found'
fi

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
echo -e "${BOLD}${GREEN}║      🚀 WEBSITE DEPLOYMENT COMPLETED SUCCESSFULLY! 🚀   ║${NC}"
echo -e "${BOLD}${GREEN}╚═══════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${CYAN}📋 DEPLOYMENT SUMMARY${NC}"
echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "  ${YELLOW}✅${NC} Server: ${GREEN}$SERVER_IP${NC}"
echo -e "  ${YELLOW}✅${NC} Primary URL: ${GREEN}https://$DOMAIN${NC}"
echo -e "  ${YELLOW}✅${NC} Alternative: ${GREEN}https://$DOMAIN_WITHOUT_WWW${NC}"
echo -e "  ${YELLOW}✅${NC} All folders deployed successfully${NC}"
echo -e "  ${YELLOW}✅${NC} Deployment ID: ${GREEN}$TIMESTAMP${NC}"
echo -e "  ${YELLOW}✅${NC} Backup created: ${GREEN}backup_$TIMESTAMP.tar.gz${NC}"
echo ""
echo -e "${CYAN}🌐 TEST YOUR WEBSITE${NC}"
echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "  Browse to:    ${YELLOW}https://$DOMAIN${NC}"
echo -e "  Check pages:  Test various pages and links${NC}"
echo ""
echo -e "${CYAN}⚡ QUICK COMMANDS${NC}"
echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "  Deploy again:     ${YELLOW}./website-deploy.sh${NC}"
echo -e "  Server setup:     ${YELLOW}./server-setup.sh${NC}"
echo -e "  Full deployment:  ${YELLOW}./website-deploy-v3.sh${NC}"
echo ""
echo -e "${CYAN}📝 ROLLBACK IF NEEDED${NC}"
echo -e "${CYAN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "  To rollback to previous version:"
echo -e "    ${YELLOW}ssh root@$SERVER_IP 'cd $BACKUP_DIR && tar -xzf backup_$TIMESTAMP.tar.gz -C $WEBSITE_DIR'${NC}"
echo ""
echo -e "${GREEN}Your website has been successfully deployed!${NC}"
echo ""