import { Modifier, DragOperation } from "@dnd-kit/abstract"
import { Coordinates } from "@dnd-kit/geometry"

export class SnapToCursor extends Modifier {
  public apply(operation: DragOperation): Coordinates {
    if (this.disabled) return operation.transform

    const { transform, shape, position } = operation

    if (shape?.current && shape?.initial) {
      if (!position?.initial) {
        return transform
      }

      const offsetX = position.initial.x - shape.initial.boundingRectangle.left
      const offsetY = position.initial.y - shape.initial.boundingRectangle.top

      return {
        x: transform.x + offsetX,
        y: transform.y + offsetY - shape.current.boundingRectangle.height / 2,
      }
    }
    return transform
  }
}
