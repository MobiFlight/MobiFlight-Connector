import { Modifier } from "@dnd-kit/abstract"
import type { DragOperation } from "@dnd-kit/abstract"

// 1. Drop in the missing utility function
function getEventCoordinates(event: Event): { x: number; y: number } | null {
  if ("touches" in event && (event as TouchEvent).touches.length > 0) {
    return {
      x: (event as TouchEvent).touches[0].clientX,
      y: (event as TouchEvent).touches[0].clientY,
    }
  }

  if (
    "changedTouches" in event &&
    (event as TouchEvent).changedTouches.length > 0
  ) {
    return {
      x: (event as TouchEvent).changedTouches[0].clientX,
      y: (event as TouchEvent).changedTouches[0].clientY,
    }
  }

  if ("clientX" in event && "clientY" in event) {
    return {
      x: (event as MouseEvent | PointerEvent).clientX,
      y: (event as MouseEvent | PointerEvent).clientY,
    }
  }

  return null
}

export class SnapToCursor extends Modifier {
  override apply(operation: DragOperation) {
    const { activatorEvent, transform, shape } = operation

    if (!shape || !activatorEvent) {
      return transform
    }

    // 2. Use the local utility function
    const activatorCoordinates = getEventCoordinates(activatorEvent)

    if (!activatorCoordinates) {
      return transform
    }

    const currentShape = shape.initial 
    const { left, top, height } = currentShape.boundingRectangle

    const offsetY = activatorCoordinates.y - top
    const offsetX = activatorCoordinates.x - left

    return {
      ...transform,
      x: transform.x + offsetX,
      y: transform.y + offsetY - height / 2,
    }
  }
}
