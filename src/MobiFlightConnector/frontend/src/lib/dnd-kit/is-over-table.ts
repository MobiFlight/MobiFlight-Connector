import type { DragDropManager } from "@dnd-kit/dom"

/**
 * Whether the pointer is over the config item table right now.
 * Reading the target inside a modifier makes the library recompute the
 * transform as soon as the target changes, even if the pointer stands still.
 */
export const isOverTable = (manager: DragDropManager): boolean => {
  const type = manager.dragOperation.target?.data?.type
  return type === "table" || type === "row"
}
