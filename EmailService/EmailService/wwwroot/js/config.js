// Configuration for Email Service Application
const CONFIG = {
    // Development settings
    development: {
        apiBase: 'http://localhost:5000/api',
        name: 'Development'
    },

    // Production settings
    production: {
        apiBase: 'https://emailservice.mcrsolicitors.co.uk/api',
        name: 'Production'
    },

    // Current environment - Change this to switch between environments
    // Options: 'development' or 'production'
    currentEnvironment: 'production',

    // Get current API base URL
    getApiBase() {
        return this[this.currentEnvironment].apiBase;
    },

    // Get current environment name
    getEnvironmentName() {
        return this[this.currentEnvironment].name;
    },

    // Check if in development mode
    isDevelopment() {
        return this.currentEnvironment === 'development';
    },

    // Check if in production mode
    isProduction() {
        return this.currentEnvironment === 'production';
    }
};

// Auto-detect environment based on hostname (optional)
if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
    console.log('Auto-detected localhost - switching to development mode');
    CONFIG.currentEnvironment = 'development';
}

// Log current configuration
console.log(`Email Service Config: ${CONFIG.getEnvironmentName()} - ${CONFIG.getApiBase()}`);