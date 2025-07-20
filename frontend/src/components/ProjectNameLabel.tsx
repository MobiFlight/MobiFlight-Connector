import { useProjectStore } from "@/stores/projectStore"
import {
  IconDotsVertical,
  IconEdit,
} from "@tabler/icons-react"
import { useTranslation } from "react-i18next"
import { Button } from "./ui/button"
import { useEffect, useRef, useState } from "react"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandMainMenuPayload, CommandProjectToolbar } from "@/types/commands"
import { Input } from "./ui/input"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "./ui/dropdown-menu"
import { AnimatedSaveButton } from "./ui/animated-save-button"

const ProjectNameLabel = () => {
  const { t } = useTranslation()
  const { project, hasChanged } = useProjectStore()
  const { publish } = publishOnMessageExchange()

  const [projectName, setProjectName] = useState<string>(project?.Name || "")
  const [tempProjectName, setTempProjectName] = useState(projectName)
  const [isEditing, setIsEditing] = useState(false)

  const inputRef = useRef<HTMLInputElement>(null)

  // if the project updates,
  // we want to update the project name
  useEffect(() => {
    if (project) {
      setProjectName(project.Name)
    }
  }, [project])

  useEffect(() => {
    if (isEditing && inputRef.current) {
      setTimeout(() => {
        inputRef?.current?.select()
      }, 200)
    }
  }, [isEditing])

  const handleMenuItemClick = (payload: CommandMainMenuPayload) => {
    publish({
      key: "CommandMainMenu",
      payload: payload,
    })
  }

  return (
    <div
      className="flex flex-row items-center gap-1 pl-1"
      data-testid="project-name-label"
    >
      {isEditing ? (
        <Input
          ref={inputRef}
          className="rounded-0 text-md md:text-md flex h-6 border-none p-1 focus-visible:ring-1"
          type="text"
          value={tempProjectName}
          onChange={(e) => setTempProjectName(e.target.value)}
          onBlur={() => setIsEditing(false)}
          onKeyDown={(e) => {
            e.stopPropagation()

            if (e.key === "Escape") {
              setIsEditing(false)
              setTempProjectName(projectName)
            }
            if (e.key === "Enter") {
              setIsEditing(false)
              setProjectName(e.currentTarget.value)
              publish({
                key: "CommandProjectToolbar",
                payload: {
                  action: "rename",
                  value: e.currentTarget.value,
                },
              } as CommandProjectToolbar)
            }
          }}
        />
      ) : (
        <span
          onClick={() => setIsEditing(true)}
          className="cursor-pointer font-semibold px-2"
        >
          {projectName}
        </span>
      )}
      <AnimatedSaveButton
        hasChanges={hasChanged}
        onSave={() => handleMenuItemClick({ action: "file.save" })}
        className="text-md"
        tooltip={hasChanged ? t("Project.Toolbar.Save.HasChanges") : t("Project.Toolbar.Save.NoChanges")}
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
                setIsEditing(true)
              }}
            >
              <IconEdit />
              {t("Project.File.Action.Rename")}
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  )
}

export default ProjectNameLabel
