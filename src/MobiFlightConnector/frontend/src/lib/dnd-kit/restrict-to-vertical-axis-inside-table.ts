import { Modifier, DragOperation } from "@dnd-kit/abstract"
import { DragDropManager } from "@dnd-kit/dom"
import { Coordinates } from "@dnd-kit/geometry"
import { isOverTable } from "./is-over-table"

/** Locks the horizontal position while the pointer is over the table */
export class RestrictToVerticalAxisInsideTable extends Modifier<DragDropManager> {
  public apply(operation: DragOperation): Coordinates {
    const { transform } = operation
    if (this.disabled || !isOverTable(this.manager)) return transform
    return { ...transform, x: 0 }
  }
}
