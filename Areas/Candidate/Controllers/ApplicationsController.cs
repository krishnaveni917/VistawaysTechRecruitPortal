using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    [Authorize(Roles = "Candidate")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity?.Name;
            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (candidate == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var applications = await _context.Applications
                .Include(a => a.Job)
                .Where(a => a.CandidateId == candidate.Id)
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();

            return View(applications);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userEmail = User.Identity?.Name;
            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (candidate == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var application = await _context.Applications
                .Include(a => a.Job)
                .Include(a => a.Candidate)
                .FirstOrDefaultAsync(a => a.Id == id && a.CandidateId == candidate.Id);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        public async Task<IActionResult> Withdraw(int id)
        {
            var userEmail = User.Identity?.Name;
            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (candidate == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var application = await _context.Applications
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.Id == id && a.CandidateId == candidate.Id);

            if (application == null)
            {
                return NotFound();
            }

            if (application.Status == "Withdrawn")
            {
                TempData["Error"] = "This application has already been withdrawn.";
                return RedirectToAction(nameof(Index));
            }

            if (application.Status == "Selected" || application.Status == "Offered")
            {
                TempData["Error"] = "Cannot withdraw an application that has been selected or offered.";
                return RedirectToAction(nameof(Index));
            }

            return View(application);
        }

        [HttpPost, ActionName("Withdraw")]
        public async Task<IActionResult> WithdrawConfirmed(int id)
        {
            var userEmail = User.Identity?.Name;
            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (candidate == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == id && a.CandidateId == candidate.Id);

            if (application != null)
            {
                application.Status = "Withdrawn";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Application withdrawn successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
