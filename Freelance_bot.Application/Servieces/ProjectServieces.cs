using Freelance_bot.Application.Feature.Projects.DTOs;
using Freelance_bot.Application.Feature.Projects.Requests;
using Freelance_bot.Application.Feature.Projects.Responses;
using Freelance_bot.Application.IServieces;
using Freelance_Bot.Domain.Entity;
using Freelance_Bot.Domain.Enum;
using Freelance_Bot.Domain.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskStatusEnum = Freelance_Bot.Domain.Enum.TaskStatus;

namespace Freelance_bot.Application.Servieces
{
    public class ProjectService(
        IProjectRepository projectRepo,
        IEventService eventService,
        ITaskRepository taskRepo,
        IUserRepository userRepository) : IProjectService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IProjectRepository _projectRepository = projectRepo;

        public async Task<ProjectResponse> CreateAsync(Guid userId, CreateProjectRequest request)
        {
            var project = new Project
            {
                UserId = userId,
                Title = request.Title,
                Description = request.Description,
                ClientId = request.ClientId,
                Budget = request.Budget,
                Currency = request.Currency,
                StartDate = request.StartDate ?? DateTime.UtcNow,
                Deadline = request.Deadline,
                Status = ProjectStatus.Active
            };

            await _projectRepository.AddAsync(project);

            await eventService.EmitAsync(userId, "Project", project.Id, "ProjectCreated", new
            {
                project.Id,
                project.Title,
                project.Deadline,
                project.ClientId
            });

            return await MapToResponseAsync(project);
        }

        public async Task<List<Project>> GetProjectsByUserIdAsync(Guid userId)
        {
           
            var projects = await _projectRepository.GetByUserIdAsync(userId);
            return projects.ToList();
        }

        public async Task<ProjectResponse?> GetByIdAsync(Guid id, Guid userId)
        {
            var project = await _projectRepository.GetWithDetailsAsync(id);
            if (project == null || project.UserId != userId) return null;
            return await MapToResponseAsync(project);
        }

        public async Task<IEnumerable<ProjectResponse>> GetAllAsync(Guid userId)
        {
            var projects = await _projectRepository.GetByUserIdAsync(userId);
            var results = new List<ProjectResponse>();
            foreach (var p in projects)
                results.Add(await MapToResponseAsync(p));
            return results;
        }

        public async Task<ProjectResponse> UpdateAsync(Guid id, Guid userId, UpdateProjectRequest request)
        {
            var project = await _projectRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Project not found");

            if (project.UserId != userId) throw new UnauthorizedAccessException();

            if (request.Title != null) project.Title = request.Title;
            if (request.Description != null) project.Description = request.Description;
            if (request.Status.HasValue) project.Status = request.Status.Value;
            if (request.Budget.HasValue) project.Budget = request.Budget.Value;
            if (request.Deadline.HasValue) project.Deadline = request.Deadline.Value;
            if (request.ProgressPct.HasValue) project.ProgressPct = request.ProgressPct.Value;

            await _projectRepository.UpdateAsync(project);
            return await MapToResponseAsync(project);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var project = await _projectRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Project not found");
            if (project.UserId != userId) throw new UnauthorizedAccessException();
            await _projectRepository.DeleteAsync(id);
        }

        // --- Bot Methods ---

        public async Task<List<ProjectBotDto>> GetByTelegramIdAsync(long telegramId)
        {
            var user = await _userRepository.GetByTelegramChatIdAsync(telegramId);

            if (user is null)
                return new List<ProjectBotDto>();

            var projects = await _projectRepository.GetByUserIdAsync(user.Id);

            return projects.Select(p => new ProjectBotDto
            {
                Id = p.Id,
                Title = p.Title
            }).ToList();
        }

        private Task<ProjectResponse> MapToResponseAsync(Project project)
        {
            var response = new ProjectResponse(
                project.Id,
                project.Title,
                project.Description,
                project.Status,
                project.Budget,
                project.Currency,
                project.StartDate,
                project.Deadline,
                project.ProgressPct,
                project.Client?.Name,
                project.Tasks.Count,
                project.Tasks.Count(t => t.Status == TaskStatusEnum.Done),
                project.Tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != TaskStatusEnum.Done),
                project.CreatedAt
            );
            return Task.FromResult(response);
        }
    }
}