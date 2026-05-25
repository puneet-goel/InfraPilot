import {
	Box,
	Button,
	Chip,
	Dialog,
	DialogActions,
	DialogContent,
	DialogTitle,
	Paper,
	Stack,
	Typography
} from '@mui/material'
import type { WorkflowExecution, WorkflowStep } from '../../models/models'

type PlanDialogBoxProps = {
	planDialogOpen: boolean
	setPlanDialogOpen: (open: boolean) => void
	selectedWorkflow: WorkflowExecution
}

const PlanDialogBox = ({
	planDialogOpen,
	setPlanDialogOpen,
	selectedWorkflow
}: PlanDialogBoxProps) => {
	return (
		<Dialog
			open={planDialogOpen}
			onClose={() => setPlanDialogOpen(false)}
			fullWidth
			maxWidth='md'
			slotProps={{
				paper: {
					sx: {
						borderRadius: 6,
						background: 'rgba(15,23,42,0.95)',
						backdropFilter: 'blur(24px)',
						border: '1px solid rgba(255,255,255,0.08)',
						color: 'white'
					}
				}
			}}
		>
			<DialogTitle
				sx={{
					borderBottom: '1px solid rgba(255,255,255,0.08)',
					py: 3
				}}
			>
				<Stack spacing={1}>
					<Typography variant='h5' sx={{ fontWeight: 800 }}>
						Workflow Plan
					</Typography>

					<Typography
						sx={{
							color: 'rgba(255,255,255,0.65)'
						}}
					>
						Inspect generated orchestration plan and execution sequence.
					</Typography>
				</Stack>
			</DialogTitle>

			<DialogContent
				sx={{
					py: 4
				}}
			>
				{selectedWorkflow && (
					<Stack spacing={2} sx={{ mt: 2 }}>
						<Box>
							<Typography
								variant='subtitle2'
								sx={{
									color: '#94a3b8',
									mb: 1
								}}
							>
								USER REQUEST
							</Typography>

							<Paper
								elevation={0}
								sx={{
									p: 3,
									borderRadius: 4,
									background: 'rgba(255,255,255,0.03)',
									border: '1px solid rgba(255,255,255,0.06)'
								}}
							>
								<Typography
									sx={{
										lineHeight: 1.8,
										color: 'aliceblue'
									}}
								>
									{selectedWorkflow.userRequest}
								</Typography>
							</Paper>
						</Box>

						<Box sx={{ display: 'flex', alignItems: 'center' }}>
							<Typography
								variant='subtitle2'
								sx={{
									color: '#94a3b8',
									mr: 1
								}}
							>
								RUNTIME ENVIRONMENT
							</Typography>

							<Chip
								label={
									JSON.parse(selectedWorkflow.workflowPlan ?? '{}')
										.RuntimeEnvironment ?? 'Linux'
								}
								sx={{
									bgcolor: 'rgba(37,99,235,0.15)',
									color: '#60a5fa',
									fontWeight: 700
								}}
							/>
						</Box>

						<Box>
							<Typography
								variant='subtitle2'
								sx={{
									color: '#94a3b8',
									mb: 2
								}}
							>
								EXECUTION STEPS
							</Typography>

							<Stack spacing={2}>
								{JSON.parse(selectedWorkflow?.workflowPlan ?? '{}')?.Steps?.map(
									(step: WorkflowStep, index: number) => (
										<Paper
											key={index}
											elevation={0}
											sx={{
												p: 3,
												borderRadius: 4,
												background: 'rgba(255,255,255,0.03)',
												border: '1px solid rgba(255,255,255,0.06)',
												position: 'relative',
												overflow: 'hidden'
											}}
										>
											<Box
												sx={{
													position: 'absolute',
													left: 0,
													top: 0,
													bottom: 0,
													width: 4,
													bgcolor: '#2563eb'
												}}
											/>
											<Stack spacing={2}>
												<Stack
													direction='row'
													sx={{
														justifyContent: 'space-between',
														alignItems: 'center'
													}}
												>
													<Typography
														sx={{
															fontWeight: 800,
															fontSize: '1.05rem',
															color: '#6986aa'
														}}
													>
														{step.AgentName}
													</Typography>

													<Chip
														label={`Step ${index + 1}`}
														size='small'
														sx={{
															bgcolor: 'rgba(37,99,235,0.12)',
															color: '#60a5fa'
														}}
													/>
												</Stack>

												<Typography
													sx={{
														color: 'rgba(255,255,255,0.75)',
														lineHeight: 1.8
													}}
												>
													{step.Task}
												</Typography>
											</Stack>
										</Paper>
									)
								)}
							</Stack>
						</Box>
					</Stack>
				)}
			</DialogContent>

			<DialogActions
				sx={{
					borderTop: '1px solid rgba(255,255,255,0.08)',
					p: 2
				}}
			>
				<Button
					onClick={() => setPlanDialogOpen(false)}
					variant='contained'
					sx={{
						borderRadius: 3,
						textTransform: 'none',
						px: 3
					}}
				>
					Close
				</Button>
			</DialogActions>
		</Dialog>
	)
}

export default PlanDialogBox
