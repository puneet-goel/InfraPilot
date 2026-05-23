using Database.Repository.Interfaces;
using Database.Infrastructure.Persistence;
using Database.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Database;

public static class DatabaseServiceExtensions
{
    public static IServiceCollection AddAgentDB(this IServiceCollection services, string connection)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connection);
        });

        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
        services.AddScoped<IWorkflowExecutionRepository, WorkflowExecutionRepository>();
        return services;
    }
}