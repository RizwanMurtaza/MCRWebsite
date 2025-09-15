/**
 * Google Reviews Display System
 * For MCR Solicitors Website
 */

// Function to create star rating HTML
function createStarRating(rating) {
    let stars = '';
    for (let i = 1; i <= 5; i++) {
        if (i <= rating) {
            stars += '<span style="color: #fbbc04;">★</span>';
        } else {
            stars += '<span style="color: #e0e0e0;">☆</span>';
        }
    }
    return stars;
}

// Function to format review date
function formatReviewDate(timestamp) {
    const date = new Date(timestamp * 1000);
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    return `${months[date.getMonth()]} ${date.getFullYear()}`;
}

// Function to get initials from name
function getInitials(name) {
    const parts = name.split(' ');
    if (parts.length >= 2) {
        return parts[0][0] + parts[parts.length - 1][0];
    }
    return name[0];
}

// Function to truncate text
function truncateText(text, maxLength = 200) {
    if (text.length <= maxLength) return text;
    return text.substr(0, maxLength) + '...';
}

// Function to load Google Reviews
async function loadGoogleReviews() {
    try {
        // Show loading state
        const container = document.getElementById('google-reviews-container');
        if (!container) {
            console.log('Reviews container not found. Creating testimonials from API...');
            return;
        }
        
        container.innerHTML = '<p style="text-align: center;">Loading reviews from Google...</p>';
        
        // Fetch reviews from PHP script
        const response = await fetch('get-google-reviews.php');
        const data = await response.json();
        
        if (data.error) {
            console.error('Error fetching reviews:', data.message);
            
            // Show instructions if API key is missing
            if (data.instructions) {
                container.innerHTML = `
                    <div style="background: #fff3cd; border: 1px solid #ffc107; padding: 20px; border-radius: 8px; margin: 20px 0;">
                        <h3 style="color: #856404; margin-top: 0;">Setup Required</h3>
                        <p style="color: #856404;">To display live Google Reviews, please:</p>
                        <ol style="color: #856404;">
                            ${data.instructions.map(instruction => `<li>${instruction}</li>`).join('')}
                        </ol>
                        <p style="color: #856404; margin-bottom: 0;">
                            For now, displaying sample testimonials. Contact your developer to complete the setup.
                        </p>
                    </div>
                `;
            }
            return;
        }
        
        // Update the main rating display
        updateMainRating(data.rating, data.total_reviews);
        
        // Display the reviews
        if (data.reviews && data.reviews.length > 0) {
            displayReviews(data.reviews);
        }
        
    } catch (error) {
        console.error('Failed to load Google Reviews:', error);
        document.getElementById('google-reviews-container').innerHTML = 
            '<p style="text-align: center; color: #666;">Unable to load reviews at this time.</p>';
    }
}

// Function to update main rating display
function updateMainRating(rating, totalReviews) {
    // Update the badge rating
    const ratingDisplay = document.querySelector('.google-rating-number');
    if (ratingDisplay) {
        ratingDisplay.textContent = rating.toFixed(1);
    }
    
    const reviewCount = document.querySelector('.google-review-count');
    if (reviewCount) {
        reviewCount.textContent = `${totalReviews} reviews`;
    }
}

// Function to display reviews in testimonials section
function displayReviews(reviews) {
    const wrapper = document.querySelector('.testimonial-slide-wrapper');
    if (!wrapper) return;
    
    // Clear existing testimonials
    wrapper.innerHTML = '';
    
    // Add each review as a slide
    reviews.forEach((review, index) => {
        const slide = document.createElement('div');
        slide.className = index === 0 ? 'slide active' : 'slide';
        
        slide.innerHTML = `
            <div class="testimonial-content">
                <div class="quote-icon">
                    <img src="Images/atros.png" alt="Quote">
                </div>
                <p class="testimonial-text">
                    "${truncateText(review.text, 250)}"
                </p>
                <div class="testimonial-author">
                    <div class="author-avatar">${getInitials(review.author_name)}</div>
                    <div class="author-info">
                        <h3 class="author-name">${review.author_name}</h3>
                        <div class="rating">
                            ${createStarRating(review.rating)}
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
    
    // Update dots to match number of reviews
    updateSliderDots(reviews.length);
}

// Function to update slider dots
function updateSliderDots(count) {
    const dotsContainer = document.querySelector('.slider-dots');
    if (!dotsContainer) return;
    
    dotsContainer.innerHTML = '';
    for (let i = 0; i < count; i++) {
        const dot = document.createElement('span');
        dot.className = i === 0 ? 'dot active' : 'dot';
        dotsContainer.appendChild(dot);
    }
}

// Alternative: Load reviews directly into existing structure
function updateExistingTestimonials(reviews) {
    // This function updates the existing testimonial slides with real Google Reviews
    const slides = document.querySelectorAll('.testimonial-slide-wrapper .slide');
    
    reviews.forEach((review, index) => {
        if (index < slides.length) {
            const slide = slides[index];
            
            // Update text
            const textElement = slide.querySelector('.testimonial-text');
            if (textElement) {
                textElement.textContent = `"${truncateText(review.text, 250)}"`;
            }
            
            // Update author name
            const nameElement = slide.querySelector('.author-name');
            if (nameElement) {
                nameElement.textContent = review.author_name;
            }
            
            // Update avatar
            const avatarElement = slide.querySelector('.author-avatar');
            if (avatarElement) {
                avatarElement.textContent = getInitials(review.author_name);
            }
            
            // Update rating
            const ratingElement = slide.querySelector('.rating');
            if (ratingElement) {
                ratingElement.innerHTML = `
                    ${createStarRating(review.rating)}
                    <span class="rating-value">${review.rating}.0</span>
                    <span style="font-size: 11px; color: #5f6368; margin-left: 8px;">
                        ${review.relative_time_description} via Google
                    </span>
                `;
            }
        }
    });
}

// Load reviews when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', loadGoogleReviews);
} else {
    loadGoogleReviews();
}