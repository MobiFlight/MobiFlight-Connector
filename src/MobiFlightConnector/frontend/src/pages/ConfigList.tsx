// import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { columns } from "@/components/tables/config-item-table/config-item-table-columns"
import { useCallback } from "react"
import { useAppMessage } from "@/lib/hooks/appMessage"
import {
  ConfigValueFullUpdate,
  ConfigValuePartialUpdate,
  ConfigValueRawAndFinalUpdate,
} from "@/types/messages"
import { IConfigItem } from "@/types"
import { useProjectStore } from "@/stores/projectStore"
import { ConfigItemDragProvider } from "@/components/providers/DragDropProvider"
import ProjectPanel from "@/components/project/ProjectPanel"
import { ConfigItemTable } from "@/components/tables/config-item-table/config-item-table"
import ErrorFallback from "@/components/ErrorFallback"
import { ErrorBoundary } from "react-error-boundary"
import { useConfigItemStateStore } from "@/stores/configItemStateStore"

const ConfigListPage = () => {
  const {
    project,
    activeConfigFileIndex,
    setActiveConfigFileIndex,
    setConfigItems,
    updateConfigItem,
  } = useProjectStore()

  const { updateConfigItemState } = useConfigItemStateStore()

  useAppMessage("ConfigValuePartialUpdate", (message) => {
    console.log("ConfigValuePartialUpdate", message.payload)
    const update = message.payload as ConfigValuePartialUpdate
    // better performance for single updates
    if (update.ConfigItems.length === 1) {
      updateConfigItem(activeConfigFileIndex, update.ConfigItems[0], true)
      return
    }
    setConfigItems(activeConfigFileIndex, update.ConfigItems)
  })

  useAppMessage("ConfigValueRawAndFinalUpdate", (message) => {
    console.log(
      "ConfigValueRawAndFinalUpdate",
      message.payload as ConfigValueRawAndFinalUpdate,
    )
    const update = message.payload as ConfigValueRawAndFinalUpdate

    // We only update our special store with runtime values
    updateConfigItemState(update.ConfigItems)
  })

  useAppMessage("ConfigValueFullUpdate", (message) => {
    const update = message.payload as ConfigValueFullUpdate
    console.log("ConfigValueFullUpdate", update)
    setConfigItems(update.ConfigIndex, update.ConfigItems)
  })

  const configItems =
    project?.ConfigFiles[activeConfigFileIndex]?.ConfigItems ?? []

  // Function to get config items from project store
  const getConfigItems = useCallback(
    (configIndex: number): IConfigItem[] => {
      return project?.ConfigFiles[configIndex]?.ConfigItems ?? []
    },
    [project],
  )

  return (
    <div className="flex flex-col gap-2  grow overflow-y-auto">
      <ConfigItemDragProvider
        initialConfigIndex={activeConfigFileIndex}
        updateConfigItems={setConfigItems}
        getConfigItems={getConfigItems}
        selectActiveFile={setActiveConfigFileIndex}
      >
        <ErrorBoundary FallbackComponent={ErrorFallback}>
          <ProjectPanel />
        </ErrorBoundary>
        <div className="flex flex-col gap-4 overflow-y-auto grow">
          <ErrorBoundary FallbackComponent={ErrorFallback}>
            <ConfigItemTable columns={columns} data={configItems} />
          </ErrorBoundary>
        </div>
      </ConfigItemDragProvider>
    </div>
  )
}

export default ConfigListPage
