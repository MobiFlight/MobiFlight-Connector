import { CommandMessage } from "@/types/commands"
import { AppMessage, AppMessageKey } from "@/types/messages"
import { MessageTransport } from "./transport"
import { createPostMessageTransport } from "./postMessageTransport"
import { createWebSocketTransport } from "./websocketTransport"
// Injected by the .NET host, or VITE_MF_WS_URL for a plain dev browser. Falls back to postMessage.
const resolveWsUrl = (): string | undefined =>
  window.__MOBIFLIGHT__?.wsUrl ?? import.meta.env.VITE_MF_WS_URL
const createTransport = (): MessageTransport => {
  const wsUrl = resolveWsUrl()
  return wsUrl ? createWebSocketTransport(wsUrl) : createPostMessageTransport()
}
// Re-sent on every reconnect after the first.
const READY_MESSAGE: CommandMessage = {
  key: "CommandFrontendState",
  payload: { route: "/start", state: "ready" },
}
// Single point of contact for whichever transport is active. Owns one connection and one dispatcher.
class MessageClient {
  private readonly transport: MessageTransport
  private readonly registeredAppMessageKeyHandlers = new Map<AppMessageKey, Set<(message: AppMessage) => void>>()
  private hasOpenedBefore = false
  constructor(transport: MessageTransport) {
    this.transport = transport
    this.transport.onMessage((message) => {
      this.registeredAppMessageKeyHandlers.get(message.key)?.forEach((listener) => listener(message))
    })
    this.transport.onOpen(() => {
      // First open: StartupProgress.tsx sends Ready itself. Later opens are reconnects - resync.
      if (!this.hasOpenedBefore) {
        this.hasOpenedBefore = true
        return
      }
      this.publish(READY_MESSAGE)
    })
  }
  publish(message: CommandMessage) {
    console.log(
      `Publishing FrontendMessage -> ${message.key} : ${message.payload ? JSON.stringify(message.payload) : "no payload"}`,
    )
    this.transport.send(message)
  }
  subscribe(key: AppMessageKey, handler: (message: AppMessage) => void) {
    let handlers = this.registeredAppMessageKeyHandlers.get(key)
    if (!handlers) {
      handlers = new Set()
      this.registeredAppMessageKeyHandlers.set(key, handlers)
    }
      handlers.add(handler)
    // provide clean up function for useEffect to remove the handler when the component unmounts or key changes
    return () => {
      handlers?.delete(handler)
    }
  }
}
// Lazy singleton - constructed on first use, not at import time
// to make sure that window is available which is required for the transport to work.
let instance: MessageClient | undefined
function getInstance(): MessageClient {
  if (!instance) {
    instance = new MessageClient(createTransport())
  }
  return instance
}
const messageClient = {
  publish: (message: CommandMessage) => getInstance().publish(message),
  subscribe: (key: AppMessageKey, handler: (message: AppMessage) => void) =>
    getInstance().subscribe(key, handler),
}
export default messageClient