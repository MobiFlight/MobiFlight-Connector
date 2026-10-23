import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { cn } from "@/lib/utils"
import {
  IconArrowBackUp,
  IconCheck,
  IconChevronDown,
} from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

type HistoryControlsProps = {
  history: { action: string }[]
  cursor: number
  historyStart: number
  onUndo: () => void
  onJump: (index: number) => void
}

const HistoryControls = ({
  history,
  cursor,
  historyStart,
  onUndo,
  onJump,
}: HistoryControlsProps) => {
  const { t } = useTranslation()

  const canUndo = cursor > 0
  const hasHistory = history.length > 1

  const undoTitle = t("Dialog.InputConfigWizard.History.Undo")
  const historyTitle = t("Dialog.InputConfigWizard.History.Title")

  return (
    <div
      data-testid="config-history-controls"
      className="group/undo border-input bg-background relative inline-flex h-9 shrink-0 items-stretch rounded-md border"
    >
      <Button
        data-testid="config-undo-button"
        type="button"
        variant="ghost"
        size="sm"
        className={cn("h-full rounded-md px-3", hasHistory && "pr-10")}
        aria-label={undoTitle}
        title={`${undoTitle} (Ctrl+Z)`}
        disabled={!canUndo}
        onClick={onUndo}
      >
        <IconArrowBackUp size={16} />
        {undoTitle}
      </Button>

      {hasHistory && (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button
              data-testid="config-history-button"
              type="button"
              variant="ghost"
              size="sm"
              className={cn(
                "border-input bg-background pointer-events-none absolute inset-y-0 right-0 h-full w-8 rounded-l-none rounded-r-md border-l p-0 opacity-0 transition-opacity",
                "group-hover/undo:pointer-events-auto group-hover/undo:opacity-100",
                "focus-visible:pointer-events-auto focus-visible:opacity-100",
                "data-[state=open]:pointer-events-auto data-[state=open]:opacity-100",
                "[@media(hover:none)]:pointer-events-auto [@media(hover:none)]:opacity-100",
              )}
              aria-label={historyTitle}
              title={historyTitle}
            >
              <IconChevronDown size={16} />
            </Button>
          </DropdownMenuTrigger>

          <DropdownMenuContent
            data-testid="config-history-panel"
            side="top"
            align="start"
            sideOffset={8}
            className="z-60 w-64 max-w-[85vw]"
          >
            <DropdownMenuLabel className="flex items-center justify-between text-sm font-semibold">
              {historyTitle}

              <span className="text-muted-foreground text-xs font-normal tabular-nums">
                {historyStart + cursor}
              </span>
            </DropdownMenuLabel>

            <DropdownMenuSeparator />

            <div className="max-h-56 overflow-y-auto">
              {history
                .map((entry, index) => ({ entry, index }))
                .reverse()
                .map(({ entry, index }) => {
                  const isCurrent = index === cursor

                  const label =
                    index === 0
                      ? historyStart === 0
                        ? t("Dialog.InputConfigWizard.History.Original")
                        : t("Dialog.InputConfigWizard.History.Oldest")
                      : entry.action

                  return (
                    <DropdownMenuItem
                      key={historyStart + index}
                      data-testid="config-history-entry"
                      aria-current={isCurrent ? "step" : undefined}
                      title={label}
                      className={cn(
                        "text-xs font-normal",
                        isCurrent && "bg-accent font-medium",
                      )}
                      onSelect={() => {
                        if (index < cursor) {
                          onJump(index)
                        }
                      }}
                    >
                      <span className="text-muted-foreground w-5 shrink-0 tabular-nums">
                        {historyStart + index}
                      </span>

                      <span className="min-w-0 flex-1 truncate">{label}</span>

                      {isCurrent && (
                        <IconCheck
                          size={14}
                          className="text-primary shrink-0"
                          aria-label={t(
                            "Dialog.InputConfigWizard.History.Current",
                          )}
                        />
                      )}
                    </DropdownMenuItem>
                  )
                })}
            </div>
          </DropdownMenuContent>
        </DropdownMenu>
      )}
    </div>
  )
}

export default HistoryControls
