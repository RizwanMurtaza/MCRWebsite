#!/bin/bash

# Create main pages
wp post create --post_type=page --post_title="Immigration" --post_status=publish --post_content="<h1>UK Immigration Solicitors in Manchester</h1><p>We at MCR Solicitors provide a wide range of UK visas & immigration services to businesses and individuals.</p>" --allow-root
wp post create --post_type=page --post_title="Family Law" --post_status=publish --post_content="<h1>Family Law Solicitors in Manchester</h1><p>MCR Solicitors recognise the difficulties and complexities of modern society and the impact that they can have upon family life.</p>" --allow-root
wp post create --post_type=page --post_title="Personal Injury" --post_status=publish --post_content="<h1>Personal Injury Solicitors in Manchester</h1><p>Our expert personal injury solicitors have vast experience in handling personal injury compensation claims.</p>" --allow-root
wp post create --post_type=page --post_title="Contact Us" --post_status=publish --post_content="<h1>Contact MCR Solicitors</h1><p>Get in touch with our team.</p>" --allow-root
wp post create --post_type=page --post_title="Book an Appointment" --post_status=publish --post_content="<h1>Book an Appointment</h1><p>Schedule a consultation with our solicitors.</p>" --allow-root
wp post create --post_type=page --post_title="Meet Our Team" --post_status=publish --post_content="<h1>Meet Our Legal Team</h1><p>Our experienced team of solicitors and legal professionals.</p>" --allow-root
wp post create --post_type=page --post_title="Our Fees" --post_status=publish --post_content="<h1>Our Fees</h1><p>Transparent pricing for our legal services.</p>" --allow-root

# Immigration sub-pages
wp post create --post_type=page --post_title="UK Family Visas" --post_status=publish --allow-root
wp post create --post_type=page --post_title="UK Work Visas" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Business Visas UK" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Visitor Visas UK" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Settle in the UK (ILR)" --post_status=publish --allow-root
wp post create --post_type=page --post_title="British Citizenship" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Student Visas UK" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Other Immigration Applications" --post_status=publish --allow-root

# Family Law sub-pages
wp post create --post_type=page --post_title="Divorce" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Judicial Separation" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Divorce Financial Settlement" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Child Arrangement Order" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Non-Molestation Order" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Prenuptial Agreements" --post_status=publish --allow-root

# Personal Injury sub-pages
wp post create --post_type=page --post_title="Road Traffic Accidents" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Accident at Work" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Whiplash Claims" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Medical Negligence" --post_status=publish --allow-root
wp post create --post_type=page --post_title="Slips, Trips and Falls" --post_status=publish --allow-root

echo "Pages created successfully!"
