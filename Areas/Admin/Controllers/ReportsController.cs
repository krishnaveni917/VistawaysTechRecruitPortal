using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalCandidates = await _context.Candidates.CountAsync();
            var totalJobs = await _context.Jobs.CountAsync();
            var totalApplications = await _context.Applications.CountAsync();
            var totalInterviews = await _context.Interviews.CountAsync();
            var selectedCandidates = await _context.Interviews.CountAsync(i => i.IsSelected);

            var model = new DashboardViewModel
            {
                TotalCandidates = totalCandidates,
                TotalJobs = totalJobs,
                TotalApplications = totalApplications,
                TotalInterviews = totalInterviews,
                SelectedCandidates = selectedCandidates
            };

            return View(model);
        }

        public async Task<IActionResult> CandidateReport()
        {
            var candidates = await _context.Candidates
                .Include(c => c.Applications)
                .OrderByDescending(c => c.RegistrationDate)
                .ToListAsync();
            return View(candidates);
        }

        public async Task<IActionResult> RecruitmentReport()
        {
            var jobs = await _context.Jobs
                .Include(j => j.Applications)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();
            return View(jobs);
        }

        public async Task<IActionResult> AssessmentReport()
        {
            var results = await _context.AssessmentResults
                .OrderByDescending(r => r.SubmittedOn)
                .ToListAsync();
            return View(results);
        }
    }

    public class DashboardViewModel
    {
        public int TotalCandidates { get; set; }
        public int TotalJobs { get; set; }
        public int TotalApplications { get; set; }
        public int TotalInterviews { get; set; }
        public int SelectedCandidates { get; set; }
    }
}
