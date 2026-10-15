using Freelance_bot.Application.IServieces;
using Microsoft.AspNetCore.Mvc;

//  https://go.microsoft.com/fwlink/?LinkID=397860

namespace Freelance_Bot.Api.Controllers
{
    [Route("api/notifications")]
    public class NotificationsController(INotificationService notifService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await notifService.GetByUserAsync(CurrentUserId));

        [HttpPost("{id:guid}/read")]
        public async Task<IActionResult> MarkRead(Guid id)
        {
            try { await notifService.MarkReadAsync(id, CurrentUserId); return NoContent(); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            await notifService.MarkAllReadAsync(CurrentUserId);
            return NoContent();
        }
    }
}
