import { Box, Chip, Container, Paper, Stack, Typography } from '@mui/material'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'

type Workflow = {
	id: string

	workflowName: string

	status: string

	createdBy: string

	createdAt: string

	updatedAt: string

	currentAgent: string
}

const workflows: Workflow[] = [
	{
		id: 'wf-1001',

		workflowName: 'Investigate nginx restart issue',

		status: 'Running',

		createdBy: 'Puneet',

		createdAt: '2026-05-24 10:12',

		updatedAt: '2026-05-24 10:15',

		currentAgent: 'InfrastructureAgent'
	},

	{
		id: 'wf-1002',

		workflowName: 'Analyze payment namespace security',

		status: 'WaitingApproval',

		createdBy: 'Puneet',

		createdAt: '2026-05-24 09:55',

		updatedAt: '2026-05-24 10:00',

		currentAgent: 'SecurityAgent'
	},

	{
		id: 'wf-1003',

		workflowName: 'Scale redis deployment',

		status: 'Completed',

		createdBy: 'Puneet',

		createdAt: '2026-05-24 08:20',

		updatedAt: '2026-05-24 08:28',

		currentAgent: 'DeploymentAgent'
	},

	{
		id: 'wf-1004',

		workflowName: 'Delete failed ingress resources',

		status: 'Failed',

		createdBy: 'Puneet',

		createdAt: '2026-05-24 07:10',

		updatedAt: '2026-05-24 07:14',

		currentAgent: 'CleanupAgent'
	}
]

function getStatusStyles(status: string) {
	switch (status) {
		case 'Running':
			return {
				background: 'rgba(37,99,235,0.15)',

				color: '#60a5fa'
			}

		case 'Completed':
			return {
				background: 'rgba(22,163,74,0.15)',

				color: '#4ade80'
			}

		case 'Failed':
			return {
				background: 'rgba(220,38,38,0.15)',

				color: '#f87171'
			}

		case 'WaitingApproval':
			return {
				background: 'rgba(245,158,11,0.15)',

				color: '#facc15'
			}

		default:
			return {
				background: 'rgba(255,255,255,0.08)',

				color: 'white'
			}
	}
}

const columns: GridColDef[] = [
	{
		field: 'id',

		headerName: 'Workflow Id',

		flex: 1.2
	},

	{
		field: 'workflowName',

		headerName: 'Workflow',

		flex: 2.5
	},

	{
		field: 'status',

		headerName: 'Status',

		flex: 1.2,

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

		flex: 1.5
	},

	{
		field: 'createdBy',

		headerName: 'Created By',

		flex: 1
	},

	{
		field: 'createdAt',

		headerName: 'Created',

		flex: 1.3
	},

	{
		field: 'updatedAt',

		headerName: 'Updated',

		flex: 1.3
	}
]

export default function Workflow() {
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
						<Chip
							label='Running'
							sx={{
								bgcolor: 'rgba(37,99,235,0.15)',
								color: '#60a5fa',
								fontWeight: 700
							}}
						/>

						<Chip
							label='Completed'
							sx={{
								bgcolor: 'rgba(22,163,74,0.15)',
								color: '#4ade80',
								fontWeight: 700
							}}
						/>

						<Chip
							label='Failed'
							sx={{
								bgcolor: 'rgba(220,38,38,0.15)',
								color: '#f87171',
								fontWeight: 700
							}}
						/>

						<Chip
							label='Waiting Approval'
							sx={{
								bgcolor: 'rgba(245,158,11,0.15)',
								color: '#facc15',
								fontWeight: 700
							}}
						/>
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
						rows={workflows}
						columns={columns}
						disableRowSelectionOnClick
						pageSizeOptions={[5, 10, 20]}
						initialState={{
							pagination: {
								paginationModel: {
									pageSize: 10
								}
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
			</Container>
		</Box>
	)
}
