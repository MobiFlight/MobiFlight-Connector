import { IConfigItem, ILogMessage } from '@/types';
import { ConfigLoadedEvent, Message, AppMessageKey, AppMessage } from '@/types/messages';
import type { Page } from '@playwright/test';

export class MFPage {
    constructor(public readonly page: Page) {
        this.page.addInitScript(() => {
            if(!window.chrome?.webview?.postMessage) {
                console.log("Setting up window.chrome.webview.postMessage for playwright testing.")
                window.chrome = {
                    webview: {
                        postMessage(message:string) {
                            window.postMessage(message, "*")
                        },
                        addEventListener(message :string, callback: (event: any) => void) {
                            window.addEventListener(message, callback)
                        },
                        removeEventListener(message:string, callback: (event: any) => void) {
                            window.removeEventListener(message, callback)
                        }
                    }
                }
            }
        })
    }

    async publishMessage(message: Message) {
        var stringifiedObject = JSON.stringify(JSON.stringify(message))
        await this.page.addScriptTag({ content: `console.log(${stringifiedObject});window.postMessage(${stringifiedObject}, "*")` })
    }

    async finishInitialLoading() {
        await this.publishMessage({ key: "StatusBarUpdate", payload: { Value: 100, Text: "Finished." } })
    }

    async loadConfig(config : ConfigLoadedEvent ) {
        var message: AppMessage = { key: "ConfigFile", payload: config }
        await this.publishMessage(message)
    }

    async loadDefaultConfig() {
        var configItems: IConfigItem[] = [
            {
                GUID: "123",
                Active: true,
                Description: "My first item",
                Device: "Device 1",	
                Component: "Component 1",
                Type: "Type 1",
                Tags: [],
                Status: [],
                RawValue: "10",
                ModifiedValue: "11"

            }
        ]
        await this.loadConfig({ FileName: "default", ConfigItems: configItems })
    }

    async addLogMessage(log: ILogMessage) {
        var message: AppMessage = { key: "LogMessage", payload: log }
        await this.publishMessage(message)
    }
}