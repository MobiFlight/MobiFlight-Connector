import { Button } from "@/components/ui/button"
import { KeyInputAction } from "@/types/config"
import { useState } from "react"

export type KeyboardInputActionPanelProps = {
  config: KeyInputAction | null
  onConfigChange: (config: KeyInputAction) => void
}

const KeyboardInputActionPanel = ({
  config,
  onConfigChange,
}: KeyboardInputActionPanelProps) => {
  const [isScanning, setIsScanning] = useState(false)
  const [scannedKeys, setScannedKeys] = useState<KeyInputAction>(config ?? {
    Type: "KeyInputAction",
    Control: false,
    Alt: false,
    Shift: false,
    Key: "",
  })

  const handleScanForInput = () => {
    setIsScanning((isScanning) => !isScanning)

    if (isScanning) {
      // If we were scanning and are now stopping, 
      // update the config with the scanned keys
      onConfigChange(scannedKeys)
    }
  }

  const handleKeyDown = (event: React.KeyboardEvent<HTMLDivElement>) => {
    event.stopPropagation()
    event.preventDefault()

    if (isScanning) {
      if (event.key === "Escape") {
        setIsScanning(false)
        return
      }
      const scannedKey = event.key
      const keyCode = event.keyCode
      const key = (scannedKey === "Control" || scannedKey === "Shift" || scannedKey === "Alt") ? 0 : keyCode

      console.log("Scanned key:", scannedKey, "Key code:", keyCode)

      const newConfig: KeyInputAction = {
        Type: "KeyInputAction",
        Control: event.ctrlKey,
        Alt: event.altKey,
        Shift: event.shiftKey,
        Key: key.toString()
      }
      setScannedKeys(newConfig)
    }
  }

  const handleKeyUp = (event: React.KeyboardEvent<HTMLDivElement>) => {
    event.stopPropagation()
    event.preventDefault()

    if (isScanning) {
      setScannedKeys({
        Type: "KeyInputAction",
        Control: event.ctrlKey,
        Alt: event.altKey,
        Shift: event.shiftKey,
        Key: "0"
      })
    }
  }

  return (
    <div className="flex flex-col gap-4" onKeyDown={handleKeyDown} onKeyUp={handleKeyUp} tabIndex={0}>
      <div className="flex flex-row gap-4">
        <Button onClick={handleScanForInput}>
          {isScanning ? "Stop scanning" : "Scan for keyboard"}
        </Button>
        <div className="flex flex-row items-center gap-2">
          <div className="text-sm font-medium">Key combo:</div>
          <div className="text-sm">
            {scannedKeys?.Control && "Ctrl + "}
            {scannedKeys?.Alt && "Alt + "}
            {scannedKeys?.Shift && "Shift + "}
            {scannedKeys?.Key !== "0" ? String.fromCharCode(Number(scannedKeys.Key)).toUpperCase() : "None"}
          </div>
        </div>
        <Button
          variant="outline"
          onClick={() =>
            onConfigChange({
              Type: "KeyInputAction",
              Control: false,
              Alt: false,
              Shift: false,
              Key: "0",
            })
          }
          disabled={isScanning}
        >
          Clear
        </Button>
      </div>
    </div>
  )
}
export default KeyboardInputActionPanel
