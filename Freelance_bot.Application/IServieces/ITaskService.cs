using Freelance_bot.Application.Feature.Automation;
using Freelance_bot.Application.Feature.Tasks.Request;
using Freelance_bot.Application.Feature.Tasks.Response;
using Freelance_Bot.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Freelance_bot.Application.IServieces
{
    public interface ITaskService
    {
      
        Task<TaskResponse> GetByIdAsync(Guid id, Guid userId);

       
        Task<TaskResponse> CreateAsync(Guid userId, CreateTaskRequest request);
        Task<IEnumerable<TaskResponse>> CreateBulkAsync(Guid userId, BulkCreateTasksRequest request);

      
        Task<bool> CreateBulkFromAutomationAsync(Guid projectId, List<AutomationTaskDto> tasks);

        Task<IEnumerable<TaskResponse>> GetByProjectAsync(Guid projectId, Guid userId);
        Task<TaskResponse> UpdateAsync(Guid id, Guid userId, UpdateTaskRequest request);
        Task DeleteAsync(Guid id, Guid userId);
        Task<IEnumerable<OverdueTaskResponse>> GetOverdueAsync(Guid userId);
        Task<List<Project>> GetProjectsByUserIdAsync(Guid userId);
        Task<IEnumerable<TaskResponse>> GetTasksByProjectIdAsync(Guid projectId);
    }
}
