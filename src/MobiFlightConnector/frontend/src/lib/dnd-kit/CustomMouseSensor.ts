import { MouseSensor, MouseSensorOptions } from "@dnd-kit/core"

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

class CustomMouseSensor extends MouseSensor {
  static activators = [
    {
      eventName: "onMouseDown" as const,
      handler: (
        { nativeEvent: event }: { nativeEvent: MouseEvent },
        { onActivation }: MouseSensorOptions,
      ) => {
        // Only activate on primary mouse button (left click)
        if (event.button !== 0) {
          return false
        }

        // Don't start drag if modifier keys are pressed
        if (event.ctrlKey || event.shiftKey || event.metaKey) {
          return false
        }

        // Prevent drag if clicking inside an editable element
        if (preventDragWhileEditing(event.target as Element)) {
          return false
        }

        onActivation?.({ event })

        return true
      },
    },
  ]
}

export { CustomMouseSensor }
