import type {
	AcceptWorkflowexecutionRequest,
	WorkflowExecution
} from '../models/models'
import { api } from './client'

export const getAllWorkflowExecutions = async () => {
	return api<WorkflowExecution[]>('/workflowExecution/getAll')
}

export const createWorkflow = async (prompt: string) => {
	return api('/workflow/create', {
		method: 'POST',
		body: JSON.stringify(prompt)
	})
}

export const rerunWorkflow = async (workflowId: string) => {
	return api(`/workflow/run`, {
		method: 'POST',
		body: JSON.stringify(workflowId)
	})
}
export const acceptWorkflowExecution = async (
	request: AcceptWorkflowexecutionRequest
) => {
	return api(`/workflowExecution/acceptWorkflowExecution`, {
		method: 'POST',
		body: JSON.stringify(request)
	})
}
