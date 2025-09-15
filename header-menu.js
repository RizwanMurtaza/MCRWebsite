/**
 * Header Menu JavaScript
 * Handles mobile menu toggle and dropdown interactions
 */

// Mobile menu toggle
function toggleMenu() {
    const headA = document.querySelector('.head-a');
    const hamburger = document.querySelector('.hamburger-menu');
    const body = document.body;
    
    if (headA) {
        headA.classList.toggle('show-menu');
        
        // Add animation to hamburger menu
        if (hamburger) {
            hamburger.classList.toggle('active');
        }
        
        // Toggle body class for overlay
        if (headA.classList.contains('show-menu')) {
            body.classList.add('menu-open');
        } else {
            body.classList.remove('menu-open');
        }
    }
}

// Close mobile menu when clicking outside
document.addEventListener('click', function(event) {
    const headA = document.querySelector('.head-a');
    const hamburger = document.querySelector('.hamburger-menu');
    
    if (headA && headA.classList.contains('show-menu')) {
        // Check if click is outside menu and hamburger
        if (!headA.contains(event.target) && !hamburger.contains(event.target)) {
            headA.classList.remove('show-menu');
            if (hamburger) {
                hamburger.classList.remove('active');
            }
        }
    }
});

// Close mobile menu when clicking the X
document.addEventListener('click', function(event) {
    const headA = document.querySelector('.head-a');
    
    if (event.target.matches('.head-a.show-menu::before')) {
        headA.classList.remove('show-menu');
    }
});

// Handle mobile dropdown toggles
document.addEventListener('DOMContentLoaded', function() {
    // Only apply on mobile
    if (window.innerWidth <= 767) {
        const dropdowns = document.querySelectorAll('.dropdown');
        
        dropdowns.forEach(dropdown => {
            const link = dropdown.querySelector('> a');
            
            link.addEventListener('click', function(e) {
                // Prevent default only for mobile dropdowns with submenus
                const hasSubmenu = dropdown.querySelector('.dropdown-content');
                if (hasSubmenu) {
                    e.preventDefault();
                    
                    // Toggle active class
                    dropdown.classList.toggle('active');
                    
                    // Close other dropdowns
                    dropdowns.forEach(otherDropdown => {
                        if (otherDropdown !== dropdown) {
                            otherDropdown.classList.remove('active');
                        }
                    });
                }
            });
        });
        
        // Handle second level dropdowns
        const dropdown2s = document.querySelectorAll('.dropdown2');
        
        dropdown2s.forEach(dropdown2 => {
            const link = dropdown2.querySelector('> a');
            
            link.addEventListener('click', function(e) {
                const hasSubmenu = dropdown2.querySelector('.dropdown2-content');
                if (hasSubmenu) {
                    e.preventDefault();
                    dropdown2.classList.toggle('active');
                }
            });
        });
    }
});

// Handle window resize
let resizeTimer;
window.addEventListener('resize', function() {
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(function() {
        const headA = document.querySelector('.head-a');
        const hamburger = document.querySelector('.hamburger-menu');
        
        // Close mobile menu on resize to desktop
        if (window.innerWidth > 767) {
            if (headA) {
                headA.classList.remove('show-menu');
            }
            if (hamburger) {
                hamburger.classList.remove('active');
            }
            
            // Remove active classes from dropdowns
            document.querySelectorAll('.dropdown').forEach(dropdown => {
                dropdown.classList.remove('active');
            });
        }
    }, 250);
});

// Smooth scroll for anchor links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
            
            // Close mobile menu after clicking anchor link
            const headA = document.querySelector('.head-a');
            if (headA && headA.classList.contains('show-menu')) {
                headA.classList.remove('show-menu');
            }
        }
    });
});