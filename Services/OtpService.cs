using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Services
{
    public class OtpService
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public OtpService(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<string> GenerateAndSendOtpAsync(string email, string purpose)
        {
            // Generate 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Invalidate any existing OTPs for this email and purpose
            var existingOtps = _context.OtpVerifications
                .Where(o => o.Email == email && o.Purpose == purpose && !o.IsUsed && o.ExpiresAt > DateTime.Now);

            foreach (var existingOtp in existingOtps)
            {
                existingOtp.IsUsed = true;
            }

            // Create new OTP record
            var otpRecord = new OtpVerification
            {
                Email = email,
                Otp = otp,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(10), // OTP expires in 10 minutes
                IsUsed = false,
                Purpose = purpose
            };

            _context.OtpVerifications.Add(otpRecord);
            await _context.SaveChangesAsync();

            // Send OTP via email
            string subject = purpose == "Registration" 
                ? "VistaWays Tech - Email Verification OTP" 
                : "VistaWays Tech - Password Reset OTP";

            string body = $@"
                <h2>VistaWays Tech - {purpose} Verification</h2>
                <p>Your One-Time Password (OTP) is: <strong>{otp}</strong></p>
                <p>This OTP will expire in 10 minutes.</p>
                <p>If you did not request this, please ignore this email.</p>
                <p>For security reasons, do not share this OTP with anyone.</p>";

            try
            {
                await _emailService.SendEmailAsync(email, subject, body);
            }
            catch (Exception)
            {
                // In development, if email is not configured, we'll still return the OTP
                // for testing purposes
                if (!_emailService.IsConfigured)
                {
                    return otp; // Return OTP for development testing
                }
                throw;
            }

            return otp;
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp, string purpose)
        {
            var otpRecord = await _context.OtpVerifications
                .FirstOrDefaultAsync(o => 
                    o.Email == email && 
                    o.Otp == otp && 
                    o.Purpose == purpose && 
                    !o.IsUsed && 
                    o.ExpiresAt > DateTime.Now);

            if (otpRecord == null)
            {
                return false;
            }

            otpRecord.IsUsed = true;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
