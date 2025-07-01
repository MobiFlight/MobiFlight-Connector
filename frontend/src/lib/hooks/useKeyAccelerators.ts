import { useEffect } from "react"
import { publishOnMessageExchange } from "./appMessage"
import { KeyAccelerator } from "@/types/accelerator"

export const useKeyAccelerators = (
  accelerators: KeyAccelerator[],
  enabled: boolean = true,
) => {
  const { publish } = publishOnMessageExchange()

  useEffect(() => {
    if (!enabled) return

    const handleKeyDown = (event: KeyboardEvent) => {
        event.stopPropagation()
        event.preventDefault()
    }

    const handleKeyUp = (event: KeyboardEvent) => {
      // Find matching accelerator
      const accelerator = accelerators.find(
        (acc) =>
          acc.key.toLowerCase() === event.key.toLowerCase() &&
          !!acc.ctrlKey === event.ctrlKey &&
          !!acc.altKey === event.altKey &&
          !!acc.shiftKey === event.shiftKey &&
          !!acc.metaKey === event.metaKey,
      )

      if (accelerator) {
        event.preventDefault()
        event.stopPropagation()

        publish(accelerator.message)
      }
    }

    document.addEventListener("keyup", handleKeyUp)
    document.addEventListener("keydown", handleKeyDown)

    return () => {
      document.removeEventListener("keyup", handleKeyUp)
      document.removeEventListener("keydown", handleKeyDown)
    }
  }, [accelerators, enabled, publish])

  // Return accelerators for documentation/UI purposes
  return { accelerators: accelerators }
}

export const GlobalKeyAccelerators: KeyAccelerator[] = [
  {
    key: "F1",
    message: {
      key: "CommandMainMenu",
      payload: { action: "help.discord" },
    },
    description: "Open Discord",
  },
  {
    key: "S",
    ctrlKey: true,
    message: {
      key: "CommandMainMenu",
      payload: { action: "file.save" },
    },
    description: "Save current project",
  },
  {
    key: "S",
    ctrlKey: true,
    shiftKey: true,
    message: {
      key: "CommandMainMenu",
      payload: { action: "file.saveas" },
    },
    description: "Save current project as",
  },
  {
    key: "O",
    ctrlKey: true,
    message: {
      key: "CommandMainMenu",
      payload: { action: "file.open" },
    },
    description: "Open project",
  },
  {
    key: "N",
    ctrlKey: true,
    message: {
      key: "CommandMainMenu",
      payload: { action: "file.new" },
    },
    description: "Create new project",
  },
  {
    key: "Q",
    ctrlKey: true,
    message: {
      key: "CommandMainMenu",
      payload: { action: "file.exit" },
    },
    description: "Exit application",
  },
]

export function ConvertKeyAcceleratorToString(accelerator: KeyAccelerator): string {
  let key = accelerator.key
  if (accelerator.ctrlKey) key = `Control+${key}`
  if (accelerator.altKey) key = `Alt+${key}`
  if (accelerator.shiftKey) key = `Shift+${key}`
  if (accelerator.metaKey) key = `Meta+${key}`

  return key
}
