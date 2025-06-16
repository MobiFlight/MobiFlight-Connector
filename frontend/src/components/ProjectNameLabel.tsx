import { useProjectStore } from "@/stores/projectStore"
import {
  IconBriefcaseFilled,
  IconDotsVertical,
  IconEdit,
} from "@tabler/icons-react"
import { useTranslation } from "react-i18next"
import { Button } from "./ui/button"
import { useEffect, useRef, useState } from "react"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandProjectToolbar } from "@/types/commands"
import { Input } from "./ui/input"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "./ui/dropdown-menu"

const ProjectNameLabel = () => {
  const { t } = useTranslation()
  const { project } = useProjectStore()
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

  return (
    <div className="flex flex-row items-center gap-2" data-testid="project-name-label">
      <IconBriefcaseFilled size={18} className="min-w-8 max-w-8 fill-primary" />
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
        <span onClick={() => setIsEditing(true)} className="cursor-pointer">
          {projectName}
        </span>
      )}
      <div className="relative">
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" className="h-8 px-2">
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
