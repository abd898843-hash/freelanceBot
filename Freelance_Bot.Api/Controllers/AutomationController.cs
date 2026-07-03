using Freelance_bot.Application.Feature.Reports.Request;
using Freelance_bot.Application.Feature.Reports.Response;
using Freelance_bot.Application.IServieces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/automation")]
public class AutomationController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IReportService _reportService; // أضفنا خدمة التقارير

    public AutomationController(
        ITaskService taskService,
        IReportService reportService)
    {
        _taskService = taskService;
        _reportService = reportService;
    }

    // الاندبوينت الخاص بإنشاء المهام (كما كان موجوداً سابقاً)
    [HttpPost("tasks")]
    public async Task<IActionResult> CreateTasks(
       [FromBody] Freelance_bot.Application.Feature.Automation.CreateTasksFromAutomationRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException());

        // قم بعمل تحويل للقائمة من TaskItemDto إلى AutomationTaskDto
        var automationTasks = request.Tasks.Select(t => new Freelance_bot.Application.Feature.Automation.AutomationTaskDto
        {
            Title = t.Title,
            Notes = t.Notes
            // أضف أي خصائص أخرى إذا كانت موجودة في الـ DTO
        }).ToList();

        // الآن مرر القائمة المحولة
        var result = await _taskService.CreateBulkFromAutomationAsync(
            request.ProjectId,
            userId,
            automationTasks
        );

        if (!result)
            return BadRequest(new { message = "Failed to create tasks via automation." });

        return Ok(new { success = true });
    }
    // الاندبوينت الجديد الخاص بـ AI Reporting Callback (مهم جداً للـ n8n)
    [HttpPost("report-callback")]
    public async Task<IActionResult> HandleAiCallback([FromBody] ReportAiResultCallback callback)
    {
        await _reportService.HandleAiCallbackAsync(callback);
        return Ok(new { success = true });
    }
}