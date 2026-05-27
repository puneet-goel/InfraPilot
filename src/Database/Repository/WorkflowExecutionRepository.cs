using Database.Domain;
using Database.Entity;
using Database.Infrastructure.Persistence;
using Database.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repository;

public class WorkflowExecutionRepository: IWorkflowExecutionRepository
{
    private readonly AppDbContext _dbContext;

    public WorkflowExecutionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> InsertWorkflowExecutionAsync(Guid workflowId)
    {   
        WorkflowExecutionEntity workflowExecution = new()
        {
            Id = Guid.NewGuid(),
            WorkflowId = workflowId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = "Pending",
        };

        await _dbContext.WorkflowExecution.AddAsync(workflowExecution);
        await _dbContext.SaveChangesAsync();

        return workflowExecution.Id;
    }

    public async Task UpdateWorkflowExecution(GetWorkflowExecution workflowExecution)
    {
        WorkflowExecutionEntity entity = await _dbContext.WorkflowExecution.FirstAsync(e => e.Id == workflowExecution.ExecutionId);

        entity.Status = workflowExecution.Status;
        entity.Reason = workflowExecution.Reason;
        entity.CurrentAgent = workflowExecution.CurrentAgent;
        entity.AgentOutput = workflowExecution.AgentOutput;
        entity.Plan = workflowExecution.WorkflowPlan;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<GetWorkflowExecution?> GetWorkflowExecutionAsync(Guid executorId)
    {
        WorkflowExecutionEntity? entity = await _dbContext.WorkflowExecution.Include(x => x.Workflow).FirstOrDefaultAsync(e => e.Id == executorId);
        return (entity == null) ? null : new()
        {
            ExecutionId = entity.Id,
            WorkflowId = entity.WorkflowId,
            UserRequest = entity.Workflow.UserRequest,
            WorkflowPlan = entity.Plan,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Status = entity.Status,
            Reason = entity.Reason,
            CurrentAgent = entity.CurrentAgent,
            AgentOutput = entity.AgentOutput
        };
    }

    public async Task<List<GetWorkflowExecution>> GetAllWorkflowExecutionAsync()
    {
        return await _dbContext.WorkflowExecution
            .Select(entity => new GetWorkflowExecution()
            {
                ExecutionId = entity.Id,
                WorkflowId = entity.WorkflowId,
                UserRequest = entity.Workflow.UserRequest,
                WorkflowPlan = entity.Plan,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Status = entity.Status,
                CurrentAgent = entity.CurrentAgent,
                AgentOutput = entity.AgentOutput,
                Reason = entity.Reason
            })
            .ToListAsync();
    }
}
