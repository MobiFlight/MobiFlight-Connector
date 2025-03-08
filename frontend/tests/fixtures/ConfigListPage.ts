import {
  AppMessage,
  ConfigLoadedEvent,
  ConfigValuePartialUpdate,
  IConfigItem,
} from "@/types"
import { MobiFlightPage } from "./MobiFlightPage"
import testdata from "../data/configlist.testdata.json" with { type: "json" }
import { CommandUpdateConfigItem } from "@/types/commands"
import { ConfigItemStatusType, IDictionary } from "@/types/config"
import { Locator } from "@playwright/test"

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
          key: "ConfigValuePartialUpdate",
          payload: {
            ConfigItems: [item]
          } as ConfigValuePartialUpdate,
        };
        
        (window as Window).postMessage(response, "*")
      },
    )
  }

  async updateConfigItemStatus(itemIndex: number, Status: IDictionary<string, ConfigItemStatusType>) {
    const item = testdata[itemIndex]
    const message: AppMessage = {
      key: "ConfigValuePartialUpdate",
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
      } as ConfigValuePartialUpdate,
    }
    await this.mobiFlightPage.publishMessage(message)
  }

  getStatusIconInRow(iconName: string, row: number) : Locator {
    const iconIndex : Record<string, number> = {
      "Precondition": 0,
      "Source": 1,
      "Device": 2,
      "Modifier": 3,
      "Test": 4,
      "ConfigRef": 5,
    }
    return this.mobiFlightPage.page.getByRole("row").nth(row).getByRole("status").nth(iconIndex[iconName]!)
  }
}
