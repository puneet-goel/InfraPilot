import { useMutation, useQueryClient } from '@tanstack/react-query'
import { createWorkflow } from '../api/workflowApi'

const useCreateWorkflow = () => {
	const queryClient = useQueryClient()

	return useMutation({
		mutationFn: createWorkflow,

		onSuccess: async () => {
			await queryClient.invalidateQueries({
				queryKey: ['getAllWorkflowExecutions']
			})
		}
	})
}

export { useCreateWorkflow }
