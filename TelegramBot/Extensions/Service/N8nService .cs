using Freelance_bot.Application.Feature.Automation.Freelance_bot.Application.Feature.Automation;
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
        public N8nService(HttpClient http) => _http = http;

        public async Task TriggerAutoTasksAsync(GenerateTasksPayload payload)
        {
           
            string n8nUrl = "http://localhost:5678/webhook-test/create-project-tasks";

            await _http.PostAsJsonAsync(n8nUrl, payload);
        }
    }
   
    public class TaskItem
    {
        public string Title { get; set; }
        public string Notes { get; set; }
    }
}
