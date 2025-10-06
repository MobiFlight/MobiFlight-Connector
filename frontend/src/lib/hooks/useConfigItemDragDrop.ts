import { useCallback, useState } from "react"
import {
  closestCenter,
  useSensor,
  useSensors,
  DragEndEvent,
  MouseSensor,
  TouchSensor,
  DragStartEvent,
  Active,
} from "@dnd-kit/core"
import {
  restrictToParentElement,
  restrictToVerticalAxis,
} from "@dnd-kit/modifiers"
import { snapToCursor } from "@/lib/dnd-kit/snap-to-cursor"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { CommandResortConfigItem } from "@/types/commands"
import { IConfigItem } from "@/types"
import { Table } from "@tanstack/react-table"

interface UseConfigItemDragDropProps<TData> {
  table: Table<TData> | null
  data: TData[]
  setItems: (items: TData[]) => void
}

export function useConfigItemDragDrop({ table, data, setItems }: UseConfigItemDragDropProps<IConfigItem>) {
  const [dragItem, setDragItem] = useState<Active | undefined>(undefined)

  const sensors = useSensors(
    useSensor(MouseSensor, {}),
    useSensor(TouchSensor, {}),
  )

  const handleDragStart = useCallback(
    (event: DragStartEvent) => {
      const { active } = event
      setDragItem(active)

      if (!table) return

      const draggedRow = table
        .getRowModel()
        .rows.find((row) => row.id === active.id)
      if (!draggedRow) return
      if (!draggedRow.getIsSelected()) {
        table.setRowSelection({ [active.id]: true })
      }
    },
    [table],
  )

  const handleDragEnd = useCallback(
    (event: DragEndEvent) => {
      const { active, over } = event

      setDragItem(undefined)
      
      if (!over?.id || !active.id || !table) return

      // we didn't really move anything
      if (active.id === over.id) return

      // Get all selected row GUIDs, or just the dragged one if nothing selected
      const selectedRows = table.getSelectedRowModel().rows
      const selectedIds = selectedRows.map(
        (row) => (row.original as IConfigItem).GUID,
      )
      const originalIndex = (data as IConfigItem[]).findIndex(
        (item) => item.GUID === active.id,
      )

      // Remove dragged items from data
      let newData = (data as IConfigItem[]).filter(
        (item) => !selectedIds.includes(item.GUID),
      )
      // Find drop index
      const newIndex = newData.findIndex((item) => item.GUID === over.id)

      // we determine drag direction
      const dragDirectionOffset = newIndex >= originalIndex ? 1 : 0

      const draggedData = (data as IConfigItem[]).filter((item) =>
        selectedIds.includes(item.GUID),
      )

      // Insert dragged items at drop index
      newData = [
        ...newData.slice(0, newIndex + dragDirectionOffset),
        ...draggedData,
        ...newData.slice(newIndex + dragDirectionOffset),
      ]

      setItems(newData)

      publishOnMessageExchange().publish({
        key: "CommandResortConfigItem",
        payload: {
          items: draggedData,
          newIndex: newIndex + dragDirectionOffset,
        },
      } as CommandResortConfigItem)
    },
    [data, setItems, table],
  )

  return {
    dragItem,
    sensors,
    handleDragStart,
    handleDragEnd,
    dndContextProps: {
      sensors,
      collisionDetection: closestCenter,
      modifiers: [
        snapToCursor,
        restrictToVerticalAxis,
        restrictToParentElement,
      ],
      onDragEnd: handleDragEnd,
      onDragStart: handleDragStart,
    }
  }
}