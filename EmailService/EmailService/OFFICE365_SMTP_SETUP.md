# Office 365 SMTP Authentication Setup Guide

## Error: 5.7.139 Authentication unsuccessful

This error occurs when Office 365 blocks SMTP authentication. Follow these steps to resolve it:

## Solution 1: Enable SMTP AUTH (Most Common Fix)

### For Individual Mailbox (PowerShell):
```powershell
# Connect to Exchange Online
Install-Module -Name ExchangeOnlineManagement
Connect-ExchangeOnline -UserPrincipalName admin@yourdomain.com

# Enable SMTP AUTH for specific mailbox
Set-CASMailbox -Identity user@yourdomain.com -SmtpClientAuthenticationDisabled $false

# Verify the setting
Get-CASMailbox -Identity user@yourdomain.com | Format-List SmtpClientAuthenticationDisabled
```

### For Entire Organization (Admin Center):
1. Go to Microsoft 365 Admin Center (https://admin.microsoft.com)
2. Navigate to Settings → Org Settings → Security & Privacy
3. Click on "Modern Authentication"
4. Check "Turn on modern authentication for Outlook 2013 for Windows and later"
5. Check "Allow Authenticated SMTP"
6. Save changes

### Alternative Admin Center Method:
1. Go to Exchange Admin Center (https://admin.exchange.microsoft.com)
2. Navigate to Settings → Mail flow
3. Click "Manage mail flow settings"
4. Under "SMTP AUTH protocol", enable "Turn on SMTP AUTH for specific users"

## Solution 2: Disable Security Defaults

If your organization has Security Defaults enabled, it blocks legacy authentication:

1. Go to Azure Active Directory admin center (https://aad.portal.azure.com)
2. Navigate to Azure Active Directory → Properties
3. Click "Manage security defaults"
4. Set "Enable security defaults" to "No"
5. Save changes

⚠️ **Warning**: Disabling security defaults reduces security. Consider using App Passwords instead.

## Solution 3: Use App Password (Recommended for MFA)

If Multi-Factor Authentication (MFA) is enabled:

1. Go to https://mysignins.microsoft.com/security-info
2. Sign in with your Office 365 account
3. Click "+ Add sign-in method"
4. Choose "App password"
5. Give it a name (e.g., "Email Service SMTP")
6. Copy the generated password
7. Use this password instead of your regular password in the SMTP settings

## Solution 4: Check License Requirements

Ensure the mailbox has proper licensing:
- Exchange Online Plan 1 or higher
- Microsoft 365 Business Basic or higher
- Office 365 E1 or higher

## Solution 5: Conditional Access Policies

Check if Conditional Access policies are blocking:

1. Azure AD Admin Center → Security → Conditional Access
2. Review policies that might block SMTP
3. Create an exclusion for the service account if necessary

## SMTP Settings Checklist

✅ **Server**: smtp.office365.com
✅ **Port**: 587
✅ **Encryption**: STARTTLS/TLS
✅ **Username**: Full email address (user@domain.com)
✅ **Password**: Regular password OR App password (if MFA enabled)
✅ **From Address**: Must match the authenticated username

## PowerShell Diagnostic Commands

```powershell
# Check SMTP AUTH status for a user
Get-CASMailbox -Identity user@yourdomain.com | FL SmtpClientAuthenticationDisabled

# Check organization-wide SMTP AUTH
Get-TransportConfig | Format-List SmtpClientAuthenticationDisabled

# Enable SMTP AUTH organization-wide (use with caution)
Set-TransportConfig -SmtpClientAuthenticationDisabled $false

# Check authentication policy
Get-AuthenticationPolicy | FL AllowBasicAuth*

# View user's authentication methods
Get-User -Identity user@yourdomain.com | FL Auth*
```

## Testing SMTP Connection

Use this PowerShell script to test SMTP:

```powershell
$EmailFrom = "sender@yourdomain.com"
$EmailTo = "recipient@yourdomain.com"
$Subject = "Test Email"
$Body = "This is a test email"
$SMTPServer = "smtp.office365.com"
$SMTPPort = 587
$Username = "sender@yourdomain.com"
$Password = ConvertTo-SecureString "YourPasswordHere" -AsPlainText -Force

$SMTPClient = New-Object System.Net.Mail.SmtpClient($SMTPServer, $SMTPPort)
$SMTPClient.EnableSsl = $true
$SMTPClient.Credentials = New-Object System.Net.NetworkCredential($Username, $Password)

$Message = New-Object System.Net.Mail.MailMessage($EmailFrom, $EmailTo, $Subject, $Body)

try {
    $SMTPClient.Send($Message)
    Write-Host "Email sent successfully!" -ForegroundColor Green
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}
```

## Common Error Codes and Solutions

| Error Code | Description | Solution |
|------------|-------------|----------|
| 5.7.139 | Authentication unsuccessful | Enable SMTP AUTH |
| 5.7.57 | Client not authenticated | Check username/password |
| 5.7.3 | Cannot relay | Verify sender address matches auth |
| 535 5.7.3 | Authentication failed | Wrong password or MFA blocking |
| 5.2.0 | Submission quota exceeded | Rate limit hit, wait and retry |

## Alternative: Use Microsoft Graph API

For modern applications, Microsoft recommends using Graph API instead of SMTP:
- OAuth 2.0 authentication
- Better security
- No need for SMTP AUTH
- RESTful API

## Need Help?

1. Check SMTP AUTH status first
2. Verify MFA settings
3. Review Conditional Access policies
4. Check mailbox licensing
5. Contact your Microsoft 365 administrator

## References
- [Microsoft Documentation](https://learn.microsoft.com/en-us/exchange/clients-and-mobile-in-exchange-online/authenticated-client-smtp-submission)
- [Enable SMTP AUTH](https://learn.microsoft.com/en-us/exchange/clients-and-mobile-in-exchange-online/enable-or-disable-authenticated-client-smtp-submission)