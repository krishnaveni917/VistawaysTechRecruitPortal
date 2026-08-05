using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    [Authorize(Roles = "Candidate")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
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

            var totalApplications = await _context.AssessmentInvitations
                .CountAsync(ai => ai.CandidateId == candidate.Id);

            var pendingAssessments = await _context.AssessmentInvitations
                .CountAsync(ai => ai.CandidateId == candidate.Id && !ai.CompletedOn.HasValue);

            var scheduledInterviews = await _context.Interviews
                .CountAsync(i => i.CandidateId == candidate.Id && 
                               i.Status == "Scheduled");

            var viewModel = new CandidateDashboardViewModel
            {
                Candidate = candidate,
                TotalApplications = totalApplications,
                PendingAssessments = pendingAssessments,
                ScheduledInterviews = scheduledInterviews
            };

            return View(viewModel);
        }
    }

    public class CandidateDashboardViewModel
    {
        public VistaWaysTechRecruitPortal.Models.Candidate Candidate { get; set; } = null!;
        public int TotalApplications { get; set; }
        public int PendingAssessments { get; set; }
        public int ScheduledInterviews { get; set; }
    }
}
