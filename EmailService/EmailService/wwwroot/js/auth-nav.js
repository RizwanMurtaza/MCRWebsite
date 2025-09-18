// Authentication and Navigation System
class AuthNavSystem {
    constructor() {
        this.init();
    }

    init() {
        this.checkAuth();
        this.createNavigation();
        this.setupEventListeners();
    }

    checkAuth() {
        const authData = this.getAuthData();
        const currentPath = window.location.pathname;

        // Allow login page without auth
        if (currentPath.includes('login.html')) {
            if (authData && this.isValidAuth(authData)) {
                // Already logged in, redirect to admin
                window.location.href = '/admin.html';
            }
            return;
        }

        // Protect admin pages
        if (currentPath.includes('admin.html') || currentPath.includes('emails.html') || currentPath === '/') {
            if (!authData || !this.isValidAuth(authData)) {
                window.location.href = '/login.html';
                return;
            }

            // Update last activity
            this.updateLastActivity();
        }
    }

    getAuthData() {
        try {
            const data = localStorage.getItem('emailServiceAuth');
            return data ? JSON.parse(data) : null;
        } catch (e) {
            return null;
        }
    }

    isValidAuth(authData) {
        if (!authData || !authData.token || !authData.loginTime) {
            return false;
        }

        // Check if session is older than 24 hours
        const now = new Date().getTime();
        const sessionAge = now - authData.loginTime;
        const maxAge = 24 * 60 * 60 * 1000; // 24 hours

        if (sessionAge > maxAge) {
            this.logout();
            return false;
        }

        return true;
    }

    updateLastActivity() {
        const authData = this.getAuthData();
        if (authData) {
            authData.lastActivity = new Date().getTime();
            localStorage.setItem('emailServiceAuth', JSON.stringify(authData));
        }
    }

    createNavigation() {
        const currentPath = window.location.pathname;

        // Don't show navigation on login page
        if (currentPath.includes('login.html')) {
            return;
        }

        const authData = this.getAuthData();
        if (!authData || !this.isValidAuth(authData)) {
            return;
        }

        const navHTML = `
            <nav class="top-nav">
                <div class="nav-container">
                    <div class="nav-brand">
                        <span class="nav-icon">📧</span>
                        <span class="nav-title">Email Service Dashboard</span>
                    </div>
                    <div class="nav-menu">
                        <a href="/admin.html" class="nav-link ${currentPath.includes('admin.html') ? 'active' : ''}">
                            <span class="nav-link-icon">⚙️</span>
                            <span>Admin Panel</span>
                        </a>
                        <a href="/emails.html" class="nav-link ${currentPath.includes('emails.html') ? 'active' : ''}">
                            <span class="nav-link-icon">📊</span>
                            <span>Email Monitor</span>
                        </a>
                    </div>
                    <div class="nav-user">
                        <div class="user-info">
                            <span class="user-icon">👤</span>
                            <span class="username">${authData.username}</span>
                        </div>
                        <button class="logout-btn" onclick="authNav.logout()">
                            <span>🚪</span>
                            <span>Logout</span>
                        </button>
                    </div>
                </div>
            </nav>
        `;

        // Insert navigation at the beginning of body
        document.body.insertAdjacentHTML('afterbegin', navHTML);

        // Add navigation styles
        this.addNavigationStyles();

        // Adjust main content margin
        this.adjustContentMargin();
    }

    addNavigationStyles() {
        const styles = `
            <style id="nav-styles">
                .top-nav {
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    padding: 0;
                    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                    position: fixed;
                    top: 0;
                    left: 0;
                    right: 0;
                    z-index: 1000;
                    height: 60px;
                }

                .nav-container {
                    display: flex;
                    align-items: center;
                    justify-content: space-between;
                    max-width: 1400px;
                    margin: 0 auto;
                    height: 100%;
                    padding: 0 20px;
                }

                .nav-brand {
                    display: flex;
                    align-items: center;
                    gap: 10px;
                    font-weight: bold;
                    font-size: 1.2rem;
                }

                .nav-icon {
                    font-size: 1.5rem;
                }

                .nav-menu {
                    display: flex;
                    gap: 5px;
                    align-items: center;
                }

                .nav-link {
                    display: flex;
                    align-items: center;
                    gap: 8px;
                    color: white;
                    text-decoration: none;
                    padding: 8px 16px;
                    border-radius: 5px;
                    transition: all 0.3s;
                    font-weight: 500;
                }

                .nav-link:hover {
                    background: rgba(255, 255, 255, 0.1);
                    transform: translateY(-1px);
                }

                .nav-link.active {
                    background: rgba(255, 255, 255, 0.2);
                    font-weight: 600;
                }

                .nav-link-icon {
                    font-size: 1.1rem;
                }

                .nav-user {
                    display: flex;
                    align-items: center;
                    gap: 15px;
                }

                .user-info {
                    display: flex;
                    align-items: center;
                    gap: 8px;
                    background: rgba(255, 255, 255, 0.1);
                    padding: 6px 12px;
                    border-radius: 20px;
                    font-size: 0.9rem;
                }

                .user-icon {
                    font-size: 1.1rem;
                }

                .username {
                    font-weight: 600;
                }

                .logout-btn {
                    display: flex;
                    align-items: center;
                    gap: 6px;
                    background: rgba(255, 255, 255, 0.1);
                    border: 1px solid rgba(255, 255, 255, 0.2);
                    color: white;
                    padding: 6px 12px;
                    border-radius: 5px;
                    cursor: pointer;
                    transition: all 0.3s;
                    font-size: 0.9rem;
                    font-weight: 500;
                }

                .logout-btn:hover {
                    background: rgba(255, 255, 255, 0.2);
                    transform: translateY(-1px);
                }

                /* Responsive design */
                @media (max-width: 768px) {
                    .nav-container {
                        padding: 0 15px;
                    }

                    .nav-brand .nav-title {
                        display: none;
                    }

                    .nav-link span:not(.nav-link-icon) {
                        display: none;
                    }

                    .nav-user .user-info .username {
                        display: none;
                    }

                    .logout-btn span:last-child {
                        display: none;
                    }
                }

                /* Adjust body padding for fixed nav */
                body {
                    padding-top: 60px;
                }

                /* Adjust container margins */
                .container, .content-section {
                    margin-top: 20px !important;
                }
            </style>
        `;

        document.head.insertAdjacentHTML('beforeend', styles);
    }

    adjustContentMargin() {
        // Adjust existing container margins
        const containers = document.querySelectorAll('.container, .content-section');
        containers.forEach(container => {
            const currentMargin = parseInt(getComputedStyle(container).marginTop) || 0;
            container.style.marginTop = Math.max(currentMargin, 20) + 'px';
        });
    }

    setupEventListeners() {
        // Auto-logout on inactivity (30 minutes)
        let inactivityTimer;
        const resetInactivityTimer = () => {
            clearTimeout(inactivityTimer);
            inactivityTimer = setTimeout(() => {
                if (this.getAuthData()) {
                    alert('Session expired due to inactivity. Please login again.');
                    this.logout();
                }
            }, 30 * 60 * 1000); // 30 minutes
        };

        // Track user activity
        ['mousedown', 'mousemove', 'keypress', 'scroll', 'touchstart'].forEach(event => {
            document.addEventListener(event, resetInactivityTimer, true);
        });

        // Initial timer setup
        if (this.getAuthData()) {
            resetInactivityTimer();
        }
    }

    logout() {
        localStorage.removeItem('emailServiceAuth');

        // Show logout message
        const confirmed = confirm('Are you sure you want to logout?');
        if (confirmed) {
            alert('You have been logged out successfully.');
            window.location.href = '/login.html';
        }
    }
}

// Initialize the authentication and navigation system
const authNav = new AuthNavSystem();