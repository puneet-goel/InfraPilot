using Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Database.Infrastructure.Persistence;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

    public DbSet<WorkflowEntity> Workflows { get; set; }

    public DbSet<WorkflowExecutionEntity> WorkflowExecution { get; set; }
}