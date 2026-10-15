using Freelance_bot.Application.Feature.Insight.Request;
using Freelance_bot.Application.Feature.Insight.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freelance_bot.Application.IServieces
{
    public interface IInsightService
    {
        Task<IEnumerable<InsightResponse>> GetActiveAsync(Guid userId);
        Task BulkCreateAsync(BulkInsightsRequest request);   // called by n8n
        Task DismissAsync(Guid insightId, Guid userId);
    }
}
