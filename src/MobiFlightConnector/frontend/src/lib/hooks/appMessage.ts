import { AppMessage, AppMessageKey } from "@/types/messages"
import { useEffect } from "react"

// Use Map for efficient lookup of registered message key handlers
// Use Set to deduplicate handlers for the same callback function
const registeredAppMessageKeyHandlers = new Map<
  AppMessageKey,
  Set<(message: AppMessage) => void>
>()

// Flag to ensure we only subscribe to the webview message event once
let appMessageSubscriptionCreated = false

// Subscribe to all messages from the webview and dispatch to registered handlers
function subscribeToAppMessageEventsOnce() {
  if (appMessageSubscriptionCreated) return
  appMessageSubscriptionCreated = true

  const webview = window.chrome?.webview
  if (!webview) return

  webview?.addEventListener("message", (event: Event) => {
    const appMessage = (event as MessageEvent)?.data as AppMessage
    if (!appMessage) {
      console.error("Error parsing message")
      return
    }

    registeredAppMessageKeyHandlers.get(appMessage.key)?.forEach((handler) => {
      try {
        handler(appMessage)
      } catch (error) {
        console.error("Error processing message", error)
      }
    })
  })
}

// create a useAppMessage hook that listens for messages
// the paramaters are the AppMessageKey and the onReceiveMessage callback
// the callback is called when a message is received
export const useAppMessage = (
  key: AppMessageKey,
  onReceiveMessage: (message: AppMessage) => void,
) => {
  useEffect(() => {
    // ensure global listener is registered
    subscribeToAppMessageEventsOnce()

    // add additional handler for this key
    let appMessageKeyHandlers = registeredAppMessageKeyHandlers.get(key)
    if (!appMessageKeyHandlers) {
      appMessageKeyHandlers = new Set()
      registeredAppMessageKeyHandlers.set(key, appMessageKeyHandlers)
    }
    appMessageKeyHandlers.add(onReceiveMessage)

    return () => {
      // simply remove the handler from the set
      appMessageKeyHandlers?.delete(onReceiveMessage)
    }
  }, [key, onReceiveMessage])
}
