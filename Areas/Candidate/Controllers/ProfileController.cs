using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    [Authorize(Roles = "Candidate")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (candidate == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(candidate);
        }

        public async Task<IActionResult> Edit()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (candidate == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new VistaWaysTechRecruitPortal.ViewModels.EditProfileViewModel
            {
                Id = candidate.Id,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                MobileNumber = candidate.MobileNumber,
                Address = candidate.Address,
                City = candidate.City,
                State = candidate.State,
                Country = candidate.Country,
                PinCode = candidate.PinCode
            };

            ViewBag.CandidateId = candidate.CandidateId;
            ViewBag.Email = candidate.Email;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VistaWaysTechRecruitPortal.ViewModels.EditProfileViewModel model)
        {
            var userEmail = User.Identity?.Name;
            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (candidate == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CandidateId = candidate.CandidateId;
                ViewBag.Email = candidate.Email;
                return View(model);
            }

            candidate.FirstName = model.FirstName;
            candidate.LastName = model.LastName;
            candidate.MobileNumber = model.MobileNumber;
            candidate.Address = model.Address;
            candidate.City = model.City;
            candidate.State = model.State;
            candidate.Country = model.Country;
            candidate.PinCode = model.PinCode;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Profile updated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
