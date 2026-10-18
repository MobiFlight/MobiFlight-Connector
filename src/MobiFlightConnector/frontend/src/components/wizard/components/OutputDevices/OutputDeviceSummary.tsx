import { OutputConfig, OutputDeviceConfig } from "@/types/config"
import { Controller } from "@/types/controller"
import OutputPanel from "./OutputPanel"

export interface OutputDeviceSummaryProps {
  config: OutputDeviceConfig
  controller: Partial<Controller> | undefined
}

const OutputDeviceSummary = ({ config, controller }: OutputDeviceSummaryProps) => {
  return (
    config.Type == "Output" && (
      <OutputPanel variant="summary" config={config as OutputConfig} controller={controller} />
    )
  )
}

export default OutputDeviceSummary