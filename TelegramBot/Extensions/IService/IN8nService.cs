using Freelance_bot.Application.Feature.Automation; 
using Freelance_bot.Application.Feature.Automation.Freelance_bot.Application.Feature.Automation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TelegramBot.Extensions.IService
{
    public interface IN8nService
    {
        
        Task TriggerAutoTasksAsync(GenerateTasksPayload payload);
    }
}