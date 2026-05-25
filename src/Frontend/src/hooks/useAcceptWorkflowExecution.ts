import { useMutation, useQueryClient } from '@tanstack/react-query'
import { acceptWorkflowExecution } from '../api/workflowApi'

const useAcceptWorkflowExecution = () => {
	const queryClient = useQueryClient()

	return useMutation({
		mutationFn: acceptWorkflowExecution,

		onSuccess: async () => {
			await queryClient.invalidateQueries({
				queryKey: ['getAllWorkflowExecutions']
			})
		}
	})
}

export { useAcceptWorkflowExecution }
