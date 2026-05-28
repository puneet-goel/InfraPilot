export type WorkflowExecution = {
	workflowId: string
	executionId: string
	userRequest: string
	workflowPlan: string
	status: string
	agentOutput: string
	currentAgent: string
	reason: string
}

export type WorkflowPlan = {
	RuntimeEnvironment: string
	Steps: WorkflowStep[]
}

export type WorkflowStep = {
	AgentName: string
	Task: string
}

export type ToolCall = {
	ToolCallId: string
	ToolName: string
	Arguments?: Record<string, unknown>
	Result?: unknown
	Exception?: unknown
}

export type AgentChatMessage = {
	Role: string
	Text: string
	ToolCalls: ToolCall[]
	IsApprovalRequired: boolean
	ApprovalStatus: 'pending' | 'accepted' | 'rejected' | ''
	ApprovalReason: string
}

export type AgentOutput = {
	AgentName: string
	Chat: AgentChatMessage[]
}

export type WorkflowPlanResult = {
	RuntimeEnvironment: string
	Steps: AgentOutput[]
}

export type AcceptWorkflowexecutionRequest = {
	executionId: string
	accept: boolean
	reason: string
}

export type ReRunWorkflowRequest = {
	workflowId: string
	executionId: string
	useSamePlan: boolean
}

export type WorkflowEventType = 'FullMessageHistory'

export type WorkflowEvent = {
	Type: WorkflowEventType
	ApprovalRequired: boolean
	Result: AgentOutput[]
}
