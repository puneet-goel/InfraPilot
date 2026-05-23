using Database.Repository.Interfaces;
using Database.Domain;
using Database.Entity;
using Database.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Database.Repository;

public class WorkflowRepository: IWorkflowRepository
{
    private readonly AppDbContext _dbContext;

    public WorkflowRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateWorkflow> InsertWorkflowAsync(string userRequest, string plan)
    {
        WorkflowEntity workflow = new()
        {
            Id = Guid.NewGuid(),
            UserRequest = userRequest,
            Plan = plan,
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