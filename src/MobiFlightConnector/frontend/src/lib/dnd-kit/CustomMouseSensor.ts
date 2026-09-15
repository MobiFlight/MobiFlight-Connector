import { PointerSensor, PointerActivationConstraints } from "@dnd-kit/dom"
const preventDragWhileEditing = (element: Element | null): boolean => {
  let currentElement = element

  // Traverse up the DOM tree
  // to check if any parent element is an input, textarea, or contenteditable
  while (currentElement) {
    if (
      currentElement.tagName === "INPUT" ||
      currentElement.tagName === "TEXTAREA" ||
      currentElement.getAttribute("contenteditable") === "true"
    ) {
      return true
    }
    currentElement = currentElement.parentElement
  }
  return false
}
const CustomMouseSensor = PointerSensor.configure({
  preventActivation(event) {
    // Only allow primary mouse button (left click)
    if (event.button !== 0) return true

    // Block modifier keys
    if (event.ctrlKey || event.shiftKey || event.metaKey) return true

    if (preventDragWhileEditing(event.target as Element | null))
      // Prevent drag while editing
      return true

    return false
  },
  activationConstraints(event) {
    if (event.pointerType === "mouse") {
      return [new PointerActivationConstraints.Distance({ value: 5 })]
    }
    return []
  },
})

export { CustomMouseSensor }
