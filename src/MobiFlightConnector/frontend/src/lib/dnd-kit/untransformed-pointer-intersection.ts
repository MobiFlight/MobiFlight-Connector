import {
  CollisionDetector,
  CollisionPriority,
  CollisionType,
} from "@dnd-kit/abstract"

const getTranslateY = (element: Element): number => {
  const { transform } = getComputedStyle(element)
  if (!transform || transform === "none") return 0
  return new DOMMatrixReadOnly(transform).m42
}

export const untransformedPointerIntersection: CollisionDetector = ({
  dragOperation,
  droppable,
}) => {
  const pointer = dragOperation.position.current
  const element = (droppable as unknown as { element?: Element }).element
  if (!pointer || !element) return null

  const rect = element.getBoundingClientRect()
  const translateY = getTranslateY(element)
  const top = rect.top - translateY
  const bottom = rect.bottom - translateY

  if (
    pointer.x < rect.left ||
    pointer.x > rect.right ||
    pointer.y < top ||
    pointer.y > bottom
  ) {
    return null
  }

  const centerX = rect.left + rect.width / 2
  const centerY = top + rect.height / 2
  const distance = Math.hypot(pointer.x - centerX, pointer.y - centerY)

  return {
    id: droppable.id,
    value: 1 / Math.max(distance, 1),
    type: CollisionType.PointerIntersection,
    priority: CollisionPriority.High,
  }
}
