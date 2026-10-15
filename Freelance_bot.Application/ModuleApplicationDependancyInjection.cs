using Freelance_bot.Application.IServieces;
using Freelance_bot.Application.Servieces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Freelance_bot.Application
{
    public static class ModuleApplicationDependancyInjection
    {
        public static void AddModuleApplicationDependancyInjection( this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(
                    typeof(ModuleApplicationDependancyInjection).Assembly));

         
            services.AddAutoMapper(cfg => { },
                     typeof(ModuleApplicationDependancyInjection).Assembly);
            services.AddScoped<IClientService, ClientService>();

            services.AddScoped<IProjectService, ProjectService>();

            services.AddScoped<IClientService, ClientService>();

            services.AddScoped<ITaskService, TaskService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEventService, EventService>();

            services.AddScoped<IInsightService, InsightService>();

            services.AddScoped<INotificationService, NotificationService>();
          //  services.AddScoped<IReportService, ReportService>();
        }

    }
}