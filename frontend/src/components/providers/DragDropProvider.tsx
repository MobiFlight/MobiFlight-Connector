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
}

export function DragDropProvider({ 
  children, 
  table, 
  data, 
  setItems 
}: DragDropProviderProps) {
  const { dragItem, dndContextProps } = useConfigItemDragDrop({
    table: table,
    data,
    setItems
  })

  return (
    <DragDropContext.Provider value={{ dragItemId: dragItem?.id as string }}>
    <DndContext {...dndContextProps}>
      {children}
    </DndContext>
    </DragDropContext.Provider>
  )
}