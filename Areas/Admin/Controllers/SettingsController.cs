using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SettingsController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EmailSettings(EmailSettings settings)
        {
            if (!ModelState.IsValid)
                return View(settings);

            TempData["Success"] = "Email settings saved successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult EmailSettings()
        {
            var settings = new EmailSettings();
            return View(settings);
        }

        [HttpPost]
        public IActionResult SystemSettings(SystemSettingsViewModel settings)
        {
            if (!ModelState.IsValid)
                return View(settings);

            TempData["Success"] = "System settings saved successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult SystemSettings()
        {
            var settings = new SystemSettingsViewModel();
            return View(settings);
        }

        public IActionResult RoleManagement()
        {
            var additionalRoles = _roleManager.Roles
                .Where(r => r.Name != "Admin" && r.Name != "Candidate")
                .OrderBy(r => r.Name)
                .Select(r => r.Name!)
                .ToList();

            return View(additionalRoles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            roleName = (roleName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(roleName))
            {
                TempData["Error"] = "Role name cannot be empty.";
                return RedirectToAction(nameof(RoleManagement));
            }

            if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                roleName.Equals("Candidate", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = $"\"{roleName}\" already exists as a built-in role.";
                return RedirectToAction(nameof(RoleManagement));
            }

            if (await _roleManager.RoleExistsAsync(roleName))
            {
                TempData["Error"] = $"Role \"{roleName}\" already exists.";
                return RedirectToAction(nameof(RoleManagement));
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (result.Succeeded)
            {
                TempData["Success"] = $"Role \"{roleName}\" created successfully.";
            }
            else
            {
                TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(RoleManagement));
        }
    }

    public class SystemSettingsViewModel
    {
        public string SiteName { get; set; } = "VistaWays Tech Recruitment Portal";
        public string SiteDescription { get; set; } = "";
        public string ContactEmail { get; set; } = "";
        public string ContactPhone { get; set; } = "";
        public bool AllowRegistrations { get; set; } = true;
        public bool RequireEmailVerification { get; set; } = true;
    }
}
