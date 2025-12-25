<?php
/**
 * Page Header Template Part
 * Displays breadcrumb and page title with orange background
 */

// Get current page info
$page_title = get_the_title();
$parent_id = wp_get_post_parent_id(get_the_ID());
$parent_title = $parent_id ? get_the_title($parent_id) : '';
$parent_link = $parent_id ? get_permalink($parent_id) : '';
?>

<div class="mcr-page-header">
    <div class="mcr-page-header-inner">
        <nav class="mcr-breadcrumb">
            <a href="<?php echo home_url(); ?>">Home</a>
            <?php if ($parent_title): ?>
                <span class="separator">/</span>
                <a href="<?php echo esc_url($parent_link); ?>"><?php echo esc_html($parent_title); ?></a>
            <?php endif; ?>
            <span class="separator">/</span>
            <span class="current"><?php echo esc_html($page_title); ?></span>
        </nav>
        <h1 class="mcr-page-title"><?php echo esc_html($page_title); ?></h1>
    </div>
</div>
