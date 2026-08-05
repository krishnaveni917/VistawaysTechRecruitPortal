using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysRecruitmentPortal.ViewModels;
using VistaWaysTechRecruitPortal.Models;
using VistaWaysTechRecruitPortal.Services;
using VistaWaysTechRecruitPortal.ViewModels;

namespace VistaWaysRecruitmentPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly CandidateIdGenerator _candidateIdGenerator;
        private readonly EmailService _emailService;
        private readonly CredentialEmailService _credentialEmailService;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            CandidateIdGenerator candidateIdGenerator,
            EmailService emailService,
            CredentialEmailService credentialEmailService,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _candidateIdGenerator = candidateIdGenerator;
            _emailService = emailService;
            _credentialEmailService = credentialEmailService;
            _environment = environment;
            _configuration = configuration;
        }

        public IActionResult RegisterEmail()
        {
            // Redirect to OTP-based registration flow
            return RedirectToAction("RegisterEmailOtp", "Otp");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterEmail(EmailRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "This email is already registered. Please login or use forgot password.");
                return View(model);
            }

            string token = Guid.NewGuid().ToString("N");

            var pendingRegistrations = _context.PendingCandidateRegistrations
                .Where(x => x.Email == model.Email && !x.IsVerified);

            foreach (var pendingRegistration in pendingRegistrations)
            {
                pendingRegistration.ExpiresOn = DateTime.Now;
            }

            var pending = new PendingCandidateRegistration
            {
                Email = model.Email,
                Token = token,
                ExpiresOn = DateTime.Now.AddHours(2)
            };

            _context.PendingCandidateRegistrations.Add(pending);
            await _context.SaveChangesAsync();

            var verificationLink = BuildAccountUrl(
                "VerifyRegistrationEmail",
                new { token });

            if (_environment.IsDevelopment() && !_emailService.IsConfigured)
            {
                TempData["Success"] = "Email was not sent because SMTP is not configured. Local development verification was used.";
                return RedirectToAction(nameof(VerifyRegistrationEmail), new { token });
            }

            try
            {
                await _emailService.SendEmailAsync(
                    model.Email,
                    "VistaWays Tech - Verify Your Email",
                    $@"
                        <h2>VistaWays Tech Candidate Registration</h2>
                        <p>Please verify your email before completing your candidate profile.</p>
                        <p><a href='{verificationLink}' style='display:inline-block;background:#0d6efd;color:white;padding:10px 18px;text-decoration:none;border-radius:5px;'>Verify Email</a></p>
                        <p>If the button does not open, copy and paste this link into the browser on the same computer running the app:</p>
                        <p><a href='{verificationLink}'>{verificationLink}</a></p>
                        <p>This verification link expires in 2 hours.</p>");
            }
            catch (InvalidOperationException ex)
            {
                if (_environment.IsDevelopment())
                {
                    TempData["Success"] = "Email was not sent because SMTP is not configured. Local development verification was used.";
                    return RedirectToAction(nameof(VerifyRegistrationEmail), new { token });
                }

                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

            TempData["Success"] = "Verification link sent. Please check your email to continue registration.";

            return View(model);
        }

        public async Task<IActionResult> VerifyRegistrationEmail(string token)
        {
            var pending = await _context.PendingCandidateRegistrations
                .FirstOrDefaultAsync(x => x.Token == token && x.ExpiresOn >= DateTime.Now);

            if (pending == null)
                return BadRequest("This registration verification link is invalid or expired.");

            pending.IsVerified = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Email verified. Please complete your candidate profile.";

            return RedirectToAction(nameof(Register), new { token = pending.Token });
        }

        public async Task<IActionResult> Register(string? token, string? email)
        {
            // Handle OTP-based registration flow
            if (!string.IsNullOrWhiteSpace(email))
            {
                var sessionEmail = HttpContext.Session.GetString("VerifiedEmail");
                if (string.IsNullOrEmpty(sessionEmail) || sessionEmail != email)
                {
                    return RedirectToAction("RegisterEmailOtp", "Otp");
                }

                return View(new RegisterViewModel
                {
                    Email = email,
                    VerificationToken = Guid.NewGuid().ToString()
                });
            }

            // Handle existing token-based flow for backward compatibility
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("RegisterEmailOtp", "Otp");

            var pending = await _context.PendingCandidateRegistrations
                .FirstOrDefaultAsync(x => x.Token == token && x.IsVerified && x.ExpiresOn >= DateTime.Now);

            if (pending == null)
                return BadRequest("Please verify your email before registration.");

            return View(new RegisterViewModel
            {
                Email = pending.Email,
                VerificationToken = pending.Token
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check OTP-based verification first
            var sessionEmail = HttpContext.Session.GetString("VerifiedEmail");
            bool isOtpVerified = !string.IsNullOrEmpty(sessionEmail) && sessionEmail == model.Email;

            // Check token-based verification for backward compatibility
            var pending = await _context.PendingCandidateRegistrations
                .FirstOrDefaultAsync(x =>
                    x.Token == model.VerificationToken &&
                    x.Email == model.Email &&
                    x.IsVerified &&
                    x.ExpiresOn >= DateTime.Now);

            if (!isOtpVerified && pending == null)
            {
                ModelState.AddModelError("", "Please verify your email before registration.");
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email already exists.");
                return View(model);
            }

            string extension = model.Resume != null
                ? Path.GetExtension(model.Resume.FileName.ToLower())
                : string.Empty;

            string[] allowed = { ".pdf", ".doc", ".docx" };

            if (model.Resume != null && !allowed.Contains(extension))
            {
                ModelState.AddModelError("", "Only PDF, DOC and DOCX files are allowed.");
                return View(model);
            }

            if (model.Resume != null && model.Resume.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("", "Resume must be less than 5 MB.");
                return View(model);
            }

            // Conditional validation based on QualificationType
            if (model.QualificationType == "UG")
            {
                if (string.IsNullOrEmpty(model.UndergraduateUniversity))
                    ModelState.AddModelError("UndergraduateUniversity", "Undergraduate university is required for UG qualification.");
                if (string.IsNullOrEmpty(model.UndergraduateDegree))
                    ModelState.AddModelError("UndergraduateDegree", "Undergraduate degree is required for UG qualification.");
                if (string.IsNullOrEmpty(model.UndergraduateSpecialization))
                    ModelState.AddModelError("UndergraduateSpecialization", "Undergraduate specialization is required for UG qualification.");
                if (!model.UndergraduatePassingYear.HasValue)
                    ModelState.AddModelError("UndergraduatePassingYear", "Undergraduate passing year is required for UG qualification.");
                if (!model.UndergraduatePercentage.HasValue)
                    ModelState.AddModelError("UndergraduatePercentage", "Undergraduate percentage is required for UG qualification.");
            }
            else if (model.QualificationType == "PG")
            {
                if (string.IsNullOrEmpty(model.PostgraduateUniversity))
                    ModelState.AddModelError("PostgraduateUniversity", "Postgraduate university is required for PG qualification.");
                if (string.IsNullOrEmpty(model.PostgraduateDegree))
                    ModelState.AddModelError("PostgraduateDegree", "Postgraduate degree is required for PG qualification.");
                if (string.IsNullOrEmpty(model.PostgraduateSpecialization))
                    ModelState.AddModelError("PostgraduateSpecialization", "Postgraduate specialization is required for PG qualification.");
                if (!model.PostgraduatePassingYear.HasValue)
                    ModelState.AddModelError("PostgraduatePassingYear", "Postgraduate passing year is required for PG qualification.");
                if (!model.PostgraduatePercentage.HasValue)
                    ModelState.AddModelError("PostgraduatePercentage", "Postgraduate percentage is required for PG qualification.");
            }
            else if (model.QualificationType == "Both")
            {
                // Require both UG and PG details
                if (string.IsNullOrEmpty(model.UndergraduateUniversity))
                    ModelState.AddModelError("UndergraduateUniversity", "Undergraduate university is required when qualification is Both.");
                if (string.IsNullOrEmpty(model.UndergraduateDegree))
                    ModelState.AddModelError("UndergraduateDegree", "Undergraduate degree is required when qualification is Both.");
                if (string.IsNullOrEmpty(model.UndergraduateSpecialization))
                    ModelState.AddModelError("UndergraduateSpecialization", "Undergraduate specialization is required when qualification is Both.");
                if (!model.UndergraduatePassingYear.HasValue)
                    ModelState.AddModelError("UndergraduatePassingYear", "Undergraduate passing year is required when qualification is Both.");
                if (!model.UndergraduatePercentage.HasValue)
                    ModelState.AddModelError("UndergraduatePercentage", "Undergraduate percentage is required when qualification is Both.");

                if (string.IsNullOrEmpty(model.PostgraduateUniversity))
                    ModelState.AddModelError("PostgraduateUniversity", "Postgraduate university is required when qualification is Both.");
                if (string.IsNullOrEmpty(model.PostgraduateDegree))
                    ModelState.AddModelError("PostgraduateDegree", "Postgraduate degree is required when qualification is Both.");
                if (string.IsNullOrEmpty(model.PostgraduateSpecialization))
                    ModelState.AddModelError("PostgraduateSpecialization", "Postgraduate specialization is required when qualification is Both.");
                if (!model.PostgraduatePassingYear.HasValue)
                    ModelState.AddModelError("PostgraduatePassingYear", "Postgraduate passing year is required when qualification is Both.");
                if (!model.PostgraduatePercentage.HasValue)
                    ModelState.AddModelError("PostgraduatePercentage", "Postgraduate percentage is required when qualification is Both.");
            }

            if (!ModelState.IsValid)
                return View(model);

            var identityUser = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            // Assign Candidate role to the user
            if (!await _userManager.IsInRoleAsync(identityUser, "Candidate"))
            {
                await _userManager.AddToRoleAsync(identityUser, "Candidate");
            }

            string candidateId = await _candidateIdGenerator.GenerateCandidateIdAsync();
            string resumePath = "";

            if (model.Resume != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/resumes");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid() + Path.GetExtension(model.Resume.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await model.Resume.CopyToAsync(stream);

                resumePath = "/uploads/resumes/" + uniqueFileName;
            }

            Candidate candidate = new Candidate
            {
                CandidateId = candidateId,
                Role = "Candidate",
                ApplicationUserId = identityUser.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                MobileNumber = model.MobileNumber,
                QualificationType = model.QualificationType,
                Qualification = model.UndergraduateDegree,
                Specialization = model.UndergraduateSpecialization,
                PassingYear = model.UndergraduatePassingYear ?? 0,
                Percentage = model.UndergraduatePercentage ?? 0,
                UndergraduateUniversity = model.UndergraduateUniversity,
                UndergraduateLocation = model.UndergraduateLocation,
                UndergraduateDegree = model.UndergraduateDegree,
                UndergraduateSpecialization = model.UndergraduateSpecialization,
                UndergraduatePassingYear = model.UndergraduatePassingYear,
                UndergraduatePercentage = model.UndergraduatePercentage,
                PostgraduateUniversity = model.PostgraduateUniversity,
                PostgraduateLocation = model.PostgraduateLocation,
                PostgraduateDegree = model.PostgraduateDegree,
                PostgraduateSpecialization = model.PostgraduateSpecialization,
                PostgraduatePassingYear = model.PostgraduatePassingYear,
                PostgraduatePercentage = model.PostgraduatePercentage,
                IntermediateCollege = model.IntermediateCollege,
                IntermediateLocation = model.IntermediateLocation,
                IntermediateBoard = model.IntermediateBoard,
                IntermediateHallTicket = model.IntermediateHallTicket,
                IntermediatePassingYear = model.IntermediatePassingYear,
                IntermediatePercentage = model.IntermediatePercentage,
                TenthInstitute = model.TenthInstitute,
                TenthLocation = model.TenthLocation,
                TenthBoard = model.TenthBoard,
                TenthHallTicket = model.TenthHallTicket,
                TenthPassingYear = model.TenthPassingYear,
                TenthPercentage = model.TenthPercentage,
                Experience = model.Experience,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Country = model.Country,
                PinCode = model.PinCode,
                ResumePath = resumePath,
                RegistrationDate = DateTime.Now,
                EmailVerified = true,
                ProfileCompleted = true,
                RecruitmentStatus = RecruitmentStage.ProfileUnderReview
            };

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            // Send credentials email with candidate ID and password
            await _credentialEmailService.SendCredentialsEmailAsync(
                candidate.Email,
                candidateId,
                model.Password,
                candidate.FirstName
            );

            TempData["Success"] = "Registration successful. Login credentials have been sent to your email.";

            return RedirectToAction(nameof(Login));
        }

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            string normalizedCandidateId = model.CandidateId.Trim();

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.CandidateId.ToUpper() == normalizedCandidateId.ToUpper());

            if (candidate == null || string.IsNullOrWhiteSpace(candidate.ApplicationUserId))
            {
                ModelState.AddModelError("", "Invalid Candidate ID.");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(candidate.ApplicationUserId);

            if (user == null)
            {
                ModelState.AddModelError("", "User account not found.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.Email!, model.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid password.");
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            // Redirect to Candidate Dashboard
            return RedirectToAction("Index", "Dashboard", new { area = "Candidate" });
        }

        public IActionResult AdminLogin()
        {
            // Redirect to Admin Area login
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }

        public IActionResult ForgotPassword()
        {
            // Redirect to OTP-based password reset flow
            return RedirectToAction("ForgotPasswordOtp", "Otp");
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string normalizedCandidateId = model.CandidateId.Trim();
            string normalizedEmail = model.Email.Trim();

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(x => x.CandidateId.ToUpper() == normalizedCandidateId.ToUpper()
                    && x.Email.ToUpper() == normalizedEmail.ToUpper());

            if (candidate == null || string.IsNullOrWhiteSpace(candidate.ApplicationUserId))
            {
                TempData["Success"] = "If the details match, a reset link will be sent.";
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(candidate.ApplicationUserId);

            if (user == null)
            {
                TempData["Success"] = "If the details match, a reset link will be sent.";
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = BuildAccountUrl(
                "ResetPassword",
                new { userId = user.Id, token });

            if (_environment.IsDevelopment() && !_emailService.IsConfigured)
            {
                TempData["Success"] = $"Email was not sent because SMTP is not configured. Local testing reset link: {resetLink}";
                return View(model);
            }

            try
            {
                await _emailService.SendEmailAsync(
                    model.Email,
                    "VistaWays Tech - Reset Your Password",
                    $@"
                        <h2>Password Reset</h2>
                        <p>Use the secure link below to reset your VistaWays Tech candidate password.</p>
                        <p><a href='{resetLink}' style='display:inline-block;background:#0d6efd;color:white;padding:10px 18px;text-decoration:none;border-radius:5px;'>Reset Password</a></p>
                        <p>If the button does not open, copy and paste this link into the browser on the same computer running the app:</p>
                        <p><a href='{resetLink}'>{resetLink}</a></p>");
            }
            catch (InvalidOperationException ex)
            {
                if (_environment.IsDevelopment())
                {
                    TempData["Success"] = $"Email was not sent because SMTP is not configured. Local testing reset link: {resetLink}";
                    return View(model);
                }

                ModelState.AddModelError("", ex.Message);
                return View(model);
            }

            TempData["Success"] = "If the details match, a reset link will be sent.";

            return View(model);
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest("Invalid password reset link.");

            return View(new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return RedirectToAction(nameof(Login));

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            TempData["Success"] = "Password changed successfully. Please login.";

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("Invalid email confirmation link.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return View("Error");

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (candidate != null)
            {
                candidate.EmailVerified = true;
                await _context.SaveChangesAsync();
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private string BuildAccountUrl(string action, object routeValues)
        {
            string relativeUrl = Url.Action(action, "Account", routeValues) ?? "/";
            string? appBaseUrl = _configuration["AppBaseUrl"];

            if (!string.IsNullOrWhiteSpace(appBaseUrl))
            {
                return $"{appBaseUrl.TrimEnd('/')}{relativeUrl}";
            }

            return Url.Action(action, "Account", routeValues, Request.Scheme) ?? relativeUrl;
        }
    }
}

