import { useCallback, useMemo } from 'react'
import { getPresetColumns, PresetColumnsProps } from './tables/preset-columns'
import { PresetDataTable } from './tables/preset-data-table'
import { useMsfsPresetStore } from '@/stores/msfsPresetStore'
import { Preset } from '@/types'

const PresetPanel = () => {
    const { presets } = useMsfsPresetStore()

    const onUse = useCallback( (preset: Preset) => { console.log(preset.code) }, [] )
    const columns = useMemo( () => getPresetColumns( { onUse : onUse } as PresetColumnsProps), [] )
    return (
        <PresetDataTable columns={columns} data={presets} />
    )
}

export default PresetPanel