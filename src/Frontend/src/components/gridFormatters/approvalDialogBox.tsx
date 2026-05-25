import GavelRoundedIcon from '@mui/icons-material/GavelRounded'
import {
	Dialog,
	DialogTitle,
	DialogContent,
	DialogActions,
	Button,
	Typography,
	Stack,
	Box,
	Chip,
	Paper,
	Divider
} from '@mui/material'
import type {
	AgentChatMessage,
	AgentOutput,
	ToolCall,
	WorkflowExecution,
	WorkflowPlanResult
} from '../../models/models'
import { useAcceptWorkflowExecution } from '../../hooks/useAcceptWorkflowExecution'
import { enqueueSnackbar } from 'notistack'

type ApprovalDialogBoxProps = {
	approvalDialogOpen: boolean
	setApprovalDialogOpen: (open: boolean) => void
	selectedWorkflow: WorkflowExecution
}

const ApprovalDialogBox = ({
	approvalDialogOpen,
	setApprovalDialogOpen,
	selectedWorkflow
}: ApprovalDialogBoxProps) => {
	const executedPlan = JSON.parse(
		selectedWorkflow?.agentOutput ?? '{}'
	) as WorkflowPlanResult
	const { mutate: acceptWorkflowExecution } = useAcceptWorkflowExecution()

	const handleAcceptance = async () => {
		await handleSubmmit(true, 'Approved by admin')
	}

	const handleRejection = async () => {
		await handleSubmmit(false, 'Rejected by admin')
	}

	const handleSubmmit = async (accept: boolean, reason: string) => {
		try {
			await acceptWorkflowExecution({
				executionId: selectedWorkflow.executionId,
				accept,
				reason
			})
			enqueueSnackbar('Workflow approved successfully!', {
				variant: 'success',
				anchorOrigin: {
					vertical: 'top',
					horizontal: 'right'
				}
			})
			setApprovalDialogOpen(false)
		} catch (error) {
			console.error('Error submitting approval decision:', error)
			enqueueSnackbar('Error while approving workflow', {
				variant: 'error',
				anchorOrigin: {
					vertical: 'top',
					horizontal: 'right'
				}
			})
		}
	}

	return (
		<Dialog
			open={approvalDialogOpen}
			onClose={() => setApprovalDialogOpen(false)}
			fullWidth
			maxWidth='lg'
			slotProps={{
				paper: {
					sx: {
						borderRadius: 6,
						background: 'rgba(15,23,42,0.96)',
						backdropFilter: 'blur(24px)',
						border: '1px solid rgba(255,255,255,0.08)',
						color: 'white',
						overflow: 'hidden',
						minHeight: '80vh'
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
						Workflow Approval
					</Typography>

					<Typography
						sx={{
							color: 'rgba(255,255,255,0.65)'
						}}
					>
						Review agent execution, tool usage and approval requirements before
						continuing workflow.
					</Typography>
				</Stack>
			</DialogTitle>

			<DialogContent
				sx={{
					py: 4
				}}
			>
				{executedPlan && (
					<Stack spacing={2} sx={{ mt: 2 }}>
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
								label={executedPlan?.RuntimeEnvironment}
								sx={{
									bgcolor: 'rgba(37,99,235,0.15)',
									color: '#60a5fa',
									fontWeight: 700
								}}
							/>
						</Box>

						<Stack spacing={4}>
							{executedPlan?.Steps?.map(
								(agent: AgentOutput, agentIndex: number) => (
									<Paper
										key={agentIndex}
										elevation={0}
										sx={{
											p: 3,
											borderRadius: 5,
											background: 'rgba(255,255,255,0.03)',
											border: '1px solid rgba(255,255,255,0.06)'
										}}
									>
										<Stack spacing={3}>
											{/* AGENT HEADER */}

											<Stack
												direction='row'
												sx={{
													justifyContent: 'space-between',
													alignItems: 'center'
												}}
											>
												<Stack spacing={0.5}>
													<Typography
														variant='h6'
														sx={{ fontWeight: 800, color: 'aliceblue' }}
													>
														{agent.AgentName}
													</Typography>

													<Typography
														sx={{
															color: 'rgba(255,255,255,0.55)',
															fontSize: '0.9rem'
														}}
													>
														Agent execution trace
													</Typography>
												</Stack>

												<Chip
													label={`Agent ${agentIndex + 1}`}
													sx={{
														bgcolor: 'rgba(37,99,235,0.12)',
														color: '#60a5fa'
													}}
												/>
											</Stack>

											<Divider
												sx={{
													borderColor: 'rgba(255,255,255,0.06)'
												}}
											/>

											<Stack spacing={3}>
												{agent?.Chat?.map(
													(message: AgentChatMessage, messageIndex: number) => (
														<Box key={messageIndex}>
															<Paper
																elevation={0}
																sx={{
																	p: 3,
																	borderRadius: 4,
																	background: 'rgba(15,23,42,0.7)',
																	border: '1px solid rgba(255,255,255,0.05)'
																}}
															>
																<Stack spacing={2}>
																	<Stack
																		direction='row'
																		sx={{
																			justifyContent: 'space-between',
																			alignItems: 'center'
																		}}
																	>
																		<Chip
																			label={message.Role}
																			size='small'
																			sx={{
																				bgcolor: 'rgba(37,99,235,0.15)',
																				color: '#60a5fa',
																				fontWeight: 700
																			}}
																		/>

																		{message.IsApprovalRequired && (
																			<Chip
																				label={
																					message.ApprovalStatus || 'pending'
																				}
																				size='small'
																				sx={{
																					bgcolor:
																						message.ApprovalStatus ===
																						'accepted'
																							? 'rgba(22,163,74,0.15)'
																							: message.ApprovalStatus ===
																								  'rejected'
																								? 'rgba(220,38,38,0.15)'
																								: 'rgba(250,204,21,0.15)',

																					color:
																						message.ApprovalStatus ===
																						'accepted'
																							? '#4ade80'
																							: message.ApprovalStatus ===
																								  'rejected'
																								? '#f87171'
																								: '#facc15',

																					fontWeight: 700
																				}}
																			/>
																		)}
																	</Stack>

																	{/* TEXT */}

																	{message.Text && (
																		<Typography
																			sx={{
																				lineHeight: 1.9,

																				color: 'rgba(255,255,255,0.82)'
																			}}
																		>
																			{message.Text}
																		</Typography>
																	)}

																	{/* TOOL CALLS */}

																	{message.ToolCalls?.length > 0 && (
																		<Stack spacing={2}>
																			{message.ToolCalls.map(
																				(tool: ToolCall, toolIndex: number) => (
																					<Paper
																						key={toolIndex}
																						elevation={0}
																						sx={{
																							p: 2.5,
																							borderRadius: 3,
																							background:
																								'rgba(255,255,255,0.03)',
																							border:
																								'1px solid rgba(255,255,255,0.05)'
																						}}
																					>
																						<Stack spacing={2}>
																							<Stack
																								direction='row'
																								sx={{
																									justifyContent:
																										'space-between',
																									alignItems: 'center'
																								}}
																							>
																								<Typography
																									sx={{ fontWeight: 800 }}
																								>
																									{tool.ToolName}
																								</Typography>

																								<Chip
																									label='Tool'
																									size='small'
																									sx={{
																										bgcolor:
																											'rgba(168,85,247,0.15)',

																										color: '#c084fc'
																									}}
																								/>
																							</Stack>

																							{/* ARGUMENTS */}

																							<Box>
																								<Typography
																									sx={{
																										color: '#94a3b8',
																										mb: 1,
																										fontSize: '0.85rem'
																									}}
																								>
																									Arguments
																								</Typography>

																								<Paper
																									elevation={0}
																									sx={{
																										p: 2,
																										borderRadius: 2,
																										background: '#020617',
																										overflow: 'auto'
																									}}
																								>
																									<pre
																										style={{
																											margin: 0,
																											color: '#cbd5e1',
																											fontSize: '0.82rem',
																											lineHeight: 1.7
																										}}
																									>
																										{JSON.stringify(
																											tool.Arguments,
																											null,
																											2
																										)}
																									</pre>
																								</Paper>
																							</Box>

																							{/* RESULT */}

																							{tool.Result && (
																								<Box>
																									<Typography
																										sx={{
																											color: '#94a3b8',
																											mb: 1,
																											fontSize: '0.85rem'
																										}}
																									>
																										Result
																									</Typography>

																									<Paper
																										elevation={0}
																										sx={{
																											p: 2,
																											borderRadius: 2,
																											background: '#020617',
																											overflow: 'auto'
																										}}
																									>
																										<pre
																											style={{
																												margin: 0,
																												color: '#4ade80',
																												fontSize: '0.82rem',
																												lineHeight: 1.7
																											}}
																										>
																											{JSON.stringify(
																												tool.Result,
																												null,
																												2
																											)}
																										</pre>
																									</Paper>
																								</Box>
																							)}

																							{/* EXCEPTION */}

																							{tool.Exception && (
																								<Box>
																									<Typography
																										sx={{
																											color: '#f87171',
																											mb: 1,
																											fontSize: '0.85rem'
																										}}
																									>
																										Exception
																									</Typography>

																									<Paper
																										elevation={0}
																										sx={{
																											p: 2,
																											borderRadius: 2,
																											background:
																												'rgba(220,38,38,0.12)',
																											overflow: 'auto'
																										}}
																									>
																										<pre
																											style={{
																												margin: 0,
																												color: '#fca5a5',
																												fontSize: '0.82rem',
																												lineHeight: 1.7
																											}}
																										>
																											{JSON.stringify(
																												tool.Exception,
																												null,
																												2
																											)}
																										</pre>
																									</Paper>
																								</Box>
																							)}
																						</Stack>
																					</Paper>
																				)
																			)}
																		</Stack>
																	)}

																	{/* APPROVAL REASON */}

																	{message.ApprovalReason && (
																		<Paper
																			elevation={0}
																			sx={{
																				p: 2,
																				borderRadius: 3,
																				background: 'rgba(250,204,21,0.08)',
																				border:
																					'1px solid rgba(250,204,21,0.15)'
																			}}
																		>
																			<Typography
																				sx={{
																					color: '#fde68a',
																					lineHeight: 1.8
																				}}
																			>
																				{message.ApprovalReason}
																			</Typography>
																		</Paper>
																	)}
																</Stack>
															</Paper>
														</Box>
													)
												)}
											</Stack>
										</Stack>
									</Paper>
								)
							)}
						</Stack>
					</Stack>
				)}
			</DialogContent>

			<DialogActions
				sx={{
					borderTop: '1px solid rgba(255,255,255,0.08)',
					p: 2.5,
					gap: 1
				}}
			>
				<Button
					onClick={() => setApprovalDialogOpen(false)}
					variant='outlined'
					sx={{
						borderRadius: 3,
						textTransform: 'none',
						borderColor: 'rgba(255,255,255,0.12)',
						color: 'rgba(255,255,255,0.8)',
						px: 3
					}}
				>
					Close
				</Button>

				<Button
					variant='contained'
					startIcon={<GavelRoundedIcon />}
					onClick={handleAcceptance}
					sx={{
						borderRadius: 3,
						textTransform: 'none',
						px: 3,
						background: 'linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%)',
						boxShadow: '0 10px 30px rgba(37,99,235,0.35)'
					}}
				>
					Approve
				</Button>

				<Button
					variant='contained'
					startIcon={<GavelRoundedIcon />}
					onClick={handleRejection}
					sx={{
						borderRadius: 3,
						textTransform: 'none',
						px: 3,
						background: 'linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%)',
						boxShadow: '0 10px 30px rgba(37,99,235,0.35)'
					}}
				>
					Reject
				</Button>
			</DialogActions>
		</Dialog>
	)
}

export default ApprovalDialogBox
