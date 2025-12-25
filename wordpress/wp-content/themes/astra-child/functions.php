<?php
/**
 * Astra Child Theme - MCR Solicitors
 */

// Enqueue parent and child theme styles
function mcr_enqueue_styles() {
    wp_enqueue_style('astra-parent-style', get_template_directory_uri() . '/style.css');
    wp_enqueue_style('mcr-child-style', get_stylesheet_uri(), array('astra-parent-style'));

    // Google Fonts
    wp_enqueue_style('mcr-google-fonts', 'https://fonts.googleapis.com/css2?family=Lora:ital,wght@0,400..700;1,400..700&family=Rubik:ital,wght@0,300..900;1,300..900&family=Montserrat:wght@300;400;500;600;700&display=swap', array(), null);
}
add_action('wp_enqueue_scripts', 'mcr_enqueue_styles');

// Add custom footer
function mcr_custom_footer() {
    ?>
    <div class="mcr-footer">
        <div class="mcr-footer-main">
            <div>
                <h2>Legal Services</h2>
                <a href="/immigration/">Visas & Immigration</a>
                <a href="/family-law/">Divorce & Family Law</a>
                <a href="/personal-injury/">Personal Injury</a>
            </div>
            <div>
                <h2>Legal</h2>
                <a href="/complaints-procedure/">Complaints Procedure</a>
                <a href="/privacy-policy/">Privacy Policy</a>
                <a href="/disclaimer/">Disclaimer</a>
                <a href="/cookie-policy/">Cookie Policy</a>
                <a href="/contact-us/">General Enquiries</a>
                <a href="/contact-us/">Contact Us</a>
            </div>
            <div class="mcr-footer-img">
                <h2>Lexcel Accredited</h2>
                <p>MCR Solicitors have been accredited with the Law Society (England and Wales) award, Lexcel.</p>
                <a href="/lexcel-accredited/" target="_blank">
                    <img src="<?php echo get_site_url(); ?>/wp-content/uploads/2025/12/footerpic1.png" alt="Lexcel Accredited Logo" style="max-width: 150px;">
                </a>
            </div>
            <div class="mcr-footer-img">
                <h2>Regulated by SRA</h2>
                <p>Authorised and regulated by the Solicitors Regulation Authority<br>under SRA ID: 648878</p>
                <a href="https://www.sra.org.uk/consumers/register/organisation/?sraNumber=648878" target="_blank">
                    <img src="<?php echo get_site_url(); ?>/wp-content/uploads/2025/12/Screenshot-2025-03-27-185444.png" alt="SRA Regulated Logo" style="max-width: 150px;">
                </a>
            </div>
        </div>
        <div class="mcr-footer-bottom">
            <p>
                MCR LAW ASSOCIATES LIMITED TRADING AS MCR SOLICITORS is a company registered by company house in
                England and Wales Registration No. 11069807 Registered Address: First Floor, 1024, Stockport Road, Manchester,
                England. M19 3WX, United Kingdom MCR Solicitors is authorised and regulated by the Solicitors Regulation Authority SRA
                ID: 648878, VAT Registration No. GB 301011396
            </p>
            <div class="copyright">© 2025 MCR Solicitors, All Rights Reserved</div>
        </div>
    </div>
    <?php
}
add_action('astra_footer_after', 'mcr_custom_footer');

// Remove default Astra footer
function mcr_remove_astra_footer() {
    remove_action('astra_footer', 'astra_footer_markup');
}
add_action('after_setup_theme', 'mcr_remove_astra_footer');

// Add theme support
function mcr_theme_setup() {
    add_theme_support('title-tag');
    add_theme_support('post-thumbnails');
    add_theme_support('custom-logo');
}
add_action('after_setup_theme', 'mcr_theme_setup');

// Handle contact form submissions (processes on page load before output)
function mcr_handle_contact_form() {
    if (!isset($_POST['mcr_contact_submit'])) {
        return;
    }

    if (!wp_verify_nonce($_POST['mcr_contact_nonce'], 'mcr_contact_form')) {
        wp_die('Security check failed');
    }

    $name = sanitize_text_field($_POST['name']);
    $email = sanitize_email($_POST['email']);
    $phone = sanitize_text_field($_POST['phone']);
    $department = sanitize_text_field($_POST['department']);
    $message = sanitize_textarea_field($_POST['message']);

    // Honeypot check
    if (!empty($_POST['website'])) {
        wp_die('Spam detected');
    }

    // Time check (form should take at least 3 seconds to fill)
    if (isset($_POST['form_time']) && (time() - intval($_POST['form_time'])) < 3) {
        wp_die('Please take your time filling the form');
    }

    $to = 'info@mcrsolicitors.co.uk';
    $subject = 'New Enquiry from ' . $name . ' - ' . $department;
    $body = "New enquiry received from MCR Solicitors website:\n\n";
    $body .= "Name: $name\n";
    $body .= "Email: $email\n";
    $body .= "Phone: $phone\n";
    $body .= "Department: $department\n\n";
    $body .= "Message:\n$message\n";

    $headers = array(
        'Content-Type: text/plain; charset=UTF-8',
        'From: MCR Solicitors Website <noreply@mcrsolicitors.co.uk>',
        'Reply-To: ' . $name . ' <' . $email . '>',
    );

    $sent = wp_mail($to, $subject, $body, $headers);

    if ($sent) {
        wp_redirect(add_query_arg('contact', 'success', wp_get_referer()));
    } else {
        wp_redirect(add_query_arg('contact', 'error', wp_get_referer()));
    }
    exit;
}
add_action('init', 'mcr_handle_contact_form');

// Display contact form success/error messages
function mcr_contact_form_messages() {
    if (isset($_GET['contact'])) {
        if ($_GET['contact'] === 'success') {
            echo '<div class="mcr-form-success">Thank you for your enquiry. We will get back to you within 24 hours.</div>';
        } elseif ($_GET['contact'] === 'error') {
            echo '<div class="mcr-form-error">There was an error sending your message. Please try again or call us directly.</div>';
        }
    }
}

// Handle appointment form submissions
function mcr_handle_appointment_form() {
    if (!isset($_POST['mcr_appointment_submit'])) {
        return;
    }

    if (!wp_verify_nonce($_POST['mcr_appointment_nonce'], 'mcr_appointment_form')) {
        wp_die('Security check failed');
    }

    $name = sanitize_text_field($_POST['name']);
    $email = sanitize_email($_POST['email']);
    $phone = sanitize_text_field($_POST['phone']);
    $method = sanitize_text_field($_POST['method']);
    $date = sanitize_text_field($_POST['preferred_date']);
    $time = sanitize_text_field($_POST['preferred_time']);
    $department = sanitize_text_field($_POST['department']);
    $message = sanitize_textarea_field($_POST['message']);

    // Honeypot check
    if (!empty($_POST['website'])) {
        wp_die('Spam detected');
    }

    // Time check
    if (isset($_POST['form_time']) && (time() - intval($_POST['form_time'])) < 3) {
        wp_die('Please take your time filling the form');
    }

    $to = 'info@mcrsolicitors.co.uk';
    $subject = 'New Appointment Request from ' . $name . ' - ' . $department;
    $body = "New appointment request from MCR Solicitors website:\n\n";
    $body .= "Name: $name\n";
    $body .= "Email: $email\n";
    $body .= "Phone: $phone\n";
    $body .= "Consultation Method: $method\n";
    $body .= "Preferred Date: $date\n";
    $body .= "Preferred Time: $time\n";
    $body .= "Department: $department\n\n";
    $body .= "Additional Information:\n$message\n";

    $headers = array(
        'Content-Type: text/plain; charset=UTF-8',
        'From: MCR Solicitors Website <noreply@mcrsolicitors.co.uk>',
        'Reply-To: ' . $name . ' <' . $email . '>',
    );

    $sent = wp_mail($to, $subject, $body, $headers);

    if ($sent) {
        wp_redirect(add_query_arg('appointment', 'success', wp_get_referer()));
    } else {
        wp_redirect(add_query_arg('appointment', 'error', wp_get_referer()));
    }
    exit;
}
add_action('init', 'mcr_handle_appointment_form');

// Display appointment form messages
function mcr_appointment_form_messages() {
    if (isset($_GET['appointment'])) {
        if ($_GET['appointment'] === 'success') {
            echo '<div class="mcr-form-success">Thank you for your appointment request. Our team will contact you shortly to confirm your appointment.</div>';
        } elseif ($_GET['appointment'] === 'error') {
            echo '<div class="mcr-form-error">There was an error sending your request. Please try again or call us directly.</div>';
        }
    }
}

// Shortcode for enquiry form - [mcr_enquiry_form]
function mcr_enquiry_form_shortcode() {
    ob_start();
    ?>
    <div class="mcr-enquiry-form">
        <h2>Enquiry Form</h2>
        <?php mcr_contact_form_messages(); ?>
        <form class="mcr-contact-form" action="" method="post">
            <?php wp_nonce_field('mcr_contact_form', 'mcr_contact_nonce'); ?>
            <input type="hidden" name="form_time" value="<?php echo time(); ?>">
            <div style="position: absolute; left: -9999px;">
                <input type="text" name="website" tabindex="-1" autocomplete="off">
            </div>
            <div class="form-group">
                <label for="name">* Name</label>
                <input type="text" name="name" id="name" placeholder="Enter Name" required>
            </div>
            <div class="form-group">
                <label for="email">* Email</label>
                <input type="email" name="email" id="email" placeholder="Enter Email" required>
            </div>
            <div class="form-group">
                <label for="phone">Phone Number</label>
                <input type="tel" name="phone" id="phone" placeholder="Enter Phone Number">
            </div>
            <div class="form-group">
                <label for="department">* Department</label>
                <select name="department" id="department" required>
                    <option value="" disabled selected>Select Department</option>
                    <option value="Visas & Immigration">Visas & Immigration</option>
                    <option value="Divorce & Family Law">Divorce & Family Law</option>
                    <option value="Personal Injury">Personal Injury</option>
                </select>
            </div>
            <div class="form-group">
                <label for="message">* Message</label>
                <textarea name="message" id="message" rows="6" placeholder="Briefly explain your legal matter" required></textarea>
            </div>
            <button type="submit" name="mcr_contact_submit" class="mcr-submit-btn">Submit</button>
        </form>
    </div>
    <?php
    return ob_get_clean();
}
add_shortcode('mcr_enquiry_form', 'mcr_enquiry_form_shortcode');

// Shortcode for appointment form - [mcr_appointment_form]
function mcr_appointment_form_shortcode() {
    ob_start();
    ?>
    <div class="mcr-enquiry-form mcr-appointment-form">
        <h2>Book an Appointment</h2>
        <?php mcr_appointment_form_messages(); ?>
        <form class="mcr-contact-form" action="" method="post">
            <?php wp_nonce_field('mcr_appointment_form', 'mcr_appointment_nonce'); ?>
            <input type="hidden" name="form_time" value="<?php echo time(); ?>">
            <div style="position: absolute; left: -9999px;">
                <input type="text" name="website" tabindex="-1" autocomplete="off">
            </div>
            <div class="form-group">
                <label for="name">* Name</label>
                <input type="text" name="name" id="name" placeholder="Enter Name" required>
            </div>
            <div class="form-group">
                <label for="email">* Email</label>
                <input type="email" name="email" id="email" placeholder="Enter Email" required>
            </div>
            <div class="form-group">
                <label for="phone">* Phone Number</label>
                <input type="tel" name="phone" id="phone" placeholder="Enter Phone Number" required>
            </div>
            <div class="form-group">
                <label for="method">* Consultation Method</label>
                <select name="method" id="method" required>
                    <option value="" disabled selected>Select Method</option>
                    <option value="In-Person (Manchester Office)">In-Person (Manchester Office)</option>
                    <option value="Zoom Video Call">Zoom Video Call</option>
                    <option value="Microsoft Teams">Microsoft Teams</option>
                    <option value="Skype">Skype</option>
                    <option value="WhatsApp">WhatsApp</option>
                    <option value="Phone Call">Phone Call</option>
                </select>
            </div>
            <div class="form-group">
                <label for="preferred_date">* Preferred Date</label>
                <input type="date" name="preferred_date" id="preferred_date" required>
            </div>
            <div class="form-group">
                <label for="preferred_time">* Preferred Time</label>
                <select name="preferred_time" id="preferred_time" required>
                    <option value="" disabled selected>Select Time</option>
                    <option value="9:00 AM">9:00 AM</option>
                    <option value="9:30 AM">9:30 AM</option>
                    <option value="10:00 AM">10:00 AM</option>
                    <option value="10:30 AM">10:30 AM</option>
                    <option value="11:00 AM">11:00 AM</option>
                    <option value="11:30 AM">11:30 AM</option>
                    <option value="12:00 PM">12:00 PM</option>
                    <option value="12:30 PM">12:30 PM</option>
                    <option value="2:00 PM">2:00 PM</option>
                    <option value="2:30 PM">2:30 PM</option>
                    <option value="3:00 PM">3:00 PM</option>
                    <option value="3:30 PM">3:30 PM</option>
                    <option value="4:00 PM">4:00 PM</option>
                    <option value="4:30 PM">4:30 PM</option>
                    <option value="5:00 PM">5:00 PM</option>
                </select>
            </div>
            <div class="form-group">
                <label for="department">* Department</label>
                <select name="department" id="department" required>
                    <option value="" disabled selected>Select Department</option>
                    <option value="Visas & Immigration">Visas & Immigration</option>
                    <option value="Divorce & Family Law">Divorce & Family Law</option>
                    <option value="Personal Injury">Personal Injury</option>
                </select>
            </div>
            <div class="form-group">
                <label for="message">Brief Description (Optional)</label>
                <textarea name="message" id="message" rows="4" placeholder="Tell us briefly about your legal matter"></textarea>
            </div>
            <button type="submit" name="mcr_appointment_submit" class="mcr-submit-btn">Request Appointment</button>
        </form>
    </div>
    <?php
    return ob_get_clean();
}
add_shortcode('mcr_appointment_form', 'mcr_appointment_form_shortcode');

// Add reCAPTCHA script if keys are set
function mcr_enqueue_recaptcha() {
    $site_key = get_option('mcr_recaptcha_site_key', '');
    if (!empty($site_key)) {
        wp_enqueue_script('google-recaptcha', 'https://www.google.com/recaptcha/api.js', array(), null, true);
    }
}
add_action('wp_enqueue_scripts', 'mcr_enqueue_recaptcha');
