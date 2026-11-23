import { Button } from "@/components/ui/button"
import { IconEdit, IconTrash } from "@tabler/icons-react"

export type SummaryButtonsProps = {
  onEdit: () => void
  editLabel: string
  onRemove?: () => void
  removeLabel?: string
}

// Edit and remove buttons on the right side of a summary row.
// The row needs the "group" class: the remove button is only visible on hover.
const SummaryButtons = ({
  onEdit,
  editLabel,
  onRemove,
  removeLabel,
}: SummaryButtonsProps) => {
  return (
    <div className="ml-auto flex w-20 flex-row justify-end pt-4">
      {onRemove && (
        <Button
          size={"sm"}
          className="invisible h-8 px-2 py-1 text-red-600 group-hover:visible hover:text-red-600"
          variant="ghost"
          onClick={onRemove}
        >
          <IconTrash />
          <span className="sr-only">{removeLabel}</span>
        </Button>
      )}
      <Button
        size={"sm"}
        className="group-hover:text-foreground text-muted-foreground h-8 px-2 py-1"
        variant="ghost"
        onClick={onEdit}
      >
        <IconEdit />
        <span className="sr-only">{editLabel}</span>
      </Button>
    </div>
  )
}

export default SummaryButtons
