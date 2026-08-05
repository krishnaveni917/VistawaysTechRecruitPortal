using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Services
{
    public class CredentialEmailService
    {
        private readonly EmailService _emailService;

        public CredentialEmailService(EmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendCredentialsEmailAsync(string email, string candidateId, string password, string firstName)
        {
            string subject = "VistaWays Tech - Your Login Credentials";
            string body = $@"
                <h2>Welcome to VistaWays Tech, {firstName}!</h2>
                <p>Your candidate profile has been successfully created. Below are your login credentials:</p>
                
                <div style='background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                    <p><strong>Candidate ID:</strong> {candidateId}</p>
                    <p><strong>Password:</strong> {password}</p>
                    <p><strong>Email:</strong> {email}</p>
                </div>
                
                <p><strong>Important Security Information:</strong></p>
                <ul>
                    <li>Your Candidate ID is your unique identifier for all VistaWays Tech communications</li>
                    <li>Please change your password after your first login for security</li>
                    <li>Keep these credentials safe and do not share them with anyone</li>
                    <li>Your date of birth and phone number are on file for identity verification</li>
                </ul>
                
                <p><strong>Next Steps:</strong></p>
                <ol>
                    <li>Login using your Candidate ID and password</li>
                    <li>Complete your profile if any sections are pending</li>
                    <li>Check for any assessment invitations</li>
                    <li>Monitor your email for interview schedules</li>
                </ol>
                
                <p style='margin-top: 30px;'>
                    <a href='https://vistawaystech.com/Account/Login' 
                       style='display:inline-block;background:#0d6efd;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;font-weight:bold;'>
                       Login to Your Account
                    </a>
                </p>
                
                <p style='margin-top: 20px; font-size: 12px; color: #666;'>
                    If you did not create this account, please contact us immediately at hr@vistawaystech.com
                </p>";

            try
            {
                await _emailService.SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the registration process
                // In production, you'd want to log this properly
                Console.WriteLine($"Failed to send credentials email: {ex.Message}");
            }
        }

        public async Task SendAssessmentInvitationEmailAsync(string email, string firstName, string assessmentUrl, DateTime expiryDate)
        {
            string subject = "VistaWays Tech - Online Assessment Invitation";
            string body = $@"
                <h2>Hello {firstName},</h2>
                <p>You have been invited to take the next step in your application with VistaWays Tech: an online assessment.</p>

                <div style='background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                    <p><strong>Expiry:</strong> {expiryDate:dd MMM yyyy, hh:mm tt}</p>
                    <p>Please log in with your candidate credentials before starting the assessment.</p>
                </div>

                <p><strong>Before you begin:</strong></p>
                <ul>
                    <li>Set aside 30 uninterrupted minutes in a quiet location</li>
                    <li>Use a laptop or desktop with a stable internet connection</li>
                    <li>The assessment will run in full-screen mode; switching tabs or windows will end the attempt</li>
                    <li>Each candidate gets a single attempt</li>
                </ul>

                <p style='margin-top: 30px;'>
                    <a href='{assessmentUrl}'
                       style='display:inline-block;background:#0d6efd;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;font-weight:bold;'>
                       Start Assessment
                    </a>
                </p>

                <p style='margin-top: 20px; font-size: 12px; color: #666;'>
                    If the button above doesn't work, copy and paste this link into your browser:<br />
                    {assessmentUrl}
                </p>

                <p style='margin-top: 20px; font-size: 12px; color: #666;'>
                    This invitation will expire on {expiryDate:dd MMM yyyy, hh:mm tt}. If you did not expect this email, please contact us at hr@vistawaystech.com
                </p>";

            try
            {
                await _emailService.SendEmailAsync(email, subject, body);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail invitation creation if email delivery fails.
                Console.WriteLine($"Failed to send assessment invitation email: {ex.Message}");
                throw;
            }
        }
    }
}
