<?php
/**
 * Template Name: Service Page
 * Description: Template for service pages with breadcrumb, main content, and sidebar enquiry form
 */

get_header();

// Get page hierarchy for breadcrumb
$ancestors = array_reverse(get_post_ancestors($post->ID));
$breadcrumb_items = array();

// Add Home
$breadcrumb_items[] = array(
    'title' => 'Home',
    'url' => home_url('/')
);

// Add ancestors
foreach ($ancestors as $ancestor_id) {
    $ancestor = get_post($ancestor_id);
    $breadcrumb_items[] = array(
        'title' => $ancestor->post_title,
        'url' => get_permalink($ancestor_id)
    );
}

// Add current page
$breadcrumb_items[] = array(
    'title' => get_the_title(),
    'url' => ''
);
?>

<div class="mcr-page-wrapper">
    <div class="mcr-breadcrumb">
        <div class="mcr-breadcrumb-inner">
            <?php
            $count = count($breadcrumb_items);
            foreach ($breadcrumb_items as $index => $item):
                if ($index < $count - 1): ?>
                    <a href="<?php echo esc_url($item['url']); ?>"><?php echo esc_html($item['title']); ?></a>
                    <span class="separator">/</span>
                <?php else: ?>
                    <span class="current"><?php echo esc_html($item['title']); ?></span>
                <?php endif;
            endforeach; ?>
        </div>
    </div>

    <div class="mcr-service-container">
    <div class="mcr-service-main">
        <article id="post-<?php the_ID(); ?>" <?php post_class(); ?>>
            <?php while (have_posts()) : the_post(); ?>
                <div class="mcr-service-content">
                    <?php the_content(); ?>
                </div>
            <?php endwhile; ?>
        </article>
    </div>

    <aside class="mcr-service-sidebar">
        <div class="mcr-enquiry-form">
            <h2>Enquiry Form</h2>
            <?php mcr_contact_form_messages(); ?>
            <form class="mcr-contact-form" action="" method="post">
                <?php wp_nonce_field('mcr_contact_form', 'mcr_contact_nonce'); ?>
                <input type="hidden" name="form_time" value="<?php echo time(); ?>">
                <!-- Honeypot field - hidden from users -->
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

        <div class="mcr-sidebar-contact">
            <h3>Need Assistance?</h3>
            <p><strong>Call us:</strong><br>
            <a href="tel:01614661280" class="phone-link">0161 466 1280</a></p>
            <p><strong>Email:</strong><br>
            <a href="mailto:info@mcrsolicitors.co.uk">info@mcrsolicitors.co.uk</a></p>
            <p><strong>Office Hours:</strong><br>
            Monday - Friday<br>
            9:00 AM - 5:30 PM</p>
        </div>
    </aside>
    </div>
</div>

<?php get_footer(); ?>
