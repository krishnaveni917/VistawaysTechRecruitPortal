using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(_settings.Mail) &&
            !string.IsNullOrWhiteSpace(_settings.Password) &&
            !_settings.Password.Equals("YOUR_APP_PASSWORD", StringComparison.OrdinalIgnoreCase);

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (!IsConfigured)
            {
                throw new InvalidOperationException(
                    "Email is not configured. Set EmailSettings:Mail and EmailSettings:Password to a valid SMTP account. " +
                    "For Gmail, use an App Password, not your regular Gmail password.");
            }

            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_settings.DisplayName, _settings.Mail));
            email.To.Add(MailboxAddress.Parse(to));

            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);

            // If not using OAuth2, ensure XOAUTH2 mechanism is not attempted by MailKit
            // (Gmail will reject plain credentials when using 2FA; use an App Password or OAuth2).
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");

            try
            {
                await smtp.AuthenticateAsync(_settings.Mail, _settings.Password);
            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                // Surface a clearer, actionable error for authentication failures.
                throw new InvalidOperationException(
                    "SMTP authentication failed. Verify the configured SMTP username and password. " +
                    "If using Gmail, ensure you use an App Password when 2-Step Verification is enabled, " +
                    "or configure OAuth2. See https://support.google.com/mail/?p=BadCredentials.",
                    ex);
            }

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}
