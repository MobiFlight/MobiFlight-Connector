import { AppMessage, IConfigItem } from "@/types"
import { MobiFlightPage } from "./MobiFlightPage"
import testdata from "../data/configlist.testdata.json" with { type: "json" }
import testProject from "../data/project.testdata.json" with { type: "json" }
import joystickDefinition from "../data/joystick.definition.json" with { type: "json" }
import midiControllerDefinition from "../data/midicontroller.definition.json" with { type: "json" }
import {
  ConfigValueFullUpdate,
  ConfigValuePartialUpdate,
  OverlayState,
  ProjectStatus,
} from "@/types/messages"
import { CommandUpdateConfigItem } from "@/types/commands"
import {
  ConfigItemStatusType,
  ConfigItemType,
  IDictionary,
} from "@/types/config"
import { Locator } from "@playwright/test"
import { ConfigValueRawAndFinalUpdate, ExecutionState } from "@/types/messages"

export class ConfigListPage {
  constructor(public readonly mobiFlightPage: MobiFlightPage) {}

  getPageUrl() {
    return "http://localhost:5173/config"
  }

  async gotoPage() {
    await this.mobiFlightPage.page.goto(this.getPageUrl(), {
      waitUntil: "networkidle",
    })
  }

  async initControllerDefinitions() {
    const message: AppMessage = {
      key: "JoystickDefinitions",
      payload: {
        Definitions: [joystickDefinition],
      },
    }
    await this.mobiFlightPage.publishMessage(message)

    const midiMessage: AppMessage = {
      key: "MidiControllerDefinitions",
      payload: {
        Definitions: [midiControllerDefinition],
      },
    }
    await this.mobiFlightPage.publishMessage(midiMessage)
  }

  async setupConfigItemEditConfirmationResponse() {
    await this.mobiFlightPage.subscribeToCommand(
      "CommandUpdateConfigItem",
      async (message) => {
        const item = (message as CommandUpdateConfigItem).payload
          .item as IConfigItem
        const response = {
          key: "ConfigValuePartialUpdate",
          payload: {
            ConfigItems: [item],
          } as ConfigValuePartialUpdate,
        }

        ;(window as Window).postMessage(response, "*")
      },
    )
  }

  async configValueFullUpdate(ConfigItems: IConfigItem[], configIndex: number = 0) {
    const message: AppMessage = {
      key: "ConfigValueFullUpdate",
      payload: {
        ConfigIndex: configIndex,
        ConfigItems: ConfigItems,
      } as ConfigValueFullUpdate,
    }
    await this.mobiFlightPage.publishMessage(message)
  }

  async addNewConfigItem(itemType: ConfigItemType, configIndex: number = 0) {
    const configItems = testProject.ConfigFiles[configIndex].ConfigItems
    const newItem = configItems.find((i) => i.Type === itemType)
    if (!newItem)
      throw new Error(`No test data found for item type ${itemType}`)

    newItem.GUID = crypto.randomUUID()
    newItem.Name = `New ${itemType} (created from test)`

    const newTestData = [...configItems, newItem]

    const message: AppMessage = {
      key: "ConfigValueFullUpdate",
      payload: {
        ConfigIndex: configIndex,
        ConfigItems: newTestData,
      } as ConfigValueFullUpdate,
    }
    await this.mobiFlightPage.publishMessage(message)
  }

  getConfigItemByIndex(itemIndex: number): IConfigItem {
    return testdata[itemIndex]
  }

  async updateConfigItemStatus(
    itemIndex: number,
    Status: IDictionary<string, ConfigItemStatusType>,
  ) {
    const item = testdata[itemIndex]
    const message: AppMessage = {
      key: "ConfigValuePartialUpdate",
      payload: {
        ConfigItems: [
          {
            ...item,
            Status: {
              ...Status,
              [Status.Key]: Status.Value,
            },
          },
        ],
      } as ConfigValuePartialUpdate,
    }
    await this.mobiFlightPage.publishMessage(message)
  }

  async updateConfigItemRawAndFinalValue(
    itemIndex: number,
    RawValue: string,
    FinalValue: string,
  ) {
    const item = testdata[itemIndex]
    const message: AppMessage = {
      key: "ConfigValueRawAndFinalUpdate",
      payload: {
        ConfigItems: [
          {
            ...item,
            RawValue: RawValue,
            Value: FinalValue,
          },
        ],
      } as ConfigValueRawAndFinalUpdate,
    }
    await this.mobiFlightPage.publishMessage(message)
  }

  async removeConfigItemStatus(itemIndex: number, keyToRemove: string) {
    const item = testdata[itemIndex]
    const updatedStatus: IDictionary<string, ConfigItemStatusType> = {
      ...item.Status,
    }

    // Remove the specified key
    delete updatedStatus[keyToRemove]

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
    }

    await this.mobiFlightPage.publishMessage(message)
  }

  getStatusIconInRow(status: ConfigItemStatusType, row: number): Locator {
    return this.mobiFlightPage.page
      .getByRole("row")
      .nth(row)
      .getByRole("status", { name: status })
  }

  async updateExecutionState(executionState: ExecutionState) {
    const message: AppMessage = {
      key: "ExecutionState",
      payload: executionState,
    }
    await this.mobiFlightPage.publishMessage(message)
  }

  async updateProjectState(projectStatus: ProjectStatus) {
    const message: AppMessage = {
      key: "ProjectStatus",
      payload: projectStatus,
    }
    await this.mobiFlightPage.publishMessage(message)
  }

  async setOverlayState(state: OverlayState) {
    const message: AppMessage = {
      key: "OverlayState",
      payload: state,
    }
    await this.mobiFlightPage.publishMessage(message)
  }

  async filterByText(text: string) {
    const filterInput = this.mobiFlightPage.page.getByPlaceholder("Filter items...")
    await filterInput.fill(text)
  }
}
