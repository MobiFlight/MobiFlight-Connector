import { columns } from './tables/preset-columns'
import { PresetDataTable } from './tables/preset-data-table'
import { useMsfsPresetStore } from '@/stores/msfsPresetStore'

const LogPanel = () => {
    const { presets } = useMsfsPresetStore()
    return (
        <PresetDataTable columns={columns} data={presets} />
    )
}

export default LogPanel