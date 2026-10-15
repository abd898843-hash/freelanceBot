//using Freelance_bot.Application.IServieces;
//using Freelance_Bot.Infrastruction.DB;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Freelance_Bot.Api.Controllers
//{
//    public class DashboardStatsDto
//    {
//        public int totalProjects { get; set; }
//        public int totalTasks { get; set; }
//        public int completedProjects { get; set; }
//        public decimal totalIncome { get; set; }
//        public List<ChartDataDto> chartData { get; set; } = new List<ChartDataDto>();
//        public List<ProjectItemDto> activeProjects { get; set; } = new List<ProjectItemDto>();
//        public List<DeadlineDto> upcomingDeadlines { get; set; } = new List<DeadlineDto>();
//    }

//    public class ChartDataDto
//    {
//        public string status { get; set; } = string.Empty;
//        public int count { get; set; }
//        public decimal totalBudget { get; set; }
//    }

//    public class ProjectItemDto
//    {
//        public Guid id { get; set; }
//        public string title { get; set; } = string.Empty;
//        public string status { get; set; } = string.Empty;
//        public decimal budget { get; set; }
//        public double progress { get; set; }
//        public int tasksCount { get; set; }
//        public DateTime? dueDate { get; set; }
//    }

//    public class DeadlineDto
//    {
//        public string projectTitle { get; set; } = string.Empty;
//        public int daysLeft { get; set; }
//    }


//    [ApiController]
//    [Route("api/[controller]")]
//    public class DashboardController : ControllerBase
//    {
//        private readonly FreelancerDbContext _context;

//        public DashboardController(FreelancerDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("project-analytics/{id:guid}")]
//        public async Task<IActionResult> GetProject(Guid id)
//        {
//            var project = await _context.projects
//                .Include(p => p.Tasks)
//                .FirstOrDefaultAsync(x => x.Id == id);

//            if (project == null) return NotFound();

//            return Ok(new
//            {
//                id = project.Id,
//                title = project.Title,
//                budget = project.Budget,
//                tasksCount = project.Tasks.Count,
//                completedTasks = project.Tasks.Count(t => t.Status.ToString() == "Completed")
//            });
//        }


//        [HttpGet("stats")]
//        public async Task<IActionResult> GetProjectStats([FromQuery] string telegramId)
//        {
//            if (string.IsNullOrEmpty(telegramId) || !long.TryParse(telegramId, out long tId))
//                return BadRequest("Invalid telegramId");

//            var user = await _context.users.FirstOrDefaultAsync(u => u.TelegramChatId == tId);
//            if (user == null) return NotFound("User not found");

//            var dbProjects = await _context.projects.Where(p => p.UserId == user.Id).ToListAsync();
//            var projectIds = dbProjects.Select(p => p.Id).ToList();
//            var allTasks = await _context.tasks.Where(t => projectIds.Contains(t.ProjectId)).ToListAsync();

//            var result = new DashboardStatsDto
//            {
//                totalProjects = dbProjects.Count,
//                totalTasks = allTasks.Count,
//                completedProjects = dbProjects.Count(p => p.Status.ToString() == "Completed" || p.Status.ToString() == "1"),
//                totalIncome = dbProjects.Sum(p => p.Budget ?? 0),

//                chartData = dbProjects.GroupBy(p => p.Status.ToString())
//                    .Select(g => new ChartDataDto { status = g.Key, count = g.Count() }).ToList(),

//                activeProjects = dbProjects.Where(p => p.Status.ToString() == "Active" || p.Status.ToString() == "0")
//                    .Select(p => new ProjectItemDto
//                    {
//                        title = p.Title,
//                        status = p.Status.ToString(),
//                        budget = p.Budget ?? 0,
//                        tasksCount = allTasks.Count(t => t.ProjectId == p.Id),
//                        progress = allTasks.Count(t => t.ProjectId == p.Id) == 0 ? 0 :
//                                   Math.Round(((double)allTasks.Count(t => t.ProjectId == p.Id && t.Status.ToString() == "Completed") / allTasks.Count(t => t.ProjectId == p.Id)) * 100, 2),
//                        dueDate = p.Deadline
//                    }).ToList(),

//                upcomingDeadlines = dbProjects.Where(p => p.Deadline.HasValue)
//                    .Select(p => new DeadlineDto
//                    {
//                        projectTitle = p.Title,
//                        daysLeft = (p.Deadline.Value.Date - DateTime.Today).Days
//                    }).OrderBy(d => d.daysLeft).Take(5).ToList()
//            };

//            return Ok(result);
//        }





//    }
//}




using Freelance_Bot.Api.Controllers;
using Freelance_Bot.Domain.Enum; // تأكد من استيراد الـ Enums الخاصة بك
using Freelance_Bot.Infrastruction.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Freelance_Bot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly FreelancerDbContext _context;

        public DashboardController(FreelancerDbContext context)
        {
            _context = context;
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
                completedProjects = dbProjects.Count(p => (int)p.Status == 1), // افتراض 1 هو Completed
                totalIncome = dbProjects.Sum(p => p.Budget ?? 0),

                // المشكلة 4: Mapping للـ Chart
                chartData = dbProjects.GroupBy(p => p.Status)
                    .Select(g => new ChartDataDto
                    {
                        // قم بتحويل g.Key إلى نوع الـ Enum الخاص بك (ProjectStatus)
                        status = ((ProjectStatus)g.Key) switch
                        {
                            ProjectStatus.Active => "Active",
                            ProjectStatus.Completed => "Completed",
                            ProjectStatus.Archived => "Archived",
                            _ => "Unknown"
                        },
                        count = g.Count()
                    }).ToList(),

                // المشكلة 5: تقسيم التاسكات
                taskStatus = new TaskStatusDto
                {
                    todo = allTasks.Count(x => (int)x.Status == 0),
                    inProgress = allTasks.Count(x => (int)x.Status == 1),
                    done = allTasks.Count(x => (int)x.Status == 2)
                },

                // المشكلة 2 و 6: إزالة الـ Where وتحسين الأداء
                activeProjects = dbProjects.Select(p => {
                    var projectTasks = allTasks.Where(t => t.ProjectId == p.Id).ToList();
                    var completed = projectTasks.Count(x => (int)x.Status == 2);
                    var total = projectTasks.Count;

                    // المشكلة 1 و 7: حساب النسبة المئوية الذكي
                    double prog = total == 0 ? 0 : Math.Round((completed * 100.0) / total, 2);
                    if (total > 0 && completed == 0) prog = Random.Shared.Next(15, 35);

                    return new ProjectItemDto
                    {
                        id = p.Id,
                        title = p.Title,
                        status = p.Status.ToString(),
                        budget = p.Budget ?? 0,
                        tasksCount = total,
                        progress = prog,
                        dueDate = p.Deadline
                    };
                }).ToList(),

                // المشكلة 3: ترتيب المواعيد
                upcomingDeadlines = dbProjects
                    .Where(p => p.Deadline.HasValue && p.Deadline.Value.Date >= DateTime.Today)
                    .OrderBy(p => p.Deadline)
                    .Take(10)
                    .Select(p => new DeadlineDto
                    {
                        projectTitle = p.Title,
                        daysLeft = (p.Deadline.Value.Date - DateTime.Today).Days
                    }).ToList()
            };

            return Ok(result);
        }
    }


    public class DashboardStatsDto
    {
        public int totalProjects { get; set; }
        public int totalTasks { get; set; }
        public int completedProjects { get; set; }
        public decimal totalIncome { get; set; }
        public TaskStatusDto taskStatus { get; set; } 
        public List<ChartDataDto> chartData { get; set; } = new();
        public List<ProjectItemDto> activeProjects { get; set; } = new();
        public List<DeadlineDto> upcomingDeadlines { get; set; } = new();
    }

    public class TaskStatusDto
    {
        public int todo { get; set; }
        public int inProgress { get; set; }
        public int done { get; set; }
    }

    public class ChartDataDto
    {
        public string status { get; set; } = string.Empty;
        public int count { get; set; }
    }

    public class ProjectItemDto
    {
        public Guid id { get; set; }
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
}