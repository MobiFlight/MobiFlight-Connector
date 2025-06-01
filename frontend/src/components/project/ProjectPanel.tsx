import FileButton from "./FileButton"
import { Button } from "../ui/button"
import {
  IconFolderPlus,
  IconMinusVertical,
  IconPlus,
} from "@tabler/icons-react"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { useProjectStore } from "@/stores/projectStore"
import { useEffect, useState } from "react"
import { useConfigStore } from "@/stores/configFileStore"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu"
import { useTranslation } from "react-i18next"
import ExecutionToolbar from "../ExecutionToolbar"
import ProjectNameLabel from "../ProjectNameLabel"

const ProjectPanel = () => {
  const { t } = useTranslation()
  const { publish } = publishOnMessageExchange()

  const { project } = useProjectStore()
  const { setItems } = useConfigStore()

  const [activeConfigFile, setActiveConfigFile] = useState(0)
  const configFiles = project?.ConfigFiles ?? []

  useEffect(() => {
    if (project === null) return

    if (project.ConfigFiles === null || project.ConfigFiles.length === 0) return

    if (activeConfigFile >= project.ConfigFiles.length) {
      setActiveConfigFile(project.ConfigFiles.length - 1)
      return
    }

    setItems(project.ConfigFiles[activeConfigFile].ConfigItems ?? [])
  }, [project, activeConfigFile, setItems])

  const selectActiveFile = (index: number) => {
    setActiveConfigFile(index)
  }

  useEffect(() => {
    publishOnMessageExchange().publish({
      key: "CommandActiveConfigFile",
      payload: {
        index: activeConfigFile,
      },
    })
  }, [activeConfigFile])

  const addConfigFile = () => {
    publishOnMessageExchange().publish({
      key: "CommandAddConfigFile",
      payload: {
        type: "create",
      },
    })

    setTimeout(() => selectActiveFile(configFiles.length), 200)
  }

  const mergeConfigFile = () => {
    publish({
      key: "CommandAddConfigFile",
      payload: {
        type: "merge",
      },
    })

    setTimeout(() => selectActiveFile(configFiles.length), 200)
  }

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
              variant={index === activeConfigFile ? "default" : "outline"}
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
