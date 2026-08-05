using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;

namespace VistaWaysTechRecruitPortal.Controllers
{
    [Authorize]
    public class CandidateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CandidateController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("Login", "Account");

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

            if (candidate == null)
                return NotFound();

            var interview = _context.Interviews
                .FirstOrDefault(x => x.CandidateId == candidate.Id);

            ViewBag.Interview = interview;
            var offer = _context.OfferLetters
                .FirstOrDefault(x => x.CandidateId == candidate.Id);

            ViewBag.Offer = offer;

            return View(candidate);
        }

    }
}