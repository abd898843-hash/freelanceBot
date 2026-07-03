using Freelance_bot.Application.IServieces;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Freelance_Bot.Api.Controllers
{
    public class DashboardStatsDto
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedProjects { get; set; }
        public decimal TotalIncome { get; set; }
        public List<ChartDataDto> ChartData { get; set; } = new List<ChartDataDto>();
        public List<ProjectItemDto> ActiveProjects { get; set; } = new List<ProjectItemDto>();
        public List<DeadlineDto> UpcomingDeadlines { get; set; } = new List<DeadlineDto>();
    }

    public class ChartDataDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalBudget { get; set; }
    }

    public class ProjectItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public double Progress { get; set; }
        public int TasksCount { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class DeadlineDto
    {
        public string ProjectTitle { get; set; } = string.Empty;
        public int DaysLeft { get; set; }
    }
}