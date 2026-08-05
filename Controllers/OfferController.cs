using Microsoft.AspNetCore.Mvc;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Controllers
{
    public class OfferController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OfferController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create(int candidateId)
        {
            ViewBag.CandidateId = candidateId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(OfferLetter offer)
        {
            if (!ModelState.IsValid)
                return View(offer);

            var candidate = await _context.Candidates.FindAsync(offer.CandidateId);

            if (candidate == null)
                return NotFound();

            offer.OfferStatus = "Pending";

            _context.OfferLetters.Add(offer);
            candidate.RecruitmentStatus = RecruitmentStage.OfferGenerated;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Offer Letter Generated Successfully.";

            return RedirectToAction("OfferList");
        }

        public IActionResult OfferList()
        {
            var offers = _context.OfferLetters.ToList();

            return View(offers);
        }

        public async Task<IActionResult> AcceptOffer(int id)
        {
            var offer = await _context.OfferLetters.FindAsync(id);

            if (offer == null)
                return NotFound();

            offer.OfferStatus = "Accepted";

            var candidate = await _context.Candidates.FindAsync(offer.CandidateId);

            if (candidate != null)
            {
                candidate.RecruitmentStatus = RecruitmentStage.OnboardingStarted;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "Candidate");
        }

        public async Task<IActionResult> RejectOffer(int id)
        {
            var offer = await _context.OfferLetters.FindAsync(id);

            if (offer == null)
                return NotFound();

            offer.OfferStatus = "Rejected";

            var candidate = await _context.Candidates.FindAsync(offer.CandidateId);

            if (candidate != null)
            {
                candidate.RecruitmentStatus = RecruitmentStage.Rejected;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "Candidate");
        }
    }
}
