export type WorkflowExecution = {
	workflowId: string
  executionId: string
	userRequest: string
  workflowPlan: string
	status: string
	agentOutput: string
	currentAgent: string
}

export type WorkflowPlan = {
	RuntimeEnvironment: string
	Steps: WorkflowStep[]
}

export type WorkflowStep = {
	AgentName: string
	Task: string
}