import { Modifier, DragOperation } from "@dnd-kit/abstract"
import { DragDropManager } from "@dnd-kit/dom"
import { Coordinates, BoundingRectangle } from "@dnd-kit/geometry"
import { isOverTable } from "@/lib/dnd-kit/is-over-table"

function restrictToBoundingRect(
  transform: Coordinates,
  rect: BoundingRectangle,
  boundingRect: BoundingRectangle,
): Coordinates {
  const value = {
    ...transform,
  }

  if (rect.bottom + transform.y >= boundingRect.top + boundingRect.height) {
    value.y = boundingRect.top + boundingRect.height - rect.bottom
  }

  if (rect.left + transform.x <= boundingRect.left) {
    value.x = boundingRect.left - rect.left
  } else if (
    rect.right + transform.x >=
    boundingRect.left + boundingRect.width
  ) {
    value.x = boundingRect.left + boundingRect.width - rect.right
  }

  return value
}

interface RestrictToBottomOfParentElementOptions {
  /** The element whose bottom, left and right edges limit the drag preview */
  element: Element | null
}

/**
 * Keeps the drag preview inside the element's bottom, left and right edges.
 * The top is deliberately left open so the user can drag up to the file tabs.
 */
export class RestrictToBottomOfParentElement extends Modifier<
  DragDropManager,
  RestrictToBottomOfParentElementOptions
> {
  public apply(operation: DragOperation): Coordinates {
    const { transform, shape } = operation
    const element = this.options?.element

    if (
      this.disabled ||
      !isOverTable(this.manager) ||
      !shape ||
      !element?.isConnected
    ) {
      return transform
    }

    // The preview at its starting place (initial centre), with its current
    // size, because the preview changes size during the drag.
    const { width, height } = shape.current.boundingRectangle
    const left = shape.initial.center.x - width / 2
    const top = shape.initial.center.y - height / 2

    const draggedRect: BoundingRectangle = {
      left,
      top,
      width,
      height,
      right: left + width,
      bottom: top + height,
    }

    return restrictToBoundingRect(
      transform,
      draggedRect,
      element.getBoundingClientRect(),
    )
  }
}
