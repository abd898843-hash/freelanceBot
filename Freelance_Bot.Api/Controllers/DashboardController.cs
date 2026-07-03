using Freelance_bot.Application.IServieces;
using Freelance_Bot.Infrastruction.DB;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Freelance_Bot.Api.Controllers
{
    public class DashboardStatsDto
    {
        public int totalProjects { get; set; }
        public int totalTasks { get; set; }
        public int completedProjects { get; set; }
        public decimal totalIncome { get; set; }
        public List<ChartDataDto> chartData { get; set; } = new List<ChartDataDto>();
        public List<ProjectItemDto> activeProjects { get; set; } = new List<ProjectItemDto>();
        public List<DeadlineDto> upcomingDeadlines { get; set; } = new List<DeadlineDto>();
    }

    public class ChartDataDto
    {
        public string status { get; set; } = string.Empty;
        public int count { get; set; }
        public decimal totalBudget { get; set; }
    }

    public class ProjectItemDto
    {
        public Guid id { get; set; } // تم الإضافة
        public string title { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public decimal budget { get; set; }
        public double progress { get; set; }
        public int tasksCount { get; set; }
        public DateTime? dueDate { get; set; }
    }

    public class DeadlineDto
    {
        public string projectTitle { get; set; } = string.Empty;
        public int daysLeft { get; set; }
    }


    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly FreelancerDbContext _context;

        public DashboardController(FreelancerDbContext context)
        {
            _context = context;
        }
      
        [HttpGet("project-analytics/{id:guid}")]
        public async Task<IActionResult> GetProject(Guid id)
        {
            var project = await _context.projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (project == null) return NotFound();

            return Ok(new
            {
                id = project.Id,
                title = project.Title,
                budget = project.Budget,
                tasksCount = project.Tasks.Count,
                completedTasks = project.Tasks.Count(t => t.Status.ToString() == "Completed")
            });
        }


        [HttpGet("stats")]
        public async Task<IActionResult> GetProjectStats([FromQuery] string telegramId)
        {
            if (string.IsNullOrEmpty(telegramId) || !long.TryParse(telegramId, out long tId))
                return BadRequest("Invalid telegramId");

            var user = await _context.users.FirstOrDefaultAsync(u => u.TelegramChatId == tId);
            if (user == null) return NotFound("User not found");

            var dbProjects = await _context.projects.Where(p => p.UserId == user.Id).ToListAsync();
            var projectIds = dbProjects.Select(p => p.Id).ToList();
            var allTasks = await _context.tasks.Where(t => projectIds.Contains(t.ProjectId)).ToListAsync();

            var result = new DashboardStatsDto
            {
                totalProjects = dbProjects.Count,
                totalTasks = allTasks.Count,
                completedProjects = dbProjects.Count(p => p.Status.ToString() == "Completed" || p.Status.ToString() == "1"),
                totalIncome = dbProjects.Sum(p => p.Budget ?? 0),

                chartData = dbProjects.GroupBy(p => p.Status.ToString())
                    .Select(g => new ChartDataDto { status = g.Key, count = g.Count() }).ToList(),

                activeProjects = dbProjects.Where(p => p.Status.ToString() == "Active" || p.Status.ToString() == "0")
                    .Select(p => new ProjectItemDto
                    {
                        title = p.Title,
                        status = p.Status.ToString(),
                        budget = p.Budget ?? 0,
                        tasksCount = allTasks.Count(t => t.ProjectId == p.Id),
                        progress = allTasks.Count(t => t.ProjectId == p.Id) == 0 ? 0 :
                                   Math.Round(((double)allTasks.Count(t => t.ProjectId == p.Id && t.Status.ToString() == "Completed") / allTasks.Count(t => t.ProjectId == p.Id)) * 100, 2),
                        dueDate = p.Deadline
                    }).ToList(),

                upcomingDeadlines = dbProjects.Where(p => p.Deadline.HasValue)
                    .Select(p => new DeadlineDto
                    {
                        projectTitle = p.Title,
                        daysLeft = (p.Deadline.Value.Date - DateTime.Today).Days
                    }).OrderBy(d => d.daysLeft).Take(5).ToList()
            };

            return Ok(result);
        }





    }
    //[ApiController]
    //[Route("api/[controller]")]
    //public class DashboardController : ControllerBase
    //{
    //    private readonly FreelancerDbContext _context;
    //    private readonly IProjectService _projectService;

    //    public DashboardController(FreelancerDbContext context, IProjectService projectService)
    //    {
    //        _context = context;
    //        _projectService = projectService;
    //    }

    //    [HttpGet("stats")]
    //    public async Task<IActionResult> GetProjectStats([FromQuery] string telegramId)
    //    {
    //        Console.WriteLine("Dashboard endpoint called. telegramId = " + telegramId);
    //        if (string.IsNullOrEmpty(telegramId))
    //        {
    //            return BadRequest(new { message = "telegramId is required and cannot be empty." });
    //        }

    //        if (!long.TryParse(telegramId, out long parsedTelegramId))
    //        {
    //            return BadRequest(new { message = "Invalid telegramId format. It must be a valid number." });
    //        }

    //        try
    //        {
    //            var user = await _context.users
    //                .FirstOrDefaultAsync(u => u.TelegramChatId == parsedTelegramId);

    //            if (user == null)
    //            {
    //                return NotFound(new { message = $"المستخدم صاحب الرقم {parsedTelegramId} غير مسجل في النظام" });
    //            }

    //            var dbProjects = await _context.projects
    //  .Where(p => p.UserId == user.Id)
    //  .ToListAsync();

    //            int totalProjectsComputed = dbProjects.Count;

    //            int completedProjectsComputed = dbProjects.Count(p =>
    //                p.Status.ToString() == "Completed" ||
    //                p.Status.ToString() == "مكتمل" ||
    //                p.Status.ToString() == "1");

    //            decimal totalIncomeComputed = dbProjects.Sum(p => p.Budget ?? 0);

    //            var chartDataCompiled = dbProjects
    //                .GroupBy(p => p.Status.ToString())
    //                .Select(g => new ChartDataDto
    //                {
    //                    status = g.Key,
    //                    count = g.Count(),
    //                    totalBudget = g.Sum(x => x.Budget ?? 0)
    //                })
    //                .ToList();

    //            var userProjectIds = await _context.projects
    //                .Where(p => p.UserId == user.Id)
    //                .Select(p => p.Id)
    //                .ToListAsync();

    //            int totalTasksComputed = await _context.tasks
    //                .CountAsync(t => userProjectIds.Contains(t.ProjectId));

    //            var allUserTasks = await _context.tasks
    //                .Where(t => userProjectIds.Contains(t.ProjectId))
    //                .Select(t => new { t.ProjectId, t.Status })
    //                .ToListAsync();



    //            var activeProjectsList = new List<ProjectItemDto>();
    //            var upcomingDeadlinesList = new List<DeadlineDto>();
    //            var today = DateTime.Today;

    //            foreach (var p in dbProjects)
    //            {
    //                string statusStr = p.Status.ToString() ?? "";
    //                var projectTasks = allUserTasks.Where(t => t.ProjectId == p.Id).ToList();
    //                int totalProjectTasks = projectTasks.Count;
    //                int completedProjectTasks = projectTasks.Count(t => t.Status.ToString() == "Completed" || t.Status.ToString() == "مكتمل" || t.Status.ToString() == "1");

    //                double progressPercent = totalProjectTasks == 0
    //                    ? 0
    //                    : Math.Round(((double)completedProjectTasks / totalProjectTasks) * 100, 2);

    //                if (statusStr == "Active" || statusStr == "نشط" || statusStr == "0")
    //                {
    //                    activeProjectsList.Add(new ProjectItemDto
    //                    {
    //                        title = p.Title ?? "",
    //                        status = statusStr,
    //                        budget = p.Budget ?? 0,
    //                        tasksCount = totalProjectTasks,
    //                        dueDate = p.Deadline,
    //                        progress = progressPercent
    //                    });
    //                }

    //                if (p.Deadline.HasValue)
    //                {
    //                    int daysLeftCalculated = (p.Deadline.Value.Date - today).Days;
    //                    upcomingDeadlinesList.Add(new DeadlineDto
    //                    {
    //                        projectTitle = p.Title ?? "",
    //                        daysLeft = daysLeftCalculated >= 0 ? daysLeftCalculated : 0
    //                    });
    //                }
    //            }

    //            upcomingDeadlinesList = upcomingDeadlinesList
    //                .OrderBy(d => d.daysLeft)
    //                .Take(5)
    //                .ToList();

    //            var result = new DashboardStatsDto 
    //            {
    //                totalProjects = totalProjectsComputed,
    //                totalTasks = totalTasksComputed,
    //                completedProjects = completedProjectsComputed,
    //                totalIncome = totalIncomeComputed,
    //                chartData = chartDataCompiled,
    //                activeProjects = activeProjectsList,
    //                upcomingDeadlines = upcomingDeadlinesList
    //            };

    //            return Ok(result);
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(new { message = "Internal error context processing failed: " + ex.Message });
    //        }
    //    }

    //    [HttpGet("api/projects/{id}/analytics")]
    //    public async Task<IActionResult> GetProjectAnalytics(Guid id)
    //    {
    //        // هات البيانات للمشروع ده تحديداً
    //        var analytics = await _projectService.GetAnalyticsAsync(id);
    //        return Ok(analytics);
    //    }
    //}
}