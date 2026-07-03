using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Extensions.Service
{
    public class GeneratedTask
    {
        public string Title { get; set; }
        public string Notes { get; set; }
    }

    public class N8NResponse
    {
        public List<GeneratedTask> Tasks { get; set; }
    }
}