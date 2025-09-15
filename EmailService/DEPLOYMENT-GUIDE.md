# MCR Solicitors Email Service - Deployment Guide

## 🚀 Production Deployment

The EmailService is now configured to work with the production environment at `https://emailservice.pricesnap.co.uk/`.

### **Production URLs:**
- **API Base**: `https://emailservice.pricesnap.co.uk/api`
- **Admin Panel**: `https://emailservice.pricesnap.co.uk/admin.html`
- **Root URL**: `https://emailservice.pricesnap.co.uk/` (redirects to admin)

### **Website Integration Status:**
✅ **Updated Files:**
- `js/email-config.js` - Production configuration
- `js/email-integration.js` - Email service integration
- `contact-us.html` - Contact form integration
- `book-an-appointment.html` - Appointment form integration
- `index.html` - Homepage form integration

## 📋 Deployment Steps

### **1. Deploy EmailService to Production**

**Option A: Manual Deployment**
```bash
# Build for production
cd EmailService/EmailService
dotnet publish -c Release -o ./publish

# Copy publish folder to production server
# Update connection string in appsettings.json for production database
```

**Option B: Docker Deployment**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY publish/ .
EXPOSE 80
ENTRYPOINT ["dotnet", "EmailService.dll"]
```

### **2. Configure Production Database**

**MySQL Connection String:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_MYSQL_HOST;Database=EmailServiceDb;User=YOUR_USER;Password=YOUR_PASSWORD;"
  }
}
```

### **3. Set Up Email Credentials**

Access the admin panel at `https://emailservice.pricesnap.co.uk/admin.html` and create credentials:

```json
{
  "appId": "mcr-solicitors-web",
  "appName": "MCR Solicitors Website",
  "smtpHost": "smtp.gmail.com",
  "smtpPort": 587,
  "smtpUsername": "noreply@mcrsolicitors.co.uk",
  "smtpPassword": "YOUR_GMAIL_APP_PASSWORD",
  "fromEmail": "noreply@mcrsolicitors.co.uk",
  "fromName": "MCR Solicitors",
  "enableSsl": true,
  "isActive": true
}
```

### **4. Configure HTTPS and SSL**

Ensure your production server has:
- ✅ SSL certificate for `emailservice.pricesnap.co.uk`
- ✅ HTTPS redirect enabled
- ✅ CORS configured for your website domain

### **5. Environment Variables**

Set these in production:
```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=https://+:443;http://+:80
```

## 🔧 Website Configuration

### **For Development/Testing:**
To test with local EmailService, update `js/email-config.js`:
```javascript
// Uncomment these lines for local development
// window.EmailServiceConfig.API_BASE = 'http://localhost:5000/api';
// window.EmailServiceConfig.ADMIN_URL = 'http://localhost:5000/admin.html';
// window.EmailServiceConfig.ENVIRONMENT = 'development';
```

### **For Production:**
The default configuration in `js/email-config.js` is already set for production.

## 📊 Monitoring and Logs

### **Database Tables:**
- `AppCredentials` - Email service configurations
- `EmailLogs` - All email attempts with status and error details

### **API Endpoints:**
- `GET /api/credentials` - List all credentials
- `POST /api/email/contact-us` - Contact form submissions
- `POST /api/email/enquiry` - Service enquiry submissions

### **Health Check:**
```bash
curl https://emailservice.pricesnap.co.uk/api/credentials
```

## 🚨 Security Considerations

1. **SMTP Passwords**: Store securely, use app passwords for Gmail
2. **Database**: Use strong passwords and restrict access
3. **CORS**: Configure to only allow requests from your website domain
4. **Rate Limiting**: Consider implementing rate limiting for form submissions
5. **Input Validation**: All form inputs are validated on both client and server

## 📧 Email Templates

The service generates professional HTML email templates with:
- **Contact Us**: Professional layout with company branding
- **Service Enquiry**: Appointment-focused template with scheduling details
- **Error Handling**: Automatic logging and user feedback

## 🔄 Form Integration

### **Add to Any Page:**
```html
<!-- Before closing </body> tag -->
<script src="js/email-config.js"></script>
<script src="js/email-integration.js"></script>
```

### **Form Requirements:**
- Use `id="consultationForm"` for contact forms
- Use `id="appointmentForm"` for enquiry forms
- Use `class="mcr-contact-form"` for generic forms

## ✅ Testing Checklist

- [ ] EmailService deployed and accessible at production URL
- [ ] Admin panel loads correctly
- [ ] Database connection working
- [ ] Email credentials configured
- [ ] Contact forms submit successfully
- [ ] Enquiry forms submit successfully
- [ ] Email delivery working
- [ ] Error handling working
- [ ] HTTPS enabled and working
- [ ] CORS configured for website domain

## 📞 Support

If you encounter issues:
1. Check browser console for JavaScript errors
2. Verify API endpoint accessibility
3. Check database connection and credentials
4. Review email service logs
5. Test with curl commands for debugging

The EmailService is ready for production use! 🎉