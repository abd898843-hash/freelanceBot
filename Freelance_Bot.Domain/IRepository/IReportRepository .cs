//using Freelance_Bot.Domain.Entity;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Freelance_Bot.Domain.IRepository
//{
//    public interface IReportRepository : IRepository<Report>
//    {
//        Task<IEnumerable<Report>> GetByProjectIdAsync(Guid projectId);
//        Task<IEnumerable<Report>> GetByUserIdAsync(Guid userId);
//        Task<Report?> GetLatestForProjectAsync(Guid projectId);
//        Task UpdateStatusAsync(Guid reportId, /*ReportStatus status,*/ string? summary = null, string? suggestions = null);
//        Task MarkAsSentAsync(Guid reportId, string channel);
//    }
//}
