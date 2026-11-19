import { PointerActivationConstraints, PointerSensor } from "@dnd-kit/dom"

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

/**
 * Pointer sensor (mouse, touch and pen) with the project's activation rules.
 * Non-primary buttons are already ignored by the PointerSensor itself.
 */
export const CustomPointerSensor = PointerSensor.configure({
  preventActivation(event, source) {
    // Modifier keys are reserved for multi-select clicks
    if (event.ctrlKey || event.shiftKey || event.metaKey) {
      return true
    }

    // Allow selecting text while renaming or editing
    if (preventDragWhileEditing(event.target as Element)) {
      return true
    }

    // Keep the library defaults (buttons, links, inputs, ...)
    return PointerSensor.defaults.preventActivation?.(event, source) ?? false
  },
  activationConstraints(event, source) {
    // Keep the library defaults for touch (press and hold)
    if (event.pointerType === "touch") {
      const defaults = PointerSensor.defaults.activationConstraints
      return typeof defaults === "function" ? defaults(event, source) : defaults
    }

    // Must move 10 pixels before a drag starts so normal clicks still work
    return [new PointerActivationConstraints.Distance({ value: 10 })]
  },
})
