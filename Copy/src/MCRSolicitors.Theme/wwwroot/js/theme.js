/**
 * MCR Solicitors Theme JavaScript
 * Handles navigation, testimonial carousel, animations
 */

(function() {
    'use strict';

    // Mobile Navigation Toggle
    function initMobileNav() {
        const toggler = document.querySelector('.navbar-toggler');
        const menu = document.querySelector('.navbar-menu');
        const dropdowns = document.querySelectorAll('.nav-item.has-dropdown');

        if (toggler && menu) {
            toggler.addEventListener('click', function() {
                menu.classList.toggle('active');
                const isExpanded = menu.classList.contains('active');
                toggler.setAttribute('aria-expanded', isExpanded);
            });
        }

        // Mobile dropdown toggle
        dropdowns.forEach(function(dropdown) {
            const link = dropdown.querySelector('.nav-link');
            if (link && window.innerWidth <= 768) {
                link.addEventListener('click', function(e) {
                    if (window.innerWidth <= 768) {
                        e.preventDefault();
                        dropdown.classList.toggle('open');
                    }
                });
            }
        });

        // Close menu on window resize
        window.addEventListener('resize', function() {
            if (window.innerWidth > 768 && menu) {
                menu.classList.remove('active');
            }
        });
    }

    // Testimonial Carousel
    function initTestimonialCarousel() {
        const containers = document.querySelectorAll('.testimonials-container');

        containers.forEach(function(container) {
            const wrapper = container.closest('.testimonials-wrapper');
            if (!wrapper) return;

            const cards = container.querySelectorAll('.testimonial-card');
            const prevBtn = wrapper.querySelector('.nav-btn.prev');
            const nextBtn = wrapper.querySelector('.nav-btn.next');
            const dots = wrapper.querySelectorAll('.dot');

            if (cards.length <= 1) return;

            let currentIndex = 0;

            function showCard(index) {
                if (index < 0) index = cards.length - 1;
                if (index >= cards.length) index = 0;

                currentIndex = index;
                const offset = -index * 100;
                container.style.transform = 'translateX(' + offset + '%)';

                dots.forEach(function(dot, i) {
                    dot.classList.toggle('active', i === index);
                });
            }

            if (prevBtn) {
                prevBtn.addEventListener('click', function() {
                    showCard(currentIndex - 1);
                });
            }

            if (nextBtn) {
                nextBtn.addEventListener('click', function() {
                    showCard(currentIndex + 1);
                });
            }

            dots.forEach(function(dot, i) {
                dot.addEventListener('click', function() {
                    showCard(i);
                });
            });

            // Auto-play
            let autoPlay = setInterval(function() {
                showCard(currentIndex + 1);
            }, 5000);

            container.addEventListener('mouseenter', function() {
                clearInterval(autoPlay);
            });

            container.addEventListener('mouseleave', function() {
                autoPlay = setInterval(function() {
                    showCard(currentIndex + 1);
                }, 5000);
            });

            // Style for carousel
            container.style.display = 'flex';
            container.style.transition = 'transform 0.5s ease';
            cards.forEach(function(card) {
                card.style.flex = '0 0 100%';
            });
        });
    }

    // Animate stats on scroll
    function initStatsAnimation() {
        const stats = document.querySelectorAll('.stat-number');

        if (stats.length === 0) return;

        const observer = new IntersectionObserver(function(entries) {
            entries.forEach(function(entry) {
                if (entry.isIntersecting) {
                    const stat = entry.target;
                    const target = parseInt(stat.getAttribute('data-count'), 10);
                    if (isNaN(target)) return;

                    animateValue(stat, 0, target, 2000);
                    observer.unobserve(stat);
                }
            });
        }, { threshold: 0.5 });

        stats.forEach(function(stat) {
            observer.observe(stat);
        });
    }

    function animateValue(element, start, end, duration) {
        let startTimestamp = null;

        function step(timestamp) {
            if (!startTimestamp) startTimestamp = timestamp;
            const progress = Math.min((timestamp - startTimestamp) / duration, 1);
            const value = Math.floor(progress * (end - start) + start);
            element.textContent = value;

            if (progress < 1) {
                window.requestAnimationFrame(step);
            } else {
                element.textContent = end;
            }
        }

        window.requestAnimationFrame(step);
    }

    // Smooth scroll for anchor links
    function initSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(function(anchor) {
            anchor.addEventListener('click', function(e) {
                const href = this.getAttribute('href');
                if (href === '#') return;

                const target = document.querySelector(href);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }

    // Header scroll effect
    function initHeaderScroll() {
        const header = document.querySelector('.mcr-header');
        if (!header) return;

        let lastScroll = 0;

        window.addEventListener('scroll', function() {
            const currentScroll = window.pageYOffset;

            if (currentScroll > 100) {
                header.classList.add('scrolled');
            } else {
                header.classList.remove('scrolled');
            }

            lastScroll = currentScroll;
        });
    }

    // Initialize all components
    function init() {
        initMobileNav();
        initTestimonialCarousel();
        initStatsAnimation();
        initSmoothScroll();
        initHeaderScroll();
    }

    // Run on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
