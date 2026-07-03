using Freelance_Bot.Domain.IRepository;
//using Freelance_Bot.Infrastruction.BackgroundServices;
using Freelance_Bot.Infrastruction.Repos;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freelance_Bot.Infrastruction
{
    public static class ModuleInfrastructureDepedences
    {
        public static void AddModuleInfrastructureDepedences( this IServiceCollection services)
        { 
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IInsightRepository, InsightRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
           // services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            // استبدل 'ReportRepository' باسم الكلاس الفعلي الذي ينفذ واجهة IReportRepository
            //services.AddScoped<IReportRepository, ReportRepository>();
            // services.AddScoped<IReportRepository, ReportRepository>();
            ///// background services
            //services.AddHostedService<OverdueTaskDetector>();
            //services.AddHostedService<DeadlineApproachingDetector>();


        }


    }
}
