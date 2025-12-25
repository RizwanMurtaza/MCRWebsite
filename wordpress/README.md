# MCR Solicitors WordPress Site

## Quick Start

The WordPress site is running in Docker containers.

### Access URLs
- **WordPress Site**: http://localhost:8080
- **WordPress Admin**: http://localhost:8080/wp-admin
- **phpMyAdmin**: http://localhost:8081

### Admin Credentials
- **Username**: admin
- **Password**: MCR@2024!
- **Email**: info@mcrsolicitors.co.uk

## Docker Commands

### Start the site
```bash
cd m:\MCRWebsite\wordpress
docker-compose up -d
```

### Stop the site
```bash
cd m:\MCRWebsite\wordpress
docker-compose down
```

### View logs
```bash
docker-compose logs -f
```

## Site Structure

### Pages (69 total, organized hierarchically)

**Main Service Pages:**
- Home
- Immigration (with 8 sub-categories, 40+ child pages)
- Family Law (6 child pages)
- Personal Injury (5 child pages)
- Contact Us
- Book an Appointment
- Meet Our Team
- Our Fees
- Lexcel Accredited

**Immigration Sub-categories:**
- UK Family Visas (5 visa types)
- UK Work Visas (6 visa types)
- Business & Investment Visas (4 visa types)
- Visitor Visas UK (5 visa types)
- Settle in the UK / ILR (5 routes)
- British Citizenship (5 types)
- Student Visas UK (5 visa types)
- Other Immigration Applications (5 applications)

**Family Law Services:**
- Divorce, Judicial Separation, Financial Settlement
- Child Arrangement Order, Non-Molestation Order
- Prenuptial Agreements

**Personal Injury Claims:**
- Road Traffic Accidents, Accident at Work
- Whiplash, Medical Negligence, Slips Trips & Falls

## Installed Plugins

| Plugin | Purpose |
|--------|---------|
| Astra | Theme (with custom child theme) |
| WPForms Lite | Contact forms |
| Yoast SEO | SEO optimization |
| Max Mega Menu | Navigation menus |
| WP Mail SMTP | Email delivery |
| WP Super Cache | Performance optimization |

## Custom Theme (astra-child)

The child theme includes:
- **Custom footer** matching original site (4 columns with accreditation badges)
- **Service page template** (`page-service.php`) with:
  - Orange breadcrumb navigation
  - Two-column layout (content + enquiry form sidebar)
  - Contact info sidebar
- **Custom CSS** with brand colors (#DE532A primary)

## Navigation Menu

The Primary Menu includes all pages from the original site with proper dropdown hierarchy:
- Home
- Immigration (with 8 sub-menus, each with 4-6 child items)
- Family Law (6 items)
- Personal Injury (5 items)
- Contact (6 items)
- Our Fees (1 item)

**Total menu items: 72**

## Next Steps (Manual Setup Required)

### 1. Create Contact Form in WPForms
1. Go to WPForms > Add New
2. Create a form with fields:
   - Name (required)
   - Email (required)
   - Phone Number
   - Department (dropdown: Visas & Immigration, Divorce & Family Law, Personal Injury)
   - Message (required)
3. Configure notifications to send to: info@mcrsolicitors.co.uk
4. Add the form shortcode to the Contact Us page

### 2. Create Appointment Form in WPForms
1. Go to WPForms > Add New
2. Create a form with fields:
   - Name (required)
   - Email (required)
   - Phone Number (required)
   - Consultation Method (dropdown: In-Person, Zoom, Teams, Skype, WhatsApp, Phone)
   - Preferred Date (date picker)
   - Preferred Time (dropdown)
   - Department (dropdown)
   - Message
3. Add the form shortcode to the Book an Appointment page

### 3. Configure Max Mega Menu (Optional)
1. Go to Mega Menu > Menu Locations
2. Enable Max Mega Menu for Primary Menu
3. The menu is already set up - just enable for enhanced dropdown styling

### 4. Set Up Email (WP Mail SMTP)
1. Go to WP Mail SMTP > Settings
2. Configure your SMTP server settings

### 5. Add Schema Markup (Yoast SEO)
1. Go to Yoast SEO > Search Appearance > Content Types
2. Configure Organization schema for Attorney/Legal Service

## Brand Colors
- Primary: #DE532A (orange)
- Secondary BG: #FFF6F3 (light cream)
- Dark Footer: #282828
- Text: #333333

## Fonts
- Google Fonts: Rubik, Montserrat, Lora

## File Structure
```
wordpress/
├── docker-compose.yml          # Docker configuration
├── wp-content/
│   ├── themes/
│   │   └── astra-child/        # Custom child theme
│   │       ├── style.css       # Custom styles
│   │       ├── functions.php   # Custom footer & enqueues
│   │       └── page-service.php # Service page template
│   └── uploads/
│       └── 2025/12/            # Uploaded images (logo, accreditation badges)
├── create-pages.php            # Script to create pages
├── fix-hierarchy.php           # Script to fix page hierarchy
├── create-menu.php             # Script to create navigation menu
└── README.md                   # This file
```
