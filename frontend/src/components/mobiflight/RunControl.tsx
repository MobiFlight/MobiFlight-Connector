import { IconPlayerSkipForward, IconPlayerStop } from '@tabler/icons-react'
import { IconPlayerPlay } from '@tabler/icons-react'
import { Switch } from '../ui/switch'
import { useGlobalSettingsStore } from '@/stores/globalSettingsStore'
import { useExecutionStateStore } from '@/stores/executionStateStore'
import { publishOnMessageExchange } from '@/lib/hooks'

const RunControl = () => {
    const { publish } = publishOnMessageExchange()
    const { state } = useExecutionStateStore()
    const { settings, updateSetting } = useGlobalSettingsStore()

    console.log("Current state", state)
    return (
        <div className='flex gap-4 items-center'>
            <div className='bg-green-800 py-2 px-2 aria-disabled:bg-gray-600 cursor-pointer aria-disabled:cursor-not-allowed' aria-disabled={state == 'Running' || state == 'Testing'}
                onClick={() => { 
                    publish({ key: 'ExecutionUpdate', payload: 'Running' })
                }}>
                <IconPlayerPlay className='h-8 w-8 text-white' />
            </div>
            <div className='bg-blue-800 py-2 px-2 aria-disabled:bg-gray-600 cursor-pointer aria-disabled:cursor-not-allowed' aria-disabled={state == 'Running' || state == 'Testing'}
                onClick={() => {
                    publish({ key: 'ExecutionUpdate', payload: 'Testing' })
                }}>
                <IconPlayerSkipForward className='h-8 w-8 text-white' />
            </div>
            <div className='bg-red-800 py-2 px-2 aria-disabled:bg-gray-600 cursor-pointer aria-disabled:cursor-not-allowed' aria-disabled={state == 'Stopped'} onClick={() => {
                publish({ key: 'ExecutionUpdate', payload: 'Stopped' })
            }}>
                <IconPlayerStop className='h-8 w-8 text-white' />
            </div>
            <div className='flex items-center gap-2'>
                <Switch className="dark:bg-gray-800 dark:data-[state=checked]:bg-gray-700" checked={settings.AutoRun} onCheckedChange={() => updateSetting({ AutoRun: !settings.AutoRun })} />AutoRun
            </div>
        </div>
    )
}

export default RunControl