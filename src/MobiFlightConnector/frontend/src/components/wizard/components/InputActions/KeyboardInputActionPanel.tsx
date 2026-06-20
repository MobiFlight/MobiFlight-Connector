import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Kbd, KbdGroup } from "@/components/ui/kbd"
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
  Code: "",
}

const skipRenderKeys = [
  "ControlLeft", "ControlRight", 
  "ShiftLeft", "ShiftRight", 
  "AltLeft", "AltRight"]


const KeyboardShortCut = ({ keys }: { keys: KeyInputAction }) => {
  const { t } = useTranslation()
  return (
    <KbdGroup>
      {keys?.Control && (
        <>
          <Kbd>Ctrl</Kbd>
          <span> + </span>
        </>
      )}
      {keys?.Alt && (
        <>
          <Kbd>Alt</Kbd>
          <span> + </span>
        </>
      )}
      {keys?.Shift && (
        <>
          <Kbd>Shift</Kbd>
          <span> + </span>
        </>
      )}
      {keys?.Code !== "" && !skipRenderKeys.includes(keys?.Code) ? (
        <Kbd>{keys?.Code?.replace("Key", "")}</Kbd>
      ) : (
        t("Dialog.InputConfigWizard.InputActions.Keyboard.None")
      )}
    </KbdGroup>
  )
}

const KeyboardInputActionPanel = ({
  variant,
  config,
  onConfigChange,
}: KeyboardInputActionPanelProps) => {
  const { t } = useTranslation()
  const [isScanning, setIsScanning] = useState(false)
  const [scannedKeys, setScannedKeys] = useState<KeyInputAction>(
    config?.Code !== undefined ? config : emptyConfig,
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
      const scannedKey = event.code
      const key =
        scannedKey === "Control" ||
        scannedKey === "Shift" ||
        scannedKey === "Alt"
          ? ""
          : scannedKey

      const newConfig: KeyInputAction = {
        Type: "KeyInputAction",
        Control: event.ctrlKey,
        Alt: event.altKey,
        Shift: event.shiftKey,
        Code: key,
      }
      setScannedKeys(newConfig)
    }
  }

  const handleKeyUp = (event: React.KeyboardEvent<HTMLDivElement>) => {
    event.stopPropagation()
    event.preventDefault()

    if (!isScanning) return

    onConfigChange(scannedKeys)
    setIsScanning(false)
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
              <KeyboardShortCut keys={config ?? emptyConfig} />
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
            <KeyboardShortCut keys={scannedKeys} />
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
