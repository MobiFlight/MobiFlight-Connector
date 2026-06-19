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
  Key: 0,
}

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
      {keys?.Key !== 0 ? (
        <Kbd>{renderLegacyKeyCode(keys.Key)}</Kbd>
      ) : (
        t("Dialog.InputConfigWizard.InputActions.Keyboard.None")
      )}
    </KbdGroup>
  )
}

const renderLegacyKeyCode = (keyCode: number): string => {
  const specialKeys: Record<number, string> = {
    8: "Backspace",
    9: "Tab",
    13: "Enter",
    16: "Shift",
    17: "Ctrl",
    18: "Alt",
    27: "Esc",
    32: "Space",
    33: "Page Up",
    34: "Page Down",
    35: "End",
    36: "Home",
    37: "Left",
    38: "Up",
    39: "Right",
    40: "Down",
    45: "Insert",
    46: "Delete",

    112: "F1",
    113: "F2",
    114: "F3",
    115: "F4",
    116: "F5",
    117: "F6",
    118: "F7",
    119: "F8",
    120: "F9",
    121: "F10",
    122: "F11",
    123: "F12",
  }

  if (specialKeys[keyCode]) {
    return specialKeys[keyCode]
  }

  if (keyCode >= 48 && keyCode <= 90) {
    return String.fromCharCode(keyCode).toUpperCase()
  }

  if (keyCode >= 96 && keyCode <= 105) {
    return `Num ${keyCode - 96}`
  }

  return `Key ${keyCode}`
}

const KeyboardInputActionPanel = ({
  variant,
  config,
  onConfigChange,
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
      console.log(
        "Scanned key:",
        event.key,
        "Key Code:",
        event.keyCode,
        "Code:",
        event.code,
      )

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
