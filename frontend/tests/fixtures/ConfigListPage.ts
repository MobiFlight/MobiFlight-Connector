import {
  AppMessage,
  Project,
  ConfigValuePartialUpdate,
  IConfigItem,
} from "@/types"
import { MobiFlightPage } from "./MobiFlightPage"
import testdata from "../data/configlist.testdata.json" with { type: "json" }
import testProject from "../data/project.testdata.json" with { type: "json" }
import { CommandUpdateConfigItem } from "@/types/commands"
import { ConfigItemStatusType, IDictionary } from "@/types/config"
import { Locator } from "@playwright/test"
import { ConfigValueRawAndFinalUpdate, ExecutionState } from "@/types/messages"

export class ConfigListPage {
  constructor(public readonly mobiFlightPage: MobiFlightPage) {}

  async gotoPage() {
    await this.mobiFlightPage.page.goto("http://localhost:5173/config", {
      waitUntil: "networkidle",
    })
  }

  async initWithEmptyData() {
    const message: AppMessage = {
      key: "Project",
      payload: {
        Name: "Test Project",
        FilePath: "SomeFilePath.mfproj",
        ConfigFiles: [],
      } as Project,
    }
    await this.mobiFlightPage.publishMessage(message)
  }

  async initWithTestData() {
    const message: AppMessage = {
      key: "Project",
      payload: testProject
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

  async updateConfigItemRawAndFinalValue (itemIndex: number, RawValue: string, FinalValue: string) {
    const item = testdata[itemIndex]
    const message: AppMessage = {
      key: "ConfigValueRawAndFinalUpdate",
      payload: {
        ConfigItems: [
          {
            ...item,
            RawValue: RawValue,
            Value: FinalValue,
          }
        ],
      } as ConfigValueRawAndFinalUpdate,
    }
    await this.mobiFlightPage.publishMessage(message)
  }

  async removeConfigItemStatus(itemIndex: number, keyToRemove: string) {
    const item = testdata[itemIndex];
    const updatedStatus: IDictionary<string, ConfigItemStatusType> = { ...item.Status };
  
    // Remove the specified key
    delete updatedStatus[keyToRemove];
  
    const message: AppMessage = {
      key: "ConfigValuePartialUpdate",
      payload: {
        ConfigItems: [
          {
            ...item,
            Status: updatedStatus,
          },
        ],
      } as ConfigValuePartialUpdate,
    };
  
    await this.mobiFlightPage.publishMessage(message);
  }

  getStatusIconInRow(status: ConfigItemStatusType, row: number) : Locator {
    return this.mobiFlightPage.page.getByRole("row").nth(row).getByRole("status",{name: status})
  }

  async updateExecutionState (executionState: ExecutionState) {
    const message: AppMessage = {
      key: "ExecutionState",
      payload: executionState,
    }
    await this.mobiFlightPage.publishMessage(message)
  }
}
