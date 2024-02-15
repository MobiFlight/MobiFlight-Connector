import { FrontendMessageType } from "@/types/messages"

export const useMessageExchange = () => ({
    publish: (message: FrontendMessageType) => {
        window.chrome?.webview?.postMessage(message as any)
    }
})