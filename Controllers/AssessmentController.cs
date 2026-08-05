using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;
using VistaWaysTechRecruitPortal.ViewModels;

namespace VistaWaysTechRecruitPortal.Controllers
{
    public class AssessmentController : Controller
    {
        private const int ExamDurationSeconds = 30 * 60;

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AssessmentController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Shown after every submission (normal, timed-out, or ended for a violation).
        // Deliberately generic - no score, no pass/fail, no reason. The candidate
        // has already been signed out by this point, so this must stay anonymous.
        [AllowAnonymous]
        public IActionResult Complete()
        {
            return View();
        }

        // Step 1: candidate clicks the link in their invitation email.
        // [Authorize] means an unauthenticated visitor is redirected to the secure
        // login page first, with this URL preserved as the returnUrl.
        [Authorize]
        public async Task<IActionResult> Start(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Invalid assessment link.");

            var invitation = await _context.AssessmentInvitations
                .FirstOrDefaultAsync(x => x.AssessmentToken == token && x.IsActive && !x.IsCompleted);

            if (invitation == null || invitation.ExpiryDate < DateTime.Now)
                return BadRequest("This assessment link is invalid or expired.");

            var candidate = await GetCurrentCandidateAsync();

            if (candidate == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Path + Request.QueryString });

            if (candidate.Id != invitation.CandidateId)
                return Forbid();

            // Step 2: show instructions and require explicit consent before the timer starts.
            return RedirectToAction(nameof(Instructions), new { token });
        }

        // Step 2: instructions & consent page.
        [Authorize]
        public async Task<IActionResult> Instructions(string token)
        {
            var candidate = await GetCurrentCandidateAsync();
            if (candidate == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Path + Request.QueryString });

            var invitation = await GetValidInvitationAsync(token, candidate.Id);
            if (invitation == null)
                return BadRequest("This assessment link is invalid, expired, or already completed.");

            // Already consented (e.g. they refreshed or came back) - go straight to the exam,
            // the countdown continues from where it left off.
            if (invitation.ConsentGivenOn != null)
                return RedirectToAction(nameof(TakeAssessment), new { token });

            ViewBag.AssessmentToken = token;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Instructions(string token, bool agree)
        {
            var candidate = await GetCurrentCandidateAsync();
            if (candidate == null)
                return RedirectToAction("Login", "Account");

            var invitation = await GetValidInvitationAsync(token, candidate.Id);
            if (invitation == null)
                return BadRequest("This assessment link is invalid, expired, or already completed.");

            if (!agree)
            {
                TempData["Error"] = "You must read the instructions and select 'I Agree' to continue.";
                ViewBag.AssessmentToken = token;
                return View();
            }

            if (invitation.ConsentGivenOn == null)
            {
                invitation.ConsentGivenOn = DateTime.Now;
                invitation.StartedOn = DateTime.Now;
                candidate.RecruitmentStatus = RecruitmentStage.AssessmentInProgress;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(TakeAssessment), new { token });
        }

        // Step 3: the timed, proctored exam itself.
        [Authorize]
        public async Task<IActionResult> TakeAssessment(string token)
        {
            var candidate = await GetCurrentCandidateAsync();

            if (candidate == null)
                return RedirectToAction("Login", "Account");

            var invitation = await GetValidInvitationAsync(token, candidate.Id);
            if (invitation == null)
                return BadRequest("This assessment link is invalid or expired.");

            bool alreadyTaken = await _context.AssessmentResults
                .AnyAsync(x => x.CandidateId == candidate.Id);

            if (alreadyTaken)
                return View("AlreadyCompleted");

            // Consent must be given first - this also sets the authoritative start time.
            if (invitation.ConsentGivenOn == null || invitation.StartedOn == null)
                return RedirectToAction(nameof(Instructions), new { token });

            int remainingSeconds = ExamDurationSeconds - (int)(DateTime.Now - invitation.StartedOn.Value).TotalSeconds;

            if (remainingSeconds <= 0)
            {
                // Time ran out before a submission was ever received (e.g. they closed the
                // tab and came back later). Finalize with no answers recorded.
                return await FinalizeAssessmentAsync(candidate, invitation, new List<TakeAssessmentViewModel>(),
                    terminatedForViolation: true, reason: "Time expired before the assessment was submitted.");
            }

            var questions = await _context.AssessmentQuestions
                .Where(x => x.IsActive)
                .Select(q => new TakeAssessmentViewModel
                {
                    QuestionId = q.Id,
                    Question = q.Question,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD
                }).ToListAsync();

            ViewBag.AssessmentToken = token;
            ViewBag.RemainingSeconds = remainingSeconds;

            return View(questions);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAssessment(List<TakeAssessmentViewModel> model, string token,
            bool terminatedForViolation = false, string? violationReason = null)
        {
            var candidate = await GetCurrentCandidateAsync();

            if (candidate == null)
                return RedirectToAction("Login", "Account");

            var invitation = await GetValidInvitationAsync(token, candidate.Id);
            if (invitation == null)
                return BadRequest("This assessment link is invalid, expired, or already completed.");

            return await FinalizeAssessmentAsync(candidate, invitation, model ?? new List<TakeAssessmentViewModel>(),
                terminatedForViolation, violationReason);
        }

        public IActionResult Index()
        {
            return View();
        }

        private async Task<IActionResult> FinalizeAssessmentAsync(Candidate candidate, AssessmentInvitation invitation,
            List<TakeAssessmentViewModel> model, bool terminatedForViolation, string? reason)
        {
            int score = 0;
            int correct = 0;

            foreach (var item in model)
            {
                var question = await _context.AssessmentQuestions
                    .FirstOrDefaultAsync(x => x.Id == item.QuestionId);

                if (question == null)
                    continue;

                bool isCorrect = question.CorrectAnswer == item.SelectedAnswer;

                if (isCorrect)
                {
                    score += question.Marks;
                    correct++;
                }

                _context.CandidateAnswers.Add(new CandidateAnswer
                {
                    CandidateId = candidate.Id,
                    QuestionId = item.QuestionId,
                    SelectedAnswer = item.SelectedAnswer,
                    IsCorrect = isCorrect,
                    MarksObtained = isCorrect ? question.Marks : 0
                });
            }

            var result = new AssessmentResult
            {
                CandidateId = candidate.Id,
                TotalQuestions = model.Count,
                CorrectAnswers = correct,
                Score = score,
                IsPassed = !terminatedForViolation && score >= 5,
                WasTerminatedForViolation = terminatedForViolation,
                TerminationReason = terminatedForViolation ? reason : null
            };

            _context.AssessmentResults.Add(result);

            invitation.IsCompleted = true;
            invitation.IsActive = false;
            invitation.CompletedOn = DateTime.Now;
            invitation.TerminatedForViolation = terminatedForViolation;
            invitation.ViolationReason = terminatedForViolation ? reason : null;

            candidate.RecruitmentStatus = terminatedForViolation
                ? RecruitmentStage.AssessmentTerminated
                : RecruitmentStage.AssessmentCompleted;

            await _context.SaveChangesAsync();

            // Requirement: candidates never see their score, pass/fail status, or
            // the reason a session ended - only a generic acknowledgement. Then
            // immediately end their authenticated session.
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Complete));
        }

        private async Task<AssessmentInvitation?> GetValidInvitationAsync(string token, int candidateId)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var invitation = await _context.AssessmentInvitations
                .FirstOrDefaultAsync(x =>
                    x.AssessmentToken == token &&
                    x.CandidateId == candidateId &&
                    x.IsActive &&
                    !x.IsCompleted);

            if (invitation == null || invitation.ExpiryDate < DateTime.Now)
                return null;

            return invitation;
        }

        private async Task<Candidate?> GetCurrentCandidateAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return null;

            return await _context.Candidates
                .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
        }
    }
}
