import React from 'react'
import ReactDOM from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import App from './App'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { SnackbarProvider } from 'notistack'

const queryClient = new QueryClient({
	defaultOptions: {
		queries: {
			retry: 1,
			refetchOnWindowFocus: false
		}
	}
})

ReactDOM.createRoot(document.getElementById('root')!).render(
	<React.StrictMode>
		<SnackbarProvider maxSnack={3}>
			<QueryClientProvider client={queryClient}>
				<BrowserRouter basename='/InfraPilot'>
					<App />
				</BrowserRouter>
			</QueryClientProvider>
		</SnackbarProvider>
	</React.StrictMode>
)
