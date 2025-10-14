import FileButton from "./FileButton"
import { Button } from "../ui/button"
import {
  IconFolderPlus,
  IconMinusVertical,
  IconPlus,
} from "@tabler/icons-react"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { useProjectStore } from "@/stores/projectStore"
import { useCallback, useEffect, useRef } from "react"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu"
import { useTranslation } from "react-i18next"
import ExecutionToolbar from "../ExecutionToolbar"
import ProjectNameLabel from "./ProjectNameLabel"
import { useConfigItemDragContext } from "@/lib/hooks/useConfigItemDragContext"

const ProjectPanel = () => {
  const { t } = useTranslation()
  const { publish } = publishOnMessageExchange()

  const { 
    activeConfigFileIndex,
    setActiveConfigFileIndex,
    project 
  } = useProjectStore()

  const configFiles = project?.ConfigFiles ?? []

  useEffect(() => {
    if (project === null) return

    if (project.ConfigFiles === null || project.ConfigFiles.length === 0) return

    if (activeConfigFileIndex >= project.ConfigFiles.length) {
      setActiveConfigFileIndex(project.ConfigFiles.length - 1)
      return
    }

  }, [project, activeConfigFileIndex, setActiveConfigFileIndex])

  const selectActiveFile = useCallback((index: number) => {
    setActiveConfigFileIndex(index)
  }, [setActiveConfigFileIndex])

  useEffect(() => {
    publishOnMessageExchange().publish({
      key: "CommandActiveConfigFile",
      payload: {
        index: activeConfigFileIndex,
      },
    })
  }, [activeConfigFileIndex])

  const addConfigFile = () => {
    publishOnMessageExchange().publish({
      key: "CommandAddConfigFile",
      payload: {
        type: "create",
      },
    })

    setTimeout(() => setActiveConfigFileIndex(configFiles.length), 200)
  }

  const mergeConfigFile = () => {
    publish({
      key: "CommandAddConfigFile",
      payload: {
        type: "merge",
      },
    })

    setTimeout(() => setActiveConfigFileIndex(configFiles.length), 200)
  }
  
  const { dragState } = useConfigItemDragContext()
  
  // Hover timer ref
  const hoverTimeoutRef = useRef<NodeJS.Timeout | null>(null)
  useEffect(() => {
    if (dragState?.ui.hoveredTabIndex !== undefined && dragState?.ui.hoveredTabIndex !== -1) {
      hoverTimeoutRef.current = setTimeout(() => {
        selectActiveFile(dragState?.ui.hoveredTabIndex)
      }, 600)
    } else {
      if (hoverTimeoutRef.current) {
        clearTimeout(hoverTimeoutRef.current)
        hoverTimeoutRef.current = null
      }
    }
  }, [dragState?.ui.hoveredTabIndex, selectActiveFile])

  return (
    <div
      className="border-b-solid flex flex-row gap-2 border-b border-b-muted-foreground/50 pb-0 pl-0 pr-2 pt-1"
      data-testid="project-panel"
    >
      <div className="flex flex-row items-center rounded-md rounded-bl-none rounded-br-none border border-b-0 border-solid border-muted-foreground/50 px-2">
        <ProjectNameLabel />
        <IconMinusVertical className="stroke-muted-foreground/50" />
        <ExecutionToolbar />
      </div>

      <div className="flex flex-row items-end gap-0 rounded-md" role="tablist">
        {configFiles?.map((file, index) => {
          return (
            <FileButton
              key={index}
              variant={index === activeConfigFileIndex ? "tabActive" : dragState?.ui.hoveredTabIndex === index ? "tabDragging" : "tabDefault"}
              file={file}
              index={index}
              selectActiveFile={selectActiveFile}
            ></FileButton>
          )
        })}
      </div>
      <div className="relative">
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <div className="py-1">
              <Button variant={"ghost"} className="h-8 px-2">
                <span className="sr-only">{t("General.Action.OpenMenu")}</span>
                <IconPlus />
              </Button>
            </div>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="start">
            <DropdownMenuItem onClick={addConfigFile}>
              <IconPlus />
              {t("Project.File.Action.New")}
            </DropdownMenuItem>
            <DropdownMenuItem onClick={mergeConfigFile}>
              <IconFolderPlus />
              {t("Project.File.Action.Merge")}
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  )
}

export default ProjectPanel
