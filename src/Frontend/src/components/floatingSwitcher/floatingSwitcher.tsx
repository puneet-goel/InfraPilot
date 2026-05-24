import { BottomNavigation, BottomNavigationAction, Box } from '@mui/material'
import HomeRoundedIcon from '@mui/icons-material/HomeRounded'
import AccountTreeRoundedIcon from '@mui/icons-material/AccountTreeRounded'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

const FloatingSwitcher = () => {
	const [tab, setTab] = useState(0)
	const navigate = useNavigate()

	return (
		<Box
			sx={{
				position: 'fixed',
				bottom: 24,
				left: '50%',
				transform: 'translateX(-50%)',
				zIndex: 1000
			}}
		>
			<BottomNavigation
				value={tab}
				onChange={(_, newValue) => {
					setTab(newValue)
					switch (newValue) {
						case 0:
							navigate('/')
							break

						case 1:
							navigate('/workflows')
							break
					}
				}}
				showLabels
				sx={{
					borderRadius: 999,
					overflow: 'hidden',
					px: 1,
					height: 72,
					background: 'rgba(15,23,42,0.75)',
					backdropFilter: 'blur(24px)',
					border: '1px solid rgba(255,255,255,0.08)',
					boxShadow: '0 20px 50px rgba(0,0,0,0.35)',
					'& .MuiBottomNavigationAction-root': {
						color: 'rgba(255,255,255,0.65)',
						minWidth: 100
					},
					'& .Mui-selected': {
						color: '#60a5fa'
					}
				}}
			>
				<BottomNavigationAction label='Home' icon={<HomeRoundedIcon />} />
				<BottomNavigationAction
					label='Workflows'
					icon={<AccountTreeRoundedIcon />}
				/>
			</BottomNavigation>
		</Box>
	)
}

export default FloatingSwitcher
