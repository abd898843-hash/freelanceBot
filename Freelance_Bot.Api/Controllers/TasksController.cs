using Freelance_bot.Application.Feature.Tasks.Request;
using Freelance_bot.Application.IServieces;
using Microsoft.AspNetCore.Mvc;

// https://go.microsoft.com/fwlink/?LinkID=397860

namespace Freelance_Bot.Api.Controllers
{
    [Route("api/tasks")]
    public class TasksController(ITaskService taskService) : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        {
            var task = await taskService.CreateAsync(CurrentUserId, request);
            return CreatedAtAction(nameof(GetByProject), new { projectId = task.ProjectId }, task);
        }


        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] BulkCreateTasksRequest request)
        {
            var tasks = await taskService.CreateBulkAsync(CurrentUserId, request);
            return Ok(tasks);
        }

        [HttpGet("project/{projectId:guid}")]
        public async Task<IActionResult> GetByProject(Guid projectId)
            => Ok(await taskService.GetByProjectAsync(projectId, CurrentUserId));

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request)
        {
            try { return Ok(await taskService.UpdateAsync(id, CurrentUserId, request)); }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try { await taskService.DeleteAsync(id, CurrentUserId); return NoContent(); }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        // For n8n notifications cron
        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdue()
            => Ok(await taskService.GetOverdueAsync(CurrentUserId));
    }

}
