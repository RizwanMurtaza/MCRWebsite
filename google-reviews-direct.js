/**
 * Direct Google Reviews Fetcher
 * Uses Google Places API directly from browser
 */

// Configuration with environment detection
const isDevelopment = window.location.hostname === 'localhost' || 
                     window.location.hostname === '127.0.0.1' ||
                     window.location.protocol === 'file:';

// Use the same key for now, but this structure allows for easy dev/prod separation
const GOOGLE_API_KEY = 'AIzaSyCy3vGEKVqb5vDjl67ZZTAlu0KyXDKcUsw';
const PLACE_ID = 'ChIJR4p01KKze0gRMqKmDoEDnws';

// Cache configuration - 6 hours in milliseconds
const CACHE_DURATION = 6 * 60 * 60 * 1000; // 6 hours
const CACHE_KEY = 'mcr_google_reviews_cache';
const CACHE_TIMESTAMP_KEY = 'mcr_google_reviews_timestamp';

// Log environment for debugging
console.log('Google Reviews API - Environment:', isDevelopment ? 'Development' : 'Production');
console.log('Google Reviews API - Host:', window.location.hostname);

// Cache helper functions
function getCachedReviews() {
    try {
        const cached = localStorage.getItem(CACHE_KEY);
        const timestamp = localStorage.getItem(CACHE_TIMESTAMP_KEY);
        
        if (!cached || !timestamp) {
            return null;
        }
        
        const age = Date.now() - parseInt(timestamp);
        
        // Check if cache is still valid (less than 6 hours old)
        if (age > CACHE_DURATION) {
            console.log('📅 Cache expired (older than 6 hours), fetching fresh data...');
            localStorage.removeItem(CACHE_KEY);
            localStorage.removeItem(CACHE_TIMESTAMP_KEY);
            return null;
        }
        
        const data = JSON.parse(cached);
        const ageMinutes = Math.floor(age / (1000 * 60));
        const ageHours = Math.floor(ageMinutes / 60);
        
        if (ageHours > 0) {
            console.log(`✅ Using cached reviews (${ageHours} hours ${ageMinutes % 60} minutes old)`);
        } else {
            console.log(`✅ Using cached reviews (${ageMinutes} minutes old)`);
        }
        
        return data;
    } catch (error) {
        console.error('Error reading cache:', error);
        return null;
    }
}

function setCachedReviews(place) {
    try {
        // Store only the data we need
        const dataToCache = {
            name: place.name,
            rating: place.rating,
            user_ratings_total: place.user_ratings_total,
            reviews: place.reviews
        };
        
        localStorage.setItem(CACHE_KEY, JSON.stringify(dataToCache));
        localStorage.setItem(CACHE_TIMESTAMP_KEY, Date.now().toString());
        console.log('💾 Reviews cached for 6 hours');
    } catch (error) {
        console.error('Error setting cache:', error);
    }
}

function clearReviewsCache() {
    localStorage.removeItem(CACHE_KEY);
    localStorage.removeItem(CACHE_TIMESTAMP_KEY);
    console.log('🗑️ Reviews cache cleared');
}

// Function to load Google Maps JavaScript API
function loadGoogleMapsAPI() {
    return new Promise((resolve, reject) => {
        if (window.google && window.google.maps) {
            resolve();
            return;
        }

        const script = document.createElement('script');
        script.src = `https://maps.googleapis.com/maps/api/js?key=${GOOGLE_API_KEY}&libraries=places`;
        script.async = true;
        script.defer = true;
        script.onload = resolve;
        script.onerror = reject;
        document.head.appendChild(script);
    });
}

// Function to fetch place details
async function fetchGoogleReviews(forceRefresh = false) {
    try {
        // Check cache first unless force refresh is requested
        if (!forceRefresh) {
            const cachedData = getCachedReviews();
            if (cachedData) {
                displayGoogleReviews(cachedData);
                return;
            }
        } else {
            console.log('🔄 Force refresh requested, bypassing cache...');
        }
        
        await loadGoogleMapsAPI();
        
        const service = new google.maps.places.PlacesService(document.createElement('div'));
        
        const request = {
            placeId: PLACE_ID,
            fields: ['name', 'rating', 'user_ratings_total', 'reviews']
        };

        service.getDetails(request, (place, status) => {
            if (status === google.maps.places.PlacesServiceStatus.OK) {
                console.log('✅ Successfully fetched Google Reviews from API!');
                console.log('Business:', place.name);
                console.log('Rating:', place.rating);
                console.log('Total Reviews:', place.user_ratings_total);
                console.log('Reviews:', place.reviews);
                
                // Cache the reviews for 6 hours
                setCachedReviews(place);
                
                // Update the display
                displayGoogleReviews(place);
            } else {
                console.error('❌ Failed to fetch reviews:', status);
                
                // Provide helpful error messages for API restrictions
                if (status === 'REQUEST_DENIED') {
                    console.error('🔐 API Key Issue: The request was denied. This usually means:');
                    console.error('   - API key has HTTP referrer restrictions that don\'t match this domain');
                    console.error('   - Current domain:', window.location.origin);
                    console.error('   - Add this domain to allowed referrers in Google Cloud Console');
                } else if (status === 'OVER_QUERY_LIMIT') {
                    console.error('📊 Quota exceeded: Too many requests. Wait and try again later.');
                } else if (status === 'INVALID_REQUEST') {
                    console.error('❓ Invalid request: Check that the Place ID is correct.');
                }
                
                showFallbackTestimonials();
            }
        });
    } catch (error) {
        console.error('Error loading Google Maps API:', error);
        
        // Check for common API loading errors
        if (error.message && error.message.includes('RefererNotAllowedMapError')) {
            console.error('🔐 HTTP Referrer Restriction Error:');
            console.error('   The API key is restricted and this domain is not allowed.');
            console.error('   Current domain:', window.location.origin);
            console.error('   Solution: Add this domain to the allowed referrers in Google Cloud Console');
        } else if (error.message && error.message.includes('InvalidKeyMapError')) {
            console.error('🔑 Invalid API Key Error:');
            console.error('   The API key is invalid or has been deleted.');
        } else if (error.message && error.message.includes('ApiNotActivatedMapError')) {
            console.error('⚠️ API Not Activated:');
            console.error('   The required APIs are not enabled for this project.');
            console.error('   Enable: Maps JavaScript API and Places API');
        }
        
        showFallbackTestimonials();
    }
}

// Function to display the reviews
function displayGoogleReviews(place) {
    // Update the rating badge
    const ratingElement = document.querySelector('.google-rating-number');
    if (ratingElement) {
        ratingElement.textContent = place.rating.toFixed(1);
    }
    
    const countElement = document.querySelector('.google-review-count');
    if (countElement) {
        countElement.textContent = `${place.user_ratings_total} reviews`;
    }
    
    // Update testimonials with real reviews
    if (place.reviews && place.reviews.length > 0) {
        updateTestimonialSlides(place.reviews);
    }
}

// Function to update testimonial slides with real reviews
function updateTestimonialSlides(reviews) {
    console.log('Updating testimonials with', reviews.length, 'reviews');
    
    const wrapper = document.querySelector('.testimonial-slide-wrapper');
    if (!wrapper) {
        console.error('Testimonial wrapper not found!');
        return;
    }
    
    // Clear existing slides
    wrapper.innerHTML = '';
    
    // Sort reviews by time (most recent first)
    const sortedReviews = [...reviews].sort((a, b) => {
        // Google provides 'time' as Unix timestamp
        return (b.time || 0) - (a.time || 0);
    });
    
    console.log('Reviews sorted by date (most recent first)');
    
    // Add each review as a slide (limit to 6 for consistency with original)
    const reviewsToShow = sortedReviews.slice(0, 6);
    
    reviewsToShow.forEach((review, index) => {
        const slide = document.createElement('div');
        slide.className = index === 0 ? 'slide active' : 'slide';
        
        // Get initials from name
        const nameParts = review.author_name.split(' ');
        const initials = nameParts.length >= 2 
            ? nameParts[0][0] + nameParts[nameParts.length - 1][0]
            : review.author_name.substring(0, 2).toUpperCase();
        
        // Create star rating
        let stars = '';
        for (let i = 1; i <= 5; i++) {
            stars += i <= review.rating ? '★' : '☆';
        }
        
        // Truncate very long reviews
        let reviewText = review.text;
        if (reviewText.length > 300) {
            reviewText = reviewText.substring(0, 297) + '...';
        }
        
        // Format the date for display
        const reviewDate = review.time ? new Date(review.time * 1000).toLocaleDateString('en-GB', {
            day: 'numeric',
            month: 'short',
            year: 'numeric'
        }) : '';
        
        // Log for debugging
        if (index === 0) {
            console.log(`Most recent review: ${review.author_name} - ${review.relative_time_description}`);
        }
        
        slide.innerHTML = `
            <div class="testimonial-content">
                <div class="quote-icon">
                    <img src="Images/atros.png" alt="Quote">
                </div>
                <p class="testimonial-text">
                    "${reviewText}"
                </p>
                <div class="testimonial-author">
                    <div class="author-avatar">${initials}</div>
                    <div class="author-info">
                        <h3 class="author-name">${review.author_name}</h3>
                        <div class="rating">
                            <span class="stars">${stars}</span>
                            <span class="rating-value">${review.rating}.0</span>
                            <span style="font-size: 11px; color: #5f6368; margin-left: 8px;">
                                ${review.relative_time_description} via Google
                            </span>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        wrapper.appendChild(slide);
    });
    
    // Update dots to match
    updateDots(reviewsToShow.length);
    
    // Re-initialize the slider with new content
    reinitializeSlider();
    
    console.log('✅ Testimonials updated with real Google Reviews!');
}

// Function to reinitialize the slider after updating content
function reinitializeSlider() {
    // Get the new slides and dots
    const slides = document.querySelectorAll('.testimonial-slide-wrapper .slide');
    const dots = document.querySelectorAll('.slider-dots .dot');
    const prevButton = document.getElementById('prevBtnt');
    const nextButton = document.getElementById('nextBtnt');
    
    if (slides.length === 0) return;
    
    let currentSlide = 0;
    let autoSlideInterval;
    
    // Function to show a specific slide
    function showSlide(index) {
        if (index >= slides.length) {
            currentSlide = 0;
        } else if (index < 0) {
            currentSlide = slides.length - 1;
        } else {
            currentSlide = index;
        }
        
        // Hide all slides
        slides.forEach(slide => {
            slide.classList.remove('active');
            slide.style.display = 'none';
        });
        
        // Show current slide
        slides[currentSlide].classList.add('active');
        slides[currentSlide].style.display = 'block';
        
        // Update dots
        dots.forEach(dot => dot.classList.remove('active'));
        if (dots[currentSlide]) {
            dots[currentSlide].classList.add('active');
        }
    }
    
    // Clear any existing event listeners by cloning buttons
    const newPrevButton = prevButton.cloneNode(true);
    const newNextButton = nextButton.cloneNode(true);
    prevButton.parentNode.replaceChild(newPrevButton, prevButton);
    nextButton.parentNode.replaceChild(newNextButton, nextButton);
    
    // Add new event listeners
    newNextButton.addEventListener('click', () => {
        showSlide(currentSlide + 1);
        resetAutoSlide();
    });
    
    newPrevButton.addEventListener('click', () => {
        showSlide(currentSlide - 1);
        resetAutoSlide();
    });
    
    // Set up dot click handlers
    dots.forEach((dot, index) => {
        dot.addEventListener('click', () => {
            showSlide(index);
            resetAutoSlide();
        });
    });
    
    // Auto-slide function
    function startAutoSlide() {
        autoSlideInterval = setInterval(() => {
            showSlide(currentSlide + 1);
        }, 5000);
    }
    
    // Reset auto slide when user interacts
    function resetAutoSlide() {
        clearInterval(autoSlideInterval);
        startAutoSlide();
    }
    
    // Initialize
    showSlide(0);
    startAutoSlide();
    
    console.log('Slider reinitialized with', slides.length, 'slides');
}

// Function to update slider dots
function updateDots(count) {
    const dotsContainer = document.querySelector('.slider-dots');
    if (!dotsContainer) return;
    
    dotsContainer.innerHTML = '';
    for (let i = 0; i < count; i++) {
        const dot = document.createElement('span');
        dot.className = i === 0 ? 'dot active' : 'dot';
        dotsContainer.appendChild(dot);
    }
}

// Function to show fallback testimonials if API fails
function showFallbackTestimonials() {
    console.log('Using existing testimonials as fallback');
    // The existing testimonials will remain
}

// Initialize when page loads
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        // Wait a bit for all sliders to initialize
        setTimeout(fetchGoogleReviews, 1000);
    });
} else {
    // If DOM is already loaded, still wait a bit for sliders
    setTimeout(fetchGoogleReviews, 1000);
}

// Expose cache management functions globally for debugging
window.MCRGoogleReviews = {
    forceRefresh: () => {
        clearReviewsCache();
        fetchGoogleReviews(true);
        console.log('🔄 Forcing refresh of Google Reviews...');
    },
    clearCache: clearReviewsCache,
    checkCacheStatus: () => {
        const timestamp = localStorage.getItem(CACHE_TIMESTAMP_KEY);
        if (!timestamp) {
            console.log('❌ No cached reviews found');
            return;
        }
        
        const age = Date.now() - parseInt(timestamp);
        const ageMinutes = Math.floor(age / (1000 * 60));
        const ageHours = Math.floor(ageMinutes / 60);
        const remainingHours = Math.floor((CACHE_DURATION - age) / (1000 * 60 * 60));
        const remainingMinutes = Math.floor(((CACHE_DURATION - age) % (1000 * 60 * 60)) / (1000 * 60));
        
        console.log('📊 Cache Status:');
        console.log(`   Age: ${ageHours} hours ${ageMinutes % 60} minutes`);
        console.log(`   Expires in: ${remainingHours} hours ${remainingMinutes} minutes`);
        console.log(`   Cache duration: 6 hours`);
    }
};

console.log('💡 Tip: Use window.MCRGoogleReviews.checkCacheStatus() to check cache status');
console.log('💡 Tip: Use window.MCRGoogleReviews.forceRefresh() to force refresh reviews');