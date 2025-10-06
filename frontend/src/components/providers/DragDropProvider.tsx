import { ReactNode } from "react"
import { DndContext } from "@dnd-kit/core"
import { useConfigItemDragDrop } from "@/lib/hooks/useConfigItemDragDrop"
import { Table } from "@tanstack/react-table"
import { IConfigItem } from "@/types"

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
  const { dndContextProps } = useConfigItemDragDrop({
    table: table,
    data,
    setItems
  })

  return (
    <DndContext {...dndContextProps}>
      {children}
    </DndContext>
  )
}