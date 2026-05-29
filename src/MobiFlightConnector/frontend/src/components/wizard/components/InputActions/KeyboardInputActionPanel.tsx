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

  const handleScanForInput = () => {
    setIsScanning((isScanning) => !isScanning)
  }

  console.log("is scanning" , isScanning)

  return (
    <div className="flex flex-col gap-4">
      <div className="flex flex-col">
        <div className="text-lg font-semibold">Keyboard input action</div>
        <div className="text-muted-foreground text-sm">
          Click the input field with your mouse and then press the desired key
          combination on your keyboard.
        </div>
      </div>
      <div className="flex flex-row gap-4">
        <Button onClick={handleScanForInput}>
          {isScanning ? "Stop scanning" : "Scan for keyboard"}
        </Button>
        <div className="flex flex-row items-center gap-2">
          <div className="text-sm font-medium">Key combo:</div>
          <div className="text-sm">
            {config?.Control && "Ctrl + "}
            {config?.Alt && "Alt + "}
            {config?.Shift && "Shift + "}
            {config?.Key || "None"}
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
              Key: "",
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
