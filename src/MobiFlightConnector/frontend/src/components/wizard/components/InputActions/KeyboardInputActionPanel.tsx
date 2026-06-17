import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { KeyInputAction } from "@/types/config"
import { IconTrash } from "@tabler/icons-react"
import { useState } from "react"
import { useTranslation } from "react-i18next"

export type KeyboardInputActionPanelProps = {
  variant: "summary" | "details"
  config: KeyInputAction | null
  onConfigChange: (config: KeyInputAction) => void
}

const emptyConfig: KeyInputAction = {
  Type: "KeyInputAction",
  Control: false,
  Alt: false,
  Shift: false,
  Key: 0,
}

const KeyboardInputActionPanel = ({
  variant,
  config,
  onConfigChange
}: KeyboardInputActionPanelProps) => {
  const { t } = useTranslation()
  const [isScanning, setIsScanning] = useState(false)
  const [scannedKeys, setScannedKeys] = useState<KeyInputAction>(
    config?.Key !== undefined ? config : emptyConfig,
  )

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
      const key =
        scannedKey === "Control" ||
        scannedKey === "Shift" ||
        scannedKey === "Alt"
          ? 0
          : keyCode

      const newConfig: KeyInputAction = {
        Type: "KeyInputAction",
        Control: event.ctrlKey,
        Alt: event.altKey,
        Shift: event.shiftKey,
        Key: key,
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
        Key: 0,
      })
    }
  }

  if (variant === "summary") {
    return (
      <div className="flex grow flex-row items-center justify-between gap-8">
        <div className="flex flex-row items-center gap-2">
          <div className="text-sm font-medium">
            <Label htmlFor="preset">
              {t(
                "Dialog.InputConfigWizard.InputActions.Keyboard.KeyComboLabel",
              )}
            </Label>
            <div className="text-sm">
              {config?.Control && "Ctrl + "}
              {config?.Alt && "Alt + "}
              {config?.Shift && "Shift + "}
              {config?.Key !== 0
                ? String.fromCharCode(config!.Key).toUpperCase()
                : t("Dialog.InputConfigWizard.InputActions.Keyboard.None")}
            </div>
          </div>
        </div>
      </div>
    )
  }

  return (
    <div
      className="flex flex-col gap-4"
      onKeyDown={handleKeyDown}
      onKeyUp={handleKeyUp}
      tabIndex={0}
    >
      <div className="flex flex-row gap-4">
        <Button onClick={handleScanForInput} size={"sm"}>
          {isScanning
            ? t("Dialog.InputConfigWizard.InputActions.Keyboard.StopScanning")
            : t(
                "Dialog.InputConfigWizard.InputActions.Keyboard.ScanForKeyboard",
              )}
        </Button>
        <div className="flex flex-row items-center gap-2">
          <div className="text-sm font-medium">
            {t("Dialog.InputConfigWizard.InputActions.Keyboard.KeyComboLabel")}
          </div>
          <div className="text-sm">
            {scannedKeys?.Control && "Ctrl + "}
            {scannedKeys?.Alt && "Alt + "}
            {scannedKeys?.Shift && "Shift + "}
            {scannedKeys?.Key !== 0
              ? String.fromCharCode(scannedKeys.Key).toUpperCase()
              : t("Dialog.InputConfigWizard.InputActions.Keyboard.None")}
          </div>
        </div>
        <Button
          variant="ghost"
          onClick={() => {
            setScannedKeys(emptyConfig)
            onConfigChange(emptyConfig)
          }}
          disabled={isScanning}
          size={"sm"}
        >
          <IconTrash />
          {t("Dialog.InputConfigWizard.InputActions.Keyboard.ClearInput")}
        </Button>
      </div>
    </div>
  )
}
export default KeyboardInputActionPanel
