using Freelance_Bot.Domain.Entity;
using Freelance_Bot.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using TaskStatusEnum = Freelance_Bot.Domain.Enum.TaskStatus;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Freelance_Bot.Infrastruction.DB;

namespace Freelance_Bot.Infrastruction.Repos
{
    public class TaskRepository(FreelancerDbContext db)
    : Repository<TaskItem>(db), ITaskRepository
    {
        public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId)
            => await _db.tasks
                .Include(t => t.SubTasks)
                .Where(t => t.ProjectId == projectId && t.ParentTaskId == null)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(Guid userId)
            => await _db.tasks
                .Include(t => t.Project).ThenInclude(p => p.Client)
                .Where(t => t.Project.UserId == userId
                         && t.DueDate < DateTime.UtcNow
                         && t.Status != TaskStatusEnum.Done
                         && t.Status != TaskStatusEnum.Cancelled)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetOverdueByProjectAsync(Guid projectId)
            => await _db.tasks
                .Where(t => t.ProjectId == projectId
                         && t.DueDate < DateTime.UtcNow
                         && t.Status != TaskStatusEnum.Done)
                .ToListAsync();

        public async Task AddRangeAsync(IEnumerable<TaskItem> tasks)
        {
            Console.WriteLine("Repository AddRange");

            await _db.tasks.AddRangeAsync(tasks);

            Console.WriteLine(_db.ChangeTracker.DebugView.LongView);

            var rows = await _db.SaveChangesAsync();

            Console.WriteLine($"Rows Saved = {rows}");
        }

        public async Task<IEnumerable<TaskItem>> GetByStatusAsync(Guid projectId, TaskStatusEnum status)
            => await _db.tasks
                .Where(t => t.ProjectId == projectId && t.Status == status)
                .ToListAsync();
    }

}
