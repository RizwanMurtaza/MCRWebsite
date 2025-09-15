/**
 * MCR Solicitors Email Service Integration
 * Handles form submissions for contact us and enquiry forms
 */

const EmailService = {
    API_BASE: window.EmailServiceConfig?.API_BASE || 'https://emailservice.pricesnap.co.uk/api',
    APP_ID: window.EmailServiceConfig?.APP_ID || 'mcr-solicitors-web',

    /**
     * Submit contact us form
     */
    async submitContactForm(formData) {
        const data = {
            appId: this.APP_ID,
            name: formData.name,
            email: formData.email,
            phone: formData.phone || '',
            subject: formData.department ? `${formData.department} - ${formData.comment?.substring(0, 50)}...` : 'General Enquiry',
            message: this.generateContactMessage(formData)
        };

        try {
            const response = await fetch(`${this.API_BASE}/email/contact-us`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();
            return { success: response.ok, data: result };
        } catch (error) {
            console.error('Contact form submission error:', error);
            return { success: false, error: 'Network error occurred' };
        }
    },

    /**
     * Submit enquiry/appointment form
     */
    async submitEnquiryForm(formData) {
        const data = {
            appId: this.APP_ID,
            name: formData.name,
            email: formData.email,
            phone: formData.phone || '',
            serviceType: formData.department || 'General Enquiry',
            enquiry: this.generateEnquiryMessage(formData),
            preferredContactMethod: formData.preferredContact || 'Email',
            preferredContactTime: formData.date && formData.time ?
                new Date(`${formData.date}T${this.convertTimeTo24Hour(formData.time)}`).toISOString() : null
        };

        try {
            const response = await fetch(`${this.API_BASE}/email/enquiry`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();
            return { success: response.ok, data: result };
        } catch (error) {
            console.error('Enquiry form submission error:', error);
            return { success: false, error: 'Network error occurred' };
        }
    },

    /**
     * Generate contact message from form data
     */
    generateContactMessage(formData) {
        let message = `Department: ${formData.department || 'General'}\n\n`;
        message += `Message: ${formData.comment || formData.message}\n\n`;

        if (formData.phone) {
            message += `Phone: ${formData.phone}\n`;
        }

        message += `\n---\nSubmitted via MCR Solicitors website contact form`;
        return message;
    },

    /**
     * Generate enquiry message from form data
     */
    generateEnquiryMessage(formData) {
        let message = `Service Required: ${formData.department || 'General Consultation'}\n\n`;

        if (formData.comment || formData.message) {
            message += `Details: ${formData.comment || formData.message}\n\n`;
        }

        if (formData.date) {
            message += `Preferred Date: ${formData.date}\n`;
        }

        if (formData.time) {
            message += `Preferred Time: ${formData.time}\n`;
        }

        if (formData.phone) {
            message += `Phone: ${formData.phone}\n`;
        }

        message += `\n---\nSubmitted via MCR Solicitors website enquiry form`;
        return message;
    },

    /**
     * Convert 12-hour time format to 24-hour format
     */
    convertTimeTo24Hour(time12h) {
        if (!time12h || time12h === 'Anytime') return '09:00';

        const [time, modifier] = time12h.split(' ');
        let [hours, minutes] = time.split(':');

        if (hours === '12') {
            hours = '00';
        }

        if (modifier === 'PM') {
            hours = parseInt(hours, 10) + 12;
        }

        return `${hours}:${minutes}`;
    },

    /**
     * Show success message
     */
    showSuccessMessage(message = 'Thank you! Your message has been sent successfully. We will get back to you soon.') {
        this.showMessage(message, 'success');
    },

    /**
     * Show error message
     */
    showErrorMessage(message = 'Sorry, there was an error sending your message. Please try again or call us directly.') {
        this.showMessage(message, 'error');
    },

    /**
     * Display message to user
     */
    showMessage(message, type) {
        // Remove existing messages
        const existingMessages = document.querySelectorAll('.email-service-message');
        existingMessages.forEach(msg => msg.remove());

        // Create message element
        const messageDiv = document.createElement('div');
        messageDiv.className = `email-service-message email-service-${type}`;
        messageDiv.style.cssText = `
            padding: 15px 20px;
            margin: 20px 0;
            border-radius: 5px;
            font-weight: 600;
            text-align: center;
            ${type === 'success' ?
                'background: #d4edda; color: #155724; border: 1px solid #c3e6cb;' :
                'background: #f8d7da; color: #721c24; border: 1px solid #f5c6cb;'
            }
        `;
        messageDiv.textContent = message;

        // Insert message after form or at the top of the page
        const form = document.querySelector('form');
        if (form) {
            form.parentNode.insertBefore(messageDiv, form.nextSibling);
        } else {
            document.body.insertBefore(messageDiv, document.body.firstChild);
        }

        // Auto-hide success messages after 5 seconds
        if (type === 'success') {
            setTimeout(() => {
                messageDiv.remove();
            }, 5000);
        }

        // Scroll to message
        messageDiv.scrollIntoView({ behavior: 'smooth', block: 'center' });
    },

    /**
     * Show loading state
     */
    showLoading(button) {
        if (button) {
            button.disabled = true;
            button.originalText = button.textContent;
            button.textContent = 'Sending...';
        }
    },

    /**
     * Hide loading state
     */
    hideLoading(button) {
        if (button) {
            button.disabled = false;
            button.textContent = button.originalText || 'Submit';
        }
    }
};

/**
 * Initialize form handlers when DOM is loaded
 */
document.addEventListener('DOMContentLoaded', function() {

    // Handle contact us form (contact-us.html)
    const contactForm = document.getElementById('consultationForm');
    if (contactForm) {
        contactForm.addEventListener('submit', async function(e) {
            e.preventDefault();

            const submitButton = this.querySelector('button[type="submit"]');
            EmailService.showLoading(submitButton);

            try {
                const formData = new FormData(this);
                const data = {
                    name: formData.get('name'),
                    email: formData.get('email'),
                    phone: formData.get('phone'),
                    department: formData.get('department'),
                    comment: formData.get('comment')
                };

                const result = await EmailService.submitContactForm(data);

                if (result.success) {
                    EmailService.showSuccessMessage();
                    this.reset(); // Clear form
                } else {
                    EmailService.showErrorMessage(result.data?.message || result.error);
                }
            } catch (error) {
                EmailService.showErrorMessage();
            } finally {
                EmailService.hideLoading(submitButton);
            }
        });
    }

    // Handle appointment/enquiry form (book-an-appointment.html)
    const appointmentForm = document.getElementById('appointmentForm');
    if (appointmentForm) {
        appointmentForm.addEventListener('submit', async function(e) {
            e.preventDefault();

            const submitButton = this.querySelector('button[type="submit"]');
            EmailService.showLoading(submitButton);

            try {
                const formData = new FormData(this);
                const data = {
                    name: formData.get('name'),
                    email: formData.get('email'),
                    phone: formData.get('phone'),
                    department: formData.get('department'),
                    date: formData.get('date'),
                    time: formData.get('time'),
                    comment: formData.get('comment') || formData.get('message'),
                    preferredContact: formData.get('preferredContact') || 'Email'
                };

                const result = await EmailService.submitEnquiryForm(data);

                if (result.success) {
                    EmailService.showSuccessMessage('Thank you! Your appointment request has been submitted. We will contact you shortly to confirm your appointment.');
                    this.reset(); // Clear form
                } else {
                    EmailService.showErrorMessage(result.data?.message || result.error);
                }
            } catch (error) {
                EmailService.showErrorMessage();
            } finally {
                EmailService.hideLoading(submitButton);
            }
        });
    }

    // Handle any generic forms with class 'mcr-contact-form'
    const genericForms = document.querySelectorAll('.mcr-contact-form');
    genericForms.forEach(form => {
        form.addEventListener('submit', async function(e) {
            e.preventDefault();

            const submitButton = this.querySelector('button[type="submit"]');
            EmailService.showLoading(submitButton);

            try {
                const formData = new FormData(this);
                const data = {
                    name: formData.get('name'),
                    email: formData.get('email'),
                    phone: formData.get('phone'),
                    department: formData.get('department') || formData.get('service') || 'General Enquiry',
                    comment: formData.get('comment') || formData.get('message') || formData.get('enquiry'),
                    date: formData.get('date'),
                    time: formData.get('time')
                };

                // Determine if it's an enquiry form (has date/time) or contact form
                const isEnquiryForm = data.date || data.time;
                const result = isEnquiryForm ?
                    await EmailService.submitEnquiryForm(data) :
                    await EmailService.submitContactForm(data);

                if (result.success) {
                    EmailService.showSuccessMessage();
                    this.reset(); // Clear form
                } else {
                    EmailService.showErrorMessage(result.data?.message || result.error);
                }
            } catch (error) {
                EmailService.showErrorMessage();
            } finally {
                EmailService.hideLoading(submitButton);
            }
        });
    });
});

// Make EmailService available globally for manual use
window.EmailService = EmailService;