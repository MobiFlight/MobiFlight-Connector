import { Input } from "@/components/ui/input"
import { VJoyInputAction } from "@/types/config"
import { Switch } from "@radix-ui/react-switch"

export type VJoyInputActionPanelProps = {
  config: VJoyInputAction | null
  setConfig: (item: VJoyInputAction) => void
}

const VJoyInputActionPanel = ( 
  { config, setConfig }: VJoyInputActionPanelProps
) => {

  const joystickOptions = [
    { id: 1, buttons: [1,2,3,4,5,6,7,8], axes: ["X", "Y", "Z"] },
    { id: 2, buttons: [1,2,3,4,5,6,7,8,9,10,11,12,13,14,15], axes: ["X", "Y", "Z"] },
  ]

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">MobiFlight Variable</div>
        <div className="text-muted-foreground text-sm">Select a variable</div>
      </div>
      <Input 
        value={config.vJoyId}
        onChange={(e) => setConfig({ ...config, vJoyId: Number(e.target.value) })}
      />
      <Input
        value={config.buttonNr}
        onChange={(e) => setConfig({ ...config, buttonNr: Number(e.target.value) })}
      />
      <Input
        value={config.axisString}
        onChange={(e) => setConfig({ ...config, axisString: e.target.value })}
      />
     <Switch
        checked={config.buttonCommand}
        onCheckedChange={(checked) => setConfig({ ...config, buttonCommand: checked })}
      />
      <Input 
        value={config.sendValue}
        onChange={(e) => setConfig({ ...config, sendValue: e.target.value })} />
    </div>
  )
}
export default VJoyInputActionPanel
