using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
using VistaWaysTechRecruitPortal.Models;

namespace VistaWaysTechRecruitPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class InterviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InterviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var interviews = await _context.Interviews
                .OrderByDescending(i => i.InterviewDate)
                .ToListAsync();
            return View(interviews);
        }

        public IActionResult Schedule()
        {
            ViewBag.Candidates = _context.Candidates.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Schedule(Interview interview)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Candidates = _context.Candidates.ToList();
                return View(interview);
            }

            interview.Status = "Scheduled";
            _context.Interviews.Add(interview);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Interview scheduled successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var interview = await _context.Interviews.FindAsync(id);
            if (interview == null)
                return NotFound();

            ViewBag.Candidates = _context.Candidates.ToList();
            return View(interview);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Interview interview)
        {
            if (id != interview.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Candidates = _context.Candidates.ToList();
                return View(interview);
            }

            _context.Update(interview);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Interview updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Feedback(int id)
        {
            var interview = await _context.Interviews.FindAsync(id);
            if (interview == null)
                return NotFound();

            return View(interview);
        }

        [HttpPost]
        public async Task<IActionResult> Feedback(int id, Interview interview)
        {
            if (id != interview.Id)
                return NotFound();

            var existingInterview = await _context.Interviews.FindAsync(id);
            if (existingInterview != null)
            {
                existingInterview.Feedback = interview.Feedback;
                existingInterview.Rating = interview.Rating;
                existingInterview.Status = interview.Status;
                existingInterview.IsSelected = interview.IsSelected;
                existingInterview.CompletedOn = DateTime.Now;

                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Feedback submitted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var interview = await _context.Interviews.FindAsync(id);
            if (interview == null)
                return NotFound();

            return View(interview);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var interview = await _context.Interviews.FindAsync(id);
            if (interview != null)
            {
                _context.Interviews.Remove(interview);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Interview deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
