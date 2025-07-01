import { CommandMessage } from "@/types/commands"

export interface KeyAccelerator {
  key: string
  ctrlKey?: boolean
  altKey?: boolean
  shiftKey?: boolean
  metaKey?: boolean
  message: CommandMessage
  description?: string
}
