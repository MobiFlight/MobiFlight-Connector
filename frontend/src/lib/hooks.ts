import { ExecutionState } from "@/stores/executionStateStore"
import { IConfigItem, IGlobalSettings } from "@/types"

export interface ExecutionUpdateMessage {
    key: "ExecutionUpdate"
    payload: ExecutionState
}

export interface EditConfigMessage {
    key: "config.edit"
    payload: IConfigItem
}

export interface GlobalSettingsUpdateMessage {
    key: "GlobalSettingsUpdate"
    payload: IGlobalSettings
}

export type MessageExchangeMessage = ExecutionUpdateMessage | EditConfigMessage	| GlobalSettingsUpdateMessage

export const useMessageExchange = () => ({
    publish: (message: MessageExchangeMessage) => {
        window.chrome?.webview?.postMessage(message as any)
    }
})