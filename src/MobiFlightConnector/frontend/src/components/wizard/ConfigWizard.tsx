import ActionBindingPanel from "@/components/wizard/InputConfig/ActionBindingPanel"
import ConfigReferencePanel from "@/components/wizard/ConfigReferences/ConfigReferencePanel"
import ConfigTrigger from "@/components/wizard/common/ConfigTrigger"
import OutputTriggerCard from "@/components/wizard/OutputConfig/OutputTriggerCard"
import PreconditionsPanel from "@/components/wizard/Precondition/PreconditionsPanel"
import { IConfigItem } from "@/types"
import { type ReactNode, RefObject, useState, useRef } from "react"
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
import { Drawer, DrawerContent, DrawerFooter } from "@/components/ui/drawer"
import { IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"
import ActionEditor from "@/components/wizard/InputConfig/ActionEditor"
import ModifiersPanel from "@/components/wizard/Modifier/ModifiersPanel"
import { Button } from "@/components/ui/button"
import type { HistoryAction } from "@/types/history"
import SourceEditor from "@/components/wizard/OutputConfig/SourceEditor"
import OutputActionBindingCard from "@/components/wizard/OutputConfig/OutputActionBindingCard"
import OutputDeviceEditor from "@/components/wizard/OutputConfig/OutputDeviceEditor"

export type ConfigWizardProps = {
  variant: ConfigItemType
  configItem: IConfigItem
  onConfigChange: (configItem: IConfigItem, actionLabel?: HistoryAction) => void
  drawerContainer?: RefObject<HTMLDivElement | null>
  liveData: {
    rawValue: string | null | undefined
    finalValue: string | null | undefined
  }
  historyControls?: ReactNode
  onDrawerOpen?: (view: string) => void
  onDrawerClose?: () => void
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
  historyControls,
  onDrawerOpen,
  onDrawerClose,
}: ConfigWizardProps) => {
  const { t } = useTranslation("wizard")
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()

  const currentDeviceType = determineInputDeviceType(configItem.Device?.Type)
  const [drawerOpen, setDrawerOpen] = useState(false)
  const [drawerSize, setDrawerSize] = useState<"small" | "large">("large")

  const [event, setEvent] = useState<{
    variant: "button" | "encoder" | "analog"
    event: string
  }>({ variant: "button", event: "" })

  // an output config either controls a device or triggers an input action
  const outputItem = configItem as OutputConfigItem

  const isOutputConfig = variant === "OutputConfigItem"

  const hasInputAction =
    outputItem.ButtonInputConfig != null || outputItem.AnalogInputConfig != null

  const currentButtonTrigger = (
    isOutputConfig
      ? outputItem.ButtonInputConfig
      : (configItem.button ??
        configItem.inputMultiplexer ??
        configItem.inputShiftRegister)
  ) as
    | (ButtonTrigger &
        Partial<ButtonHoldOptions> &
        Partial<ButtonLongReleaseOptions>)
    | undefined

  const currentButtonOptions: Partial<ButtonHoldOptions> &
    Partial<ButtonLongReleaseOptions> = currentButtonTrigger ?? {
    HoldDelay: 350,
    LongReleaseDelay: 350,
    RepeatDelay: 0,
  }

  const currentAction: Action | null =
    event.variant === "button"
      ? (currentButtonTrigger?.[
          event.event as "onPress" | "onRelease" | "onHold" | "onLongRelease"
        ] ?? null)
      : event.variant === "encoder"
        ? (configItem.encoder?.[event.event as keyof EncoderTrigger] ?? null)
        : ((isOutputConfig
            ? outputItem.AnalogInputConfig?.onChange
            : configItem.analog?.onChange) ?? null)

  const handleActionChange = (
    action: Action | null,
    options?: Partial<ButtonHoldOptions> & Partial<ButtonLongReleaseOptions>,
    actionLabel?: HistoryAction,
  ) => {
    if (event.variant === "button") {
      const updatedButton = {
        ...currentButtonTrigger,
        [event.event]: action,
        ...options,
      } as ButtonTrigger

      if (isOutputConfig) {
        onConfigChange(
          {
            ...outputItem,
            ButtonInputConfig: updatedButton,
          } as OutputConfigItem,
          actionLabel,
        )
      } else {
        onConfigChange(
          {
            ...configItem,
            button: updatedButton,
          },
          actionLabel,
        )
      }
      return
    }

    if (event.variant === "encoder") {
      onConfigChange(
        {
          ...configItem,
          encoder: {
            ...configItem.encoder,
            [event.event]: action,
          } as EncoderTrigger,
        },
        actionLabel,
      )
      return
    }

    const updatedAnalog = {
      ...(isOutputConfig ? outputItem.AnalogInputConfig : configItem.analog),
      onChange: action ?? undefined,
    } as AnalogTrigger

    if (isOutputConfig) {
      onConfigChange(
        {
          ...outputItem,
          AnalogInputConfig: updatedAnalog,
        } as OutputConfigItem,
        actionLabel,
      )
    } else {
      onConfigChange(
        {
          ...configItem,
          analog: updatedAnalog,
        },
        actionLabel,
      )
    }
  }

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
    (eventVariant: "button" | "analog") => (eventName: string) => {
      setEvent({ variant: eventVariant, event: eventName })
      navigateToDetailView("action")
    }

  const detailView = searchParams.get("detail")
  const drawerClosingRef = useRef(false)
  const navigateToDetailView = (
    view: string,
    size: "small" | "large" = "large",
  ) => {
    drawerClosingRef.current = false
    onDrawerOpen?.(view)
    setDrawerOpen(true)
    setDrawerSize(size)
    navigate(`?detail=${view}`)
  }

  const closeDetailView = (open: boolean) => {
    if (open || drawerClosingRef.current) return
    drawerClosingRef.current = true
    onDrawerClose?.()
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
          setConfigItem={(item: IConfigItem, actionLabel?: HistoryAction) => {
            // Update the configItem state here
            onConfigChange(item, actionLabel)
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
          aria-label={t("ConfigWizard.Preconditions.AddButton")}
        >
          <IconPlus className="" />
          {t("ConfigWizard.Preconditions.Label")}
        </Button>
        <Button
          variant="outline"
          size={"sm"}
          onClick={() => navigateToDetailView("configReference")}
          aria-label={t("ConfigWizard.ConfigReferences.AddButton")}
        >
          <IconPlus className="" />
          {t("ConfigWizard.ConfigReferences.Label")}
        </Button>
        <Button
          variant="outline"
          size={"sm"}
          onClick={() => navigateToDetailView("modifier", "small")}
          aria-label={t("ConfigWizard.Modifiers.AddButton")}
        >
          <IconPlus className="" />
          {t("ConfigWizard.Modifiers.Label")}
        </Button>
      </div>

      {(hasReferences || hasPreconditions) && (
        <div className="flex flex-col gap-2 lg:flex-row">
          {hasReferences && (
            <div className="lg:w-1/2">
              <ConfigReferencePanel
                mode={configItem.Type}
                configGuid={configItem.GUID}
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
          onActionEdit={(selectedEvent) => {
            setEvent({
              variant: "button",
              event: selectedEvent,
            })
            navigateToDetailView("action")
          }}
          trigger={
            configItem.button ??
            configItem.inputMultiplexer ??
            configItem.inputShiftRegister
          }
          onTriggerChange={(trigger, actionLabel) => {
            const updatedConfigItem = {
              ...configItem,
              button: trigger as ButtonTrigger,
            }
            onConfigChange(
              updatedConfigItem,
              actionLabel ?? "Dialog.History.TriggerChanged",
            )
          }}
        />
      )}
      {variant == "InputConfigItem" && currentDeviceType === "Encoder" && (
        <ActionBindingPanel
          variant="encoder"
          onActionEdit={(selectedEvent) => {
            setEvent({
              variant: "encoder",
              event: selectedEvent,
            })
            navigateToDetailView("action")
          }}
          trigger={configItem.encoder}
          onTriggerChange={(trigger, actionLabel) => {
            onConfigChange(
              {
                ...configItem,
                encoder: trigger as EncoderTrigger,
              },
              actionLabel ?? "Dialog.History.TriggerChanged",
            )
          }}
        />
      )}
      {variant == "InputConfigItem" && currentDeviceType === "AnalogInput" && (
        <ActionBindingPanel
          variant="analog"
          onActionEdit={(selectedEvent) => {
            setEvent({
              variant: "analog",
              event: selectedEvent,
            })
            navigateToDetailView("action")
          }}
          trigger={configItem.analog}
          onTriggerChange={(trigger, actionLabel) => {
            onConfigChange(
              {
                ...configItem,
                analog: trigger as AnalogTrigger,
              },
              actionLabel ?? "Dialog.History.TriggerChanged",
            )
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
                  onPreconditionsChange={(preconditions, actionLabel) => {
                    onConfigChange(
                      {
                        ...configItem,
                        Preconditions: preconditions,
                      },
                      actionLabel,
                    )
                  }}
                  preconditions={configItem.Preconditions ?? []}
                  variant="details"
                  openDetailsPanel={() => {}}
                />
              )}
              {detailView === "configReference" && (
                <ConfigReferencePanel
                  mode={configItem.Type}
                  configGuid={configItem.GUID}
                  configName={configItem.Name}
                  onConfigReferencesChange={(configReferences, actionLabel) => {
                    onConfigChange(
                      {
                        ...configItem,
                        ConfigRefs: configReferences,
                      },
                      actionLabel,
                    )
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
                  buttonOptions={
                    event.variant === "button"
                      ? currentButtonOptions
                      : undefined
                  }
                  action={currentAction}
                  onActionChange={handleActionChange}
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
            <DrawerFooter className="shrink-0 flex-row items-center justify-between border-t">
              <div>{historyControls}</div>
              <Button onClick={() => closeDetailView(false)}>
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
