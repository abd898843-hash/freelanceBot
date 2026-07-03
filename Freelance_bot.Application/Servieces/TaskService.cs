using Freelance_bot.Application.Feature.Automation;
using Freelance_bot.Application.Feature.Tasks.Request;
using Freelance_bot.Application.Feature.Tasks.Response;
using Freelance_bot.Application.IServieces;
using Freelance_Bot.Domain.Entity;
using Freelance_Bot.Domain.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatusEnum = Freelance_Bot.Domain.Enum.TaskStatus;

namespace Freelance_bot.Application.Servieces
{
    public class TaskService(
    ITaskRepository taskRepo,
    IProjectRepository projectRepo,
    IEventService eventService) : ITaskService
    {
        public async Task<TaskResponse> CreateAsync(Guid userId, CreateTaskRequest request)
        {
            // Verify project ownership
            var project = await projectRepo.GetByIdAsync(request.ProjectId)
                ?? throw new KeyNotFoundException("Project not found");
            if (project.UserId != userId) throw new UnauthorizedAccessException();

            var task = new TaskItem
            {
                ProjectId = request.ProjectId,
                Title = request.Title,
                Notes = request.Notes,
                Priority = request.Priority,
                DueDate = request.DueDate,
                ParentTaskId = request.ParentTaskId
            };

            await taskRepo.AddAsync(task);
            await projectRepo.RecalculateProgressAsync(request.ProjectId);

            return MapToResponse(task, project.Title);
        }

        public async Task<IEnumerable<TaskResponse>> CreateBulkAsync(Guid userId, BulkCreateTasksRequest request)
        {
            var project = await projectRepo.GetByIdAsync(request.ProjectId)
                ?? throw new KeyNotFoundException("Project not found");
            if (project.UserId != userId) throw new UnauthorizedAccessException();

            var tasks = request.Tasks.Select(t => new TaskItem
            {
                ProjectId = request.ProjectId,
                Title = t.Title,
                Notes = t.Notes,
                Priority = t.Priority,
                DueDate = t.DueDate,
                IsDefault = true // flagged as Smart Setup tasks
            }).ToList();

            await taskRepo.AddRangeAsync(tasks);
            await projectRepo.RecalculateProgressAsync(request.ProjectId);

            return tasks.Select(t => MapToResponse(t, project.Title));
        }

        public async Task<IEnumerable<TaskResponse>> GetByProjectAsync(Guid projectId, Guid userId)
        {
            var project = await projectRepo.GetByIdAsync(projectId)
                ?? throw new KeyNotFoundException("Project not found");
            if (project.UserId != userId) throw new UnauthorizedAccessException();

            var tasks = await taskRepo.GetByProjectIdAsync(projectId);
            return tasks.Select(t => MapToResponse(t, project.Title));
        }

        public async Task<TaskResponse> UpdateAsync(Guid id, Guid userId, UpdateTaskRequest request)
        {
            var task = await taskRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Task not found");

            var project = await projectRepo.GetByIdAsync(task.ProjectId)!;
            if (project!.UserId != userId) throw new UnauthorizedAccessException();

            if (request.Title != null) task.Title = request.Title;
            if (request.Notes != null) task.Notes = request.Notes;
            if (request.Status.HasValue)
            {
                task.Status = request.Status.Value;
                if (request.Status.Value == TaskStatusEnum.Done)
                    task.CompletedAt = DateTime.UtcNow;

                // Emit TaskOverdue event if marking as overdue
                if (task.DueDate < DateTime.UtcNow && request.Status.Value != TaskStatusEnum.Done)
                    await eventService.EmitAsync(userId, "Task", task.Id, "TaskOverdue",
                        new { task.Id, task.Title, task.ProjectId, task.DueDate });
            }
            if (request.Priority.HasValue) task.Priority = request.Priority.Value;
            if (request.DueDate.HasValue) task.DueDate = request.DueDate.Value;

            await taskRepo.UpdateAsync(task);
            await projectRepo.RecalculateProgressAsync(task.ProjectId);

            return MapToResponse(task, project.Title);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var task = await taskRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Task not found");
            var project = await projectRepo.GetByIdAsync(task.ProjectId);
            if (project?.UserId != userId) throw new UnauthorizedAccessException();

            await taskRepo.DeleteAsync(id);
            await projectRepo.RecalculateProgressAsync(task.ProjectId);
        }

        public async Task<IEnumerable<OverdueTaskResponse>> GetOverdueAsync(Guid userId)
        {
            var tasks = await taskRepo.GetOverdueTasksAsync(userId);
            return tasks.Select(t => new OverdueTaskResponse(
                t.Id, t.Title, t.Project.Title, t.ProjectId,
                t.Project.Client?.Name, t.DueDate!.Value,
                (int)(DateTime.UtcNow - t.DueDate!.Value).TotalDays
            ));
        }

        private static TaskResponse MapToResponse(TaskItem task, string projectTitle)
            => new(
                task.Id, task.ProjectId, projectTitle, task.Title, task.Notes,
                task.Status, task.Priority, task.DueDate,
                task.DueDate < DateTime.UtcNow && task.Status != TaskStatusEnum.Done,
                task.IsDefault,
                task.SubTasks?.Select(s => MapToResponse(s, projectTitle)).ToList() ?? [],
                task.CreatedAt
            );
        public async Task<TaskResponse> GetByIdAsync(Guid id, Guid userId)
        {
            var task = await taskRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Task not found");

            var project = await projectRepo.GetByIdAsync(task.ProjectId)
                ?? throw new KeyNotFoundException("Project not found");

            if (project.UserId != userId)
                throw new UnauthorizedAccessException();

            return MapToResponse(task, project.Title);
        }
        // هذه هي الميثود التي كانت ناقصة في الـ Implementation
        public async Task<bool> CreateBulkFromAutomationAsync(Guid projectId, Guid userId, List<AutomationTaskDto> tasks)
        {
            var project = await projectRepo.GetByIdAsync(projectId)
                ?? throw new KeyNotFoundException("Project not found");

            if (project.UserId != userId)
                throw new UnauthorizedAccessException();

            if (tasks == null || !tasks.Any()) return false;

            var tasksToSave = tasks.Select(t => new TaskItem
            {
                ProjectId = project.Id,
                Title = t.Title,
                Notes = t.Notes,
                Priority = Freelance_Bot.Domain.Enum.TaskPriority.Medium, // تأكد من الـ Enum الصحيح
                DueDate = DateTime.UtcNow.AddDays(3),
                IsDefault = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            await taskRepo.AddRangeAsync(tasksToSave);
            await projectRepo.RecalculateProgressAsync(project.Id);

            return true;
        }
        public async Task<IEnumerable<TaskResponse>> GetTasksByProjectIdAsync(Guid projectId)
        {
            // جلب المشروع أولاً للتأكد من وجوده وجلب اسمه
            var project = await projectRepo.GetByIdAsync(projectId)
                ?? throw new KeyNotFoundException("Project not found");

            // جلب التاسكات
            var tasks = await taskRepo.GetByProjectIdAsync(projectId);

            // تحويلها إلى TaskResponse
            return tasks.Select(t => MapToResponse(t, project.Title));
        }
        public async Task<List<Project>> GetProjectsByUserIdAsync(Guid userId)
        {
            // جلب البيانات وتحويلها إلى قائمة (List) لتتوافق مع نوع الإرجاع
            var projects = await projectRepo.GetByUserIdAsync(userId);
            return projects.ToList();
        }
    }


}
