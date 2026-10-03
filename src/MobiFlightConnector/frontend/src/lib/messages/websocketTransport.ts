import { CommandMessage } from "@/types/commands"
import { AppMessage } from "@/types/messages"
import { MessageTransport } from "./transport"

const INITIAL_BACKOFF_MS = 1_000
const MAX_BACKOFF_MS = 30_000

// Reconnects with capped exponential backoff, buffers outbound commands while disconnected.
export const createWebSocketTransport = (url: string): MessageTransport => {
  const messageListeners = new Set<(message: AppMessage) => void>()
  const openListeners = new Set<() => void>()
  const outboundQueue: CommandMessage[] = []

  let socket: WebSocket | null = null
  let reconnectTimer: ReturnType<typeof setTimeout> | undefined
  let backoffMs = INITIAL_BACKOFF_MS

  const flushQueue = () => {
    while (outboundQueue.length > 0 && socket?.readyState === WebSocket.OPEN) {
      socket.send(JSON.stringify(outboundQueue.shift()))
    }
  }

  const scheduleReconnect = () => {
    if (reconnectTimer) return
    reconnectTimer = setTimeout(() => {
      reconnectTimer = undefined
      backoffMs = Math.min(backoffMs * 2, MAX_BACKOFF_MS)
      connect()
    }, backoffMs)
  }

  const connect = () => {
    socket = new WebSocket(url)

    socket.onopen = () => {
      backoffMs = INITIAL_BACKOFF_MS
      flushQueue()
      openListeners.forEach((listener) => listener())
    }

    socket.onmessage = (event) => {
      try {
        const message = JSON.parse(event.data as string) as AppMessage
        messageListeners.forEach((listener) => listener(message))
      } catch (error) {
        console.error("Error parsing message", error)
      }
    }

    socket.onclose = scheduleReconnect
    socket.onerror = (event) => {
      console.error("WebSocket transport error", event)
    }
  }

  connect()

  return {
    send: (message) => {
      if (socket?.readyState === WebSocket.OPEN) {
        socket.send(JSON.stringify(message))
      } else {
        outboundQueue.push(message)
      }
    },
    onMessage: (callback) => {
      messageListeners.add(callback)
      return () => messageListeners.delete(callback)
    },
    onOpen: (callback) => {
      openListeners.add(callback)
      return () => openListeners.delete(callback)
    },
  }
}
