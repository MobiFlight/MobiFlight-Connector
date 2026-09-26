import type { BoundingRectangle, Coordinates } from "@dnd-kit/geometry"
export function restrictToBoundingRect(
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
