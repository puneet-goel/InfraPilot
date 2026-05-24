using Database.Domain;
using Database.Entity;
using Database.Infrastructure.Persistence;
using Database.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
            UpdatedAt = DateTime.UtcNow,
            AgentOutput = JsonSerializer.Serialize(new List<string>()),
            CurrentAgent = "OrchestratorAgent",
            Status = "Started",
            Reason = string.Empty
        };

        await _dbContext.WorkflowExecution.AddAsync(workflowExecution);
        await _dbContext.SaveChangesAsync();

        return workflowExecution.Id;
    }

    public async Task UpdateWorkflowExecutionStatus(Guid executionId, string status, string? reason)
    {
        WorkflowExecutionEntity entity = await _dbContext.WorkflowExecution.FirstAsync(e => e.Id == executionId);

        entity.Status = status;
        if (reason != null)
        {
            entity.Reason = reason;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateWorkflowExecutionAgent(Guid executionId, string currentAgent, string agentOutput)
    {
        WorkflowExecutionEntity entity = await _dbContext.WorkflowExecution.FirstAsync(e => e.Id == executionId);

        entity.CurrentAgent = currentAgent;
        entity.AgentOutput = agentOutput;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<GetWorkflowExecution?> GetWorkflowExecutionAsync(Guid workflowId)
    {
        WorkflowExecutionEntity? entity = await _dbContext.WorkflowExecution.FirstOrDefaultAsync(e => e.Id == workflowId);
        return (entity == null) ? null : new()
        {
            WorkflowId = entity.WorkflowId,
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
                WorkflowId = entity.WorkflowId,
                Status = entity.Status,
                Reason = entity.Reason,
                CurrentAgent = entity.CurrentAgent,
                AgentOutput = entity.AgentOutput
            })
            .ToListAsync();
    }
}
