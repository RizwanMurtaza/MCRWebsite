<?php
/**
 * Create WordPress Navigation Menu
 * Run via: wp eval-file create-menu.php --allow-root
 */

// Delete existing menus to start fresh
$existing_menu = wp_get_nav_menu_object('Primary Menu');
if ($existing_menu) {
    wp_delete_nav_menu('Primary Menu');
    echo "Deleted existing Primary Menu\n";
}

// Create the menu
$menu_id = wp_create_nav_menu('Primary Menu');
echo "Created menu with ID: {$menu_id}\n";

// Helper function to get page ID by title
function get_page_id_by_title($title) {
    global $wpdb;
    $page = $wpdb->get_row(
        $wpdb->prepare(
            "SELECT ID FROM $wpdb->posts WHERE post_title = %s AND post_type = 'page' AND post_status = 'publish' LIMIT 1",
            $title
        )
    );
    return $page ? $page->ID : 0;
}

// Helper function to add menu item
function add_menu_item($menu_id, $title, $page_title = null, $parent_id = 0, $url = null) {
    $page_id = $page_title ? get_page_id_by_title($page_title) : 0;

    $args = [
        'menu-item-title' => $title,
        'menu-item-status' => 'publish',
        'menu-item-parent-id' => $parent_id,
    ];

    if ($url) {
        $args['menu-item-type'] = 'custom';
        $args['menu-item-url'] = $url;
    } elseif ($page_id) {
        $args['menu-item-type'] = 'post_type';
        $args['menu-item-object'] = 'page';
        $args['menu-item-object-id'] = $page_id;
    } else {
        $args['menu-item-type'] = 'custom';
        $args['menu-item-url'] = '#';
    }

    $item_id = wp_update_nav_menu_item($menu_id, 0, $args);
    echo "Added menu item: {$title} (ID: {$item_id})\n";
    return $item_id;
}

// Build the menu structure
echo "\nBuilding menu structure...\n\n";

// Home
$home = add_menu_item($menu_id, 'Home', 'Home');

// Immigration
$immigration = add_menu_item($menu_id, 'Immigration', 'Immigration');

// Immigration > UK Family Visas
$family_visas = add_menu_item($menu_id, 'UK Family Visas', 'UK Family Visas', $immigration);
add_menu_item($menu_id, 'Fiance(e) Visa UK', 'Fiance Visa UK', $family_visas);
add_menu_item($menu_id, 'Spouse Visa UK', 'Spouse Visa UK', $family_visas);
add_menu_item($menu_id, 'Unmarried Partner Visa UK', 'Unmarried Partner Visa UK', $family_visas);
add_menu_item($menu_id, 'Civil Partner Visa UK', 'Civil Partner Visa UK', $family_visas);
add_menu_item($menu_id, 'Parents of British Citizen', 'Parents of British Citizen', $family_visas);

// Immigration > UK Work Visas
$work_visas = add_menu_item($menu_id, 'UK Work Visas', 'UK Work Visas', $immigration);
add_menu_item($menu_id, 'Skilled Worker Visa UK', 'Skilled Worker Visa UK', $work_visas);
add_menu_item($menu_id, 'Health And Care Worker Visa', 'Health and Care Worker Visa', $work_visas);
add_menu_item($menu_id, 'Minister of Religion Visa', 'Minister of Religion Visa', $work_visas);
add_menu_item($menu_id, 'International Sportsperson Visa', 'International Sportsperson Visa', $work_visas);
add_menu_item($menu_id, 'Graduate Visa (Post-Study)', 'Graduate Visa', $work_visas);
add_menu_item($menu_id, 'Youth Mobility Scheme Visa', 'Youth Mobility Scheme Visa', $work_visas);

// Immigration > Business Visas
$business_visas = add_menu_item($menu_id, 'Business Visas UK', 'Business Investment Visas', $immigration);
add_menu_item($menu_id, 'Innovator Visa UK', 'Innovator Visa UK', $business_visas);
add_menu_item($menu_id, 'Start-up Visa UK', 'Start-up Visa UK', $business_visas);
add_menu_item($menu_id, 'Global Talent Visa', 'Global Talent Visa', $business_visas);
add_menu_item($menu_id, 'UK Expansion Worker Visa', 'UK Expansion Worker Visa', $business_visas);

// Immigration > Visitor Visas
$visitor_visas = add_menu_item($menu_id, 'Visitor Visas UK', 'Visitor Visas UK', $immigration);
add_menu_item($menu_id, 'Standard Visitor Visa UK', 'Standard Visitor Visa UK', $visitor_visas);
add_menu_item($menu_id, 'Marriage Visitor Visa UK', 'Marriage Visitor Visa UK', $visitor_visas);
add_menu_item($menu_id, 'Family Visitor Visa UK', 'Family Visitor Visa UK', $visitor_visas);
add_menu_item($menu_id, 'Business Visitor Visa UK', 'Business Visitor Visa UK', $visitor_visas);
add_menu_item($menu_id, 'Child Visitor Visa UK', 'Child Visitor Visa UK', $visitor_visas);

// Immigration > ILR
$ilr = add_menu_item($menu_id, 'Settle in the UK (ILR)', 'Indefinite Leave to Remain (ILR)', $immigration);
add_menu_item($menu_id, 'ILR Long Residence (10 Year Route)', 'ILR Long Residence 10 Year Route', $ilr);
add_menu_item($menu_id, 'ILR as a Spouse', 'ILR as a Spouse', $ilr);
add_menu_item($menu_id, 'ILR as a Parent of British Child', 'ILR as Parent of British Child', $ilr);
add_menu_item($menu_id, 'ILR as a Skilled Worker', 'ILR as a Skilled Worker', $ilr);
add_menu_item($menu_id, 'ILR as a Bereaved Partner', 'ILR as a Bereaved Partner', $ilr);

// Immigration > British Citizenship
$citizenship = add_menu_item($menu_id, 'British Citizenship', 'British Citizenship', $immigration);
add_menu_item($menu_id, 'British Citizenship by Naturalisation', 'British Citizenship by Naturalisation', $citizenship);
add_menu_item($menu_id, 'British Citizenship by Marriage', 'British Citizenship by Marriage', $citizenship);
add_menu_item($menu_id, 'Register Child as a British Citizen', 'Register Child as British Citizen', $citizenship);
add_menu_item($menu_id, 'British Citizenship by Registration', 'British Citizenship by Registration', $citizenship);
add_menu_item($menu_id, 'British Citizenship by Descent', 'British Citizenship by Descent', $citizenship);

// Immigration > Student Visas
$student_visas = add_menu_item($menu_id, 'Student Visas UK', 'Student Visas UK', $immigration);
add_menu_item($menu_id, 'Student Visa UK', 'Student Visa UK', $student_visas);
add_menu_item($menu_id, 'Child Student Visa UK', 'Child Student Visa UK', $student_visas);
add_menu_item($menu_id, 'Student Visitor Visa UK', 'Student Visitor Visa UK', $student_visas);
add_menu_item($menu_id, 'Switching To Student Visa UK', 'Switching to Student Visa UK', $student_visas);
add_menu_item($menu_id, 'Extension of UK Student Visa', 'Extension of UK Student Visa', $student_visas);

// Immigration > Other Applications
$other_apps = add_menu_item($menu_id, 'Other Immigration Applications', 'Immigration Applications', $immigration);
add_menu_item($menu_id, 'BRP Replacement Application', 'BRP Replacement Application', $other_apps);
add_menu_item($menu_id, 'No Time Limit (NTL) Application', 'No Time Limit Application', $other_apps);
add_menu_item($menu_id, 'Subject Access Request (SAR)', 'Subject Access Request', $other_apps);
add_menu_item($menu_id, 'Transfer of Conditions (TOC)', 'Transfer of Conditions', $other_apps);
add_menu_item($menu_id, 'Certificate of Entitlement to Right of Abode', 'Right of Abode', $other_apps);

// Family Law
$family_law = add_menu_item($menu_id, 'Family Law', 'Family Law');
add_menu_item($menu_id, 'Getting a Divorce', 'Divorce', $family_law);
add_menu_item($menu_id, 'Judicial Separation', 'Judicial Separation', $family_law);
add_menu_item($menu_id, 'Divorce Financial Settlement', 'Divorce Financial Settlement', $family_law);
add_menu_item($menu_id, 'Child Arrangement Order', 'Child Arrangement Order', $family_law);
add_menu_item($menu_id, 'Non Molestation & Occupation Orders', 'Non-Molestation Order', $family_law);
add_menu_item($menu_id, 'Prenuptial Agreements', 'Prenuptial Agreements', $family_law);

// Personal Injury
$personal_injury = add_menu_item($menu_id, 'Personal Injury', 'Personal Injury');
add_menu_item($menu_id, 'Road Traffic Accident Claims', 'Road Traffic Accidents', $personal_injury);
add_menu_item($menu_id, 'Accident at Work Claims', 'Accident at Work', $personal_injury);
add_menu_item($menu_id, 'Whiplash Injury Claims', 'Whiplash', $personal_injury);
add_menu_item($menu_id, 'Medical Negligence Claims', 'Medical Negligence', $personal_injury);
add_menu_item($menu_id, 'Slips, Trips & Falls Claims', 'Slips Trips and Falls', $personal_injury);

// Contact
$contact = add_menu_item($menu_id, 'Contact', null, 0, '#');
add_menu_item($menu_id, 'Manchester Office', 'Contact Us', $contact);
add_menu_item($menu_id, 'General Enquiries', 'Contact Us', $contact);
add_menu_item($menu_id, 'Book an Appointment', 'Book an Appointment', $contact);
add_menu_item($menu_id, 'Map & Driving Directions', null, $contact, 'https://goo.gl/maps/aQ9BQEMnRuizYTNV9');
add_menu_item($menu_id, 'Meet Our Team', 'Meet Our Team', $contact);
add_menu_item($menu_id, 'Lexcel Accredited', 'Lexcel Accredited', $contact);

// Our Fees
$fees = add_menu_item($menu_id, 'Our Fees', 'Our Fees');
add_menu_item($menu_id, 'Fees (UK Immigration Matters)', 'Immigration Fees', $fees);

// Assign menu to primary location
$locations = get_theme_mod('nav_menu_locations');
$locations['primary'] = $menu_id;
$locations['mobile_menu'] = $menu_id;
set_theme_mod('nav_menu_locations', $locations);

echo "\nMenu created and assigned to primary location!\n";
echo "Total menu items created.\n";
