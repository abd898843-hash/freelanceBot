//using Freelance_Bot.Domain.Entity;
//using Freelance_Bot.Domain.IRepository;
//using Freelance_Bot.Infrastruction.DB;
//using Microsoft.EntityFrameworkCore;
//using System;

//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Freelance_Bot.Infrastruction.Repos
//{
//    public class ReportRepository(FreelancerDbContext db)
//    : Repository<Report>(db), IReportRepository
//    {
//        public async Task<IEnumerable<Report>> GetByProjectIdAsync(Guid projectId)
//            => await _db.reports
//                .Where(r => r.ProjectId == projectId)
//                .OrderByDescending(r => r.CreatedAt)
//                .ToListAsync();

//        public async Task<IEnumerable<Report>> GetByUserIdAsync(Guid userId)
//            => await _db.reports
//                .Include(r => r.Project)
//                .Where(r => r.Project.UserId == userId)
//                .OrderByDescending(r => r.CreatedAt)
//                .ToListAsync();

//        public async Task<Report?> GetLatestForProjectAsync(Guid projectId)
//            => await _db.reports
//                .Where(r => r.ProjectId == projectId)
//                .OrderByDescending(r => r.CreatedAt)
//                .FirstOrDefaultAsync();

//        public async Task UpdateStatusAsync(Guid reportId,
//            string? summary = null,
//            string? suggestions = null)
//        {
//            var report = await _db.reports.FindAsync(reportId);

//            if (report == null)
//                return;

//            if (summary != null)
//                report.AiSummary = summary;

//            if (suggestions != null)
//                report.AiSuggestions = suggestions;

//            report.GeneratedAt = DateTime.UtcNow;

//            await _db.SaveChangesAsync();
//        }

//        public async Task MarkAsSentAsync(Guid reportId, string channel)
//        {
//            var report = await _db.reports.FindAsync(reportId);

//            if (report == null)
//                return;

//            report.SentVia = channel;
//            report.SentAt = DateTime.UtcNow;

//            await _db.SaveChangesAsync();
//        }
//    }
//}
