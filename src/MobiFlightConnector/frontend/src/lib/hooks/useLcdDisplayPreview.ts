import { useEffect, useState } from "react"
import { useShallow } from "zustand/react/shallow"
import { useAppMessage } from "@/lib/hooks/appMessage"
import messageExchange from "@/lib/messageExchange"
import { useConfigItemStateStore } from "@/stores/configItemStateStore"
import { OutputConfigItem } from "@/types/config"
import { LcdDisplayPreview } from "@/types/messages"

// Asks the backend to render the LCD text for the current draft config item.
// The backend uses the same code that drives the hardware,
// so the preview is exactly what the display will show.
// Only asks while the preview is enabled, and again whenever the draft
// or the live value of this config or one of its references changes.
// Returns one string per display row, or null until the first answer arrives.
export const useLcdDisplayPreview = (
  configItem: OutputConfigItem | undefined,
  enabled: boolean,
) => {
  const [previewLines, setPreviewLines] = useState<string[] | null>(null)

  // this config first, then its active references
  const guids = [
    configItem?.GUID,
    ...(configItem?.ConfigRefs ?? []).filter((r) => r.Active).map((r) => r.Ref),
  ]
  // the live values from run or test mode, useShallow only re-renders when one of them changes
  const liveValues = useConfigItemStateStore(
    useShallow((s) =>
      guids.map((guid) => (guid ? s.values[guid]?.Value : undefined)),
    ),
  )

  useEffect(() => {
    if (!enabled || !configItem) return
    
    messageExchange.publish({
      key: "CommandLcdDisplayPreview",
      // the backend fills "$" with this value and looks up the references itself
      payload: { item: { ...configItem, Value: liveValues[0] } },
    })
  }, [enabled, configItem, liveValues])

  useAppMessage("LcdDisplayPreview", (message) => {
    const preview = message.payload as LcdDisplayPreview
    if (preview.GUID === configItem?.GUID) setPreviewLines(preview.Lines)
  })

  return previewLines
}
