import { useQuery } from '@tanstack/react-query'

import { getAllWorkflowExecutions } from '../api/workflowApi'

const useGetWorkflowExecutions = () => {
	return useQuery({
		queryKey: ['getAllWorkflowExecutions'],
		queryFn: getAllWorkflowExecutions
	})
}

export { useGetWorkflowExecutions }
