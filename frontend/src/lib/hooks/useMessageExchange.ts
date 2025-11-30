import { CommandMessage } from "@/types/commands"

const useMessageExchange = () => ({
  publish: (message: CommandMessage) => {
    console.log(`Publishing FrontendMessage -> ${message.key}`)
    window.chrome?.webview?.postMessage(message)
  },
})

export default useMessageExchange