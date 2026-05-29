using Agents.Engine;
using Agents.Workflow;
using Api.Application.DTO;
using Api.Application.Interface;
using Database.Domain;
using Database.Repository.Interfaces;
using Hangfire;
using System.Text.Json;

namespace Api.Application.Service;

public class WorkflowExecutionService(IWorkflowExecutionRepository workflowExecutionRepository) : IWorkflowExecutionService
{
    private readonly IWorkflowExecutionRepository _workflowExecutionRepository = workflowExecutionRepository;

    public async Task<GetWorkflowExecution?> GetWorkflowExecutionStatusAsync(string executionId)
    {
        return await _workflowExecutionRepository.GetWorkflowExecutionAsync(Guid.Parse(executionId));
    }

    public async Task<List<GetWorkflowExecution>> GetAllWorkflowExecutionStatusAsync()
    {
        return await _workflowExecutionRepository.GetAllWorkflowExecutionAsync();
    }

    public async Task AcceptWorkflowExecution(AcceptWorkflowExecution req)
    {
        GetWorkflowExecution? workflowExecution = await _workflowExecutionRepository.GetWorkflowExecutionAsync(Guid.Parse(req.ExecutionId));
        if (workflowExecution == null)
        {
            throw new Exception($"Wrong Id");
        }

        WorkflowPlanResult? lastOutputSaved = JsonSerializer.Deserialize<WorkflowPlanResult>(workflowExecution.AgentOutput);

        AgentOutput lastAgentOutput = lastOutputSaved.Steps.Last();
        if (req.Message != null)
        {
            lastAgentOutput.Chat.Add(new()
            {
                Role = "user",
                Text = req.Message,
                IsApprovalRequired = false
            });
        }

        AgentChatMessage lastAgentChatMessage = lastAgentOutput.Chat.Last();

        lastAgentChatMessage.ApprovalStatus = req.Accept ? "Approved" : "Rejected";
        lastAgentChatMessage.ApprovalReason = req.Reason;
        workflowExecution.AgentOutput = JsonSerializer.Serialize(lastOutputSaved);
        workflowExecution.Status = req.Accept ? "Approved" : "Rejected";

        await _workflowExecutionRepository.UpdateWorkflowExecution(workflowExecution);

        if (req.Accept)
        {
            BackgroundJob.Enqueue<IWorkflowEngine>(x => x.ExecuteAsync(Guid.Parse(req.ExecutionId)));
        }
    }
}