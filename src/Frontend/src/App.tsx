import { Navigate, Route, Routes } from 'react-router-dom'
import Home from './components/home/home'
import AppNavbar from './components/appNavbar/appNavbar'
import FloatingSwitcher from './components/floatingSwitcher/floatingSwitcher'
import Workflow from './components/workflows/workflows'

const App = () => {
	return (
		<>
			<AppNavbar />
			<Routes>
				<Route path='/' element={<Home />} />
				<Route path='/workflows' element={<Workflow />} />
				<Route path='*' element={<Navigate to='/' replace />} />
			</Routes>
			<FloatingSwitcher />
		</>
	)
}

export default App
