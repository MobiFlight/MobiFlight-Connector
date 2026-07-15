import { Button } from "@/components/ui/button"
import { IconChevronDown, IconChevronUp } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

type MoveUpDownButtonsProps = {
  onMoveUp: () => void
  onMoveDown: () => void
  isFirst?: boolean
  isLast?: boolean
}

const MoveUpDownButtons = ({
  onMoveUp,
  onMoveDown,
  isFirst,
  isLast,
}: MoveUpDownButtonsProps) => {
  const { t } = useTranslation()
  return (
    <div className="flex flex-col items-center justify-center">
      <Button
        className="group-hover:text-foreground text-muted-foreground h-5 w-5 p-1"
        variant="ghost"
        onClick={onMoveUp}
        disabled={isFirst}
      >
        <IconChevronUp />
        <span className="sr-only">{t("Dialog.Modifiers.Editor.MoveUp")}</span>
      </Button>
      <Button
        className="group-hover:text-foreground text-muted-foreground h-5 w-5 p-1"
        variant="ghost"
        onClick={onMoveDown}
        disabled={isLast}
      >
        <IconChevronDown />
        <span className="sr-only">{t("Dialog.Modifiers.Editor.MoveDown")}</span>
      </Button>
    </div>
  )
}
export default MoveUpDownButtons
