<?php
/**
 * Create WPForms properly
 * Run via: wp eval-file create-wpforms.php --allow-root
 */

// Contact/Enquiry Form
$contact_form_content = array(
    'id' => 1,
    'field_id' => 6,
    'fields' => array(
        '1' => array(
            'id' => '1',
            'type' => 'name',
            'label' => 'Name',
            'format' => 'simple',
            'required' => '1',
            'size' => 'large',
        ),
        '2' => array(
            'id' => '2',
            'type' => 'email',
            'label' => 'Email',
            'required' => '1',
            'size' => 'large',
        ),
        '3' => array(
            'id' => '3',
            'type' => 'phone',
            'label' => 'Phone Number',
            'format' => 'us',
            'required' => '0',
            'size' => 'large',
        ),
        '4' => array(
            'id' => '4',
            'type' => 'select',
            'label' => 'Department',
            'required' => '1',
            'size' => 'large',
            'placeholder' => 'Select Department',
            'choices' => array(
                '1' => array(
                    'label' => 'Visas & Immigration',
                    'value' => '',
                ),
                '2' => array(
                    'label' => 'Divorce & Family Law',
                    'value' => '',
                ),
                '3' => array(
                    'label' => 'Personal Injury',
                    'value' => '',
                ),
            ),
        ),
        '5' => array(
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
        'form_desc' => '',
        'submit_text' => 'Submit',
        'submit_text_processing' => 'Sending...',
        'honeypot' => '1',
        'notification_enable' => '1',
        'notifications' => array(
            '1' => array(
                'email' => 'info@mcrsolicitors.co.uk',
                'subject' => 'New Enquiry from {field_id="1"}',
                'sender_name' => 'MCR Solicitors Website',
                'sender_address' => '{admin_email}',
                'replyto' => '{field_id="2"}',
                'message' => '{all_fields}',
            ),
        ),
        'confirmations' => array(
            '1' => array(
                'type' => 'message',
                'message' => '<p>Thank you for your enquiry. We will get back to you within 24 hours.</p>',
                'message_scroll' => '1',
            ),
        ),
        'anti_spam' => array(
            'time_limit' => array(
                'enable' => '1',
            ),
        ),
    ),
    'meta' => array(
        'template' => 'simple-contact-form-template',
    ),
);

// Appointment Form
$appointment_form_content = array(
    'id' => 2,
    'field_id' => 9,
    'fields' => array(
        '1' => array(
            'id' => '1',
            'type' => 'name',
            'label' => 'Name',
            'format' => 'simple',
            'required' => '1',
            'size' => 'large',
        ),
        '2' => array(
            'id' => '2',
            'type' => 'email',
            'label' => 'Email',
            'required' => '1',
            'size' => 'large',
        ),
        '3' => array(
            'id' => '3',
            'type' => 'phone',
            'label' => 'Phone Number',
            'format' => 'us',
            'required' => '1',
            'size' => 'large',
        ),
        '4' => array(
            'id' => '4',
            'type' => 'select',
            'label' => 'Consultation Method',
            'required' => '1',
            'size' => 'large',
            'placeholder' => 'Select Method',
            'choices' => array(
                '1' => array('label' => 'In-Person (Manchester Office)', 'value' => ''),
                '2' => array('label' => 'Zoom Video Call', 'value' => ''),
                '3' => array('label' => 'Microsoft Teams', 'value' => ''),
                '4' => array('label' => 'Skype', 'value' => ''),
                '5' => array('label' => 'WhatsApp', 'value' => ''),
                '6' => array('label' => 'Phone Call', 'value' => ''),
            ),
        ),
        '5' => array(
            'id' => '5',
            'type' => 'date-time',
            'label' => 'Preferred Date & Time',
            'required' => '1',
            'size' => 'large',
            'date_placeholder' => 'Select Date',
            'date_format' => 'd/m/Y',
            'date_type' => 'datepicker',
            'time_placeholder' => 'Select Time',
            'time_format' => 'g:i A',
            'time_interval' => '30',
        ),
        '6' => array(
            'id' => '6',
            'type' => 'select',
            'label' => 'Department',
            'required' => '1',
            'size' => 'large',
            'placeholder' => 'Select Department',
            'choices' => array(
                '1' => array('label' => 'Visas & Immigration', 'value' => ''),
                '2' => array('label' => 'Divorce & Family Law', 'value' => ''),
                '3' => array('label' => 'Personal Injury', 'value' => ''),
            ),
        ),
        '7' => array(
            'id' => '7',
            'type' => 'textarea',
            'label' => 'Brief Description (Optional)',
            'required' => '0',
            'size' => 'large',
            'placeholder' => 'Tell us briefly about your legal matter',
        ),
    ),
    'settings' => array(
        'form_title' => 'Book an Appointment',
        'form_desc' => '',
        'submit_text' => 'Request Appointment',
        'submit_text_processing' => 'Submitting...',
        'honeypot' => '1',
        'notification_enable' => '1',
        'notifications' => array(
            '1' => array(
                'email' => 'info@mcrsolicitors.co.uk',
                'subject' => 'New Appointment Request from {field_id="1"}',
                'sender_name' => 'MCR Solicitors Website',
                'sender_address' => '{admin_email}',
                'replyto' => '{field_id="2"}',
                'message' => '{all_fields}',
            ),
        ),
        'confirmations' => array(
            '1' => array(
                'type' => 'message',
                'message' => '<p>Thank you for your appointment request. Our team will contact you shortly to confirm your appointment.</p>',
                'message_scroll' => '1',
            ),
        ),
        'anti_spam' => array(
            'time_limit' => array(
                'enable' => '1',
            ),
        ),
    ),
    'meta' => array(
        'template' => 'simple-contact-form-template',
    ),
);

// Create Contact Form
$contact_post = array(
    'post_title'   => 'Enquiry Form',
    'post_status'  => 'publish',
    'post_type'    => 'wpforms',
);

$contact_id = wp_insert_post($contact_post);
if ($contact_id) {
    $contact_form_content['id'] = $contact_id;
    update_post_meta($contact_id, 'wpforms_form', wp_json_encode($contact_form_content));
    wp_update_post(array(
        'ID' => $contact_id,
        'post_content' => wp_json_encode($contact_form_content),
    ));
    update_option('mcr_enquiry_form_id', $contact_id);
    echo "Contact Form created: ID {$contact_id}\n";
    echo "Shortcode: [wpforms id=\"{$contact_id}\"]\n\n";
}

// Create Appointment Form
$appointment_post = array(
    'post_title'   => 'Book an Appointment',
    'post_status'  => 'publish',
    'post_type'    => 'wpforms',
);

$appointment_id = wp_insert_post($appointment_post);
if ($appointment_id) {
    $appointment_form_content['id'] = $appointment_id;
    update_post_meta($appointment_id, 'wpforms_form', wp_json_encode($appointment_form_content));
    wp_update_post(array(
        'ID' => $appointment_id,
        'post_content' => wp_json_encode($appointment_form_content),
    ));
    update_option('mcr_appointment_form_id', $appointment_id);
    echo "Appointment Form created: ID {$appointment_id}\n";
    echo "Shortcode: [wpforms id=\"{$appointment_id}\"]\n";
}

echo "\nForms created successfully!\n";
