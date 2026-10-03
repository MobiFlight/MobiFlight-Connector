import { AppMessage } from "@/types/messages"
import { MessageTransport } from "./transport"

// The WebView2 postMessage bridge. Used when no wsUrl is available.
// Fallback for: Playwright, a plain dev browser, and 
// the auth WebView(see docs / architecture / frontend - backend - messaging.md).
export const createPostMessageTransport = (): MessageTransport => {
  const openListeners = new Set<() => void>()
  // No real "open" event here - fire once, async so subscribers registered right after don't miss it.
  queueMicrotask(() => openListeners.forEach((listener) => listener()))
  return {
    send: (message) => {
      window.chrome?.webview?.postMessage(message)
    },
    onMessage: (callback) => {
      const handler = (event: Event) => {
        try {
          callback((event as MessageEvent).data as AppMessage)
        } catch (error) {
          console.error("Error parsing message", error)
        }
      }
      window.chrome?.webview?.addEventListener("message", handler)
      return () => {
        window.chrome?.webview?.removeEventListener("message", handler)
      }
    },
    onOpen: (callback) => {
      openListeners.add(callback)
      return () => openListeners.delete(callback)
    },
  }
}