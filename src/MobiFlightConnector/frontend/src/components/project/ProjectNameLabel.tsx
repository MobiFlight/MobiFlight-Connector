import { useProjectStore } from "@/stores/projectStore"
import {
  IconDeviceGamepad2,
  IconDotsVertical,
  IconPencil,
  IconSettings,
  IconArrowBackUp,
  IconChevronDown,
  IconCheck,
} from "@tabler/icons-react"
import { useTranslation } from "react-i18next"
import { Button } from "../ui/button"
import { useEffect, useRef, useState } from "react"
import messageExchange from "@/lib/messageExchange"
import { CommandMainMenuPayload, CommandProjectToolbar } from "@/types/commands"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  DropdownMenuLabel,
  DropdownMenuSeparator,
} from "../ui/dropdown-menu"
import { AnimatedSaveButton } from "../ui/AnimatedSaveButton"
import { InlineEditLabel, InlineEditLabelRef } from "../InlineEditLabel"
import { Project } from "@/types"
import { useProjectModal } from "@/lib/hooks/useProjectModal"
import { ProjectInfo } from "@/types/project"
import { useModal } from "@/lib/hooks/useModal"
import ToolTip from "@/components/ToolTip"
import { cn } from "@/lib/utils"
import { CommandUndoConfigList } from "@/types/commands"

export type ProjectNameLabelProps = {
  project: Project | null
}

const ProjectNameLabel = () => {
  const { t } = useTranslation()
  const {
    project,
    hasChanged,
    canUndoConfigList,
    configListUndoHistory,
    configListUndoHistoryStart,
  } = useProjectStore()
  const { publish } = messageExchange
  const label = project?.Name ?? "Untitled Project"
  const [optimisticLabel, setOptimisticLabel] = useState(label)
  const { showOverlay: showModalOverlay } = useModal()

  // Sync optimisticLabel when label changes from backend
  useEffect(() => {
    setOptimisticLabel(label)
  }, [label])

  const inlineEditRef = useRef<InlineEditLabelRef>(null)

  const handleMenuItemClick = (payload: CommandMainMenuPayload) => {
    publish({
      key: "CommandMainMenu",
      payload: payload,
    })
  }

  const handleProjectNameSave = (newName: string) => {
    setOptimisticLabel(newName)
    publish({
      key: "CommandProjectToolbar",
      payload: {
        action: "rename",
        value: newName,
      },
    } as CommandProjectToolbar)
  }

  const { showOverlay } = useProjectModal()
  const handleSettingsClick = () => {
    showOverlay({ mode: "edit", project: project as ProjectInfo })
  }

  const historyCount = configListUndoHistory.length
  const hasHistory = historyCount > 0

  const historyLabels: Record<string, string> = {
    Add: "Added",
    Duplicate: "Duplicated",
    Delete: "Deleted",
    BulkDelete: "Deleted",
    Toggle: "Toggled",
    Resort: "Moved",
    ProfileAdd: "Profile added",
    ProfileRename: "Profile renamed",
  }

  const getHistoryLabel = (action: string) =>
    t(`ConfigList.History.${action}`, {
      defaultValue: historyLabels[action] ?? action,
    })

  const undoSteps = (steps: number) => {
    if (steps <= 0 || !canUndoConfigList) return

    publish({
      key: "CommandUndoConfigList",
      payload: { steps },
    } as CommandUndoConfigList)
  }

  return (
    <div
      className="flex flex-row items-center gap-1 pl-1"
      data-testid="project-name-label"
    >
      <InlineEditLabel
        labelClassName="max-w-72 xl:max-w-96 3xl:max-w-120 truncate"
        ref={inlineEditRef}
        value={optimisticLabel}
        onSave={handleProjectNameSave}
      />

      <AnimatedSaveButton
        hasChanged={hasChanged}
        onSave={() => handleMenuItemClick({ action: "file.save" })}
        className="text-md"
        saveTooltip={t("Project.Toolbar.Save.HasChanges")}
        successTooltip={t("Project.Toolbar.Save.Success")}
        noChangesTooltip={t("Project.Toolbar.Save.NoChanges")}
      />

      <ToolTip content={t("Dialog.InputConfigWizard.History.Undo")}>
        <div
          className="group/list-undo relative inline-flex h-8 items-center"
          data-testid="config-list-history-controls"
        >
          <ToolTip content={t("Dialog.InputConfigWizard.History.Undo")}>
            <span className="inline-flex">
              <Button
                type="button"
                variant="ghost"
                className={cn("h-8 p-1 [&_svg]:size-6", hasHistory && "pr-8")}
                disabled={!canUndoConfigList}
                data-testid="config-list-undo-button"
                aria-label={t("Dialog.InputConfigWizard.History.Undo")}
                onClick={() => undoSteps(1)}
              >
                <IconArrowBackUp
                  className={
                    canUndoConfigList
                      ? "stroke-foreground"
                      : "stroke-muted-foreground"
                  }
                />
              </Button>
            </span>
          </ToolTip>

          {hasHistory && (
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  data-testid="config-list-history-button"
                  aria-label={t("Dialog.InputConfigWizard.History.Title")}
                  className={cn(
                    "pointer-events-none absolute inset-y-0 right-0 h-8 w-7 rounded-l-none p-0 opacity-0 transition-opacity",
                    "group-hover/list-undo:pointer-events-auto group-hover/list-undo:opacity-100",
                    "focus-visible:pointer-events-auto focus-visible:opacity-100",
                    "data-[state=open]:pointer-events-auto data-[state=open]:opacity-100",
                    "[@media(hover:none)]:pointer-events-auto [@media(hover:none)]:opacity-100",
                  )}
                >
                  <IconChevronDown size={16} />
                </Button>
              </DropdownMenuTrigger>

              <DropdownMenuContent
                align="start"
                side="bottom"
                sideOffset={8}
                className="z-60 w-64 max-w-[85vw]"
                data-testid="config-list-history-panel"
              >
                <DropdownMenuLabel className="flex items-center justify-between text-sm font-semibold">
                  {t("Dialog.InputConfigWizard.History.Title")}

                  <span className="text-muted-foreground text-xs font-normal tabular-nums">
                    {configListUndoHistoryStart + historyCount}
                  </span>
                </DropdownMenuLabel>

                <DropdownMenuSeparator />

                <div className="max-h-56 overflow-y-auto">
                  {configListUndoHistory
                    .map((action, index) => ({ action, index }))
                    .reverse()
                    .map(({ action, index }) => {
                      const isCurrent = index === historyCount - 1
                      const label = getHistoryLabel(action)
                      const steps = historyCount - index - 1

                      return (
                        <DropdownMenuItem
                          key={configListUndoHistoryStart + index + 1}
                          data-testid="config-list-history-entry"
                          disabled={isCurrent}
                          aria-current={isCurrent ? "step" : undefined}
                          title={label}
                          onSelect={() => undoSteps(steps)}
                          className={cn(
                            "text-xs font-normal",
                            isCurrent && "bg-accent font-medium",
                          )}
                        >
                          <span className="text-muted-foreground w-6 shrink-0 tabular-nums">
                            {configListUndoHistoryStart + index + 1}
                          </span>

                          <span className="min-w-0 flex-1 truncate">
                            {label}
                          </span>

                          {isCurrent && (
                            <IconCheck
                              size={14}
                              className="text-primary shrink-0"
                            />
                          )}
                        </DropdownMenuItem>
                      )
                    })}

                  <DropdownMenuSeparator />

                  <DropdownMenuItem
                    data-testid="config-list-history-oldest"
                    onSelect={() => undoSteps(historyCount)}
                    className="text-xs"
                  >
                    {configListUndoHistoryStart === 0
                      ? t("Dialog.InputConfigWizard.History.Original")
                      : t("Dialog.InputConfigWizard.History.Oldest")}
                  </DropdownMenuItem>
                </div>
              </DropdownMenuContent>
            </DropdownMenu>
          )}
        </div>
      </ToolTip>

      <div className="relative">
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" className="h-8 w-4 px-2">
              <span className="sr-only">{t("General.Action.OpenMenu")}</span>
              <IconDotsVertical className="h-4 w-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuItem
              onClick={() => {
                inlineEditRef.current?.startEditing()
              }}
            >
              <IconPencil />
              {t("Project.File.Action.Rename")}
            </DropdownMenuItem>
            <DropdownMenuItem onClick={handleSettingsClick}>
              <IconSettings />
              {t("Project.Toolbar.Settings")}
            </DropdownMenuItem>
            <DropdownMenuItem
              onClick={() => showModalOverlay({ route: "/bindings" })}
            >
              <IconDeviceGamepad2 />
              {t("MainMenu.Extras.ControllerBindings")}
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  )
}

export default ProjectNameLabel
