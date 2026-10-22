import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Source } from "@/types/config"
import { FsuipcOffsetSummary } from "@/components/wizard/components/InputActions/FsuipcOffsetInputActionPanel"

export interface SourceSummaryProps {
  source: Source
  onEdit: () => void
}

const SourceSummary = ({ source, onEdit }: SourceSummaryProps) => {
  if (source.Type === "SimConnectSource") {
    return (
      <div className="flex flex-row items-center justify-between">
        <div className="flex flex-col gap-1">
          <div>Source</div>
          <div>Microsoft Flight Simulator</div>
        </div>
        <div className="flex flex-col gap-1">
          <div>Code</div>
          <div>{source.SimConnectValue?.Value}</div>
        </div>
        <Button onClick={onEdit}>Edit</Button>
      </div>
    )
  }

  if (source.Type === "XplaneSource") {
    return (
      <div className="flex flex-row items-center justify-between">
        <div className="flex flex-col gap-1">
          <Label>Source</Label>
          <div>X-Plane</div>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Code</Label>
          <div>{source.XplaneDataRef?.Path}</div>
        </div>
        <Button onClick={onEdit}>Edit</Button>
      </div>
    )
  }

  if (source.Type === "VariableSource") {
    return (
      <div className="flex flex-row items-center justify-between">
        <div className="flex flex-col gap-1">
          <Label>Source</Label>
          <div>Variable</div>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Name</Label>
          <div>{source.MobiFlightVariable?.Name ?? "N/A"} {source.MobiFlightVariable?.TYPE}</div>
        </div>
        <Button onClick={onEdit}>Edit</Button>
      </div>
    )
  }

  if (source.Type === "FsuipcSource") {
    return (
      <div className="flex flex-row items-center justify-between">
        <div className="flex flex-col gap-1">
          <Label>Source</Label>
          <div>Fsuipc</div>
        </div>
        <FsuipcOffsetSummary config={source.FSUIPC} />
        <Button onClick={onEdit}>Edit</Button>
      </div>
    )
  }

  if (source.Type === "ProSimSource") {
    return (
      <div className="flex flex-row items-center justify-between">
        <div className="flex flex-col gap-1">
          <Label>Source</Label>
          <div>ProSim</div>
        </div>
        <div className="flex flex-col gap-1">
          <Label>Code</Label>
          <div>{source.ProSimDataRef?.Path}</div>
        </div>
        <Button onClick={onEdit}>Edit</Button>
      </div>
    )
  }

  return null
}
export default SourceSummary
