import { createContext, ReactNode, useContext } from "react"
import { DndContext } from "@dnd-kit/core"
import { useConfigItemDragDrop } from "@/lib/hooks/useConfigItemDragDrop"
import { Table } from "@tanstack/react-table"
import { IConfigItem } from "@/types"

interface DragDropContextType {
  dragItemId?: string
}

const DragDropContext = createContext<DragDropContextType>({})

export const useDragDropContext = () => {
  return useContext(DragDropContext)
}

interface DragDropProviderProps {
  children: ReactNode
  table: Table<IConfigItem> | null
  data: IConfigItem[]
  setItems: (items: IConfigItem[]) => void
  configIndex: number
}

export function DragDropProvider({ 
  children, 
  table, 
  data, 
  setItems,
  configIndex
}: DragDropProviderProps) {
  const { dragItem, dndContextProps } = useConfigItemDragDrop({
    table: table,
    data,
    setItems,
    configIndex
  })

  return (
    <DragDropContext.Provider value={{ dragItemId: dragItem?.id as string }}>
    <DndContext {...dndContextProps}>
      {children}
    </DndContext>
    </DragDropContext.Provider>
  )
}