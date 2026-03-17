import { useProjectStore } from "@/stores/projectStore"
import { IconDeviceGamepad2, IconDotsVertical, IconPencil, IconSettings } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"
import { Button } from "../ui/button"
import { useEffect, useRef, useState } from "react"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandMainMenuPayload, CommandProjectToolbar } from "@/types/commands"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu"
import { AnimatedSaveButton } from "../ui/AnimatedSaveButton"
import { InlineEditLabel, InlineEditLabelRef } from "../InlineEditLabel"
import { Project } from "@/types"
import { useProjectModal } from "@/lib/hooks/useProjectModal"
import { ProjectInfo } from "@/types/project"
import { useModal } from "@/lib/hooks/useModal"

export type ProjectNameLabelProps = {
  project: Project | null
}

const ProjectNameLabel = () => {
  const { t } = useTranslation()
  const { project, hasChanged } = useProjectStore()
  const { publish } = publishOnMessageExchange()
  const label = project?.Name ?? "Untitled Project"
  const [optimisticLabel, setOptimisticLabel] = useState(label)
  const { showOverlay : showModalOverlay } = useModal()

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
