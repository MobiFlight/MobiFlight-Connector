import { CommandMessage } from "@/types/commands"

const useMessageExchange = () => ({
  publish: (message: CommandMessage) => {
    console.log(`Publishing FrontendMessage -> ${message.key} : ${message.payload ? JSON.stringify(message.payload) : "no payload"}`)
    window.chrome?.webview?.postMessage(message)
  },
})

export default useMessageExchange