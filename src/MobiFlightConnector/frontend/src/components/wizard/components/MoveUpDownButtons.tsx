import { Button } from "@/components/ui/button"
import { IconChevronDown, IconChevronUp } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

type MoveUpDownButtonsProps = {
  onMoveUp: () => void
  onMoveDown: () => void
  isFirst?: boolean
  isLast?: boolean
  i18nPath?: string
}

const MoveUpDownButtons = ({
  onMoveUp,
  onMoveDown,
  isFirst,
  isLast,
  i18nPath = "Dialog.Modifiers.Editor",
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
        <span className="sr-only">{t(`${i18nPath}.MoveUp`)}</span>
      </Button>
      <Button
        className="group-hover:text-foreground text-muted-foreground h-5 w-5 p-1"
        variant="ghost"
        onClick={onMoveDown}
        disabled={isLast}
      >
        <IconChevronDown />
        <span className="sr-only">{t(`${i18nPath}.MoveDown`)}</span>
      </Button>
    </div>
  )
}
export default MoveUpDownButtons
