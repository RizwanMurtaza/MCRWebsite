/**
 * MCR Solicitors Email Service Configuration
 * Production configuration for email service integration
 */

window.EmailServiceConfig = {
    // Production API endpoint
    API_BASE: 'https://emailservice.mcrsolicitors.co.uk/api',

    // Application identifier for MCR Solicitors
    APP_ID: 'mcr-solicitors-web',

    // Admin panel URL
    ADMIN_URL: 'https://emailservice.mcrsolicitors.co.uk/admin.html',

    // Environment
    ENVIRONMENT: 'production'
};

// For development/testing, you can override these values:
// window.EmailServiceConfig.API_BASE = 'http://localhost:5000/api';
// window.EmailServiceConfig.ADMIN_URL = 'http://localhost:5000/admin.html';
// window.EmailServiceConfig.ENVIRONMENT = 'development';