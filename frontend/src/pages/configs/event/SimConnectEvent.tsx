import { ComboBox } from "@/components/mobiflight/ComboBox"
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

  const [searchValue, setSearchValue] = React.useState("")
  const [vendor, setVendor] = React.useState("Microsoft")
  const [aircraft, setAircraft] = React.useState("A320")
  const [system, setSystem] = React.useState("Electrical")

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-row h-auto items-center gap-4">
        <Label className="w-16">Code</Label>
        <Textarea  value={ (config.Event.Settings as SimConnectVarEventSettings).Value }>
        </Textarea>
      </div>
      <div className="flex flex-row h-auto items-center gap-4">
        <Label className="w-16">Search</Label>
        <Input
          value={searchValue}
          className="w-auto"
          onChange={(e) => setSearchValue(e.target.value)}
        />
        <Label>Vendor</Label>
        <ComboBox
          options={options.vendor.map((v) => ({ label: v, value: v }))}
          onSelect={() => {}}
          value={vendor}
        ></ComboBox>
        <Label>Aircraft</Label>
        <ComboBox
          options={options.aircraft.map((v) => ({ label: v, value: v }))}
          onSelect={() => {}}
          value={aircraft}
        ></ComboBox>
        <Label>System</Label>
        <ComboBox
          options={options.system.map((v) => ({ label: v, value: v }))}
          onSelect={() => {}}
          value={system}
        ></ComboBox>

        <Button>Reset</Button>

      </div>
    </div>
  )
}

export default SimConnectEvent
