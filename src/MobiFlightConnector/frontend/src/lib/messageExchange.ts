import { CommandMessage } from "@/types/commands"
import messageClient from "@/lib/messages/messageClient"

const messageExchange = {
  publish: (message: CommandMessage) => messageClient.publish(message),
}

export default messageExchange
