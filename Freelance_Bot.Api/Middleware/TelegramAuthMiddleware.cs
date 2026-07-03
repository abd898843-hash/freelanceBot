//using Microsoft.AspNetCore.Http;
//using System.Threading.Tasks;
//using Freelance_Bot.Domain.IRepository;

//namespace Freelance_Bot.Api.Middleware
//{
//    public class TelegramAuthMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public TelegramAuthMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
//        {
//            if (context.Request.Headers.TryGetValue("X-Telegram-User-Id", out var telegramIdStr))
//            {
//                if (!string.IsNullOrEmpty(telegramIdStr))
//                {
//                    var user = await userRepository.GetByTelegramIdAsync(telegramIdStr.ToString());
//                    if (user != null)
//                    {
//                        context.Items["CurrentUserId"] = user.Id;
//                        context.Items["TelegramUserId"] = telegramIdStr.ToString();
//                    }
//                }
//            }

//            await _next(context);
//        }
//    }
//}
