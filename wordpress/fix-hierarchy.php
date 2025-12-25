<?php
/**
 * Fix page hierarchy and add content
 * Run via: wp eval-file fix-hierarchy.php --allow-root
 */

// Define page hierarchy
$hierarchy = [
    'Immigration' => [
        'UK Family Visas' => [
            'Fiance Visa UK',
            'Spouse Visa UK',
            'Unmarried Partner Visa UK',
            'Civil Partner Visa UK',
            'Parents of British Citizen'
        ],
        'UK Work Visas' => [
            'Skilled Worker Visa UK',
            'Health and Care Worker Visa',
            'Minister of Religion Visa',
            'International Sportsperson Visa',
            'Graduate Visa',
            'Youth Mobility Scheme Visa'
        ],
        'Business Investment Visas' => [
            'Innovator Visa UK',
            'Start-up Visa UK',
            'Global Talent Visa',
            'UK Expansion Worker Visa'
        ],
        'Visitor Visas UK' => [
            'Standard Visitor Visa UK',
            'Marriage Visitor Visa UK',
            'Family Visitor Visa UK',
            'Business Visitor Visa UK',
            'Child Visitor Visa UK'
        ],
        'Indefinite Leave to Remain (ILR)' => [
            'ILR Long Residence 10 Year Route',
            'ILR as a Spouse',
            'ILR as Parent of British Child',
            'ILR as a Skilled Worker',
            'ILR as a Bereaved Partner'
        ],
        'British Citizenship' => [
            'British Citizenship by Naturalisation',
            'British Citizenship by Marriage',
            'Register Child as British Citizen',
            'British Citizenship by Registration',
            'British Citizenship by Descent'
        ],
        'Student Visas UK' => [
            'Student Visa UK',
            'Child Student Visa UK',
            'Student Visitor Visa UK',
            'Switching to Student Visa UK',
            'Extension of UK Student Visa'
        ],
        'Immigration Applications' => [
            'BRP Replacement Application',
            'No Time Limit Application',
            'Subject Access Request',
            'Transfer of Conditions',
            'Right of Abode'
        ]
    ],
    'Family Law' => [
        'Divorce',
        'Judicial Separation',
        'Divorce Financial Settlement',
        'Child Arrangement Order',
        'Non-Molestation Order',
        'Prenuptial Agreements'
    ],
    'Personal Injury' => [
        'Road Traffic Accidents',
        'Accident at Work',
        'Whiplash',
        'Medical Negligence',
        'Slips Trips and Falls'
    ],
    'Our Fees' => [
        'Immigration Fees'
    ]
];

// Helper function to get page by title
function get_page_by_title_custom($title) {
    global $wpdb;
    $page = $wpdb->get_row(
        $wpdb->prepare(
            "SELECT * FROM $wpdb->posts WHERE post_title = %s AND post_type = 'page' AND post_status = 'publish' LIMIT 1",
            $title
        )
    );
    return $page;
}

// Function to set parent for a page
function set_page_parent($page_title, $parent_id) {
    $page = get_page_by_title_custom($page_title);
    if ($page) {
        wp_update_post([
            'ID' => $page->ID,
            'post_parent' => $parent_id
        ]);
        echo "Set parent for '{$page_title}' to parent ID {$parent_id}\n";
        return $page->ID;
    } else {
        echo "Page not found: {$page_title}\n";
        return 0;
    }
}

// Process hierarchy
function process_hierarchy($hierarchy, $parent_id = 0) {
    foreach ($hierarchy as $parent_title => $children) {
        if (is_array($children)) {
            // This is a parent with children
            $page = get_page_by_title_custom($parent_title);
            if ($page) {
                // Update parent if needed
                if ($parent_id > 0) {
                    wp_update_post([
                        'ID' => $page->ID,
                        'post_parent' => $parent_id
                    ]);
                    echo "Set parent for '{$parent_title}' to ID {$parent_id}\n";
                }

                // Process children
                foreach ($children as $child_key => $child_value) {
                    if (is_array($child_value)) {
                        // Child has its own children (sub-category)
                        $child_page = get_page_by_title_custom($child_key);
                        if ($child_page) {
                            wp_update_post([
                                'ID' => $child_page->ID,
                                'post_parent' => $page->ID
                            ]);
                            echo "Set parent for '{$child_key}' to '{$parent_title}' (ID {$page->ID})\n";

                            // Process grandchildren
                            foreach ($child_value as $grandchild) {
                                set_page_parent($grandchild, $child_page->ID);
                            }
                        }
                    } else {
                        // Simple child page
                        set_page_parent($child_value, $page->ID);
                    }
                }
            } else {
                echo "Parent page not found: {$parent_title}\n";
            }
        }
    }
}

echo "Starting hierarchy fix...\n\n";
process_hierarchy($hierarchy);
echo "\nHierarchy fix complete!\n";

// Now set page templates for all service pages
echo "\nSetting page templates...\n";

$service_pages = [
    'Immigration', 'UK Family Visas', 'UK Work Visas', 'Business Investment Visas',
    'Visitor Visas UK', 'Indefinite Leave to Remain (ILR)', 'British Citizenship',
    'Student Visas UK', 'Immigration Applications', 'Family Law', 'Personal Injury', 'Our Fees'
];

foreach ($service_pages as $page_title) {
    $page = get_page_by_title_custom($page_title);
    if ($page) {
        update_post_meta($page->ID, '_wp_page_template', 'page-service.php');
        echo "Set template for '{$page_title}'\n";
    }
}

// Set template for all child pages too
$all_pages = get_posts([
    'post_type' => 'page',
    'posts_per_page' => -1,
    'post_status' => 'publish'
]);

foreach ($all_pages as $page) {
    if ($page->post_parent > 0) {
        update_post_meta($page->ID, '_wp_page_template', 'page-service.php');
    }
}

echo "\nAll templates set!\n";
