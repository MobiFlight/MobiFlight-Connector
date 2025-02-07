import {
  AppMessage,
  ConfigLoadedEvent,
  ConfigValueUpdate,
  IConfigItem,
} from "@/types"
import { MobiFlightPage } from "./MobiFlightPage"
import testdata from "../data/configlist.testdata.json" with { type: "json" }
import { CommandUpdateConfigItem } from "@/types/commands"
import { ConfigItemStatusType, IDictionary } from "@/types/config"

export class ConfigListPage {
  constructor(public readonly mobiFlightPage: MobiFlightPage) {}

  async gotoPage() {
    await this.mobiFlightPage.page.goto("http://localhost:5173/config", {
      waitUntil: "networkidle",
    })
  }

  async initWithEmptyData() {
    const message: AppMessage = {
      key: "ConfigFile",
      payload: {
        FileName: "empty-config.json",
        ConfigItems: [],
      } as ConfigLoadedEvent,
    }
    await this.mobiFlightPage.publishMessage(message)
  }

  async initWithTestData() {
    const message: AppMessage = {
      key: "ConfigFile",
      payload: {
        FileName: "empty-config.json",
        ConfigItems: testdata,
      } as ConfigLoadedEvent,
    }
    await this.mobiFlightPage.publishMessage(message)
  }

  async setupConfigItemEditConfirmationResponse() {
    await this.mobiFlightPage.subscribeToCommand(
      "CommandUpdateConfigItem",
      async (message) => {
        const item = (message as CommandUpdateConfigItem).payload.item as IConfigItem
        const response = {
          key: "ConfigValueUpdate",
          payload: {
            ConfigItems: [item]
          } as ConfigValueUpdate,
        };
        
        (window as Window).postMessage(response, "*")
      },
    )
  }

  async updateConfigItemStatus(itemIndex: number, Status: IDictionary<string, ConfigItemStatusType>) {
    const item = testdata[itemIndex]
    const message: AppMessage = {
      key: "ConfigValueUpdate",
      payload: {
        ConfigItems: [
          {
            ...item,
            Status: {
              ...Status,
              [Status.Key]: Status.Value
            }
          }
        ],
      } as ConfigValueUpdate,
    }
    await this.mobiFlightPage.publishMessage(message)
  }
}
