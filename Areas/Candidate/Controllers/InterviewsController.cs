using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;

namespace VistaWaysTechRecruitPortal.Areas.Candidate.Controllers
{
    [Area("Candidate")]
    [Authorize(Roles = "Candidate")]
    public class InterviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InterviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<VistaWaysTechRecruitPortal.Models.Candidate?> GetCurrentCandidateAsync()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return null;

            return await _context.Candidates.FirstOrDefaultAsync(c => c.Email == userEmail);
        }

        public async Task<IActionResult> Index()
        {
            var candidate = await GetCurrentCandidateAsync();

            if (candidate == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var interviews = await _context.Interviews
                .Where(i => i.CandidateId == candidate.Id)
                .OrderByDescending(i => i.InterviewDate)
                .ToListAsync();

            return View(interviews);
        }

        public async Task<IActionResult> Details(int id)
        {
            var candidate = await GetCurrentCandidateAsync();

            if (candidate == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var interview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.Id == id && i.CandidateId == candidate.Id);

            if (interview == null)
                return NotFound();

            return View(interview);
        }

        public async Task<IActionResult> Confirm(int id)
        {
            var candidate = await GetCurrentCandidateAsync();

            if (candidate == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var interview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.Id == id && i.CandidateId == candidate.Id);

            if (interview == null)
                return NotFound();

            return View(interview);
        }

        [HttpPost, ActionName("Confirm")]
        public async Task<IActionResult> ConfirmConfirmed(int id)
        {
            var candidate = await GetCurrentCandidateAsync();

            if (candidate == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var interview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.Id == id && i.CandidateId == candidate.Id);

            if (interview == null)
                return NotFound();

            if (interview.Status == "Scheduled")
            {
                interview.Status = "Confirmed";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Interview confirmed successfully.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Reschedule(int id)
        {
            var candidate = await GetCurrentCandidateAsync();

            if (candidate == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var interview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.Id == id && i.CandidateId == candidate.Id);

            if (interview == null)
                return NotFound();

            return View(interview);
        }

        [HttpPost, ActionName("Reschedule")]
        public async Task<IActionResult> RescheduleConfirmed(int id, DateTime preferredDate, string preferredTime, string reason)
        {
            var candidate = await GetCurrentCandidateAsync();

            if (candidate == null)
                return RedirectToAction("Login", "Account", new { area = "" });

            var interview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.Id == id && i.CandidateId == candidate.Id);

            if (interview == null)
                return NotFound();

            interview.Status = "Reschedule Requested";
            interview.Feedback = $"Candidate requested reschedule to {preferredDate:dd-MMM-yyyy} at {preferredTime}. Reason: {reason}";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Your reschedule request has been submitted. HR will contact you shortly.";

            return RedirectToAction(nameof(Index));
        }
    }
}
