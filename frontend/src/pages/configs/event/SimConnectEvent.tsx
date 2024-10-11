import { ComboBox } from "@/components/mobiflight/ComboBox"
import PresetPanel from "@/components/mobiflight/PresetPanel"
import { PresetDataTable } from "@/components/mobiflight/tables/preset-data-table"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { IConfigItem } from "@/types"
import { SimConnectVarEventSettings } from "@/types/config"
import { Label } from "@radix-ui/react-label"
import React from "react"

interface ISimConnectEventProps {
  config: IConfigItem
}
const SimConnectEvent = (props: ISimConnectEventProps) => {
  const { config } = props
  const options = {
    vendor: ["Microsoft", "Asobo", "iniBuilds", "PMDG", "Aerosoft"],
    aircraft: ["A320", "B747", "B787", "C172", "B737"],
    system: ["Electrical", "Hydraulics", "Fuel", "Engine", "APU"],
  }

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-row h-auto items-center gap-4">
        <Label className="w-16">Code</Label>
        <Textarea  value={ (config.Event.Settings as SimConnectVarEventSettings).Value }>
        </Textarea>
      </div>
      <div className="flex flex-row h-auto items-center gap-4 align-text-top">
        <Label className="w-16">Presets</Label>
        <PresetPanel />
      </div>
    </div>
  )
}

export default SimConnectEvent
