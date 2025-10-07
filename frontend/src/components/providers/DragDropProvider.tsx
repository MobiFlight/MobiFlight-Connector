import { createContext, ReactNode, useCallback, useContext, useState } from "react"
import { DndContext } from "@dnd-kit/core"
import { useConfigItemDragDrop } from "@/lib/hooks/useConfigItemDragDrop"
import { Table } from "@tanstack/react-table"
import { IConfigItem } from "@/types"

interface DragDropContextType {
  dragItemId?: string,
  isDragging?: boolean
  table: Table<IConfigItem> | null
  setTable: (table: Table<IConfigItem> | null) => void
}

const DragDropContext = createContext<DragDropContextType>({
  isDragging: false,
  table: null,
  setTable: () => {},
})

export const useDragDropContext = () => {
  return useContext(DragDropContext)
}

interface DragDropProviderProps {
  children: ReactNode
  data: IConfigItem[]
  setItems: (items: IConfigItem[]) => void
  configIndex: number
}

export function DragDropProvider({ 
  children, 
  data, 
  setItems,
  configIndex
}: DragDropProviderProps) {
  const [table, setTable] = useState<Table<IConfigItem> | null>(null)

  const { dragItem, dndContextProps } = useConfigItemDragDrop({
    table: table,
    data,
    setItems,
    configIndex
  })

  const handleSetTable = useCallback((newTable: Table<IConfigItem> | null) => {
    console.log("Setting table in context:", newTable)
    setTable(newTable)
  }, [])

  return (
    <DragDropContext.Provider value={{ 
      dragItemId: dragItem?.id as string,
      isDragging: !!dragItem,
      table,
      setTable: handleSetTable
    }}>
    <DndContext {...dndContextProps}>
      {children}
    </DndContext>
    </DragDropContext.Provider>
  )
}