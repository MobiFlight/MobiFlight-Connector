import { OutputConfig, OutputDeviceConfig } from "@/types/config"
import OutputPanel from "./OutputPanel"

export interface OutputDeviceSummaryProps {
  config: OutputDeviceConfig
}

const OutputDeviceSummary = ({ config }: OutputDeviceSummaryProps) => {
  return (
    config.Type == "Output" && (
      <OutputPanel variant="summary" config={config as OutputConfig} />
    )
  )
}

export default OutputDeviceSummary