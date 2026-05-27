import type {
	AcceptWorkflowexecutionRequest,
	WorkflowExecution,
	ReRunWorkflowRequest
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

export const rerunWorkflow = async (req: ReRunWorkflowRequest) => {
	return api(`/workflow/run`, {
		method: 'POST',
		body: JSON.stringify(req)
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
