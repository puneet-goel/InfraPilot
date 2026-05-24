const API_URL = '/api'

export async function api<T>(
	endpoint: string,
	options?: RequestInit
): Promise<T> {
	const response = await fetch(`${API_URL}${endpoint}`, {
		headers: {
			'Content-Type': 'application/json'
		},

		...options
	})

	if (!response.ok) {
		throw new Error('Something went wrong')
	}

	return response.json()
}
