import { useEffect } from "react"
import { publishOnMessageExchange } from "./appMessage"
import { KeyAccelerator } from "@/types/accelerator"

export const useKeyAccelerators = (accelerators: KeyAccelerator[], enabled: boolean = true) => {
  const { publish } = publishOnMessageExchange()

  useEffect(() => {
    if (!enabled) return

    const handleKeyDown = (event: KeyboardEvent) => {
      // Find matching accelerator
      const accelerator = accelerators.find(acc => 
        acc.key.toLowerCase() === event.key.toLowerCase() &&
        !!acc.ctrlKey === event.ctrlKey &&
        !!acc.altKey === event.altKey &&
        !!acc.shiftKey === event.shiftKey &&
        !!acc.metaKey === event.metaKey
      )

      if (accelerator) {
        event.preventDefault()
        event.stopPropagation()
        
        publish({
          key: "CommandMainMenu",
          payload: { action: accelerator.action }
        })
      }
    }

    document.addEventListener('keydown', handleKeyDown)
    
    return () => {
      document.removeEventListener('keydown', handleKeyDown)
    }
  }, [accelerators, enabled, publish])

  // Return accelerators for documentation/UI purposes
  return { accelerators: accelerators }
}


export const GlobalKeyAccelerators: KeyAccelerator[] = [
  {
    key: "F1",
    action: "help.discord",
    description: "Open Discord",
  },
  {
    key: "S",
    ctrlKey: true,
    action: "file.save",
    description: "Save current project",
  },
  {
    key: "S",
    ctrlKey: true,
    shiftKey: true,
    action: "file.saveas",
    description: "Save current project as",
  },
  {
    key: "O",
    ctrlKey: true,
    action: "file.open",
    description: "Open project",
  },
  {
    key: "N",
    ctrlKey: true,
    action: "file.new",
    description: "Create new project",
  },
  {
    key: "Q",
    ctrlKey: true,
    action: "file.exit",
    description: "Exit application",
  },
]