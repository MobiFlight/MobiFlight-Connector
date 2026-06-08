import { Button } from "@/components/ui/button"
import { useClipboardAction, useClipboardCopy } from "@/stores/clipboardStore"
import { Action } from "@/types/config"
import { IconClipboard, IconCopy } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export type CopyPasteActionPanelProps = {
  action: Action | undefined
  onActionChange: (action: Action) => void
}

const CopyPasteActionPanel = ({
  action,
  onActionChange,
}: CopyPasteActionPanelProps) => {
  const { t } = useTranslation()
  const copy = useClipboardCopy()
  const clipBoardAction = useClipboardAction()
  const copyEnabled = action !== undefined
  const pasteEnabled = clipBoardAction !== null

  return (
    <div className="flex flex-row gap-2">
      <Button
        title={t("Dialog.InputConfigWizard.Action.Copy")}
        size="sm"
        className="[&_svg]:size-5 px-2"
        variant="ghost"
        disabled={!copyEnabled}
        onClick={() => copyEnabled && copy(action)}
      >
        <IconCopy />
        <div className="text-sm">{t("General.Copy")}</div>
      </Button>
      <Button
        title={t("Dialog.InputConfigWizard.Action.Paste")}
        size="sm"
        className="[&_svg]:size-5 px-2 flex flex-row items-center gap-1"
        variant="ghost"
        disabled={!pasteEnabled}
        onClick={() => clipBoardAction && onActionChange(clipBoardAction)}
      >
        <IconClipboard />
        <div className="text-sm">{t("General.Paste")}</div>
      </Button>
    </div>
  )
}
export default CopyPasteActionPanel
