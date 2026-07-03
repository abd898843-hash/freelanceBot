using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Extensions.IService;

namespace TelegramBot.Extensions.Service
{
    public class N8nService : IN8nService
    {
        private readonly HttpClient _http;

        public N8nService(HttpClient http)
        {
            _http = http;
        }

        public async Task TriggerAutoTasksAsync(Guid projectId)
        {
            await _http.PostAsJsonAsync(
                "https://xxxxxxxx.ngrok-free.app/webhook/auto-generate-tasks",
                new
                {
                    ProjectId = projectId
                });
        }
    }
    //public class N8NResponse
    //{
    //    // الاسم هنا يجب أن يطابق تماماً المفتاح المرسل من الـ n8n (حالة الأحرف مهمة)
    //    public List<TaskItem> Tasks { get; set; }
    //}

    public class TaskItem
    {
        public string Title { get; set; }
        public string Notes { get; set; }
    }
}
