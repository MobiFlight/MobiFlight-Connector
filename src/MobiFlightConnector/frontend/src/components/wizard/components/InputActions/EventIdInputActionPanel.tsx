import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group"
import { EventIdInputAction, PmdgEventIdInputAction } from "@/types/config"

export type EventIdInputActionPanelProps = {
  variant: "default" | "pmdg"
  config: EventIdInputAction | PmdgEventIdInputAction | null
  onConfigChange: (config: EventIdInputAction | PmdgEventIdInputAction) => void
}

const EventIdInputActionPanel = ({
  variant,
  config,
  onConfigChange,
}: EventIdInputActionPanelProps) => {
  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">Event ID Input Action</div>
        <div className="text-muted-foreground text-sm">
          Configure the Event ID for this input action
        </div>
      </div>
      {variant === "pmdg" && (
        <div className="flex flex-col gap-2">
          <div className="text-sm font-semibold">PMDG Aircraft</div>
          <RadioGroup
            defaultValue="B737"
            className="flex flex-row"
            value={(config as PmdgEventIdInputAction).AircraftType}
            onValueChange={(value) =>
              onConfigChange({
                ...(config as PmdgEventIdInputAction),
                AircraftType: value,
              } as PmdgEventIdInputAction)
            }
          >
            <div className="flex items-center gap-3">
              <RadioGroupItem value="B737" id="b737" />
              <Label htmlFor="B737">B737</Label>
            </div>
            <div className="flex items-center gap-3">
              <RadioGroupItem value="B747" id="b747" />
              <Label htmlFor="B747">B747</Label>
            </div>
            <div className="flex items-center gap-3">
              <RadioGroupItem value="B777" id="b777" />
              <Label htmlFor="B777">B777</Label>
            </div>
          </RadioGroup>
        </div>
      )}
      <div>
        <Label htmlFor="eventId">Event ID</Label>
        <Input
          id="eventId"
          value={config?.EventId ?? ""}
          onChange={(e) =>
            onConfigChange({ ...config, EventId: e.target.value } as
              | EventIdInputAction
              | PmdgEventIdInputAction)
          }
        />
      </div>
      <div>
        <Label htmlFor="param">Param</Label>
        <Input
          id="param"
          value={config?.Param ?? ""}
          onChange={(e) =>
            onConfigChange({ ...config, Param: e.target.value } as
              | EventIdInputAction
              | PmdgEventIdInputAction)
          }
        />
      </div>
    </div>
  )
}
export default EventIdInputActionPanel
