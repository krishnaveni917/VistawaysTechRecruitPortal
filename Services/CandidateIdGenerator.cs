using Microsoft.EntityFrameworkCore;
using VistawaysTechRecruitPortal.Data;
//using VistaWaysTechRecruitPortal.Data;

namespace VistaWaysTechRecruitPortal.Services
{
    public class CandidateIdGenerator
    {
        private readonly ApplicationDbContext _context;

        public CandidateIdGenerator(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateCandidateIdAsync()
        {
            int count = await _context.Candidates.CountAsync() + 1;

            return $"VWT{DateTime.Now.Year}{count:D5}";
        }
    }
}