import FileButton from "./FileButton"
import { Button } from "../ui/button"
import {
  IconFolderPlus,
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

const ProjectPanel = () => {
  const { publish } = publishOnMessageExchange()

  const { project } = useProjectStore()
  const { setItems } = useConfigStore()

  const [activeConfigFile, setActiveConfigFile] = useState(0)

  useEffect(() => {
    if (project === null) return
    if (project.ConfigFiles === null || project.ConfigFiles.length === 0) return
    if (activeConfigFile >= project.ConfigFiles.length) return
    setItems(project.ConfigFiles[activeConfigFile].ConfigItems ?? [])
  }, [project, activeConfigFile, setItems])

  const configFiles = project?.ConfigFiles

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
  }

  const mergeConfigFile = () => {
    publish({
      key: "CommandAddConfigFile",
      payload: {
        type: "merge",
      },
    })
  }

  return (
    <div className="flex flex-row gap-2 pl-0 pr-2 pt-1 pb-0 border-b-solid border-b border-b-muted-foreground/50">
      <div className="flex flex-row items-end gap-0 rounded-md">
        {configFiles?.map((file, index) => {
          return (
            <FileButton
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
            <Button variant={"ghost"} className="px-2 h-8">
              <span className="sr-only">Add file</span>
              <IconPlus />
            </Button>
            </div>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="start">
            <DropdownMenuItem onClick={addConfigFile}>
              <IconPlus />
              Add new config
            </DropdownMenuItem>
            <DropdownMenuItem onClick={mergeConfigFile}>
              <IconFolderPlus />
              Add existing config
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  )
}

export default ProjectPanel
