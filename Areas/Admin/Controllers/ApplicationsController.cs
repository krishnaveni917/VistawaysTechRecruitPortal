using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, string status)
        {
            var applications = _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.Job)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                applications = applications.Where(a =>
                    (a.Candidate != null && (a.Candidate.FirstName.Contains(search) ||
                                              a.Candidate.LastName.Contains(search) ||
                                              a.Candidate.Email.Contains(search) ||
                                              a.Candidate.CandidateId.Contains(search))) ||
                    (a.Job != null && a.Job.JobTitle.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                applications = applications.Where(a => a.Status == status);
            }

            ViewBag.Search = search;
            ViewBag.Status = status;

            var result = await applications
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();

            return View(result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var application = await _context.Applications
                .Include(a => a.Candidate)
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var application = await _context.Applications.FindAsync(id);

            if (application == null)
                return NotFound();

            application.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Application status updated successfully.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
