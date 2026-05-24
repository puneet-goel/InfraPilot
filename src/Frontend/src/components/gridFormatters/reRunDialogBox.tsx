import {
	Button,
	Dialog,
	DialogActions,
	DialogContent,
	DialogTitle,
	Stack,
	Typography
} from '@mui/material'
import ReplayRoundedIcon from '@mui/icons-material/ReplayRounded'
import type { WorkflowExecution } from '../../models/models'
import { useRerunWorkflow } from '../../hooks/useRerunWorkflow'
import { enqueueSnackbar } from 'notistack'

type ReRunDialogBoxProps = {
	rerunDialogOpen: boolean
	setRerunDialogOpen: (open: boolean) => void
	selectedWorkflow: WorkflowExecution
}

const ReRunDialogBox = ({
	rerunDialogOpen,
	setRerunDialogOpen,
	selectedWorkflow
}: ReRunDialogBoxProps) => {
	const rerunWorkflow = useRerunWorkflow()

	const handleSubmit = async () => {
		try {
			await rerunWorkflow.mutateAsync(selectedWorkflow.workflowId)
			enqueueSnackbar('Workflow created successfully!', {
				variant: 'success',
				anchorOrigin: {
					vertical: 'top',
					horizontal: 'right'
				}
			})
      setRerunDialogOpen(false)
		} catch (err){
      console.error('Error while re-running workflow:', err)
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
		<Dialog
			open={rerunDialogOpen}
			onClose={() => setRerunDialogOpen(false)}
			fullWidth
			maxWidth='sm'
			slotProps={{
				paper: {
					sx: {
						borderRadius: 6,
						background: 'rgba(15,23,42,0.96)',
						backdropFilter: 'blur(24px)',
						border: '1px solid rgba(255,255,255,0.08)',
						color: 'white',
						overflow: 'hidden'
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
						Re-Run Workflow
					</Typography>

					<Typography
						sx={{
							color: 'rgba(255,255,255,0.65)'
						}}
					>
						Generate a fresh plan using the same user request.
					</Typography>
				</Stack>
			</DialogTitle>

			<DialogContent
				sx={{
					p: '0 !important',
					background: 'rgba(37,99,235,0.10)',
					border: '1px solid rgba(37,99,235,0.18)'
				}}
			>
				<Typography
					sx={{
						lineHeight: 1.8,
						p: 3,
						color: '#bfdbfe'
					}}
				>
					Do you want to rerun this workflow with a new orchestration plan?
				</Typography>
			</DialogContent>

			<DialogActions
				sx={{
					borderTop: '1px solid rgba(255,255,255,0.08)',
					p: 2.5,
					gap: 1
				}}
			>
				<Button
					onClick={() => setRerunDialogOpen(false)}
					variant='outlined'
					sx={{
						borderRadius: 3,
						textTransform: 'none',
						borderColor: 'rgba(255,255,255,0.12)',
						color: 'rgba(255,255,255,0.8)',
						px: 3
					}}
				>
					No
				</Button>

				<Button
					variant='contained'
					startIcon={<ReplayRoundedIcon />}
					onClick={handleSubmit}
					sx={{
						borderRadius: 3,
						textTransform: 'none',
						px: 3,
						background: 'linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%)',
						boxShadow: '0 10px 30px rgba(37,99,235,0.35)'
					}}
				>
					Yes, Re-Run
				</Button>
			</DialogActions>
		</Dialog>
	)
}

export default ReRunDialogBox
