using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistaWaysTechRecruitPortal.ViewModels;
using VistawaysTechRecruitPortal.Data;

namespace VistaWaysTechRecruitPortal.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(CandidateLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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

            var result = await _signInManager.PasswordSignInAsync(
                user.Email!,
                model.Password,
                false,
                false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.IsNotAllowed
                    ? "Your account email is not confirmed yet. Please check your inbox or contact support."
                    : "Invalid password.");
                return View(model);
            }

            return RedirectToAction("Index", "Dashboard", new { area = "Candidate" });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all password fields.";
                return RedirectToAction("Settings");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["Error"] = "New password and confirmation password do not match.";
                return RedirectToAction("Settings");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account", new { area = "Candidate" });

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                TempData["Error"] = "Failed to change password. Please check your current password.";
                return RedirectToAction("Settings");
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Password changed successfully.";

            return RedirectToAction("Settings");
        }
    }
}
