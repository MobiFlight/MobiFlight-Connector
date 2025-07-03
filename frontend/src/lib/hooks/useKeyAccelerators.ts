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

    document.addEventListener("keydown", handleKeyDown)
    return () => {
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
      payload: { action: "help.docs" },
    },
    description: "Open Documentation",
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

export function ConvertKeyAcceleratorToString(
  accelerator: KeyAccelerator,
): string {
  const key = [] as string[]
  if (accelerator.ctrlKey) key.push("Control")
  if (accelerator.shiftKey) key.push("Shift")
  if (accelerator.altKey) key.push("Alt")
  if (accelerator.metaKey) key.push("Meta")

  key.push(accelerator.key)

  return key.join("+")
}
