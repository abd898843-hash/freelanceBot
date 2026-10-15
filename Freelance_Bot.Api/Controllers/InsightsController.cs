using Freelance_bot.Application.Feature.Insight.Request;
using Freelance_bot.Application.IServieces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//  https://go.microsoft.com/fwlink/?LinkID=397860

namespace Freelance_Bot.Api.Controllers
{
    [Route("api/insights")]
    public class InsightsController(IInsightService insightService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetActive()
            => Ok(await insightService.GetActiveAsync(CurrentUserId));

        [HttpPost("bulk")]
        [AllowAnonymous]
        public async Task<IActionResult> BulkCreate(
            [FromBody] BulkInsightsRequest request,
            [FromHeader(Name = "X-N8N-Secret")] string? secret,
            IConfiguration config)
        {
            if (secret != config["N8n:SharedSecret"]) return Unauthorized();
            await insightService.BulkCreateAsync(request);
            return Ok(new { message = $"{request.Insights.Count} insights created" });
        }

        [HttpPost("{id:guid}/dismiss")]
        public async Task<IActionResult> Dismiss(Guid id)
        {
            try { await insightService.DismissAsync(id, CurrentUserId); return NoContent(); }
            catch (KeyNotFoundException) { return NotFound(); }
        }
    }
}

