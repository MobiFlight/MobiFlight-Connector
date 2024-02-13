import { ExecutionState } from "@/stores/executionStateStore"
import { IConfigItem } from "@/types"

export interface ExecutionUpdateMessage {
    key: "ExecutionUpdate"
    payload: ExecutionState
}

export interface EditConfigMessage {
    key: "config.edit"
    payload: IConfigItem
}

export type MessageExchangeMessage = ExecutionUpdateMessage | EditConfigMessage	

export const useMessageExchange = () => ({
    publish: (message: MessageExchangeMessage) => {
        window.chrome?.webview?.postMessage(message as any)
    }
})