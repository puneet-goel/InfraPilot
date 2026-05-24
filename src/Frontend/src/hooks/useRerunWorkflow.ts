import { useMutation } from '@tanstack/react-query'
import { rerunWorkflow } from '../api/workflowApi'

const useRerunWorkflow = () => {
	return useMutation({
		mutationFn: rerunWorkflow
	})
}

export { useRerunWorkflow }