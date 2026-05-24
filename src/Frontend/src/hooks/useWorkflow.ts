import { useQuery } from '@tanstack/react-query'

import { getWorkflows } from '../api/workflowApi'

const useWorkflows = () => {
	return useQuery({
		queryKey: ['workflows'],

		queryFn: getWorkflows
	})
}

export { useWorkflows }
