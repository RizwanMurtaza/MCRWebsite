<?php
/**
 * Update homepage content with Meet Our Team section
 * Run via: wp eval-file update-homepage-team.php --allow-root
 */

$homepage_content = '<!-- Hero Section with Contact Form -->
<div class="mcr-hero-section" style="background: linear-gradient(rgba(0,0,0,0.4), rgba(0,0,0,0.4)), url(\'/wp-content/uploads/2025/12/welcome.jpg\'); background-size: cover; background-position: center; min-height: 600px; display: flex; align-items: center; padding: 60px 40px;">
    <div style="max-width: 1200px; margin: 0 auto; display: flex; gap: 40px; flex-wrap: wrap; align-items: center; width: 100%;">
        <div style="flex: 1; min-width: 300px; color: white;">
            <h1 style="color: #fff; font-size: 48px; font-weight: 700; margin-bottom: 20px; line-height: 1.2;">Multi-Service Law Firm in Manchester, UK</h1>
            <p style="font-size: 18px; line-height: 1.6; margin-bottom: 30px; color: #fff;">MCR Solicitors provides expert legal services in UK Immigration, Family Law, Personal Injury, and more. Our Manchester-based team of qualified solicitors is here to help you navigate complex legal matters with professionalism and care.</p>
            <a href="/book-an-appointment/" style="padding: 15px 30px; border-radius: 5px; font-size: 18px; font-weight: 600; text-decoration: none; background-color: #DE532A; color: white; display: inline-block;">Book an Appointment</a>
        </div>
        <div class="mcr-hero-form" style="flex: 0 0 400px; background: rgba(255,255,255,0.95); padding: 30px; border-radius: 10px;">
            <h3 style="color: #DE532A; font-size: 24px; margin: 0 0 20px 0; text-align: center;">Get in Touch</h3>
            [mcr_enquiry_form]
        </div>
    </div>
</div>

<!-- Services Section -->
<div style="max-width: 1200px; margin: 0 auto; padding: 60px 20px;">
    <h2 style="text-align: center; color: #DE532A; margin-bottom: 40px; font-size: 32px;">Our Legal Services</h2>
    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 30px; margin-bottom: 60px;">
        <div style="background: #fff; border: 1px solid #eee; border-radius: 10px; padding: 30px; transition: all 0.3s ease;">
            <h3 style="color: #DE532A; margin-bottom: 15px; font-size: 22px;">UK Visas &amp; Immigration</h3>
            <p style="color: #666; line-height: 1.7; margin-bottom: 20px;">We provide a wide range of UK visas &amp; immigration services to businesses and individuals including UK work visas, UK visitor visas, family visas, student visas, Indefinite Leave to Remain (ILR) and British citizenship.</p>
            <a href="/immigration/" style="color: #DE532A; font-weight: 600; text-decoration: none;">Learn More →</a>
        </div>
        <div style="background: #fff; border: 1px solid #eee; border-radius: 10px; padding: 30px; transition: all 0.3s ease;">
            <h3 style="color: #DE532A; margin-bottom: 15px; font-size: 22px;">Divorce &amp; Family Law</h3>
            <p style="color: #666; line-height: 1.7; margin-bottom: 20px;">MCR Solicitors recognise the difficulties and complexities of modern society and the impact that they can have upon family life, whether it\'s a relationship breakdown, separation, divorce, or a dispute concerning your children.</p>
            <a href="/family-law/" style="color: #DE532A; font-weight: 600; text-decoration: none;">Learn More →</a>
        </div>
        <div style="background: #fff; border: 1px solid #eee; border-radius: 10px; padding: 30px; transition: all 0.3s ease;">
            <h3 style="color: #DE532A; margin-bottom: 15px; font-size: 22px;">Personal Injury Claims</h3>
            <p style="color: #666; line-height: 1.7; margin-bottom: 20px;">Our expert personal injury solicitors in Manchester have vast experience in handling personal injury compensation claims on behalf of people across the UK. We will help you from start to finish.</p>
            <a href="/personal-injury/" style="color: #DE532A; font-weight: 600; text-decoration: none;">Learn More →</a>
        </div>
    </div>
</div>

<!-- Meet Our Team Section -->
<div style="background: #FFF6F3; padding: 60px 20px;">
    <div style="max-width: 1200px; margin: 0 auto;">
        <h2 style="text-align: center; color: #DE532A; margin-bottom: 40px; font-size: 32px;">Meet Our Team</h2>
        <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 25px;">

            <div style="background: #fff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); transition: transform 0.3s ease;">
                <div style="position: relative;">
                    <img src="/wp-content/uploads/2025/12/Atif.jpg" alt="Atif Abbas - Director/Solicitor" style="width: 100%; height: 250px; object-fit: cover;">
                    <div style="position: absolute; bottom: 0; left: 0; right: 0; background: linear-gradient(transparent, rgba(0,0,0,0.8)); padding: 20px 15px 15px; color: white;">
                        <h4 style="margin: 0 0 5px 0; font-size: 18px; color: #fff;">Atif Abbas</h4>
                        <p style="margin: 0; font-size: 14px; color: #ddd;">Director / Solicitor</p>
                    </div>
                </div>
            </div>

            <div style="background: #fff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); transition: transform 0.3s ease;">
                <div style="position: relative;">
                    <img src="/wp-content/uploads/2025/12/team1.jpg" alt="Ayesha Aslam - Director/Solicitor" style="width: 100%; height: 250px; object-fit: cover;">
                    <div style="position: absolute; bottom: 0; left: 0; right: 0; background: linear-gradient(transparent, rgba(0,0,0,0.8)); padding: 20px 15px 15px; color: white;">
                        <h4 style="margin: 0 0 5px 0; font-size: 18px; color: #fff;">Ayesha Aslam</h4>
                        <p style="margin: 0; font-size: 14px; color: #ddd;">Director / Solicitor</p>
                    </div>
                </div>
            </div>

            <div style="background: #fff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); transition: transform 0.3s ease;">
                <div style="position: relative;">
                    <img src="/wp-content/uploads/2025/12/team5.jpg" alt="Sajjad Mahmood - Solicitor/Advocate" style="width: 100%; height: 250px; object-fit: cover;">
                    <div style="position: absolute; bottom: 0; left: 0; right: 0; background: linear-gradient(transparent, rgba(0,0,0,0.8)); padding: 20px 15px 15px; color: white;">
                        <h4 style="margin: 0 0 5px 0; font-size: 18px; color: #fff;">Sajjad Mahmood</h4>
                        <p style="margin: 0; font-size: 14px; color: #ddd;">Solicitor / Advocate (HRA)</p>
                    </div>
                </div>
            </div>

            <div style="background: #fff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); transition: transform 0.3s ease;">
                <div style="position: relative;">
                    <img src="/wp-content/uploads/2025/12/team6.jpg" alt="Asad Malik - Solicitor" style="width: 100%; height: 250px; object-fit: cover;">
                    <div style="position: absolute; bottom: 0; left: 0; right: 0; background: linear-gradient(transparent, rgba(0,0,0,0.8)); padding: 20px 15px 15px; color: white;">
                        <h4 style="margin: 0 0 5px 0; font-size: 18px; color: #fff;">Asad Malik</h4>
                        <p style="margin: 0; font-size: 14px; color: #ddd;">Solicitor</p>
                    </div>
                </div>
            </div>

            <div style="background: #fff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); transition: transform 0.3s ease;">
                <div style="position: relative;">
                    <img src="/wp-content/uploads/2025/12/team4.jpg" alt="Shakra Azhar - Fee Earner" style="width: 100%; height: 250px; object-fit: cover;">
                    <div style="position: absolute; bottom: 0; left: 0; right: 0; background: linear-gradient(transparent, rgba(0,0,0,0.8)); padding: 20px 15px 15px; color: white;">
                        <h4 style="margin: 0 0 5px 0; font-size: 18px; color: #fff;">Shakra Azhar</h4>
                        <p style="margin: 0; font-size: 14px; color: #ddd;">Fee Earner</p>
                    </div>
                </div>
            </div>

            <div style="background: #fff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); transition: transform 0.3s ease;">
                <div style="position: relative;">
                    <img src="/wp-content/uploads/2025/12/team3.jpg" alt="Abdul Rehman - Case Worker" style="width: 100%; height: 250px; object-fit: cover;">
                    <div style="position: absolute; bottom: 0; left: 0; right: 0; background: linear-gradient(transparent, rgba(0,0,0,0.8)); padding: 20px 15px 15px; color: white;">
                        <h4 style="margin: 0 0 5px 0; font-size: 18px; color: #fff;">Abdul Rehman</h4>
                        <p style="margin: 0; font-size: 14px; color: #ddd;">Case Worker</p>
                    </div>
                </div>
            </div>

            <div style="background: #fff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); transition: transform 0.3s ease;">
                <div style="position: relative;">
                    <img src="/wp-content/uploads/2025/12/team2.jpg" alt="Hassan Imtisal - Accounts Manager" style="width: 100%; height: 250px; object-fit: cover;">
                    <div style="position: absolute; bottom: 0; left: 0; right: 0; background: linear-gradient(transparent, rgba(0,0,0,0.8)); padding: 20px 15px 15px; color: white;">
                        <h4 style="margin: 0 0 5px 0; font-size: 18px; color: #fff;">Hassan Imtisal</h4>
                        <p style="margin: 0; font-size: 14px; color: #ddd;">Accounts Manager</p>
                    </div>
                </div>
            </div>

        </div>
        <div style="text-align: center; margin-top: 30px;">
            <a href="/meet-our-team/" style="display: inline-block; padding: 12px 30px; background: #DE532A; color: white; text-decoration: none; border-radius: 5px; font-weight: 600;">View Full Team</a>
        </div>
    </div>
</div>

<!-- Google Reviews -->
<div style="background: linear-gradient(135deg, #f5f5f5 0%, #ffffff 100%); border-radius: 15px; padding: 30px; max-width: 600px; margin: 60px auto; box-shadow: 0 4px 20px rgba(0,0,0,0.1); text-align: center;">
    <div style="display: flex; align-items: center; justify-content: center; flex-wrap: wrap; gap: 20px;">
        <div style="display: flex; align-items: center; gap: 10px;">
            <svg width="24" height="24" viewBox="0 0 48 48"><path fill="#4285F4" d="M45.12 24.5c0-1.56-.14-3.06-.4-4.5H24v8.51h11.84c-.51 2.75-2.06 5.08-4.39 6.64v5.52h7.11c4.16-3.83 6.56-9.47 6.56-16.17z"/><path fill="#34A853" d="M24 46c5.94 0 10.92-1.97 14.56-5.33l-7.11-5.52c-1.97 1.32-4.49 2.1-7.45 2.1-5.73 0-10.58-3.87-12.31-9.07H4.34v5.7C7.96 41.07 15.4 46 24 46z"/><path fill="#FBBC05" d="M11.69 28.18C11.25 26.86 11 25.45 11 24s.25-2.86.69-4.18v-5.7H4.34C2.85 17.09 2 20.45 2 24c0 3.55.85 6.91 2.34 9.88l7.35-5.7z"/><path fill="#EA4335" d="M24 10.75c3.23 0 6.13 1.11 8.41 3.29l6.31-6.31C34.91 4.18 29.93 2 24 2 15.4 2 7.96 6.93 4.34 14.12l7.35 5.7c1.73-5.2 6.58-9.07 12.31-9.07z"/></svg>
            <span style="font-size: 18px; color: #5f6368; font-weight: 500;">Google Reviews</span>
        </div>
        <div style="display: flex; align-items: center; gap: 12px;">
            <div style="color: #fbbc04; font-size: 28px;">★★★★★</div>
            <div>
                <div style="font-size: 24px; font-weight: bold; color: #202124;">5.0</div>
                <div style="font-size: 14px; color: #333;">174 reviews</div>
            </div>
        </div>
    </div>
    <div style="margin-top: 15px;">
        <a href="https://www.google.com/maps/place/MCR+Solicitors" target="_blank" style="display: inline-block; padding: 10px 20px; background: #4285f4; color: white; text-decoration: none; border-radius: 5px; font-size: 14px;">View All Reviews on Google →</a>
    </div>
</div>

<!-- Why Choose Us -->
<div style="max-width: 1200px; margin: 0 auto; padding: 0 20px 60px;">
    <h2 style="text-align: center; color: #DE532A; margin-bottom: 40px; font-size: 32px;">Why Choose MCR Solicitors?</h2>
    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 20px; text-align: center;">
        <div style="padding: 30px; border: 1px solid #eee; border-radius: 10px;">
            <h4 style="color: #333; margin-bottom: 10px; font-size: 18px;">SRA Regulated</h4>
            <p style="color: #666; font-size: 14px;">Authorised and regulated by the Solicitors Regulation Authority</p>
        </div>
        <div style="padding: 30px; border: 1px solid #eee; border-radius: 10px;">
            <h4 style="color: #333; margin-bottom: 10px; font-size: 18px;">Lexcel Accredited</h4>
            <p style="color: #666; font-size: 14px;">Law Society Practice Management Standard</p>
        </div>
        <div style="padding: 30px; border: 1px solid #eee; border-radius: 10px;">
            <h4 style="color: #333; margin-bottom: 10px; font-size: 18px;">5-Star Reviews</h4>
            <p style="color: #666; font-size: 14px;">174+ Google Reviews with 5.0 rating</p>
        </div>
        <div style="padding: 30px; border: 1px solid #eee; border-radius: 10px;">
            <h4 style="color: #333; margin-bottom: 10px; font-size: 18px;">Free Consultation</h4>
            <p style="color: #666; font-size: 14px;">Initial consultation to discuss your case</p>
        </div>
    </div>
</div>

<!-- Contact Section -->
<div style="max-width: 1200px; margin: 0 auto; padding: 0 20px 60px;">
    <h2 style="text-align: center; color: #DE532A; margin-bottom: 40px; font-size: 32px;">Contact Us</h2>
    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 40px;">
        <div>
            <p style="margin-bottom: 20px;"><strong style="color: #DE532A;">Address:</strong><br>5 Stockport Road, Levenshulme<br>Manchester M19 3AB</p>
            <p style="margin-bottom: 20px;"><strong style="color: #DE532A;">Phone:</strong><br><a href="tel:01614661280" style="color: #333; text-decoration: none; font-size: 20px; font-weight: 600;">0161 466 1280</a></p>
            <p style="margin-bottom: 20px;"><strong style="color: #DE532A;">Email:</strong><br><a href="mailto:info@mcrsolicitors.co.uk" style="color: #333; text-decoration: none;">info@mcrsolicitors.co.uk</a></p>
            <p style="margin-bottom: 20px;"><strong style="color: #DE532A;">Opening Hours:</strong><br>Monday - Friday: 9:00 AM - 5:30 PM<br>Saturday - Sunday: Closed</p>
        </div>
        <div>
            <iframe src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d1295.8028408732268!2d-2.1920947160132207!3d53.44141823133916!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x487bb3a2d4748a47%3A0xb9f03810ea6a232!2sMCR%20Solicitors!5e0!3m2!1sen!2suk!4v1657348500786!5m2!1sen!2suk" width="100%" height="350" style="border:0; border-radius: 10px;" allowfullscreen="" loading="lazy"></iframe>
        </div>
    </div>
</div>';

// Update homepage directly by ID
$result = wp_update_post([
    'ID' => 6,
    'post_content' => $homepage_content
]);

if (is_wp_error($result)) {
    echo "Error: " . $result->get_error_message() . "\n";
} elseif ($result) {
    echo "Homepage (ID: 6) updated with Meet Our Team section!\n";
} else {
    echo "Failed to update homepage.\n";
}
