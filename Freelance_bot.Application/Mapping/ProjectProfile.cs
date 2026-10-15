using AutoMapper;
using Freelance_bot.Application.Feature.Projects.DTOs;
using Freelance_bot.Application.Feature.Projects.Responses;
using Freelance_Bot.Domain.Entity;

namespace Freelance_bot.Application.Mapping
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectResponse>();
           
        }
    }
}