using Database.Repository.Interfaces;
using Database.Domain;
using Database.Entity;
using Database.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Database.Repository;

public class WorkflowRepository: IWorkflowRepository
{
    private readonly AppDbContext _dbContext;

    public WorkflowRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateWorkflow> InsertWorkflowAsync(string userRequest)
    {
        WorkflowEntity workflow = new()
        {
            Id = Guid.NewGuid(),
            UserRequest = userRequest,
            Plan = JsonSerializer.Serialize(new object()),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Workflows
            .Add(workflow);

        await _dbContext
            .SaveChangesAsync();

        return new()
        {
            Id = workflow.Id,
            Plan = workflow.Plan
        };
    }

    public async Task UpdateWorkflowAsync(Guid workflowId, string plan)
    {
        WorkflowEntity? entity = await _dbContext
            .Workflows
            .FirstOrDefaultAsync(x => x.Id == workflowId);

        if (entity == null)
        {
            return;
        }

        entity.Plan = plan;

        await _dbContext
            .SaveChangesAsync();
    }

    public async Task<GetWorkflow?> GetWorkflowAsync(Guid workflowId)
    {
         WorkflowEntity? entity = await _dbContext
            .Workflows
            .FirstOrDefaultAsync(x => x.Id == workflowId);

        return (entity == null) ? null : new()
        {
            Id = entity.Id,
            UserRequest = entity.UserRequest,
            Plan = entity.Plan
        };
    }

    public async Task<List<GetWorkflow>> GetAllWorkflowAsync()
    {
        return await _dbContext
           .Workflows
           .Select(entity => new GetWorkflow()
           {
               Id = entity.Id,
               UserRequest = entity.UserRequest,
               Plan = entity.Plan
           })
           .ToListAsync();
    }
}