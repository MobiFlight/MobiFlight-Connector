import { OutputConfig, OutputDeviceConfig } from "@/types/config"
import { Controller } from "@/types/controller"
import OutputPanel from "./OutputPanel"
import { Button } from "@/components/ui/button"
import { IconEdit } from "@tabler/icons-react"

export interface OutputDeviceSummaryProps {
  config: OutputDeviceConfig
  controller: Partial<Controller> | undefined
  onEdit: () => void
}

const OutputDeviceSummary = ({ config, controller, onEdit }: OutputDeviceSummaryProps) => {
  return (
    config.Type == "Output" && (
      <div className="flex flex-row items-center gap-2 grow">
        <OutputPanel variant="summary" config={config as OutputConfig} controller={controller} />
                <Button size={"sm"} variant="ghost" onClick={onEdit}>
          <IconEdit size={16} />
        </Button>
      </div>
    )
  )
}

export default OutputDeviceSummary