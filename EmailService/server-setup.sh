#!/bin/bash

# Server Configuration
SERVER_IP="172.237.101.211"
SERVER_USER="root"
SERVER_PASSWORD="Braveheart1190!12"
DOMAIN="emailservice.pricesnap.co.uk"
MYSQL_PASSWORD="rizwan321"
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

# Function to check if a command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to check if a package is installed (for apt-based systems)
package_installed() {
    dpkg -l | grep -q "^ii.*$1"
}

# Update system packages
update_system() {
    print_status "Updating system packages..."
    apt-get update -y
    apt-get upgrade -y
}

# Install Nginx
install_nginx() {
    print_status "Checking Nginx installation..."

    if command_exists nginx; then
        print_warning "Nginx is already installed"
        nginx -v
    else
        print_status "Installing Nginx..."
        apt-get install -y nginx

        if [ $? -eq 0 ]; then
            print_status "Nginx installed successfully"
            systemctl enable nginx
            systemctl start nginx
            nginx -v
        else
            print_error "Failed to install Nginx"
            exit 1
        fi
    fi
}

# Install .NET 9 Runtime
install_dotnet() {
    print_status "Checking .NET 9 Runtime installation..."

    if command_exists dotnet && dotnet --list-runtimes | grep -q "Microsoft.AspNetCore.App 9"; then
        print_warning ".NET 9 Runtime is already installed"
        dotnet --list-runtimes | grep "9\."
    else
        print_status "Installing .NET 9 Runtime..."

        # Add Microsoft package repository
        wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
        dpkg -i packages-microsoft-prod.deb
        rm packages-microsoft-prod.deb

        # Update package list
        apt-get update -y

        # Install .NET 9 Runtime
        apt-get install -y aspnetcore-runtime-9.0

        if [ $? -eq 0 ]; then
            print_status ".NET 9 Runtime installed successfully"
            dotnet --list-runtimes | grep "9\."
        else
            print_error "Failed to install .NET 9 Runtime"
            exit 1
        fi
    fi
}

# Install MySQL
install_mysql() {
    print_status "Checking MySQL installation..."

    if command_exists mysql; then
        print_warning "MySQL is already installed"
        mysql --version
    else
        print_status "Installing MySQL Server..."

        # Set MySQL root password non-interactively
        export DEBIAN_FRONTEND=noninteractive
        debconf-set-selections <<< "mysql-server mysql-server/root_password password $MYSQL_PASSWORD"
        debconf-set-selections <<< "mysql-server mysql-server/root_password_again password $MYSQL_PASSWORD"

        # Install MySQL
        apt-get install -y mysql-server mysql-client

        if [ $? -eq 0 ]; then
            print_status "MySQL installed successfully"

            # Start and enable MySQL service
            systemctl enable mysql
            systemctl start mysql

            # Secure MySQL installation
            print_status "Securing MySQL installation..."
            mysql -u root -p"$MYSQL_PASSWORD" <<EOF
ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '$MYSQL_PASSWORD';
DELETE FROM mysql.user WHERE User='';
DELETE FROM mysql.user WHERE User='root' AND Host NOT IN ('localhost', '127.0.0.1', '::1');
DROP DATABASE IF EXISTS test;
DELETE FROM mysql.db WHERE Db='test' OR Db='test\\_%';
FLUSH PRIVILEGES;
EOF

            mysql --version
            print_status "MySQL root password set to: $MYSQL_PASSWORD"
        else
            print_error "Failed to install MySQL"
            exit 1
        fi
    fi
}

# Install Certbot
install_certbot() {
    print_status "Checking Certbot installation..."

    if command_exists certbot; then
        print_warning "Certbot is already installed"
        certbot --version
    else
        print_status "Installing Certbot..."

        # Install snapd if not installed
        if ! command_exists snap; then
            apt-get install -y snapd
            systemctl enable snapd
            systemctl start snapd
            sleep 5
        fi

        # Install certbot via snap
        snap install core
        snap refresh core
        snap install --classic certbot

        # Create symlink for certbot
        ln -sf /snap/bin/certbot /usr/bin/certbot

        if command_exists certbot; then
            print_status "Certbot installed successfully"
            certbot --version
        else
            print_error "Failed to install Certbot"
            exit 1
        fi
    fi
}

# Configure Nginx for the domain
configure_nginx() {
    print_status "Configuring Nginx for domain: $DOMAIN"

    # Create Nginx configuration for the domain
    cat > /etc/nginx/sites-available/$PROJECT_NAME <<EOF
server {
    listen 80;
    listen [::]:80;
    server_name $DOMAIN;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_cache_bypass \$http_upgrade;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
    }
}
EOF

    # Enable the site
    ln -sf /etc/nginx/sites-available/$PROJECT_NAME /etc/nginx/sites-enabled/

    # Test Nginx configuration
    nginx -t

    if [ $? -eq 0 ]; then
        print_status "Nginx configuration is valid"
        systemctl reload nginx
    else
        print_error "Nginx configuration test failed"
        exit 1
    fi
}

# Setup SSL certificate
setup_ssl() {
    print_status "Setting up SSL certificate for domain: $DOMAIN"

    # Check if certificate already exists
    if [ -d "/etc/letsencrypt/live/$DOMAIN" ]; then
        print_warning "SSL certificate already exists for $DOMAIN"
    else
        print_status "Obtaining SSL certificate..."
        certbot --nginx -d $DOMAIN --non-interactive --agree-tos --email admin@$DOMAIN --redirect

        if [ $? -eq 0 ]; then
            print_status "SSL certificate obtained successfully"

            # Setup auto-renewal
            print_status "Setting up SSL certificate auto-renewal..."
            (crontab -l 2>/dev/null; echo "0 0,12 * * * /usr/bin/certbot renew --quiet") | crontab -
            print_status "SSL auto-renewal configured"
        else
            print_warning "Failed to obtain SSL certificate. You may need to set it up manually."
        fi
    fi
}

# Create systemd service for the .NET application
create_systemd_service() {
    print_status "Creating systemd service for $PROJECT_NAME..."

    cat > /etc/systemd/system/$PROJECT_NAME.service <<EOF
[Unit]
Description=$PROJECT_NAME .NET Web API
After=network.target

[Service]
Type=notify
User=www-data
WorkingDirectory=/var/www/$PROJECT_NAME
ExecStart=/usr/bin/dotnet /var/www/$PROJECT_NAME/$PROJECT_NAME.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=$PROJECT_NAME
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOF

    systemctl daemon-reload
    print_status "Systemd service created for $PROJECT_NAME"
}

# Main installation function
main() {
    print_status "Starting server setup..."
    print_status "Server IP: $SERVER_IP"
    print_status "Domain: $DOMAIN"
    echo ""

    # Check if running as root
    if [ "$EUID" -ne 0 ]; then
        print_error "This script must be run as root"
        exit 1
    fi

    # Update system
    update_system

    # Install required packages
    install_nginx
    install_dotnet
    install_mysql
    install_certbot

    # Configure services
    configure_nginx

    # Create application directory
    mkdir -p /var/www/$PROJECT_NAME
    chown -R www-data:www-data /var/www/$PROJECT_NAME
    print_status "Application directory created: /var/www/$PROJECT_NAME"

    # Create systemd service
    create_systemd_service

    # Setup SSL (optional - uncomment if domain is already pointing to server)
    # setup_ssl

    print_status "==================== Setup Summary ===================="
    print_status "✓ Nginx installed and configured"
    print_status "✓ .NET 9 Runtime installed"
    print_status "✓ MySQL installed (root password: $MYSQL_PASSWORD)"
    print_status "✓ Certbot installed"
    print_status "✓ Systemd service created for $PROJECT_NAME"
    print_status "✓ Application directory: /var/www/$PROJECT_NAME"
    print_status ""
    print_status "Next steps:"
    print_status "1. Deploy your application to /var/www/$PROJECT_NAME"
    print_status "2. Start the service: systemctl start $PROJECT_NAME"
    print_status "3. Enable auto-start: systemctl enable $PROJECT_NAME"
    print_status "4. Setup SSL certificate: certbot --nginx -d $DOMAIN"
    print_status "========================================================"

    print_status "Server setup completed successfully!"
}

# Run main function
main