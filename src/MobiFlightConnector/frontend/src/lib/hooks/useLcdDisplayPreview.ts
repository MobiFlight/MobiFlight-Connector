import { useEffect, useState } from "react"
import { useAppMessage } from "@/lib/hooks/appMessage"
import messageExchange from "@/lib/messageExchange"
import { useConfigItemStateValue } from "@/stores/configItemStateStore"
import { OutputConfigItem } from "@/types/config"
import { LcdDisplayPreview } from "@/types/messages"

// Asks the backend to render the LCD text for the current draft config item.
// The backend uses the same code that drives the hardware,
// so the preview is exactly what the display will show.
// Returns one string per display row, or null until the first answer arrives.
export const useLcdDisplayPreview = (configItem?: OutputConfigItem) => {
  const [previewLines, setPreviewLines] = useState<string[] | null>(null)
  // the live value from run or test mode, the backend fills "$" with it
  const liveValue = useConfigItemStateValue(configItem?.GUID)?.Value

  useEffect(() => {
    if (!configItem) return
    messageExchange.publish({
      key: "CommandLcdDisplayPreview",
      payload: { item: { ...configItem, Value: liveValue } },
    })
  }, [configItem, liveValue])

  useAppMessage("LcdDisplayPreview", (message) => {
    const preview = message.payload as LcdDisplayPreview
    if (preview.GUID === configItem?.GUID) setPreviewLines(preview.Lines)
  })

  return previewLines
}
