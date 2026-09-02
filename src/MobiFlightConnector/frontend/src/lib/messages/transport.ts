import { CommandMessage } from "@/types/commands"
import { AppMessage } from "@/types/messages"

// Contract every transport implements. Components only ever talk to messageClient.
export interface MessageTransport {
  send: (message: CommandMessage) => void
  onMessage: (callback: (message: AppMessage) => void) => () => void
  // Fires once for postMessage, and on every (re)connect for the WebSocket transport.
  onOpen: (callback: () => void) => () => void
}
