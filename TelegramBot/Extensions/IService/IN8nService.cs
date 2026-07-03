using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Extensions.IService
{
    public interface IN8nService
    {
        Task TriggerAutoTasksAsync(Guid projectId);
    }
}
