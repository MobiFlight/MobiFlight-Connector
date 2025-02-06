import { CommandMessage } from "@/types/commands"
import { AppMessage, AppMessageKey } from "@/types/messages"
import { useEffect } from "react"

export const publishOnMessageExchange = () => ({
  publish: (message: CommandMessage) => {
    console.log(`Publishing FrontendMessage -> ${message.key}`)
    window.chrome?.webview?.postMessage(message)
  },
})

// create a useAppMessage hook that listens for messages
// the paramaters are the AppMessageKey and the onReceiveMessage callback
// the callback is called when a message is received
export const useAppMessage = (
  key: AppMessageKey,
  onReceiveMessage: (message: AppMessage) => void
) => {
  useEffect(() => {
    const onReveiveMessageHandler = (event: Event) => {
      try {
        const appMessage = (event as MessageEvent).data as AppMessage
        if (appMessage.key === key) {
        onReceiveMessage(appMessage)
        }
      } catch (error) {
        console.error("Error parsing message", error)
      }
    }

    // add an event listener for the message key
    window.chrome?.webview?.addEventListener("message", onReveiveMessageHandler)
    return () => {
      // remove the event listener when the component is unmounted
      window.chrome?.webview?.removeEventListener(
        "message",
        onReveiveMessageHandler
      )
    }
  }, [key, onReceiveMessage])
}