<?php
/**
 * MCR Solicitors WordPress Page Creator
 * Run this script via WP-CLI: wp eval-file create-pages.php
 *
 * This creates all navigation pages with proper hierarchy and real content
 */

// Page structure with content from original HTML files
$pages = [
    // Main service pages (top level)
    'immigration' => [
        'title' => 'Immigration',
        'slug' => 'immigration',
        'template' => 'page-service.php',
        'content' => '<h1>Immigration Solicitors Manchester</h1>

<p>Our team of professional immigration lawyers in Manchester specialised in all immigration matters, including visa applications, British citizenship and Settlement (ILR).</p>

<p>If you are looking for an immigration solicitor in Manchester for UK visa & immigration applications then look no further than MCR Solicitors based in Levenshulme Manchester.</p>

<div class="highlight-box">
<h4>Call us on <a href="tel:+441614661280">0161 466 1280</a> for legal advice and assistance with your UK visa application. Our team of immigration solicitors in Manchester is ready to help you.</h4>
</div>

<p>We are located very close to Royal Nawaab (1-minute walk) and Levenshulme Station (6-minute walk).</p>

<h2>UK Visas & Immigration Applications</h2>

<p><strong>Below is a list of common categories of UK visa and immigration applications:</strong></p>

<ul>
<li><a href="/immigration/uk-family-visas/">Family Visas UK (Apply, Extend or Switch)</a></li>
<li><a href="/immigration/uk-work-visas/">Work Visas UK (Apply, Extend or Switch)</a></li>
<li><a href="/immigration/business-investment-visas/">Business & Investment Visas UK</a></li>
<li><a href="/immigration/indefinite-leave-to-remain/">Indefinite Leave to Remain (ILR) to Settle in the UK</a></li>
<li><a href="/immigration/british-citizenship/">British Citizenship (Naturalisation or Registration)</a></li>
<li><a href="/immigration/visitor-visas/">Visitor Visas UK (Short-Term Stay)</a></li>
<li><a href="/immigration/student-visas/">Student Visas UK (Study in the UK)</a></li>
<li><a href="/immigration/other-applications/">Other Immigration Applications</a></li>
</ul>

<h2>Family and Spouse Visa Solicitors</h2>

<p>The UK Family visas allow you to travel to the UK and live with a family member in the UK for more than 6 months. If you\'re visiting the UK for less than 6 months, then you might need to apply for a Standard Visitor visa or Marriage Visitor visa.</p>

<p>Our team of specialist immigration solicitors deal with all types of UK Family visa applications such as Fiance visa, Spouse visa, Unmarried Partner visa, Spouse visa extension, ILR as a Spouse, Marriage Visitor visa and so on.</p>

<h2>UK Work Visa Solicitors</h2>

<p>UK work visas allow you to work in the UK on a short or long-term basis depending upon your skills, qualifications and the nature of your job/work. If you are a non-EEA or Swiss national and you are interested in working in the UK, you will need to apply for a UK Work Visa.</p>

<p>Apply for a UK Work visa if you wish to live & work in the UK. Our Skilled Worker Visa solicitors in Manchester can help if you want to Apply, Extend or Switch to a UK Work visa.</p>

<h2>Solicitors for Settlement (ILR) Applications</h2>

<p>Indefinite Leave to Remain (ILR) commonly known as \'Settlement\' allows you to settle in the UK on a permanent basis and work in the UK without restrictions.</p>

<p>You can apply for Indefinite Leave to Remain (ILR) in the UK once you have lived in the UK for a certain number of years as required by the Immigration Rules.</p>

<h2>British Citizenship Lawyers</h2>

<p>Becoming a British citizen is a significant life event. Once you become a British citizen, it will give you the right to live and work in the UK permanently, without any immigration restrictions. It will also allow you to apply for a British passport.</p>

<p>You can apply for British citizenship either by Naturalisation or Registration to become a British citizen if you meet all the eligibility requirements.</p>

<h2>Why Choose MCR Solicitors?</h2>

<p><strong>1. Expertise and knowledge:</strong> Immigration law is complex, and it can be challenging for someone who is not familiar with the system to navigate. Our team of immigration solicitors has specialised knowledge and expertise in this area of law and can provide guidance and support throughout the immigration process.</p>

<p><strong>2. Assistance with paperwork:</strong> The immigration process often involves a significant amount of paperwork, and it can be easy to make mistakes or overlook important details. Our immigration solicitor can help ensure that all necessary documents are completed accurately and submitted on time.</p>

<p><strong>3. Increased chances of success:</strong> Our experienced immigration lawyer can assess your circumstances and provide legal advice on the best course of action. We can also provide representation, increasing your chances of success in your immigration matter.</p>

<p><strong>4. Protection of rights:</strong> Immigration law is constantly changing, and it can be challenging to keep up with the latest developments. Our immigration solicitor can help ensure that your rights are protected and that you are not unfairly disadvantaged by changes in the law.</p>

<p><strong>5. Peace of mind:</strong> The immigration process can be stressful and overwhelming. By hiring an immigration lawyer, you can have peace of mind knowing that you have someone on your side who is working to protect your interests and achieve your immigration goals.</p>

<h2>Need legal advice & assistance?</h2>

<p>Our highly experienced UK immigration solicitors deal with all types of UK visas and immigration applications. We\'re authorised and regulated by the Solicitors Regulation Authority (SRA), so you know you\'re in safe hands.</p>

<div class="highlight-box">
<h4>Contact our UK visa and immigration solicitors today for a free initial consultation and assessment.<br><br>You can call us on <a href="tel:+441614661280">0161 466 1280</a> or leave your details <a href="/contact-us/">here</a> for a callback request regarding your immigration matter.</h4>
</div>',
        'meta_desc' => 'Expert immigration solicitors in Manchester specializing in UK visa applications, British citizenship, ILR, spouse visas, work visas. SRA regulated with proven success rates.',
        'children' => [
            // UK Family Visas
            'uk-family-visas' => [
                'title' => 'UK Family Visas',
                'slug' => 'uk-family-visas',
                'template' => 'page-service.php',
                'content' => '<h1>UK Family Visas</h1>
<p>The UK Family visas allow you to travel to the UK and live with a family member in the UK for more than 6 months.</p>
<p>Our specialist immigration solicitors deal with all types of UK Family visa applications.</p>

<h2>Family Visa Services</h2>
<ul>
<li><a href="/immigration/uk-family-visas/fiance-visa/">Fiance(e) Visa UK</a></li>
<li><a href="/immigration/uk-family-visas/spouse-visa/">Spouse Visa UK</a></li>
<li><a href="/immigration/uk-family-visas/unmarried-partner-visa/">Unmarried Partner Visa UK</a></li>
<li><a href="/immigration/uk-family-visas/civil-partner-visa/">Civil Partner Visa UK</a></li>
<li><a href="/immigration/uk-family-visas/parents-of-british-citizen/">Parents of British Citizen</a></li>
</ul>

<div class="highlight-box">
<h4>Contact our family visa solicitors today for expert advice. Call <a href="tel:+441614661280">0161 466 1280</a></h4>
</div>',
                'children' => [
                    'fiance-visa' => ['title' => 'Fiance(e) Visa UK', 'slug' => 'fiance-visa'],
                    'spouse-visa' => ['title' => 'Spouse Visa UK', 'slug' => 'spouse-visa'],
                    'unmarried-partner-visa' => ['title' => 'Unmarried Partner Visa UK', 'slug' => 'unmarried-partner-visa'],
                    'civil-partner-visa' => ['title' => 'Civil Partner Visa UK', 'slug' => 'civil-partner-visa'],
                    'parents-of-british-citizen' => ['title' => 'Parents of British Citizen', 'slug' => 'parents-of-british-citizen'],
                ]
            ],
            // UK Work Visas
            'uk-work-visas' => [
                'title' => 'UK Work Visas',
                'slug' => 'uk-work-visas',
                'template' => 'page-service.php',
                'content' => '<h1>UK Work Visas</h1>
<p>UK work visas allow you to work in the UK on a short or long-term basis depending upon your skills, qualifications and the nature of your job/work.</p>

<h2>Work Visa Services</h2>
<ul>
<li><a href="/immigration/uk-work-visas/skilled-worker-visa/">Skilled Worker Visa UK</a></li>
<li><a href="/immigration/uk-work-visas/health-care-worker-visa/">Health And Care Worker Visa</a></li>
<li><a href="/immigration/uk-work-visas/minister-of-religion-visa/">Minister of Religion Visa</a></li>
<li><a href="/immigration/uk-work-visas/international-sportsperson-visa/">International Sportsperson Visa</a></li>
<li><a href="/immigration/uk-work-visas/graduate-visa/">Graduate Visa (Post-Study)</a></li>
<li><a href="/immigration/uk-work-visas/youth-mobility-scheme/">Youth Mobility Scheme Visa</a></li>
</ul>

<div class="highlight-box">
<h4>Contact our work visa solicitors today. Call <a href="tel:+441614661280">0161 466 1280</a></h4>
</div>',
                'children' => [
                    'skilled-worker-visa' => ['title' => 'Skilled Worker Visa UK', 'slug' => 'skilled-worker-visa'],
                    'health-care-worker-visa' => ['title' => 'Health And Care Worker Visa', 'slug' => 'health-care-worker-visa'],
                    'minister-of-religion-visa' => ['title' => 'Minister of Religion Visa', 'slug' => 'minister-of-religion-visa'],
                    'international-sportsperson-visa' => ['title' => 'International Sportsperson Visa', 'slug' => 'international-sportsperson-visa'],
                    'graduate-visa' => ['title' => 'Graduate Visa (Post-Study)', 'slug' => 'graduate-visa'],
                    'youth-mobility-scheme' => ['title' => 'Youth Mobility Scheme Visa', 'slug' => 'youth-mobility-scheme'],
                ]
            ],
            // Business Investment Visas
            'business-investment-visas' => [
                'title' => 'Business & Investment Visas',
                'slug' => 'business-investment-visas',
                'template' => 'page-service.php',
                'content' => '<h1>Business & Investment Visas UK</h1>
<p>Business and investment visas allow entrepreneurs and investors to establish or invest in UK businesses.</p>

<h2>Business Visa Services</h2>
<ul>
<li><a href="/immigration/business-investment-visas/innovator-visa/">Innovator Visa UK</a></li>
<li><a href="/immigration/business-investment-visas/start-up-visa/">Start-up Visa UK</a></li>
<li><a href="/immigration/business-investment-visas/global-talent-visa/">Global Talent Visa</a></li>
<li><a href="/immigration/business-investment-visas/uk-expansion-worker-visa/">UK Expansion Worker Visa</a></li>
</ul>

<div class="highlight-box">
<h4>Contact our business visa solicitors today. Call <a href="tel:+441614661280">0161 466 1280</a></h4>
</div>',
                'children' => [
                    'innovator-visa' => ['title' => 'Innovator Visa UK', 'slug' => 'innovator-visa'],
                    'start-up-visa' => ['title' => 'Start-up Visa UK', 'slug' => 'start-up-visa'],
                    'global-talent-visa' => ['title' => 'Global Talent Visa', 'slug' => 'global-talent-visa'],
                    'uk-expansion-worker-visa' => ['title' => 'UK Expansion Worker Visa', 'slug' => 'uk-expansion-worker-visa'],
                ]
            ],
            // Visitor Visas
            'visitor-visas' => [
                'title' => 'Visitor Visas UK',
                'slug' => 'visitor-visas',
                'template' => 'page-service.php',
                'content' => '<h1>Visitor Visas UK</h1>
<p>Visitor visas allow you to visit the UK for short-term stays for tourism, business meetings, or family visits.</p>

<h2>Visitor Visa Services</h2>
<ul>
<li><a href="/immigration/visitor-visas/standard-visitor-visa/">Standard Visitor Visa UK</a></li>
<li><a href="/immigration/visitor-visas/marriage-visitor-visa/">Marriage Visitor Visa UK</a></li>
<li><a href="/immigration/visitor-visas/family-visitor-visa/">Family Visitor Visa UK</a></li>
<li><a href="/immigration/visitor-visas/business-visitor-visa/">Business Visitor Visa UK</a></li>
<li><a href="/immigration/visitor-visas/child-visitor-visa/">Child Visitor Visa UK</a></li>
</ul>

<div class="highlight-box">
<h4>Contact our visitor visa solicitors today. Call <a href="tel:+441614661280">0161 466 1280</a></h4>
</div>',
                'children' => [
                    'standard-visitor-visa' => ['title' => 'Standard Visitor Visa UK', 'slug' => 'standard-visitor-visa'],
                    'marriage-visitor-visa' => ['title' => 'Marriage Visitor Visa UK', 'slug' => 'marriage-visitor-visa'],
                    'family-visitor-visa' => ['title' => 'Family Visitor Visa UK', 'slug' => 'family-visitor-visa'],
                    'business-visitor-visa' => ['title' => 'Business Visitor Visa UK', 'slug' => 'business-visitor-visa'],
                    'child-visitor-visa' => ['title' => 'Child Visitor Visa UK', 'slug' => 'child-visitor-visa'],
                ]
            ],
            // ILR
            'indefinite-leave-to-remain' => [
                'title' => 'Settle in the UK (ILR)',
                'slug' => 'indefinite-leave-to-remain',
                'template' => 'page-service.php',
                'content' => '<h1>Indefinite Leave to Remain (ILR)</h1>
<p>Indefinite Leave to Remain (ILR) commonly known as \'Settlement\' allows you to settle in the UK on a permanent basis and work in the UK without restrictions.</p>

<h2>ILR Services</h2>
<ul>
<li><a href="/immigration/indefinite-leave-to-remain/ilr-long-residence/">ILR Long Residence (10 Year Route)</a></li>
<li><a href="/immigration/indefinite-leave-to-remain/ilr-as-spouse/">ILR as a Spouse</a></li>
<li><a href="/immigration/indefinite-leave-to-remain/ilr-parent-british-child/">ILR as a Parent of British Child</a></li>
<li><a href="/immigration/indefinite-leave-to-remain/ilr-skilled-worker/">ILR as a Skilled Worker</a></li>
<li><a href="/immigration/indefinite-leave-to-remain/ilr-bereaved-partner/">ILR as a Bereaved Partner</a></li>
</ul>

<div class="highlight-box">
<h4>Contact our ILR solicitors today. Call <a href="tel:+441614661280">0161 466 1280</a></h4>
</div>',
                'children' => [
                    'ilr-long-residence' => ['title' => 'ILR Long Residence (10 Year Route)', 'slug' => 'ilr-long-residence'],
                    'ilr-as-spouse' => ['title' => 'ILR as a Spouse', 'slug' => 'ilr-as-spouse'],
                    'ilr-parent-british-child' => ['title' => 'ILR as a Parent of British Child', 'slug' => 'ilr-parent-british-child'],
                    'ilr-skilled-worker' => ['title' => 'ILR as a Skilled Worker', 'slug' => 'ilr-skilled-worker'],
                    'ilr-bereaved-partner' => ['title' => 'ILR as a Bereaved Partner', 'slug' => 'ilr-bereaved-partner'],
                ]
            ],
            // British Citizenship
            'british-citizenship' => [
                'title' => 'British Citizenship',
                'slug' => 'british-citizenship',
                'template' => 'page-service.php',
                'content' => '<h1>British Citizenship</h1>
<p>Becoming a British citizen is a significant life event. Once you become a British citizen, it will give you the right to live and work in the UK permanently.</p>

<h2>Citizenship Services</h2>
<ul>
<li><a href="/immigration/british-citizenship/naturalisation/">British Citizenship by Naturalisation</a></li>
<li><a href="/immigration/british-citizenship/by-marriage/">British Citizenship by Marriage</a></li>
<li><a href="/immigration/british-citizenship/register-child/">Register Child as a British Citizen</a></li>
<li><a href="/immigration/british-citizenship/by-registration/">British Citizenship by Registration</a></li>
<li><a href="/immigration/british-citizenship/by-descent/">British Citizenship by Descent</a></li>
</ul>

<div class="highlight-box">
<h4>Contact our citizenship solicitors today. Call <a href="tel:+441614661280">0161 466 1280</a></h4>
</div>',
                'children' => [
                    'naturalisation' => ['title' => 'British Citizenship by Naturalisation', 'slug' => 'naturalisation'],
                    'by-marriage' => ['title' => 'British Citizenship by Marriage', 'slug' => 'by-marriage'],
                    'register-child' => ['title' => 'Register Child as British Citizen', 'slug' => 'register-child'],
                    'by-registration' => ['title' => 'British Citizenship by Registration', 'slug' => 'by-registration'],
                    'by-descent' => ['title' => 'British Citizenship by Descent', 'slug' => 'by-descent'],
                ]
            ],
            // Student Visas
            'student-visas' => [
                'title' => 'Student Visas UK',
                'slug' => 'student-visas',
                'template' => 'page-service.php',
                'content' => '<h1>Student Visas UK</h1>
<p>Student visas allow you to study in the UK at accredited educational institutions.</p>

<h2>Student Visa Services</h2>
<ul>
<li><a href="/immigration/student-visas/student-visa/">Student Visa UK</a></li>
<li><a href="/immigration/student-visas/child-student-visa/">Child Student Visa UK</a></li>
<li><a href="/immigration/student-visas/student-visitor-visa/">Student Visitor Visa UK</a></li>
<li><a href="/immigration/student-visas/switching-to-student/">Switching To Student Visa UK</a></li>
<li><a href="/immigration/student-visas/extension/">Extension of UK Student Visa</a></li>
</ul>

<div class="highlight-box">
<h4>Contact our student visa solicitors today. Call <a href="tel:+441614661280">0161 466 1280</a></h4>
</div>',
                'children' => [
                    'student-visa' => ['title' => 'Student Visa UK', 'slug' => 'student-visa'],
                    'child-student-visa' => ['title' => 'Child Student Visa UK', 'slug' => 'child-student-visa'],
                    'student-visitor-visa' => ['title' => 'Student Visitor Visa UK', 'slug' => 'student-visitor-visa'],
                    'switching-to-student' => ['title' => 'Switching To Student Visa UK', 'slug' => 'switching-to-student'],
                    'extension' => ['title' => 'Extension of UK Student Visa', 'slug' => 'extension'],
                ]
            ],
            // Other Applications
            'other-applications' => [
                'title' => 'Other Immigration Applications',
                'slug' => 'other-applications',
                'template' => 'page-service.php',
                'content' => '<h1>Other Immigration Applications</h1>
<p>We handle various other immigration applications and services.</p>

<h2>Other Services</h2>
<ul>
<li><a href="/immigration/other-applications/brp-replacement/">BRP Replacement Application</a></li>
<li><a href="/immigration/other-applications/no-time-limit/">No Time Limit (NTL) Application</a></li>
<li><a href="/immigration/other-applications/subject-access-request/">Subject Access Request (SAR)</a></li>
<li><a href="/immigration/other-applications/transfer-of-conditions/">Transfer of Conditions (TOC)</a></li>
<li><a href="/immigration/other-applications/right-of-abode/">Certificate of Entitlement to Right of Abode</a></li>
</ul>

<div class="highlight-box">
<h4>Contact us today. Call <a href="tel:+441614661280">0161 466 1280</a></h4>
</div>',
                'children' => [
                    'brp-replacement' => ['title' => 'BRP Replacement Application', 'slug' => 'brp-replacement'],
                    'no-time-limit' => ['title' => 'No Time Limit (NTL) Application', 'slug' => 'no-time-limit'],
                    'subject-access-request' => ['title' => 'Subject Access Request (SAR)', 'slug' => 'subject-access-request'],
                    'transfer-of-conditions' => ['title' => 'Transfer of Conditions (TOC)', 'slug' => 'transfer-of-conditions'],
                    'right-of-abode' => ['title' => 'Certificate of Entitlement to Right of Abode', 'slug' => 'right-of-abode'],
                ]
            ],
        ]
    ],

    // Family Law
    'family-law' => [
        'title' => 'Family Law',
        'slug' => 'family-law',
        'template' => 'page-service.php',
        'content' => '<h1>Divorce & Family Law</h1>

<p>MCR Solicitors recognise the difficulties and complexities of modern society and the impact that they can have upon family life - whether it\'s because of a relationship breakdown, separation or divorce, or perhaps a dispute concerning your children.</p>

<p>You will therefore need an adviser who has detailed legal knowledge of family law and has an understanding and sympathetic approach to your case. Our team has many years of experience in dealing with family disputes and has established an excellent reputation for dealing with matters, professionally, competently and cost-effectively.</p>

<p>We will tell you where you stand legally and what you should do next. We understand the sensitive nature of family problems and everything you say to us will be in total confidence.</p>

<h2>Divorce & Family Law Services</h2>

<ul>
<li><a href="/family-law/divorce/">Getting a Divorce</a></li>
<li><a href="/family-law/financial-settlement/">Divorce Financial Settlement Solicitors</a></li>
<li><a href="/family-law/child-arrangement-order/">Child Arrangement Order</a></li>
<li><a href="/family-law/judicial-separation/">Judicial Separation</a></li>
<li><a href="/family-law/prenuptial-agreements/">Prenuptial Agreement Solicitors</a></li>
<li><a href="/family-law/non-molestation-order/">Non Molestation Order and Occupation Order</a></li>
</ul>

<h2>Need legal advice & assistance?</h2>

<p>Our highly experienced family law solicitors offer legal support in a sensitive and efficient manner to reduce the distress to you and your family. We\'re authorised and regulated by the Solicitors Regulation Authority (SRA), so you know you\'re in safe hands.</p>

<div class="highlight-box">
<h4>Contact our divorce and family law solicitors today for a free initial consultation and assessment.<br><br>You can call us on <a href="tel:+441614661280">0161 466 1280</a> or leave your details <a href="/contact-us/">here</a> for a callback request regarding your legal matters.</h4>
</div>',
        'meta_desc' => 'Fixed fee divorce & family law solicitors in Manchester. Expert legal help for divorce, child arrangements, financial settlements. SRA regulated. Free consultation available.',
        'children' => [
            'divorce' => ['title' => 'Getting a Divorce', 'slug' => 'divorce'],
            'judicial-separation' => ['title' => 'Judicial Separation', 'slug' => 'judicial-separation'],
            'financial-settlement' => ['title' => 'Divorce Financial Settlement', 'slug' => 'financial-settlement'],
            'child-arrangement-order' => ['title' => 'Child Arrangement Order', 'slug' => 'child-arrangement-order'],
            'non-molestation-order' => ['title' => 'Non Molestation & Occupation Orders', 'slug' => 'non-molestation-order'],
            'prenuptial-agreements' => ['title' => 'Prenuptial Agreements', 'slug' => 'prenuptial-agreements'],
        ]
    ],

    // Personal Injury
    'personal-injury' => [
        'title' => 'Personal Injury',
        'slug' => 'personal-injury',
        'template' => 'page-service.php',
        'content' => '<h1>Personal Injury Claims</h1>

<p>Our expert personal injury solicitors in Manchester have vast experience in handling personal injury compensation claims on behalf of people across the UK.</p>

<p>Making a personal injury compensation claim is straightforward with MCR Solicitors in Manchester. We will help you from start to finish and will remain committed to achieving the best result.</p>

<p>We can help you get the compensation you deserve. Contact our expert team of personal injury solicitors today. We will guide you every step of the way to ensure the negligent party is held responsible.</p>

<h2>Personal Injury Claims & Compensation</h2>

<ul>
<li><a href="/personal-injury/road-traffic-accidents/">Road Traffic Accident Compensation Claims</a></li>
<li><a href="/personal-injury/accident-at-work/">Accident at Work Compensation Claims</a></li>
<li><a href="/personal-injury/whiplash/">Whiplash Injury Compensation Claims</a></li>
<li><a href="/personal-injury/medical-negligence/">Medical Negligence Compensation Claims</a></li>
<li><a href="/personal-injury/slips-trips-falls/">Slips, Trips & Falls Compensation Claims</a></li>
</ul>

<h2>Looking for a Personal Injury Compensation Claim Solicitor?</h2>

<p>Making a personal injury compensation claim is simple and straightforward with MCR Solicitors. Our highly experienced personal injury solicitors in Manchester deal with all types of personal injury claims.</p>

<p>All you have to do is contact our personal injury solicitors and we will deal with all of the paperwork and correspondence, including the court proceedings (if necessary), filings and just about all aspects of making a claim. We will work hard to get the deserved compensation.</p>

<div class="highlight-box">
<h4>We\'re authorised and regulated by the Solicitors Regulation Authority (SRA), so you know you\'re in safe hands</h4>
</div>

<h2>Need Legal Assistance with Your Personal Injury Claim?</h2>

<div class="highlight-box">
<h4>Contact our personal injury solicitors today for a free initial consultation and eligibility assessment.<br><br>You can call us on <a href="tel:+441614661280">0161 466 1280</a> or leave your details <a href="/contact-us/">here</a> for a callback request regarding your personal injury claim matter.</h4>
</div>',
        'meta_desc' => 'Expert personal injury solicitors in Manchester. No win no fee. Road accidents, workplace injuries, medical negligence claims. Free consultation.',
        'children' => [
            'road-traffic-accidents' => ['title' => 'Road Traffic Accident Claims', 'slug' => 'road-traffic-accidents'],
            'accident-at-work' => ['title' => 'Accident at Work Claims', 'slug' => 'accident-at-work'],
            'whiplash' => ['title' => 'Whiplash Injury Claims', 'slug' => 'whiplash'],
            'medical-negligence' => ['title' => 'Medical Negligence Claims', 'slug' => 'medical-negligence'],
            'slips-trips-falls' => ['title' => 'Slips, Trips & Falls Claims', 'slug' => 'slips-trips-falls'],
        ]
    ],

    // Contact Pages (standalone)
    'contact-us' => [
        'title' => 'Contact Us',
        'slug' => 'contact-us',
        'template' => '',
        'content' => '<h1>Contact MCR Solicitors</h1>

<p>If you have any questions or would like to schedule a consultation, please get in touch with us using the information below or fill out the contact form.</p>

<h3>Get in Touch</h3>

<p><strong>Address:</strong><br>5 Stockport Road<br>Levenshulme<br>Manchester M19 3AB</p>

<p><strong>Phone:</strong><br><a href="tel:01614661280">0161 466 1280</a></p>

<p><strong>Email:</strong><br><a href="mailto:info@mcrsolicitors.co.uk">info@mcrsolicitors.co.uk</a></p>

<p><strong>Opening Hours:</strong><br>Monday - Friday: 9:00 AM - 5:30 PM<br>Saturday - Sunday: Closed</p>

<h3>Follow Us</h3>
<p><a href="https://www.facebook.com/share/16VB2Pi4fR/" target="_blank">Facebook</a> | <a href="https://www.tiktok.com/@mcr.solicitors" target="_blank">TikTok</a></p>

<h3>Our Location</h3>
<iframe src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d1295.8028408732268!2d-2.1920947160132207!3d53.44141823133916!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x487bb3a2d4748a47%3A0xb9f03810ea6a232!2sMCR%20Solicitors!5e0!3m2!1sen!2suk!4v1657348500786!5m2!1sen!2suk" width="100%" height="400" style="border:0; border-radius: 10px;" allowfullscreen="" loading="lazy"></iframe>

<h3>Areas We Serve</h3>
<p>MCR Solicitors serves clients throughout Greater Manchester including Manchester City Centre, Levenshulme, Longsight, Bolton, Oldham, Rochdale, Salford, Stockport, Trafford, and Wigan.</p>',
    ],

    'book-an-appointment' => [
        'title' => 'Book an Appointment',
        'slug' => 'book-an-appointment',
        'template' => '',
        'content' => '<h1>Book an Appointment</h1>

<p>Schedule a consultation with one of our experienced solicitors. We offer in-person, Zoom, Teams, Skype, WhatsApp, and phone consultations to suit your needs.</p>

<h3>Consultation Options</h3>

<p><strong>In-Person</strong><br>Visit our Manchester office</p>

<p><strong>Video Call</strong><br>Zoom, Teams, or Skype</p>

<p><strong>WhatsApp</strong><br>Convenient messaging</p>

<p><strong>Phone</strong><br>Traditional phone consultation</p>

<h3>Office Hours</h3>
<p>Monday - Friday<br>9:00 AM - 5:30 PM</p>

<h3>Phone</h3>
<p><a href="tel:01614661280" style="font-size: 24px; font-weight: bold; color: #DE532A;">0161 466 1280</a></p>',
    ],

    'meet-our-team' => [
        'title' => 'Meet Our Team',
        'slug' => 'meet-our-team',
        'template' => '',
        'content' => '<h1>Meet Our Team</h1>

<p>Our team of experienced solicitors is dedicated to providing you with the best legal advice and representation.</p>

<p>MCR Solicitors has a team of qualified professionals who specialise in Immigration Law, Family Law, and Personal Injury Claims.</p>

<div class="highlight-box">
<h4>Contact our team today for expert legal advice. Call <a href="tel:+441614661280">0161 466 1280</a></h4>
</div>',
    ],

    'our-fees' => [
        'title' => 'Our Fees',
        'slug' => 'our-fees',
        'template' => 'page-service.php',
        'content' => '<h1>Our Fees</h1>

<p>We believe in transparent pricing. Below you\'ll find information about our fee structure for various legal services.</p>

<h2>Immigration Fees</h2>
<p>For detailed immigration fee information, please visit our <a href="/our-fees/immigration-fees/">Immigration Fees</a> page.</p>

<div class="highlight-box">
<h4>Contact us for a detailed quote. Call <a href="tel:+441614661280">0161 466 1280</a></h4>
</div>',
        'children' => [
            'immigration-fees' => ['title' => 'Immigration Fees', 'slug' => 'immigration-fees'],
        ]
    ],

    'lexcel-accredited' => [
        'title' => 'Lexcel Accredited',
        'slug' => 'lexcel-accredited',
        'template' => '',
        'content' => '<h1>Lexcel Accredited</h1>

<p>MCR Solicitors have been accredited with the Law Society (England and Wales) award, Lexcel.</p>

<p>Lexcel is the Law Society\'s legal practice quality mark for excellence in legal practice management and client care. It demonstrates our commitment to providing the highest quality of service to our clients.</p>

<div class="highlight-box">
<h4>We are proud to maintain the highest standards of legal practice management.</h4>
</div>',
    ],
];

/**
 * Create pages recursively
 */
function create_pages_recursive($pages, $parent_id = 0) {
    foreach ($pages as $key => $page) {
        // Check if page exists
        $existing = get_page_by_path($page['slug']);

        if ($existing) {
            echo "Page already exists: {$page['title']} (ID: {$existing->ID})\n";
            $page_id = $existing->ID;
        } else {
            // Create the page
            $page_data = [
                'post_title' => $page['title'],
                'post_name' => $page['slug'],
                'post_content' => $page['content'] ?? '',
                'post_status' => 'publish',
                'post_type' => 'page',
                'post_parent' => $parent_id,
            ];

            $page_id = wp_insert_post($page_data);

            if (is_wp_error($page_id)) {
                echo "Error creating page {$page['title']}: " . $page_id->get_error_message() . "\n";
                continue;
            }

            echo "Created page: {$page['title']} (ID: {$page_id})\n";
        }

        // Set page template if specified
        if (!empty($page['template'])) {
            update_post_meta($page_id, '_wp_page_template', $page['template']);
        }

        // Set Yoast SEO meta description if available
        if (!empty($page['meta_desc'])) {
            update_post_meta($page_id, '_yoast_wpseo_metadesc', $page['meta_desc']);
        }

        // Create child pages if any
        if (!empty($page['children'])) {
            create_pages_recursive($page['children'], $page_id);
        }
    }
}

// Run the page creation
echo "Starting page creation...\n\n";
create_pages_recursive($pages);
echo "\nPage creation complete!\n";
