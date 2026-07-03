using FreelanceOS.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TelegramBot.Handlers;

namespace FreelanceOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramController : ControllerBase
    {
        private readonly BotUpdateHandler _handler;

        public TelegramController(BotUpdateHandler handler)
        {
            _handler = handler;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            // تمرير الـ update بالكامل للـ Handler
            await _handler.HandleAsync(update);
            return Ok();
        }
    }
}