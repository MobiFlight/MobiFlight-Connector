import { IConfigItem } from "@/types"
import { ColumnDef, Table } from "@tanstack/react-table"
import ProjectPanel from "@/components/project/ProjectPanel"
import { useDragDropContext } from "@/components/providers/DragDropProvider"
import { ConfigItemTable } from "@/components/tables/config-item-table/config-item-table"

// Create a separate component that can access the context
export const ConfigTableWrapper = ({ 
  activeConfigFileIndex, 
  configItems, 
  mySetItems, 
  tableRef, 
  columns 
}: {
  activeConfigFileIndex: number
  configItems: IConfigItem[]
  mySetItems: (items: IConfigItem[]) => void
  tableRef: React.RefObject<Table<IConfigItem> | null>
  columns: ColumnDef<IConfigItem>[]
}) => {
  // Now we can use the context because we're inside the provider
  const { dragItemId } = useDragDropContext()

  return (
    <>
      <ProjectPanel />
      <div className="flex flex-col gap-4 overflow-y-auto">
        <ConfigItemTable
          configIndex={activeConfigFileIndex}
          columns={columns}
          data={configItems}
          setItems={mySetItems}
          dragItemId={dragItemId}
          dataTableRef={tableRef}
        />
      </div>
    </>
  )
}