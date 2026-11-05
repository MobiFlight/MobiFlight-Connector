import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Source } from "@/types/config"
import { FsuipcOffsetSummary } from "@/components/wizard/components/InputActions/FsuipcOffsetInputActionPanel"
import MsfsSourcePanel from "@/components/wizard/OutputConfig/Sources/MsfsSourcePanel"
import { Badge } from "@/components/ui/badge"
import { IconEdit, IconTrash } from "@tabler/icons-react"

export interface SourceSummaryProps {
  source: Source
  onEdit: () => void
  onRemove: () => void
}

const SourceSummary = ({ source, onEdit, onRemove }: SourceSummaryProps) => {
  return (
    <div
      className="group hover:bg-accent/30 flex flex-row items-center justify-between gap-4 rounded-md p-2"
      onDoubleClick={onEdit}
    >
      <div className="flex flex-col gap-1">
        <Label>Source</Label>
        <Badge variant={"outline"} className="w-fit">
          {source.Type}
        </Badge>
      </div>
      {source.Type === "SimConnectSource" && (
        <MsfsSourcePanel
          variant="summary"
          source={source}
          onSourceChange={() => {}}
        />
      )}

      {source.Type === "XplaneSource" && (
        <div className="flex flex-row items-center justify-between grow">
          <div className="flex flex-col gap-1">
            <Label>Code</Label>
            <div>{source.XplaneDataRef?.Path}</div>
          </div>
        </div>
      )}

      {source.Type === "VariableSource" && (
        <div className="flex flex-row items-center justify-between grow">
          <div className="flex flex-col gap-1">
            <Label>Name</Label>
            <div>
              {source.MobiFlightVariable?.Name ?? "N/A"}{" "}
              {source.MobiFlightVariable?.TYPE}
            </div>
          </div>
        </div>
      )}

      {source.Type === "FsuipcSource" && (
        <div className="flex flex-row items-center justify-between grow">
          <FsuipcOffsetSummary config={source.FSUIPC} />
        </div>
      )}

      {source.Type === "ProSimSource" && (
        <div className="flex flex-row items-center justify-between grow">
          <div className="flex flex-col gap-1">
            <Label>Code</Label>
            <div>{source.ProSimDataRef?.Path}</div>
          </div>
        </div>
      )}

      <div className="flex w-20 flex-row justify-end pt-4">
        <Button
          size={"sm"}
          className="invisible h-8 px-2 py-1 text-red-600 group-hover:visible hover:text-red-600"
          variant="ghost"
          onClick={onRemove}
        >
          <IconTrash />
          <span className="sr-only">Remove</span>
        </Button>
        <Button
          size={"sm"}
          className="group-hover:text-foreground text-muted-foreground h-8 px-2 py-1"
          variant="ghost"
          onClick={onEdit}
        >
          <IconEdit />
        </Button>
      </div>
    </div>
  )
}

export default SourceSummary
