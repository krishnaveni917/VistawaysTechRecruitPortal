using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;
using VistaWaysTechRecruitPortal.Services;
using VistaWaysTechRecruitPortal.ViewModels;

namespace VistaWaysRecruitmentPortal.Controllers
{
    public class OtpController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OtpService _otpService;
        private readonly IWebHostEnvironment _environment;

        public OtpController(
            ApplicationDbContext context,
            OtpService otpService,
            IWebHostEnvironment environment)
        {
            _context = context;
            _otpService = otpService;
            _environment = environment;
        }

        public IActionResult RegisterEmailOtp()
        {
            return View(new EmailRegistrationViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> RegisterEmailOtp(EmailRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check if email is already registered
            var existingCandidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == model.Email);

            if (existingCandidate != null)
            {
                ModelState.AddModelError("", "This email is already registered. Please login or use forgot password.");
                return View(model);
            }

            try
            {
                string otp = await _otpService.GenerateAndSendOtpAsync(model.Email, "Registration");

                if (_environment.IsDevelopment())
                {
                    TempData["DevelopmentOtp"] = otp;
                    TempData["Info"] = $"Development mode: Your OTP is {otp}";
                }

                return RedirectToAction(nameof(VerifyOtp), new { email = model.Email });
            }
            catch (Exception)
            {
                if (_environment.IsDevelopment())
                {
                    // In development, show OTP if email service is not configured
                    TempData["DevelopmentOtp"] = "123456"; // Fallback for testing
                    TempData["Info"] = "Email service not configured. Using test OTP: 123456";
                    return RedirectToAction(nameof(VerifyOtp), new { email = model.Email });
                }

                ModelState.AddModelError("", "Failed to send OTP. Please try again.");
                return View(model);
            }
        }

        public IActionResult VerifyOtp(string email)
        {
            return View(new OtpVerificationViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(OtpVerificationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool isValid = await _otpService.VerifyOtpAsync(model.Email, model.Otp, "Registration");

            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid or expired OTP. Please try again.");
                return View(model);
            }

            // Store verified email in session for registration
            HttpContext.Session.SetString("VerifiedEmail", model.Email);
            HttpContext.Session.SetString("EmailVerifiedAt", DateTime.Now.ToString());

            return RedirectToAction("Register", "Account", new { email = model.Email });
        }

        public async Task<IActionResult> ResendOtp(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return RedirectToAction(nameof(RegisterEmailOtp));

            try
            {
                string otp = await _otpService.GenerateAndSendOtpAsync(email, "Registration");

                if (_environment.IsDevelopment())
                {
                    TempData["DevelopmentOtp"] = otp;
                    TempData["Info"] = $"Development mode: Your new OTP is {otp}";
                }
                else
                {
                    TempData["Success"] = "New OTP sent to your email.";
                }

                return RedirectToAction(nameof(VerifyOtp), new { email });
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to resend OTP. Please try again.";
                return RedirectToAction(nameof(VerifyOtp), new { email });
            }
        }

        public IActionResult ForgotPasswordOtp()
        {
            return View(new EmailRegistrationViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordOtp(EmailRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == model.Email);

            if (candidate == null)
            {
                // Don't reveal if email exists
                TempData["Success"] = "If the email is registered, an OTP will be sent.";
                return View(model);
            }

            try
            {
                string otp = await _otpService.GenerateAndSendOtpAsync(model.Email, "PasswordReset");

                if (_environment.IsDevelopment())
                {
                    TempData["DevelopmentOtp"] = otp;
                    TempData["Info"] = $"Development mode: Your OTP is {otp}";
                }
                else
                {
                    TempData["Success"] = "OTP sent to your email.";
                }

                return RedirectToAction(nameof(VerifyPasswordResetOtp), new { email = model.Email });
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to send OTP. Please try again.";
                return View(model);
            }
        }

        public IActionResult VerifyPasswordResetOtp(string email)
        {
            return View(new OtpVerificationViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPasswordResetOtp(OtpVerificationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool isValid = await _otpService.VerifyOtpAsync(model.Email, model.Otp, "PasswordReset");

            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid or expired OTP. Please try again.");
                return View(model);
            }

            // Store verified email in session for password reset
            HttpContext.Session.SetString("PasswordResetEmail", model.Email);

            return RedirectToAction("ResetPassword", "Account", new { email = model.Email });
        }
    }
}
