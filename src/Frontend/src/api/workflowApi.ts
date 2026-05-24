import { api } from './client'

export const getWorkflows = async () => {
	return api<any[]>('/workflow/get')
}

export const createWorkflow = async (prompt: string) => {
	return api('/workflow/create', {
		method: 'POST',
		body: prompt
	})
}
