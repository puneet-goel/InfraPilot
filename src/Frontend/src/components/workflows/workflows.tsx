import { Box, Chip, IconButton, Paper, Stack, Typography } from '@mui/material'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import { useGetWorkflowExecutions } from '../../hooks/useGetWorkflowExecution'
import VisibilityRoundedIcon from '@mui/icons-material/VisibilityRounded'
import { useState } from 'react'
import type { WorkflowExecution } from '../../models/models'
import PlanDialogBox from '../gridFormatters/planDialogBox'
import ReplayRoundedIcon from '@mui/icons-material/ReplayRounded'
import ReRunDialogBox from '../gridFormatters/reRunDialogBox'
import Tooltip from '@mui/material/Tooltip'
import GavelRoundedIcon from '@mui/icons-material/GavelRounded'
import ApprovalDialogBox from '../gridFormatters/approvalDialogBox'

const WORKFLOW_STATUSES = [
	'Pending',
	'Running',
	'Completed',
	'Failed',
	'ApprovalRequired',
	'Approved',
	'Rejected'
]

const getStatusStyles = (status: string) => {
	switch (status) {
		case 'Pending':
			return {
				background: 'rgba(245,158,11,0.15)',
				color: '#fbbf24'
			}

		case 'Running':
			return {
				background: 'rgba(59,130,246,0.15)',
				color: '#60a5fa'
			}

		case 'Completed':
			return {
				background: 'rgba(34,197,94,0.15)',
				color: '#4ade80'
			}

		case 'Failed':
			return {
				background: 'rgba(239,68,68,0.15)',
				color: '#f87171'
			}

		case 'ApprovalRequired':
			return {
				background: 'rgba(168,85,247,0.15)',
				color: '#c084fc'
			}

		case 'Approved':
			return {
				background: 'rgba(16,185,129,0.15)',
				color: '#34d399'
			}

		case 'Rejected':
			return {
				background: 'rgba(244,63,94,0.15)',
				color: '#fb7185'
			}

		default:
			return {
				background: 'rgba(255,255,255,0.08)',
				color: '#e5e7eb'
			}
	}
}

const Workflow = () => {
	const { data: workflowExecutions = [], isLoading } =
		useGetWorkflowExecutions()
	const [planDialogOpen, setPlanDialogOpen] = useState(false)
	const [selectedWorkflow, setSelectedWorkflow] =
		useState<WorkflowExecution | null>(null)
	const [rerunDialogOpen, setRerunDialogOpen] = useState(false)
	const [approvalDialogOpen, setApprovalDialogOpen] = useState(false)

	const columns: GridColDef[] = [
		{
			field: 'workflowId'
		},
		{
			field: 'executionId'
		},
		{
			field: 'userRequest',
			headerName: 'User Request',
			flex: 1.5
		},
		{
			field: 'status',
			headerName: 'Status',
			width: 150,
			renderCell: (params) => {
				const styles = getStatusStyles(params.value)
				return (
					<Chip
						label={params.value}
						size='small'
						sx={{
							bgcolor: styles.background,
							color: styles.color,
							fontWeight: 700,
							borderRadius: 999
						}}
					/>
				)
			}
		},
		{
			field: 'currentAgent',
			headerName: 'Current Agent',
			width: 150,
		},
		{
			field: 'reason',
			headerName: 'Reason',
			flex: 1
		},
		{
			field: 'createdAt',
			headerName: 'Created At',
			type: 'dateTime',
			width: 180,
			valueGetter: (value) => value && new Date(value),
			valueFormatter: (value) => {
				if (!value) {
					return ''
				}

				return new Intl.DateTimeFormat('en-IN', {
					day: '2-digit',
					month: 'short',
					year: 'numeric',
					hour: '2-digit',
					minute: '2-digit'
				}).format(value)
			}
		},
		{
			field: 'updatedAt',
			headerName: 'Updated At',
			type: 'dateTime',
			width: 180,
			valueGetter: (value) => value && new Date(value),
			valueFormatter: (value) => {
				if (!value) {
					return ''
				}

				return new Intl.DateTimeFormat('en-IN', {
					day: '2-digit',
					month: 'short',
					year: 'numeric',
					hour: '2-digit',
					minute: '2-digit'
				}).format(value)
			}
		},
				{
			field: 'agentOutput',
			headerName: 'Trace',
			width: 70,
			sortable: false,
			filterable: false,
			renderCell: (params) => {
				return (
					<Tooltip title='Review approvals' arrow>
						<IconButton
							onClick={() => {
								setSelectedWorkflow(params.row)
								setApprovalDialogOpen(true)
							}}
							sx={{
								color: '#facc15',
								'&:hover': {
									background: 'rgba(250,204,21,0.12)'
								}
							}}
						>
							<GavelRoundedIcon />
						</IconButton>
					</Tooltip>
				)
			}
		},
		{
			field: 'workflowPlan',
			headerName: 'Plan',
			width: 65,
			sortable: false,
			renderCell: (params) => {
				return (
					<Tooltip title='View Workflow Plan' arrow>
						<IconButton
							onClick={() => {
								setSelectedWorkflow(params.row)
								setPlanDialogOpen(true)
							}}
							sx={{
								color: '#60a5fa',
								'&:hover': {
									background: 'rgba(37,99,235,0.12)'
								}
							}}
						>
							<VisibilityRoundedIcon />
						</IconButton>
					</Tooltip>
				)
			}
		},
		{
			field: 'actions',
			headerName: 'Actions',
			width: 100,
			sortable: false,
			filterable: false,
			renderCell: (params) => {
				return (
					<Tooltip title='Re-run workflow' arrow>
						<IconButton
							onClick={() => {
								setSelectedWorkflow(params.row)
								setRerunDialogOpen(true)
							}}
							sx={{
								color: '#60a5fa',
								'&:hover': {
									background: 'rgba(37,99,235,0.12)'
								}
							}}
						>
							<ReplayRoundedIcon />
						</IconButton>
					</Tooltip>
				)
			}
		}
	]

	return (
		<Box
			sx={{
				minHeight: '100vh',
				background: 'radial-gradient(circle at top, #172554 0%, #020617 55%)',
				display: 'flex',
				flexDirection: 'column'
			}}
		>
			<Box
				sx={{
					flex: 1,
					display: 'flex',
					alignItems: 'center',
					justifyContent: 'center',
					mx: 3,
					py: 6,
					flexDirection: 'column'
				}}
			>
				<Stack spacing={2} sx={{ width: '100%', mt: 4, mb: 2 }}>
					<Typography
						variant='h3'
						sx={{
							fontWeight: 900,
							letterSpacing: '-0.05em',
							color: 'aliceblue'
						}}
					>
						Workflow Status
					</Typography>

					<Typography
						sx={{
							color: 'rgba(255,255,255,0.7)',
							mt: '0 !important'
						}}
					>
						Monitor workflow executions, track orchestration state, and inspect
						active agent execution status.
					</Typography>

					<Stack
						direction='row'
						spacing={1.5}
						sx={{ flexWrap: 'wrap', justifyContent: 'end' }}
					>
						{WORKFLOW_STATUSES.map((status) => {
							const styles = getStatusStyles(status)
							return (
								<Chip
									label={status}
									key={status}
									sx={{
										...styles,
										fontWeight: 700
									}}
								/>
							)
						})}
					</Stack>
				</Stack>

				<Paper
					elevation={0}
					sx={{
						height: '60vh',
						width: '100%',
						borderRadius: 6,
						overflow: 'hidden',
						background: 'rgba(15,23,42,0.72)',
						backdropFilter: 'blur(24px)',
						border: '1px solid rgba(255,255,255,0.08)',
						boxShadow: '0 25px 60px rgba(0,0,0,0.35)'
					}}
				>
					<DataGrid
						rows={workflowExecutions}
						columns={columns}
						loading={isLoading}
						disableRowSelectionOnClick
						pageSizeOptions={[5, 10, 20]}
						getRowId={(row) => row.executionId}
						columnVisibilityModel={{
							executionId: false,
							workflowId: false
						}}
						initialState={{
							pagination: {
								paginationModel: {
									pageSize: 10
								}
							},
							sorting: {
								sortModel: [{ field: 'updatedAt', sort: 'desc' }]
							}
						}}
						sx={{
							border: 'none',
							color: 'white',
							backgroundColor: 'transparent',

							'--DataGrid-containerBackground': '#0f172a',

							'--DataGrid-pinnedBackground': '#0f172a',

							/* HEADER */

							'& .MuiDataGrid-columnHeaders': {
								backgroundColor: '#0f172a !important',

								color: 'white !important',

								borderBottom: '1px solid rgba(255,255,255,0.08)'
							},

							'& .MuiDataGrid-columnHeader': {
								backgroundColor: '#0f172a !important',

								color: 'white !important'
							},

							'& .MuiDataGrid-columnHeaderTitle': {
								color: 'white !important',

								fontWeight: 800,

								fontSize: '0.95rem'
							},

							/* CELLS */

							'& .MuiDataGrid-cell': {
								color: 'white',

								borderBottom: '1px solid rgba(255,255,255,0.05)'
							},

							'& .MuiDataGrid-row': {
								backgroundColor: 'transparent'
							},

							'& .MuiDataGrid-row:hover': {
								backgroundColor: 'rgba(255,255,255,0.04)'
							},

							/* FOOTER */

							'& .MuiDataGrid-footerContainer': {
								backgroundColor: '#0f172a',

								color: 'white',

								borderTop: '1px solid rgba(255,255,255,0.08)'
							},

							'& .MuiTablePagination-root': {
								color: 'white'
							},

							/* ICONS */

							'& .MuiSvgIcon-root': {
								color: 'rgba(255,255,255,0.7)'
							},

							'& .MuiDataGrid-sortIcon': {
								color: 'rgba(255,255,255,0.7)'
							},

							'& .MuiDataGrid-menuIconButton': {
								color: 'rgba(255,255,255,0.7)'
							},

							'& .MuiCheckbox-root': {
								color: 'rgba(255,255,255,0.7)'
							},

							/* EMPTY OVERLAY */

							'& .MuiDataGrid-overlay': {
								backgroundColor: '#0f172a',

								color: 'white'
							},

							/* SCROLLBAR */

							'& ::-webkit-scrollbar': {
								width: 10,
								height: 10
							},

							'& ::-webkit-scrollbar-thumb': {
								background: 'rgba(255,255,255,0.12)',

								borderRadius: 999
							},

							'& ::-webkit-scrollbar-track': {
								background: 'transparent'
							}
						}}
					/>
				</Paper>

				{selectedWorkflow && (
					<PlanDialogBox
						planDialogOpen={planDialogOpen}
						setPlanDialogOpen={setPlanDialogOpen}
						selectedWorkflow={selectedWorkflow}
					/>
				)}

				{selectedWorkflow && (
					<ReRunDialogBox
						rerunDialogOpen={rerunDialogOpen}
						setRerunDialogOpen={setRerunDialogOpen}
						selectedWorkflow={selectedWorkflow}
					/>
				)}

				{selectedWorkflow && (
					<ApprovalDialogBox
						approvalDialogOpen={approvalDialogOpen}
						setApprovalDialogOpen={setApprovalDialogOpen}
						selectedWorkflow={selectedWorkflow}
					/>
				)}
			</Box>
		</Box>
	)
}

export default Workflow
