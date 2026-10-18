import { Button } from "@/components/ui/button"
import { Source } from "@/types/config"

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
  return null
}
export default SourceSummary
