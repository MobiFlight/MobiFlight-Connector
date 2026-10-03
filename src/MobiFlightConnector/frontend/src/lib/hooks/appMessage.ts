import { AppMessage, AppMessageKey } from "@/types/messages"
import { useEffect } from "react"
import messageClient from "@/lib/messages/messageClient"

// create a useAppMessage hook that listens for messages
// the paramaters are the AppMessageKey and the onReceiveMessage callback
// the callback is called when a message is received
export const useAppMessage = (
  key: AppMessageKey,
  onReceiveMessage: (message: AppMessage) => void,
) => {
  useEffect(() => {
    return messageClient.subscribe(key, onReceiveMessage)
  }, [key, onReceiveMessage])
}
