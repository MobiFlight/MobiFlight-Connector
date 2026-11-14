import ActionBindingPanel from "@/components/wizard/InputConfig/ActionBindingPanel"
import ConfigReferencePanel from "@/components/wizard/ConfigReferences/ConfigReferencePanel"
import ConfigTrigger from "@/components/wizard/common/ConfigTrigger"
import OutputTriggerCard from "@/components/wizard/OutputConfig/OutputTriggerCard"
import PreconditionsPanel from "@/components/wizard/Precondition/PreconditionsPanel"
import { IConfigItem } from "@/types"
import { RefObject, useState } from "react"
import { useNavigate, useSearchParams } from "react-router"
import {
  Action,
  AnalogTrigger,
  ButtonTrigger,
  ButtonHoldOptions,
  ButtonLongReleaseOptions,
  EncoderTrigger,
  ConfigItemType,
  OutputConfigItem,
} from "@/types/config"
import {
  Drawer,
  DrawerContent,
  DrawerFooter,
} from "@/components/ui/drawer"
import { IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"
import ActionEditor from "@/components/wizard/InputConfig/ActionEditor"
import ModifiersPanel from "@/components/wizard/Modifier/ModifiersPanel"
import { Button } from "@/components/ui/button"
import SourceEditor from "@/components/wizard/OutputConfig/SourceEditor"
import OutputActionBindingCard from "@/components/wizard/OutputConfig/OutputActionBindingCard"
import OutputDeviceEditor from "@/components/wizard/OutputConfig/OutputDeviceEditor"

export type ConfigWizardProps = {
  variant: ConfigItemType
  configItem: IConfigItem
  onConfigChange: (configItem: IConfigItem) => void
  drawerContainer?: RefObject<HTMLDivElement | null>
  liveData: {
    rawValue: string | null | undefined
    finalValue: string | null | undefined
  }
}

const determineInputDeviceType = (
  deviceType: string | undefined,
): "Button" | "Encoder" | "AnalogInput" | null => {
  switch (deviceType) {
    case "InputShiftRegister":
    case "InputMultiplexer":
    case "Button":
      return "Button"
    case "Encoder":
      return "Encoder"
    case "AnalogInput":
      return "AnalogInput"
    default:
      return null // Default to null if type is unknown
  }
}

const drawerVariant = {
  small:
    "data-[vaul-drawer-direction=right]:w-170 data-[vaul-drawer-direction=right]:sm:max-w-170",
  large:
    "data-[vaul-drawer-direction=right]:w-200 data-[vaul-drawer-direction=right]:sm:max-w-200",
}

const ConfigWizard = ({
  variant,
  configItem,
  onConfigChange,
  drawerContainer,
  liveData,
}: ConfigWizardProps) => {
  const { t } = useTranslation()
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()

  const currentDeviceType = determineInputDeviceType(configItem.Device?.Type)
  const [drawerOpen, setDrawerOpen] = useState(false)
  const [drawerSize, setDrawerSize] = useState<"small" | "large">("large")

  const [editAction, setEditAction] = useState<Action | null>(null)
  const [event, setEvent] = useState<{
    variant: "button" | "encoder" | "analog"
    event: string
  }>({ variant: "button", event: "" })

  const initialButton =
    configItem.button ?? (configItem as OutputConfigItem).ButtonInputConfig
  const initialButtonOptions = initialButton
    ? (initialButton as Partial<ButtonHoldOptions> &
        Partial<ButtonLongReleaseOptions>)
    : {
        HoldDelay: 350,
        LongReleaseDelay: 350,
        RepeatDelay: 0,
      }

  const [buttonOptions, setButtonOptions] = useState<
    Partial<ButtonHoldOptions> & Partial<ButtonLongReleaseOptions>
  >(initialButtonOptions)

  const [onActionChange, setOnActionChange] = useState<
    | ((
        action: Action | null,
        buttonOptions?: Partial<ButtonHoldOptions> &
          Partial<ButtonLongReleaseOptions>,
      ) => void)
    | null
  >(null)

  // an output config either controls a device or triggers an input action
  const outputItem = configItem as OutputConfigItem
  const hasInputAction =
    outputItem.ButtonInputConfig != null || outputItem.AnalogInputConfig != null

  const startInputAction = (kind: "button" | "analog") => {
    onConfigChange({
      ...outputItem,
      Controller: undefined,
      Device: null,
      ButtonInputConfig: kind === "button" ? {} : undefined,
      AnalogInputConfig: kind === "analog" ? {} : undefined,
    } as OutputConfigItem)
  }

  const removeInputAction = () => {
    onConfigChange({
      ...outputItem,
      ButtonInputConfig: undefined,
      AnalogInputConfig: undefined,
    } as OutputConfigItem)
  }

  const removeOutputDevice = () => {
    onConfigChange({
      ...outputItem,
      Controller: undefined,
      Device: null,
    } as OutputConfigItem)
  }

  // opens the action editor for an event of the input action of an output config
  const handleActionEdit =
    (eventVariant: "button" | "analog") =>
    (
      eventName: string,
      action: Action | null,
      onActionChange: (
        config: Action,
        buttonOptions?: Partial<ButtonHoldOptions> &
          Partial<ButtonLongReleaseOptions>,
      ) => void,
    ) => {
      setEvent({ variant: eventVariant, event: eventName })
      setEditAction(action)
      setOnActionChange(() => onActionChange)
      navigateToDetailView("action")
    }

  const detailView = searchParams.get("detail")
  const navigateToDetailView = (
    view: string,
    size: "small" | "large" = "large",
  ) => {
    setDrawerOpen(true)
    setDrawerSize(size)
    navigate(`?detail=${view}`)
  }

  const closeDetailView = (open: boolean) => {
    if (open) return
    setDrawerOpen(false)
    setTimeout(() => navigate(-1), 500)
  }

  const hasReferences = (configItem.ConfigRefs?.length ?? 0) > 0
  const hasPreconditions = (configItem.Preconditions?.length ?? 0) > 0
  const hasModifiers = (configItem.Modifiers?.Items.length ?? 0) > 0

  return (
    <div className="flex flex-col gap-4">
      {variant === "InputConfigItem" && (
        <ConfigTrigger
          configItem={configItem}
          setConfigItem={(item: IConfigItem) => {
            // Update the configItem state here
            onConfigChange(item)
          }}
        />
      )}

      {variant === "OutputConfigItem" && (
        <OutputTriggerCard
          configItem={configItem}
          setConfigItem={(item) => {
            // Update the configItem state here
            onConfigChange(item)
          }}
          navigateToDetailView={navigateToDetailView}
          liveData={liveData}
        />
      )}

      <div className="flex flex-row items-center justify-center gap-2 px-6">
        <Button
          variant="outline"
          size={"sm"}
          onClick={() => navigateToDetailView("precondition")}
          aria-label={t("Dialog.InputConfigWizard.Preconditions.AddButton")}
        >
          <IconPlus className="" />
          {t("Dialog.InputConfigWizard.Preconditions.Label")}
        </Button>
        <Button
          variant="outline"
          size={"sm"}
          onClick={() => navigateToDetailView("configReference")}
          aria-label={t("Dialog.InputConfigWizard.ConfigReferences.AddButton")}
        >
          <IconPlus className="" />
          {t("Dialog.InputConfigWizard.ConfigReferences.Label")}
        </Button>
        <Button
          variant="outline"
          size={"sm"}
          onClick={() => navigateToDetailView("modifier", "small")}
          aria-label={t("Dialog.Modifiers.AddButton")}
        >
          <IconPlus className="" />
          {t("Dialog.Modifiers.Label")}
        </Button>
      </div>

      {(hasReferences || hasPreconditions) && (
        <div className="flex flex-col gap-2 lg:flex-row">
          {hasReferences && (
            <div className="lg:w-1/2">
              <ConfigReferencePanel
                mode={configItem.Type}
                configName={configItem.Name}
                configReferences={configItem.ConfigRefs ?? []}
                variant="summary"
                openDetailsPanel={() => navigateToDetailView("configReference")}
              />
            </div>
          )}
          {hasPreconditions && (
            <div className="lg:w-1/2">
              <PreconditionsPanel
                mode={configItem.Type}
                configName={configItem.Name}
                preconditions={configItem.Preconditions ?? []}
                variant="summary"
                openDetailsPanel={() => navigateToDetailView("precondition")}
              />
            </div>
          )}
        </div>
      )}

      {hasModifiers && (
        <ModifiersPanel
          configItem={configItem}
          variant="summary"
          onConfigChange={onConfigChange}
          openDetailsPanel={() => navigateToDetailView("modifier")}
          liveData={liveData}
        />
      )}

      {variant == "InputConfigItem" && currentDeviceType === "Button" && (
        <ActionBindingPanel
          variant="button"
          onActionEdit={(
            event: string,
            action: Action | null,
            onActionChange: (
              config: Action,
              buttonOptions?: Partial<ButtonHoldOptions> &
                Partial<ButtonLongReleaseOptions>,
            ) => void,
          ) => {
            setEvent({ variant: "button", event })
            setEditAction(action)
            setButtonOptions(buttonOptions)
            setOnActionChange(() => onActionChange)
            navigateToDetailView("action")
          }}
          trigger={
            configItem.button ??
            configItem.inputMultiplexer ??
            configItem.inputShiftRegister
          }
          onTriggerChange={(trigger) => {
            const updatedConfigItem = {
              ...configItem,
              button: trigger as ButtonTrigger,
            }
            onConfigChange(updatedConfigItem)
          }}
        />
      )}
      {variant == "InputConfigItem" && currentDeviceType === "Encoder" && (
        <ActionBindingPanel
          variant="encoder"
          onActionEdit={(
            event: string,
            action: Action | null,
            onConfigChange: (config: Action) => void,
          ) => {
            setEvent({ variant: "encoder", event })
            setEditAction(action)
            setOnActionChange(() => onConfigChange)
            navigateToDetailView("action")
          }}
          trigger={configItem.encoder}
          onTriggerChange={(trigger) => {
            onConfigChange({
              ...configItem,
              encoder: trigger as EncoderTrigger,
            })
          }}
        />
      )}
      {variant == "InputConfigItem" && currentDeviceType === "AnalogInput" && (
        <ActionBindingPanel
          variant="analog"
          onActionEdit={(
            event: string,
            action: Action | null,
            onActionChange: (config: Action) => void,
          ) => {
            setEvent({ variant: "analog", event })
            setEditAction(action)
            setOnActionChange(() => onActionChange)
            navigateToDetailView("action")
          }}
          trigger={configItem.analog}
          onTriggerChange={(trigger) => {
            onConfigChange({
              ...configItem,
              analog: trigger as AnalogTrigger,
            })
          }}
        />
      )}
      {variant == "InputConfigItem" && currentDeviceType === null && (
        <ActionBindingPanel
          variant="none"
          onActionEdit={() => {}}
          onTriggerChange={() => {}}
        />
      )}
      {variant == "OutputConfigItem" && !hasInputAction && (
        <OutputActionBindingCard
          config={configItem as OutputConfigItem}
          onEdit={() => {
            navigateToDetailView("display")
          }}
          onAddOutputDevice={() => {
            navigateToDetailView("display")
          }}
          onAddButtonAction={() => startInputAction("button")}
          onAddAnalogAction={() => startInputAction("analog")}
          onRemoveOutputDevice={removeOutputDevice}
        />
      )}
      {variant == "OutputConfigItem" &&
        outputItem.ButtonInputConfig != null && (
          <ActionBindingPanel
            variant="button"
            onActionEdit={handleActionEdit("button")}
            onRemove={removeInputAction}
            trigger={outputItem.ButtonInputConfig}
            onTriggerChange={(trigger) => {
              onConfigChange({
                ...configItem,
                ButtonInputConfig: trigger as ButtonTrigger,
              } as OutputConfigItem)
            }}
          />
        )}
      {variant == "OutputConfigItem" &&
        outputItem.AnalogInputConfig != null && (
          <ActionBindingPanel
            variant="analog"
            onActionEdit={handleActionEdit("analog")}
            onRemove={removeInputAction}
            trigger={outputItem.AnalogInputConfig}
            onTriggerChange={(trigger) => {
              onConfigChange({
                ...configItem,
                AnalogInputConfig: trigger as AnalogTrigger,
              } as OutputConfigItem)
            }}
          />
        )}

      {detailView && (
        <Drawer
          dismissible={false}
          container={drawerContainer?.current || undefined}
          direction="right"
          open={drawerOpen}
          onClose={() => closeDetailView(false)}
        >
          <DrawerContent className={`pt-4 ${drawerVariant[drawerSize]}`}>
            <div className="flex flex-1 flex-col overflow-y-auto px-4 pb-4">
              {detailView === "precondition" && (
                <PreconditionsPanel
                  mode={configItem.Type}
                  configName={configItem.Name}
                  onPreconditionsChange={(preconditions) => {
                    onConfigChange({
                      ...configItem,
                      Preconditions: preconditions,
                    })
                  }}
                  preconditions={configItem.Preconditions ?? []}
                  variant="details"
                  openDetailsPanel={() => {}}
                />
              )}
              {detailView === "configReference" && (
                <ConfigReferencePanel
                  mode={configItem.Type}
                  configName={configItem.Name}
                  onConfigReferencesChange={(configReferences) => {
                    onConfigChange({
                      ...configItem,
                      ConfigRefs: configReferences,
                    })
                  }}
                  configReferences={configItem.ConfigRefs ?? []}
                  variant="details"
                  openDetailsPanel={() => {}}
                />
              )}
              {detailView === "modifier" && (
                <ModifiersPanel
                  configItem={configItem}
                  onConfigChange={onConfigChange}
                  openDetailsPanel={() => {}}
                  variant="details"
                  liveData={{
                    rawValue: liveData.rawValue,
                    finalValue: liveData.finalValue,
                  }}
                />
              )}
              {detailView === "action" && (
                <ActionEditor
                  mode={configItem.Type}
                  configName={configItem.Name}
                  event={event}
                  buttonOptions={buttonOptions}
                  action={editAction}
                  onActionChange={(action, buttonOptions) => {
                    onActionChange?.(action, buttonOptions)
                    setEditAction(action)
                    if (buttonOptions) {
                      setButtonOptions((prev) => ({
                        ...prev,
                        ...buttonOptions,
                      }))
                    }
                  }}
                />
              )}

              {detailView === "source" && (
                <SourceEditor
                  configName={configItem.Name}
                  source={(configItem as OutputConfigItem).Source}
                  onSourceChange={(source) => {
                    onConfigChange({
                      ...configItem,
                      Source: source,
                    } as OutputConfigItem)
                  }}
                />
              )}

              {detailView === "display" && (
                <OutputDeviceEditor
                  configItem={configItem as OutputConfigItem}
                  onConfigChange={(config) => {
                    onConfigChange(config)
                  }}
                />
              )}
            </div>
            <DrawerFooter className={`flex flex-row items-center justify-end`}>
              <Button
                onClick={() => closeDetailView(false)}
              >
                {t("Dialog.General.Done")}
              </Button>
            </DrawerFooter>
          </DrawerContent>
        </Drawer>
      )}
    </div>
  )
}
export default ConfigWizard
