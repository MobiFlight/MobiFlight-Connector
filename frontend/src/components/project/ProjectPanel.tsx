import FileButton from "./FileButton"
import { Button } from "../ui/button"
import { IconFolderPlus, IconPlus } from "@tabler/icons-react"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { useProjectStore } from "@/stores/projectStore"
import { useEffect, useState } from "react"
import { useConfigStore } from "@/stores/configFileStore"

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
    <div className="flex flex-row items-center gap-2">
      <div className="text-sm">Project files:</div>
      <div className="flex flex-row gap-2 overflow-x-auto">
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
      <Button variant="ghost" className="h-8" onClick={addConfigFile}>
        <IconPlus />
        Add new
      </Button>
      <Button variant="ghost" className="h-8" onClick={mergeConfigFile}>
        <IconFolderPlus />
        Add existing
      </Button>
    </div>
  )
}

export default ProjectPanel
