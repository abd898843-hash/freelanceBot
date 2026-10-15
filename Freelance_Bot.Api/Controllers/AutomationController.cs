using Freelance_bot.Application.Feature.Automation;
using Freelance_bot.Application.IServieces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

[ApiController]
[Route("api/automation")]

public class AutomationController(ITaskService taskService, ITelegramBotClient botClient) : ControllerBase
{
    [HttpPost("tasks")]
    public async Task<IActionResult> CreateBulkFromAutomation([FromBody] CreateTasksFromAutomationRequest request)
    {
        var automationTasks = request.Tasks.Select(x => new AutomationTaskDto
        {
            Title = x.Title,
            Notes = x.Notes
        }).ToList();

      
        var result = await taskService.CreateBulkFromAutomationAsync(request.ProjectId, automationTasks);

        if (!result) return BadRequest();
        return Ok(new { success = true });
    }

    [HttpPost("tasks-callback")]
    public async Task<IActionResult> HandleTasksCallback([FromBody] TasksCallbackRequest request)
    {
        try
        {
           
            await botClient.SendTextMessageAsync(
                chatId: request.ChatId,
                text: $"✅ تم إنشاء {request.TasksCount} مهمة بنجاح للمشروع!",
                replyMarkup: new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithCallbackData("📋 عرض المهام", $"tasks_project:{request.ProjectId}")
                )
            );

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Error sending telegram notification", error = ex.Message });
        }
    }

    public class TasksCallbackRequest
    {
        public long ChatId { get; set; }
        public Guid ProjectId { get; set; }
        public int TasksCount { get; set; }
    }
}