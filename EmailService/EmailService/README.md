# Email Service API

A .NET 9 Web API for sending emails with MySQL logging and app-based credentials management.

## Features

- **App-based Email Configuration**: Each app has its own SMTP credentials
- **Email Logging**: All sent emails are logged to MySQL database
- **Contact Us Forms**: Dedicated endpoint for contact form submissions
- **Service Enquiries**: Specialized endpoint for service enquiry forms
- **Custom Email Sending**: Generic endpoint for custom email types
- **MySQL Database**: Stores email logs and app credentials

## API Endpoints

### Email Endpoints

#### 1. Contact Us Email
```
POST /api/email/contact-us
```

**Request Body:**
```json
{
  "appId": "mcr-solicitors-web",
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "+44 123 456 7890",
  "subject": "Legal Consultation Request",
  "message": "I need help with immigration law..."
}
```

#### 2. Service Enquiry Email
```
POST /api/email/enquiry
```

**Request Body:**
```json
{
  "appId": "mcr-solicitors-web",
  "name": "Jane Smith",
  "email": "jane@example.com",
  "phone": "+44 987 654 3210",
  "serviceType": "Family Law",
  "enquiry": "I need assistance with divorce proceedings...",
  "preferredContactMethod": "Email",
  "preferredContactTime": "2024-01-15T10:00:00Z"
}
```

#### 3. Custom Email
```
POST /api/email/send
```

**Request Body:**
```json
{
  "appId": "mcr-solicitors-web",
  "toEmail": "recipient@example.com",
  "subject": "Custom Subject",
  "body": "<h1>Custom HTML Body</h1>",
  "emailType": "Newsletter",
  "metadata": "{\"source\": \"newsletter_signup\"}"
}
```

### Credentials Management Endpoints

#### 1. Get All Credentials
```
GET /api/credentials
```

#### 2. Get Credentials by App ID
```
GET /api/credentials/{appId}
```

#### 3. Create New Credentials
```
POST /api/credentials
```

**Request Body:**
```json
{
  "appId": "mcr-solicitors-web",
  "appName": "MCR Solicitors Website",
  "smtpHost": "smtp.gmail.com",
  "smtpPort": 587,
  "smtpUsername": "noreply@mcrsolicitors.co.uk",
  "smtpPassword": "your-app-password",
  "fromEmail": "noreply@mcrsolicitors.co.uk",
  "fromName": "MCR Solicitors",
  "enableSsl": true,
  "isActive": true
}
```

#### 4. Update Credentials
```
PUT /api/credentials/{appId}
```

#### 5. Delete Credentials
```
DELETE /api/credentials/{appId}
```

## Database Schema

### EmailLogs Table
- `Id`: Primary key
- `AppId`: Application identifier
- `ToEmail`: Recipient email
- `FromEmail`: Sender email
- `Subject`: Email subject
- `Body`: Email body (HTML)
- `EmailType`: Type of email (ContactUs, Enquiry, etc.)
- `IsHtml`: Whether body is HTML
- `Status`: Email status (Sent, Failed, Pending)
- `ErrorMessage`: Error details if failed
- `SentAt`: When email was sent
- `CreatedAt`: When record was created
- `Metadata`: Additional data as JSON

### AppCredentials Table
- `Id`: Primary key
- `AppId`: Unique application identifier
- `AppName`: Display name for the app
- `SmtpHost`: SMTP server host
- `SmtpPort`: SMTP server port
- `SmtpUsername`: SMTP username
- `SmtpPassword`: SMTP password
- `FromEmail`: Default sender email
- `FromName`: Default sender name
- `EnableSsl`: Whether to use SSL
- `IsActive`: Whether credentials are active
- `CreatedAt`: Creation timestamp
- `UpdatedAt`: Last update timestamp

## Configuration

### Database Connection
Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EmailServiceDb;User=root;Password=your-password;"
  }
}
```

### Running the Service

1. **Start MySQL server**
2. **Update connection string** in appsettings.json
3. **Run the service**:
   ```bash
   cd EmailService
   dotnet run
   ```

The database will be created automatically on first run.

## Integration with MCR Solicitors Website

To integrate with your website, update your contact forms to POST to these endpoints:

### Contact Us Form Integration
```javascript
const contactFormData = {
    appId: 'mcr-solicitors-web',
    name: document.getElementById('name').value,
    email: document.getElementById('email').value,
    phone: document.getElementById('phone').value,
    subject: document.getElementById('subject').value,
    message: document.getElementById('message').value
};

fetch('http://localhost:5000/api/email/contact-us', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify(contactFormData)
})
.then(response => response.json())
.then(data => {
    if (data.success) {
        alert('Message sent successfully!');
    } else {
        alert('Failed to send message.');
    }
});
```

### Service Enquiry Form Integration
```javascript
const enquiryFormData = {
    appId: 'mcr-solicitors-web',
    name: document.getElementById('name').value,
    email: document.getElementById('email').value,
    phone: document.getElementById('phone').value,
    serviceType: document.getElementById('service').value,
    enquiry: document.getElementById('enquiry').value,
    preferredContactMethod: document.getElementById('contact-method').value
};

fetch('http://localhost:5000/api/email/enquiry', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify(enquiryFormData)
})
.then(response => response.json())
.then(data => {
    if (data.success) {
        alert('Enquiry submitted successfully!');
    } else {
        alert('Failed to submit enquiry.');
    }
});
```

## Security Notes

- Store SMTP passwords securely
- Use HTTPS in production
- Consider rate limiting for email endpoints
- Validate and sanitize all input data
- Use environment variables for sensitive configuration