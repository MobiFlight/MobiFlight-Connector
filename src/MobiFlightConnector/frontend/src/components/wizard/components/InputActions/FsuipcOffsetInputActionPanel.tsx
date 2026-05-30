import { Input } from "@/components/ui/input"
import { FsuipcOffsetInputAction } from "@/types/config"
import { Switch } from "@radix-ui/react-switch"

export type FsuipcOffsetInputActionPanelProps = {
  config: FsuipcOffsetInputAction | null
  onConfigChange: (config: FsuipcOffsetInputAction) => void
}

const FsuipcOffsetInputActionPanel = ({
  config,
  onConfigChange,
}: FsuipcOffsetInputActionPanelProps) => {
  const defaultConfig: FsuipcOffsetInputAction = {
    Type: "FsuipcOffsetInputAction",
    FSUIPC: {
      OffsetType: "Integer",
      Offset: 0,
      Size: 1,
      Mask: 0,
      BcdMode: false,
    },
    Modifiers: [],
    Value: "",
  }
  const currentConfig = config ?? defaultConfig

  console.log("FSUIPC Default Config in Panel:", defaultConfig)
  console.log("FSUIPC Config in Panel:", currentConfig)

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">FSUIPC Offset</div>
        <div className="text-muted-foreground text-sm">Select an offset</div>
      </div>
      <Input
        value={currentConfig.FSUIPC.Size}
        onChange={(e) =>
          onConfigChange({
            ...currentConfig,
            FSUIPC: { ...currentConfig.FSUIPC, Size: Number(e.target.value) },
          } as FsuipcOffsetInputAction)
        }
      />
      <Input
        value={currentConfig.FSUIPC.Offset}
        onChange={(e) =>
          onConfigChange({
            ...currentConfig,
            FSUIPC: { ...currentConfig.FSUIPC, Offset: Number(e.target.value) },
          } as FsuipcOffsetInputAction)
        }
      />
      <Input
        value={currentConfig.FSUIPC.Mask}
        onChange={(e) =>
          onConfigChange({
            ...currentConfig,
            FSUIPC: { ...currentConfig.FSUIPC, Mask: Number(e.target.value) },
          } as FsuipcOffsetInputAction)
        }
      />
     <Switch
        checked={currentConfig.FSUIPC.BcdMode}
        onCheckedChange={(e) =>
          onConfigChange({
            ...currentConfig,
            FSUIPC: { ...currentConfig.FSUIPC, BcdMode: e },
          } as FsuipcOffsetInputAction)
        }
      />
    </div>
  )
}
export default FsuipcOffsetInputActionPanel
