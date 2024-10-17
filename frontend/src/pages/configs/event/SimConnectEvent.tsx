import {
  getPresetColumns,
  PresetColumnsProps,
} from "@/components/mobiflight/tables/preset-columns"
import { PresetDataTable } from "@/components/mobiflight/tables/preset-data-table"
import { Textarea } from "@/components/ui/textarea"
import { useMsfsPresetStore } from "@/stores/presetStore"
import { IConfigItem, Preset } from "@/types"
import { SimConnectVarEventSettings } from "@/types/config"
import { Label } from "@radix-ui/react-label"
import { useCallback, useMemo } from "react"

interface ISimConnectEventProps {
  config: IConfigItem
  onChange: (config: IConfigItem) => void
}

const SimConnectEvent = (props: ISimConnectEventProps) => {
  const { config, onChange } = props
  const presets = useMsfsPresetStore().presets.filter(
    (preset) => preset.presetType === "Output",
  )
  
  const onUse = useCallback((preset: Preset) => {
    onChange({
      ...config,
      Event: {
        ...config.Event,
        Settings: {
          UUID: preset.id!,
          Value: preset.code,
          VarType: "CODE"
        },
      },
    })
  }, [])

  const onTextareaChange = useCallback(
    (e: React.ChangeEvent<HTMLTextAreaElement>) => {
      const newValue = e.target.value
      onChange({
        ...config,
        Event: {
          ...config.Event,
          Settings: {
            ...(config.Event.Settings as SimConnectVarEventSettings),
            Value: newValue,
          },
        },
      })
    },
    [],
  )

  const columns = useMemo(
    () => getPresetColumns({ onUse: onUse } as PresetColumnsProps),
    [],
  )

  return (
    <div className="flex flex-col gap-4">
      <div className="flex h-auto flex-row items-center gap-4 align-text-top">
        <Label className="w-16">Presets</Label>
        <div>
          <PresetDataTable columns={columns} data={presets}></PresetDataTable>
        </div>
      </div>
      <div className="flex h-auto flex-row items-center gap-4">
        <Label className="w-16">Code</Label>
        <Textarea
          onChange={onTextareaChange}
          value={ (config.Event.Settings as SimConnectVarEventSettings).Value }
        ></Textarea>
      </div>
    </div>
  )
}

export default SimConnectEvent
