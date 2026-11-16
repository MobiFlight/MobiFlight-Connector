import ComboBox from "@/components/ComboBox"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import SectionHeadline from "@/components/wizard/common/SectionHeadline"
import { useAppMessage } from "@/lib/hooks/appMessage"
import messageExchange from "@/lib/messageExchange"

import { useControllerStore } from "@/stores/controllerStore"
import { CommandScanForInput } from "@/types/commands"
import { IConfigItem } from "@/types/config"
import {
  DeviceReference,
  Controller,
  InputDeviceType,
} from "@/types/controller"
import { HistoryAction } from "@/types/history"
import { ScanForInputResult } from "@/types/messages"
import { InputDeviceTypes } from "@/types/typesOptions"
import { IconLoader2, IconTrash } from "@tabler/icons-react"
import { useState } from "react"
import { useTranslation } from "react-i18next"

export type ConfigTriggerProps = {
  configItem: IConfigItem
  setConfigItem: (item: IConfigItem, actionLabel?: HistoryAction) => void
}

export const ControllerComboBox = ({
  completeControllers,
  selectedController,
  onChange,
  variant = "default",
}: {
  completeControllers: Partial<Controller>[]
  selectedController: Partial<Controller> | undefined
  onChange: (controller: Partial<Controller> | undefined) => void
  variant?: "full-width" | "default"
}) => {
  const { t } = useTranslation("wizard")
  return (
    <ComboBox
      widthClass={variant === "full-width" ? "w-full" : "w-56"}
      align="start"
      getLabel={(controller) => (controller as Controller).Name}
      getValue={(controller) =>
        (controller as Controller).Serial ?? (controller as Controller).Name
      }
      isSelected={(controller, selected) =>
        (controller as Controller).Serial === selected?.Serial ||
        (!(controller as Controller).Serial &&
          (controller as Controller).Name === selected?.Name)
      }
      items={completeControllers}
      selected={selectedController}
      placeholder={t("ConfigWizard.Input.ConfigTrigger.SelectController")}
      searchPlaceholder={t(
        "ConfigWizard.Input.ConfigTrigger.SearchController",
      )}
      noOptionsPlaceholder={t(
        "ConfigWizard.Input.ConfigTrigger.NoControllerFound",
      )}
      setSelected={onChange}
    />
  )
}

export const DeviceComboBox = ({
  devices,
  selectedDevice,
  selectedController,
  onChange,
  variant = "default",
}: {
  devices: DeviceReference[]
  selectedDevice: DeviceReference | undefined
  selectedController: Partial<Controller> | undefined
  onChange: (device: DeviceReference | undefined) => void
  variant?: "full-width" | "default"
}) => {
  const { t } = useTranslation("wizard")
  return (
    <ComboBox
      widthClass={variant === "full-width" ? "w-full" : "w-56"}
      align="start"
      getLabel={(device) =>
        (device as DeviceReference)?.Label ?? (device as DeviceReference)?.Name
      }
      getValue={(device) => (device as DeviceReference)?.Name}
      isSelected={(device, selected) =>
        (device as DeviceReference).Name === selected?.Name
      }
      items={devices}
      selected={selectedDevice}
      placeholder={t("ConfigWizard.Input.ConfigTrigger.SelectDevice")}
      searchPlaceholder={t(
        "ConfigWizard.Input.ConfigTrigger.SearchDevice",
      )}
      noOptionsPlaceholder={t(
        "ConfigWizard.Input.ConfigTrigger.NoDeviceFound",
      )}
      disabled={!selectedController}
      setSelected={onChange}
    />
  )
}

const ConfigTrigger = ({ configItem, setConfigItem }: ConfigTriggerProps) => {
  const { t } = useTranslation("wizard")
  const { controllers } = useControllerStore()

  const [selectedController, setSelectedController] = useState<
    Partial<Controller> | undefined
  >(configItem.Controller)

  const connectedController = selectedController
    ? controllers.find(
        (controller) =>
          controller.Serial === selectedController.Serial ||
          (!selectedController.Serial &&
            controller.Name === selectedController.Name),
      )
    : undefined

  // Add selected controller to the list
  // because it won't be included in the list of controllers
  // if it's not currently connected
  const completeControllers: Partial<Controller>[] =
    selectedController && !connectedController
      ? [...controllers, selectedController]
      : controllers

  const selectedMatchesConfigController =
    selectedController != null &&
    configItem.Controller != null &&
    (selectedController.Serial === configItem.Controller.Serial ||
      (!selectedController.Serial &&
        selectedController.Name === configItem.Controller.Name))

  const inputDeviceFilter = (device: DeviceReference) => {
    return InputDeviceTypes.includes(device.Type as InputDeviceType)
  }

  const devices: DeviceReference[] = [
    ...(connectedController?.Devices?.filter(inputDeviceFilter) ?? []),
  ]

  const configuredDevice: DeviceReference | undefined =
    configItem.Device != null
      ? {
          Name: configItem.Device.Name,
          Type: configItem.Device.Type,
          Label: configItem.Device.Name,
        }
      : undefined

  const [selectedDevice, setSelectedDevice] = useState<
    DeviceReference | undefined
  >(configuredDevice)

  if (
    selectedMatchesConfigController &&
    configuredDevice != null &&
    !devices.some((device) => device.Name === configuredDevice.Name)
  ) {
    devices.push(configuredDevice)
  }

  const updateConfigItem = (
    controller: Partial<Controller> | undefined,
    device: DeviceReference | undefined,
  ) => {
    setConfigItem(
      {
        ...configItem,
        Controller: controller
          ? {
              ...controller,
              // unset Devices
              // before sending it back to backend.
              Devices: undefined,
            }
          : undefined,
        Device: device
          ? {
              Name: device.Name,
              Type: device.Type,
            }
          : null,
      },
      "ConfigWizard.History.TriggerChanged",
    )
  }

  const [scanning, setScanning] = useState(false)

  useAppMessage("ScanForInputResult", (message) => {
    const { Controller, Device } = message.payload as ScanForInputResult
    setSelectedController(Controller)
    setSelectedDevice({ ...Device, Label: Device.Name })
    updateConfigItem(Controller, Device)
    setScanning(false)
  })

  const scanForInput = () => {
    const { publish } = messageExchange
    publish({
      key: "CommandScanForInput",
      payload: {
        isScanning: !scanning,
      },
    } as CommandScanForInput)
    setScanning(!scanning)
  }

  return (
    <Card
      data-testid="trigger-panel"
      className="border-primary transition-shadow hover:shadow-md"
    >
      <CardContent className="flex flex-col gap-4 pt-4">
        <SectionHeadline
          title={t("ConfigWizard.Input.ConfigTrigger.Title")}
          description={t("ConfigWizard.Input.ConfigTrigger.Description")}
        />
        <div className="flex flex-row items-center gap-2">
          <Button onClick={scanForInput} className="px-5" size="sm">
            {scanning ? (
              <div className="flex flex-row items-center gap-2 text-sm">
                <IconLoader2 className="animate-spin" />
                {t("ConfigWizard.Input.ConfigTrigger.UseAnyInput")}
              </div>
            ) : (
              t("ConfigWizard.Input.ConfigTrigger.ScanForInput")
            )}
          </Button>
          <div className="flex flex-col gap-2">
            <ControllerComboBox
              completeControllers={completeControllers}
              selectedController={selectedController}
              onChange={(controller) => {
                setSelectedController(controller)
                setSelectedDevice(undefined)
                updateConfigItem(controller, undefined)
              }}
            />
          </div>

          <div className="flex flex-col gap-2">
            <DeviceComboBox
              devices={devices}
              selectedDevice={selectedDevice}
              selectedController={selectedController}
              onChange={(device) => {
                setSelectedDevice(device)
                updateConfigItem(selectedController, device)
              }}
            />
          </div>
          <Button
            variant="ghost"
            size={"sm"}
            className="gap-1"
            onClick={() => {
              setSelectedController(undefined)
              setSelectedDevice(undefined)
              updateConfigItem(undefined, undefined)
            }}
          >
            <IconTrash className="mr-2" />
            <span className="sr-only text-sm lg:not-sr-only">
              {t("ConfigWizard.Input.ConfigTrigger.ClearInput")}
            </span>
          </Button>
        </div>
      </CardContent>
    </Card>
  )
}
export default ConfigTrigger
