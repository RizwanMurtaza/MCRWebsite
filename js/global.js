// Global JavaScript functions for MCR Solicitors website

// Global toggleMenu function for mobile navigation
function toggleMenu() {
    console.log('toggleMenu called');
    const headA = document.querySelector('.head-a');
    const hamburger = document.querySelector('.hamburger-menu');
    
    console.log('Menu elements found:', {
        headA: !!headA,
        hamburger: !!hamburger,
        headAClasses: headA ? headA.className : 'not found',
        hamburgerClasses: hamburger ? hamburger.className : 'not found'
    });
    
    if (headA) {
        headA.classList.toggle('show-menu');
        if (hamburger) {
            hamburger.classList.toggle('active');
        }
        console.log('Menu toggled. Show-menu class:', headA.classList.contains('show-menu'));
    } else {
        console.error('Menu element not found - header may not be loaded yet');
        // Try again after a short delay in case header is still loading
        setTimeout(function() {
            const retryHeadA = document.querySelector('.head-a');
            const retryHamburger = document.querySelector('.hamburger-menu');
            if (retryHeadA) {
                retryHeadA.classList.toggle('show-menu');
                if (retryHamburger) {
                    retryHamburger.classList.toggle('active');
                }
                console.log('Menu toggled on retry');
            } else {
                console.error('Menu still not found after retry');
            }
        }, 100);
    }
}

// Initialize event listeners after DOM and components are loaded
function initializeMenuEvents() {
    let touchTimeout;
    
    // Mobile menu dropdown handling
    function initializeMobileDropdowns() {
        const dropdowns = document.querySelectorAll('.head-a .dropdown');
        
        dropdowns.forEach(dropdown => {
            const mainLink = dropdown.querySelector('a');
            const dropdownContent = dropdown.querySelector('.dropdown-content');
            
            if (mainLink && dropdownContent) {
                // Add mobile touch hover behavior with passive listeners
                mainLink.addEventListener('touchstart', handleMobileTouchStart, { passive: false });
                dropdown.addEventListener('touchend', handleMobileTouchEnd, { passive: true });
                dropdown.addEventListener('mouseleave', handleMobileMouseLeave, { passive: true });
                
                // Handle secondary dropdowns (dropdown2)
                const dropdown2Items = dropdownContent.querySelectorAll('.dropdown2');
                dropdown2Items.forEach(dropdown2 => {
                    const secondaryLink = dropdown2.querySelector('a');
                    const dropdown2Content = dropdown2.querySelector('.dropdown2-content');
                    
                    if (secondaryLink && dropdown2Content) {
                        secondaryLink.addEventListener('touchstart', handleMobileTouch2Start, { passive: false });
                        dropdown2.addEventListener('touchend', handleMobileTouch2End, { passive: true });
                        dropdown2.addEventListener('mouseleave', handleMobileMouseLeave2, { passive: true });
                    }
                });
            }
        });
    }
    
    // Handle touch start for main dropdowns
    function handleMobileTouchStart(event) {
        const hamburger = document.querySelector('.hamburger-menu');
        if (!hamburger || getComputedStyle(hamburger).display === 'none') {
            return; // Desktop mode, use normal behavior
        }
        
        event.preventDefault(); // Prevent navigation on touch
        const dropdown = event.target.closest('.dropdown');
        
        // Clear any existing hover states
        document.querySelectorAll('.dropdown.mobile-hover, .dropdown2.mobile-hover').forEach(item => {
            if (item !== dropdown) {
                item.classList.remove('mobile-hover');
            }
        });
        
        // Add hover state to simulate desktop hover
        dropdown.classList.add('mobile-hover');
    }
    
    // Handle touch end for main dropdowns  
    function handleMobileTouchEnd(event) {
        const hamburger = document.querySelector('.hamburger-menu');
        if (!hamburger || getComputedStyle(hamburger).display === 'none') {
            return;
        }
        
        // Keep the hover state active, it will be removed by touch outside or manual removal
    }
    
    // Handle mouse leave to remove hover state
    function handleMobileMouseLeave(event) {
        const dropdown = event.currentTarget;
        dropdown.classList.remove('mobile-hover');
    }
    
    // Handle touch start for secondary dropdowns
    function handleMobileTouch2Start(event) {
        const hamburger = document.querySelector('.hamburger-menu');
        if (!hamburger || getComputedStyle(hamburger).display === 'none') {
            return;
        }
        
        event.preventDefault();
        event.stopPropagation(); // Prevent parent dropdown from being affected
        const dropdown2 = event.target.closest('.dropdown2');
        
        // Clear other dropdown2 hover states
        document.querySelectorAll('.dropdown2.mobile-hover').forEach(item => {
            if (item !== dropdown2) {
                item.classList.remove('mobile-hover');
            }
        });
        
        dropdown2.classList.add('mobile-hover');
    }
    
    // Handle touch end for secondary dropdowns
    function handleMobileTouch2End(event) {
        const hamburger = document.querySelector('.hamburger-menu');
        if (!hamburger || getComputedStyle(hamburger).display === 'none') {
            return;
        }
        // Keep hover state active
    }
    
    // Handle mouse leave for secondary dropdowns
    function handleMobileMouseLeave2(event) {
        const dropdown2 = event.currentTarget;
        dropdown2.classList.remove('mobile-hover');
    }
    
    // Close menu when clicking outside or touching outside
    document.addEventListener('touchstart', function(event) {
        const menu = document.querySelector('.head-a');
        const hamburger = document.querySelector('.hamburger-menu');
        
        // Remove hover states when touching outside dropdowns
        if (menu && menu.classList.contains('show-menu')) {
            const touchedDropdown = event.target.closest('.dropdown');
            const touchedDropdown2 = event.target.closest('.dropdown2');
            
            if (!touchedDropdown) {
                // Remove all main dropdown hover states
                document.querySelectorAll('.dropdown.mobile-hover').forEach(dropdown => {
                    dropdown.classList.remove('mobile-hover');
                });
            }
            
            if (!touchedDropdown2) {
                // Remove all secondary dropdown hover states
                document.querySelectorAll('.dropdown2.mobile-hover').forEach(dropdown2 => {
                    dropdown2.classList.remove('mobile-hover');
                });
            }
        }
        
        // Close entire menu if touched outside
        if (menu && menu.classList.contains('show-menu')) {
            if (!menu.contains(event.target) && !hamburger.contains(event.target)) {
                menu.classList.remove('show-menu');
                if (hamburger) {
                    hamburger.classList.remove('active');
                }
                // Remove all hover states
                document.querySelectorAll('.dropdown.mobile-hover, .dropdown2.mobile-hover').forEach(item => {
                    item.classList.remove('mobile-hover');
                });
            }
        }
    });
    
    // Also handle click events for non-touch devices
    document.addEventListener('click', function(event) {
        const menu = document.querySelector('.head-a');
        const hamburger = document.querySelector('.hamburger-menu');
        
        if (menu && menu.classList.contains('show-menu')) {
            if (!menu.contains(event.target) && !hamburger.contains(event.target)) {
                menu.classList.remove('show-menu');
                if (hamburger) {
                    hamburger.classList.remove('active');
                }
                document.querySelectorAll('.dropdown.mobile-hover, .dropdown2.mobile-hover').forEach(item => {
                    item.classList.remove('mobile-hover');
                });
            }
        }
    });
    
    // Initialize mobile dropdowns when header loads
    initializeMobileDropdowns();
    
    // Re-initialize when header is reloaded
    const headerContainer = document.getElementById('header-container');
    if (headerContainer) {
        const observer = new MutationObserver(function(mutations) {
            mutations.forEach(function(mutation) {
                if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                    const headerAdded = Array.from(mutation.addedNodes).some(node => 
                        node.nodeType === 1 && (node.querySelector('.head-a') || node.classList?.contains('head-a'))
                    );
                    if (headerAdded) {
                        setTimeout(initializeMobileDropdowns, 100);
                    }
                }
            });
        });
        observer.observe(headerContainer, { childList: true, subtree: true });
    }
}

// Wait for DOM to be ready, then initialize
document.addEventListener('DOMContentLoaded', function() {
    initializeMenuEvents();
    
    // Use MutationObserver to detect when header is loaded
    const observer = new MutationObserver(function(mutations) {
        mutations.forEach(function(mutation) {
            if (mutation.type === 'childList') {
                const hamburger = document.querySelector('.hamburger-menu');
                if (hamburger && !hamburger.hasAttribute('data-listener-added')) {
                    // Remove any existing onclick attribute to prevent conflicts
                    hamburger.removeAttribute('onclick');
                    hamburger.addEventListener('click', toggleMenu);
                    hamburger.setAttribute('data-listener-added', 'true');
                    console.log('Mobile menu initialized');
                    observer.disconnect(); // Stop observing once initialized
                }
            }
        });
    });
    
    // Start observing the header container for changes
    const headerContainer = document.getElementById('header-container');
    if (headerContainer) {
        observer.observe(headerContainer, { childList: true, subtree: true });
    }
    
    // Fallback timeout in case MutationObserver doesn't work
    setTimeout(function() {
        const hamburger = document.querySelector('.hamburger-menu');
        if (hamburger && !hamburger.hasAttribute('data-listener-added')) {
            hamburger.removeAttribute('onclick');
            hamburger.addEventListener('click', toggleMenu);
            hamburger.setAttribute('data-listener-added', 'true');
            console.log('Mobile menu initialized (fallback)');
        }
    }, 1000);
});

// Ensure the function is available globally
window.toggleMenu = toggleMenu;