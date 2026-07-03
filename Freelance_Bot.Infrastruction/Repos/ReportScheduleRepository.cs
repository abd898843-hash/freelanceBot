//using Domain.Entities;
//using Freelance_Bot.Domain.IRepository;
//using Freelance_Bot.Infrastruction.DB;
//using Microsoft.EntityFrameworkCore;

//namespace Freelance_Bot.Infrastruction.Repos
//{
//    public class ReportScheduleRepository
//        : Repository<ReportSchedule>, IReportScheduleRepository
//    {
//        public ReportScheduleRepository(FreelancerDbContext db)
//            : base(db)
//        {
//        }

//        public async Task<List<ReportSchedule>> GetDueSchedulesAsync()
//        {
//            return await _db.ReportSchedules
//                .Include(x => x.Project)
//                .Where(x =>
//                    x.IsActive &&
//                    x.NextRun != null &&
//                    x.NextRun <= DateTime.UtcNow)
//                .OrderBy(x => x.NextRun)
//                .ToListAsync();
//        }

//        public async Task<List<ReportSchedule>> GetByProjectAsync(Guid projectId)
//        {
//            return await _db.ReportSchedules
//                .Where(x => x.ProjectId == projectId)
//                .OrderBy(x => x.CreatedAt)
//                .ToListAsync();
//        }

//        public async Task UpdateAsync(ReportSchedule schedule)
//        {
//            _db.ReportSchedules.Update(schedule);
//            await _db.SaveChangesAsync();
//        }
//        public virtual async Task AddAsync(ReportSchedule schedule)
//        {
//            await _db.Set<ReportSchedule>().AddAsync(schedule);
//            await _db.SaveChangesAsync();
//        }
//    }
//}