import { FrontendMessageType } from "@/types/messages"

export const usePublishOnMessageExchange = () => ({
  publish: (message: FrontendMessageType) => {
    console.log(`Publishing FrontendMessage -> ${message.key}`)
    window.chrome?.webview?.postMessage(message)
  },
})