import ComboBox from "@/components/ComboBox"
import { Card, CardContent } from "@/components/ui/card"
import OutputPanel from "@/components/wizard/components/OutputDevices/OutputPanel"
import { useControllerStore } from "@/stores/controllerStore"
import { OutputConfig, OutputConfigItem } from "@/types/config"
import { Controller } from "@/types/controller"
import { useState } from "react"
import { useTranslation } from "react-i18next"

export interface OutputDeviceEditorProps {
  configItem: OutputConfigItem
  onConfigChange: (config: OutputConfigItem) => void
}

const OutputDeviceEditor = ({
  configItem,
  // onConfigChange,
}: OutputDeviceEditorProps) => {
  const deviceConfig = configItem?.Device as OutputConfig
  const controller = configItem?.Controller
  const { t } = useTranslation()
  const { controllers } = useControllerStore()

  const [selectedController, setSelectedController] = useState<
    Partial<Controller> | undefined
  >(configItem.Controller)

  // const connectedController = selectedController
  //   ? controllers.find(
  //       (controller) =>
  //         controller.Serial === selectedController.Serial ||
  //         (!selectedController.Serial &&
  //           controller.Name === selectedController.Name),
  //     )
  //   : undefined

  const hasControllerOption = selectedController
    ? controllers.some(
        (controller) =>
          controller.Serial === selectedController.Serial ||
          (!selectedController.Serial &&
            controller.Name === selectedController.Name),
      )
    : false

  const completeControllers: Partial<Controller>[] =
    selectedController && !hasControllerOption
      ? [...controllers, selectedController]
      : controllers

  return (
    <div className="flex flex-col gap-4" data-testid="source-editor">
      <div className="flex flex-col gap-1">
        <div className="text-lg font-semibold">
          {t("Dialog.ConfigWizard.Output.Title")}
        </div>
        <div className="text-muted-foreground text-sm">
          {t("Dialog.ConfigWizard.Output.Description")}
        </div>
      </div>
      <Card>
        <CardContent className="pt-4">
          <div className="flex flex-col gap-1">
            <div className="text-lg font-semibold">Step 1 - Display type</div>
            <div className="text-muted-foreground text-sm">
              Select the type of device you want to configure.
            </div>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <ComboBox
              widthClass="w-56"
              align="start"
              getLabel={(controller) => (controller as Controller).Name}
              getValue={(controller) =>
              (controller as Controller).Serial ??
                (controller as Controller).Name
              }
              isSelected={(controller, selected) =>
                (controller as Controller).Serial === selected?.Serial ||
                (!(controller as Controller).Serial &&
                  (controller as Controller).Name === selected?.Name)
              }
              items={completeControllers}
              selected={selectedController}
              placeholder={t(
                "Dialog.InputConfigWizard.ConfigTrigger.SelectController",
              )}
              searchPlaceholder={t(
                "Dialog.InputConfigWizard.ConfigTrigger.SearchController",
              )}
              noOptionsPlaceholder={t(
                "Dialog.InputConfigWizard.ConfigTrigger.NoControllerFound",
              )}
              setSelected={(controller) => {
                setSelectedController(controller)
                // setSelectedDevice(undefined)
                // updateConfigItem(controller, undefined)
              }}
            />
          </div>
        </CardContent>
      </Card>
      {configItem?.Device?.Type == "Output" && (
        <OutputPanel
          variant="details"
          config={deviceConfig}
          controller={controller}
        />
      )}
    </div>
  )
}
export default OutputDeviceEditor
