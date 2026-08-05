using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    [Authorize(Roles = "Candidate")]
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm = "", string department = "", string location = "")
        {
            var query = _context.Jobs.Where(j => j.IsActive);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(j => j.JobTitle.Contains(searchTerm) ||
                                       j.Description.Contains(searchTerm) ||
                                       j.Department.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(j => j.Department == department);
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(j => j.Location.Contains(location));
            }

            var jobs = await query
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();

            var userEmail = User.Identity?.Name;
            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            var appliedJobIds = new List<int>();
            if (candidate != null)
            {
                appliedJobIds = await _context.Applications
                    .Where(a => a.CandidateId == candidate.Id)
                    .Select(a => a.JobId)
                    .ToListAsync();
            }

            ViewBag.AppliedJobIds = appliedJobIds;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Department = department;
            ViewBag.Location = location;

            return View(jobs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            var userEmail = User.Identity?.Name;
            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            bool hasApplied = false;
            if (candidate != null)
            {
                hasApplied = await _context.Applications
                    .AnyAsync(a => a.CandidateId == candidate.Id && a.JobId == id);
            }

            ViewBag.HasApplied = hasApplied;

            return View(job);
        }

        [HttpPost]
        public async Task<IActionResult> Apply(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            var userEmail = User.Identity?.Name;
            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (candidate == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var existingApplication = await _context.Applications
                .FirstOrDefaultAsync(a => a.CandidateId == candidate.Id && a.JobId == id);

            if (existingApplication != null)
            {
                TempData["Error"] = "You have already applied for this position.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var application = new Application
            {
                CandidateId = candidate.Id,
                JobId = id,
                AppliedDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Application submitted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
