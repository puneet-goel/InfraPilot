import { AppBar, Box, Stack, Toolbar, Typography } from '@mui/material'
import AutoAwesomeRoundedIcon from '@mui/icons-material/AutoAwesomeRounded'

const AppNavbar = () => {
	return (
		<AppBar
			position='fixed'
			elevation={0}
			color='transparent'
			sx={{
				backdropFilter: 'blur(20px)',
				background: 'rgba(2,6,23,0.55)',
				borderBottom: '1px solid rgba(255,255,255,0.08)'
			}}
		>
			<Toolbar>
				<Stack direction='row' spacing={1.5} sx={{ alignItems: 'center' }}>
					<Box
						sx={{
							width: 38,
							height: 38,
							borderRadius: '12px',
							bgcolor: '#2563eb',
							display: 'flex',
							alignItems: 'center',
							justifyContent: 'center'
						}}
					>
						<AutoAwesomeRoundedIcon />
					</Box>

					<Typography
						variant='h5'
						sx={{
							fontWeight: 900,
							letterSpacing: '-0.04em',
							color: 'aliceblue'
						}}
					>
						InfraPilot
					</Typography>
				</Stack>
			</Toolbar>
		</AppBar>
	)
}

export default AppNavbar
