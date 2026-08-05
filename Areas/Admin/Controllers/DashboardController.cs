using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalCandidates = await _context.Candidates.CountAsync();
            var pendingCandidates = await _context.Candidates.CountAsync(c => c.RecruitmentStatus == RecruitmentStage.Registered);
            var shortlistedCandidates = await _context.Candidates.CountAsync(c => c.RecruitmentStatus == RecruitmentStage.ShortlistedForInterview);
            var hiredCandidates = await _context.Candidates.CountAsync(c => c.RecruitmentStatus == RecruitmentStage.OfferAccepted);

            var recentCandidates = await _context.Candidates
                .OrderByDescending(c => c.RegistrationDate)
                .Take(10)
                .ToListAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalCandidates = totalCandidates,
                PendingCandidates = pendingCandidates,
                ShortlistedCandidates = shortlistedCandidates,
                HiredCandidates = hiredCandidates,
                RecentCandidates = recentCandidates
            };

            return View(viewModel);
        }
    }

    public class AdminDashboardViewModel
    {
        public int TotalCandidates { get; set; }
        public int PendingCandidates { get; set; }
        public int ShortlistedCandidates { get; set; }
        public int HiredCandidates { get; set; }
        public List<VistaWaysTechRecruitPortal.Models.Candidate> RecentCandidates { get; set; } = new();
    }
}
