import {
	Box,
	Button,
	Card,
	CardContent,
	Container,
	Stack,
	TextField,
	Typography
} from '@mui/material'
import SendRoundedIcon from '@mui/icons-material/SendRounded'
import { useState } from 'react'
import { useCreateWorkflow } from '../../hooks/useCreateWorkflow'
import { enqueueSnackbar } from 'notistack'

const Home = () => {
	const [prompt, setPrompt] = useState('')
	const createWorkflow = useCreateWorkflow()

	const handleOnSubmit = async () => {
		try {
			await createWorkflow.mutateAsync(prompt)
			setPrompt('')
			enqueueSnackbar('Workflow created successfully!', {
				variant: 'success',
				anchorOrigin: {
					vertical: 'top',
					horizontal: 'right'
				}
			})
		} catch (err){
			console.error('Error while submitting workflow:', err)
			enqueueSnackbar('Error while submitting workflow', {
				variant: 'error',
				anchorOrigin: {
					vertical: 'top',
					horizontal: 'right'
				}
			})
		}
	}

	return (
		<Box
			sx={{
				minHeight: '100vh',
				background: 'radial-gradient(circle at top, #172554 0%, #020617 55%)',
				display: 'flex',
				flexDirection: 'column'
			}}
		>
			<Container
				maxWidth='xl'
				sx={{
					flex: 1,
					display: 'flex',
					alignItems: 'center',
					justifyContent: 'center',
					py: 6
				}}
			>
				<Card
					sx={{
						width: '100%',
						maxWidth: 1200,
						borderRadius: 8,
						background: 'rgba(15,23,42,0.65)',
						backdropFilter: 'blur(24px)',
						border: '1px solid rgba(255,255,255,0.08)',
						boxShadow: '0 25px 60px rgba(0,0,0,0.35)'
					}}
				>
					<CardContent
						sx={{
							p: {
								xs: 3,
								md: 5
							}
						}}
					>
						<Stack spacing={4}>
							<Box>
								<Typography
									variant='h2'
									sx={{
										fontWeight: 900,
										letterSpacing: '-0.06em',
										color: 'aliceblue',
										fontSize: {
											xs: '2.8rem',
											md: '5rem'
										}
									}}
								>
									InfraPilot
								</Typography>

								<Typography
									variant='h6'
									sx={{
										opacity: 0.7,
										color: 'aliceblue',
										mt: 1
									}}
								>
									AI workflow orchestration for infrastructure operations.
								</Typography>
							</Box>

							<TextField
								fullWidth
								multiline
								minRows={4}
								value={prompt}
								placeholder='What do you want InfraPilot to do?'
								onChange={(e) => setPrompt(e.target.value)}
								sx={{
									'& .MuiOutlinedInput-root': {
										borderRadius: 5,
										bgcolor: 'rgba(255,255,255,0.03)',
										color: 'white',
										fontSize: '1.1rem',
										p: 1,
										'& fieldset': {
											borderColor: 'rgba(255,255,255,0.08)'
										},
										'&:hover fieldset': {
											borderColor: '#2563eb'
										},
										'&.Mui-focused fieldset': {
											borderColor: '#2563eb'
										}
									},
									'& textarea': {
										color: 'white'
									}
								}}
							/>

							<Stack direction='row' sx={{ justifyContent: 'flex-end' }}>
								<Button
									variant='contained'
									size='large'
									endIcon={<SendRoundedIcon />}
									sx={{
										height: 56,
										px: 4,
										borderRadius: 4,
										fontWeight: 700,
										textTransform: 'none',
										fontSize: '1rem'
									}}
									onClick={handleOnSubmit}
								>
									Submit Workflow
								</Button>
							</Stack>
						</Stack>
					</CardContent>
				</Card>
			</Container>
		</Box>
	)
}

export default Home
