import { useProjectStore } from "@/stores/projectStore"
import { IconBriefcaseFilled, IconDotsVertical, IconEdit } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"
import { Button } from "./ui/button"
import { useEffect, useRef, useState } from "react"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandProjectToolbar } from "@/types/commands"
import { Input } from "./ui/input"
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "./ui/dropdown-menu"

const ProjectNameLabel = () => {
  const { t } = useTranslation()
  const { project } = useProjectStore()
  const { publish } = publishOnMessageExchange()

  const intialProjectName = project?.Name ?? "Dummy Project Name"
  const [projectName, setProjectName] = useState(intialProjectName)
  const [isEditing, setIsEditing] = useState(false)

  const inputRef = useRef<HTMLInputElement>(null)
  
  useEffect(() => {
    if (isEditing && inputRef.current) {
      setTimeout(() => { inputRef?.current?.select() }, 200)
    }
  }, [isEditing])

  return (
    <div className="flex flex-row items-center gap-2">
      <IconBriefcaseFilled size={18} className="min-w-8 max-w-8 fill-primary"/>
      {isEditing ? (
        <Input
          ref={inputRef}
          className="flex rounded-0 h-6 border-none p-1 text-md md:text-md focus-visible:ring-1"
          type="text"
          value={projectName}
          onChange={(e) => setProjectName(e.target.value)}
          onBlur={() => setIsEditing(false)}
          onKeyDown={(e) => {
            e.stopPropagation()

            if (e.key === "Escape") {
              setIsEditing(false)
              setProjectName(intialProjectName)
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
          className="cursor-pointer"
        >
          {projectName}
        </span>
      )}
      <div className="relative">
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            {/* <Button variant={variant} className={cn(groupHoverStyle, "w-8 rounded-l-none p-0 rounded-b-none pb-0 border-l-0 border-b-0")}> */}
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
