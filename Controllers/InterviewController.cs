using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;
using VistaWaysTechRecruitPortal.Services;

namespace VistaWaysTechRecruitPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InterviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public InterviewController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public IActionResult Schedule(int candidateId)
        {
            ViewBag.CandidateId = candidateId;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Schedule(Interview interview)
        {
            if (!ModelState.IsValid)
                return View(interview);

            interview.Status = "Scheduled";

            // Retrieve the candidate from the database
            var candidate = await _context.Candidates.FindAsync(interview.CandidateId);
            if (candidate == null)
            {
                ModelState.AddModelError("", "Candidate not found.");
                return View(interview);
            }

            _context.Interviews.Add(interview);
            candidate.RecruitmentStatus = RecruitmentStage.ShortlistedForInterview;

            _context.SaveChanges();

            TempData["Success"] = "Interview Scheduled Successfully";
            await _emailService.SendEmailAsync(candidate.Email, "Interview Invitation",
                  $@"
                    Dear {candidate.FirstName},

                     Congratulations!

                          Your interview has been scheduled.

                     Date: {interview.InterviewDate:dd-MM-yyyy}

                  Time: {interview.InterviewTime}

                   Mode: {interview.InterviewMode}

                    Meeting Link:
                     {interview.MeetingLink}

                    Regards,

                    VistaWays Tech HR
                    ");

            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            var interviews = _context.Interviews.ToList();

            return View(interviews);
        }
        public IActionResult Edit(int id)
        {
            var interview = _context.Interviews.Find(id);

            if (interview == null)
                return NotFound();

            return View(interview);
        }
        [HttpPost]
        public IActionResult Edit(Interview interview)
        {
            if (!ModelState.IsValid)
                return View(interview);

            _context.Interviews.Update(interview);

            _context.SaveChanges();

            TempData["Success"] = "Interview Updated Successfully";

            return RedirectToAction("Index");
        }
    }
}
