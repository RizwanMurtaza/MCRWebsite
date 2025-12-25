<?php
/**
 * Create WPForms Contact and Appointment Forms
 * Run via: wp eval-file setup-forms.php --allow-root
 */

// Check if WPForms is active
if (!function_exists('wpforms')) {
    echo "WPForms is not active. Please activate it first.\n";
    exit;
}

// Contact Form Configuration
$contact_form = array(
    'field_id' => 5,
    'fields' => array(
        1 => array(
            'id' => '1',
            'type' => 'name',
            'label' => 'Name',
            'format' => 'simple',
            'required' => '1',
            'size' => 'large',
        ),
        2 => array(
            'id' => '2',
            'type' => 'email',
            'label' => 'Email',
            'required' => '1',
            'size' => 'large',
        ),
        3 => array(
            'id' => '3',
            'type' => 'phone',
            'label' => 'Phone Number',
            'format' => 'international',
            'required' => '0',
            'size' => 'large',
        ),
        4 => array(
            'id' => '4',
            'type' => 'select',
            'label' => 'Department',
            'required' => '1',
            'size' => 'large',
            'choices' => array(
                1 => array('label' => 'Visas & Immigration', 'value' => 'immigration'),
                2 => array('label' => 'Divorce & Family Law', 'value' => 'family'),
                3 => array('label' => 'Personal Injury', 'value' => 'injury'),
            ),
        ),
        5 => array(
            'id' => '5',
            'type' => 'textarea',
            'label' => 'Message',
            'required' => '1',
            'size' => 'large',
            'placeholder' => 'Briefly explain your legal matter',
        ),
    ),
    'settings' => array(
        'form_title' => 'Enquiry Form',
        'submit_text' => 'Submit',
        'submit_text_processing' => 'Sending...',
        'notification_enable' => '1',
        'notifications' => array(
            1 => array(
                'notification_name' => 'Default Notification',
                'email' => 'info@mcrsolicitors.co.uk',
                'subject' => 'New Enquiry from {field_id="1"}',
                'sender_name' => 'MCR Solicitors Website',
                'sender_address' => '{admin_email}',
                'replyto' => '{field_id="2"}',
                'message' => "New enquiry received:\n\nName: {field_id=\"1\"}\nEmail: {field_id=\"2\"}\nPhone: {field_id=\"3\"}\nDepartment: {field_id=\"4\"}\n\nMessage:\n{field_id=\"5\"}",
            ),
        ),
        'confirmations' => array(
            1 => array(
                'type' => 'message',
                'message' => '<p>Thank you for your enquiry. We will get back to you within 24 hours.</p>',
            ),
        ),
    ),
);

// Appointment Form Configuration
$appointment_form = array(
    'field_id' => 9,
    'fields' => array(
        1 => array(
            'id' => '1',
            'type' => 'name',
            'label' => 'Name',
            'format' => 'simple',
            'required' => '1',
            'size' => 'large',
        ),
        2 => array(
            'id' => '2',
            'type' => 'email',
            'label' => 'Email',
            'required' => '1',
            'size' => 'large',
        ),
        3 => array(
            'id' => '3',
            'type' => 'phone',
            'label' => 'Phone Number',
            'format' => 'international',
            'required' => '1',
            'size' => 'large',
        ),
        4 => array(
            'id' => '4',
            'type' => 'select',
            'label' => 'Consultation Method',
            'required' => '1',
            'size' => 'large',
            'choices' => array(
                1 => array('label' => 'In-Person (Manchester Office)', 'value' => 'in-person'),
                2 => array('label' => 'Zoom Video Call', 'value' => 'zoom'),
                3 => array('label' => 'Microsoft Teams', 'value' => 'teams'),
                4 => array('label' => 'Skype', 'value' => 'skype'),
                5 => array('label' => 'WhatsApp', 'value' => 'whatsapp'),
                6 => array('label' => 'Phone Call', 'value' => 'phone'),
            ),
        ),
        5 => array(
            'id' => '5',
            'type' => 'date-time',
            'label' => 'Preferred Date',
            'required' => '1',
            'size' => 'large',
            'date_format' => 'd/m/Y',
            'date_type' => 'datepicker',
            'time_format' => 'H:i',
            'time_interval' => 30,
        ),
        6 => array(
            'id' => '6',
            'type' => 'select',
            'label' => 'Preferred Time',
            'required' => '1',
            'size' => 'large',
            'choices' => array(
                1 => array('label' => '9:00 AM', 'value' => '09:00'),
                2 => array('label' => '9:30 AM', 'value' => '09:30'),
                3 => array('label' => '10:00 AM', 'value' => '10:00'),
                4 => array('label' => '10:30 AM', 'value' => '10:30'),
                5 => array('label' => '11:00 AM', 'value' => '11:00'),
                6 => array('label' => '11:30 AM', 'value' => '11:30'),
                7 => array('label' => '12:00 PM', 'value' => '12:00'),
                8 => array('label' => '12:30 PM', 'value' => '12:30'),
                9 => array('label' => '2:00 PM', 'value' => '14:00'),
                10 => array('label' => '2:30 PM', 'value' => '14:30'),
                11 => array('label' => '3:00 PM', 'value' => '15:00'),
                12 => array('label' => '3:30 PM', 'value' => '15:30'),
                13 => array('label' => '4:00 PM', 'value' => '16:00'),
                14 => array('label' => '4:30 PM', 'value' => '16:30'),
                15 => array('label' => '5:00 PM', 'value' => '17:00'),
            ),
        ),
        7 => array(
            'id' => '7',
            'type' => 'select',
            'label' => 'Department',
            'required' => '1',
            'size' => 'large',
            'choices' => array(
                1 => array('label' => 'Visas & Immigration', 'value' => 'immigration'),
                2 => array('label' => 'Divorce & Family Law', 'value' => 'family'),
                3 => array('label' => 'Personal Injury', 'value' => 'injury'),
            ),
        ),
        8 => array(
            'id' => '8',
            'type' => 'textarea',
            'label' => 'Brief Description of Your Matter',
            'required' => '0',
            'size' => 'large',
            'placeholder' => 'Optional - Tell us briefly about your legal matter',
        ),
    ),
    'settings' => array(
        'form_title' => 'Book an Appointment',
        'submit_text' => 'Request Appointment',
        'submit_text_processing' => 'Submitting...',
        'notification_enable' => '1',
        'notifications' => array(
            1 => array(
                'notification_name' => 'Admin Notification',
                'email' => 'info@mcrsolicitors.co.uk',
                'subject' => 'New Appointment Request from {field_id="1"}',
                'sender_name' => 'MCR Solicitors Website',
                'sender_address' => '{admin_email}',
                'replyto' => '{field_id="2"}',
                'message' => "New appointment request:\n\nName: {field_id=\"1\"}\nEmail: {field_id=\"2\"}\nPhone: {field_id=\"3\"}\nConsultation Method: {field_id=\"4\"}\nPreferred Date: {field_id=\"5\"}\nPreferred Time: {field_id=\"6\"}\nDepartment: {field_id=\"7\"}\n\nAdditional Info:\n{field_id=\"8\"}",
            ),
        ),
        'confirmations' => array(
            1 => array(
                'type' => 'message',
                'message' => '<p>Thank you for your appointment request. Our team will contact you shortly to confirm your appointment.</p>',
            ),
        ),
    ),
);

// Create Contact Form
echo "Creating Contact Form...\n";
$contact_form_data = array(
    'post_title' => 'Enquiry Form',
    'post_status' => 'publish',
    'post_type' => 'wpforms',
    'post_content' => wp_json_encode($contact_form),
);

$contact_form_id = wp_insert_post($contact_form_data);
if ($contact_form_id) {
    echo "Contact Form created with ID: {$contact_form_id}\n";
    update_option('mcr_contact_form_id', $contact_form_id);
    update_option('mcr_enquiry_form_id', $contact_form_id);
} else {
    echo "Failed to create Contact Form\n";
}

// Create Appointment Form
echo "Creating Appointment Form...\n";
$appointment_form_data = array(
    'post_title' => 'Book an Appointment',
    'post_status' => 'publish',
    'post_type' => 'wpforms',
    'post_content' => wp_json_encode($appointment_form),
);

$appointment_form_id = wp_insert_post($appointment_form_data);
if ($appointment_form_id) {
    echo "Appointment Form created with ID: {$appointment_form_id}\n";
    update_option('mcr_appointment_form_id', $appointment_form_id);
} else {
    echo "Failed to create Appointment Form\n";
}

echo "\n=== Form Shortcodes ===\n";
echo "Contact Form: [wpforms id=\"{$contact_form_id}\"]\n";
echo "Appointment Form: [wpforms id=\"{$appointment_form_id}\"]\n";

echo "\nForms created successfully!\n";
