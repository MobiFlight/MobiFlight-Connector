import ActionBindingPanel from "@/components/wizard/components/ActionBindingPanel"
import ConfigReferencePanel from "@/components/wizard/ConfigReferences/ConfigReferencePanel"
import ConfigTrigger from "@/components/wizard/components/ConfigTrigger"
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
} from "@/types/config"
import {
  Drawer,
  DrawerContent,
  DrawerHeader,
  DrawerTitle,
  DrawerClose,
  DrawerFooter,
} from "@/components/ui/drawer"
import { IconArrowBack, IconPlus } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"
import ActionEditor from "@/components/wizard/components/ActionEditor"
import ModifiersPanel from "@/components/wizard/Modifier/ModifiersPanel"
import { Button } from "@/components/ui/button"

export type ConfigWizardProps = {
  configItem: IConfigItem
  onConfigChange: (configItem: IConfigItem) => void
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
  configItem,
  onConfigChange,
  drawerContainer,
  liveData,
  historyControls,
  onDrawerOpen,
  onDrawerClose,
}: ConfigWizardProps) => {
  const { t } = useTranslation()
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()

  const currentDeviceType = determineInputDeviceType(configItem.Device?.Type)
  const [drawerOpen, setDrawerOpen] = useState(false)
  const [drawerSize, setDrawerSize] = useState<"small" | "large">("large")

  const [event, setEvent] = useState<{
    variant: "button" | "encoder" | "analog"
    event: string
  }>({ variant: "button", event: "" })

  const currentButtonTrigger = (configItem.button ??
    configItem.inputMultiplexer ??
    configItem.inputShiftRegister) as
    | (ButtonTrigger &
        Partial<ButtonHoldOptions> &
        Partial<ButtonLongReleaseOptions>)
    | undefined

  const currentAction: Action | null =
    event.variant === "button"
      ? (currentButtonTrigger?.[
          event.event as "onPress" | "onRelease" | "onHold" | "onLongRelease"
        ] ?? null)
      : event.variant === "encoder"
        ? (configItem.encoder?.[event.event as keyof EncoderTrigger] ?? null)
        : (configItem.analog?.onChange ?? null)

  const handleActionChange = (
    action: Action | null,
    options?: Partial<ButtonHoldOptions> & Partial<ButtonLongReleaseOptions>,
  ) => {
    if (event.variant === "button") {
      onConfigChange({
        ...configItem,
        button: {
          ...currentButtonTrigger,
          [event.event]: action,
          ...options,
        } as ButtonTrigger,
      })
      return
    }

    if (event.variant === "encoder") {
      onConfigChange({
        ...configItem,
        encoder: {
          ...configItem.encoder,
          [event.event]: action,
        } as EncoderTrigger,
      })
      return
    }

    onConfigChange({
      ...configItem,
      analog: {
        ...configItem.analog,
        onChange: action ?? undefined,
      } as AnalogTrigger,
    })
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
      <ConfigTrigger
        configItem={configItem}
        setConfigItem={(item: IConfigItem) => {
          // Update the configItem state here
          onConfigChange(item)
        }}
      />
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
                configReferences={configItem.ConfigRefs ?? []}
                variant="summary"
                openDetailsPanel={() => navigateToDetailView("configReference")}
              />
            </div>
          )}
          {hasPreconditions && (
            <div className="lg:w-1/2">
              <PreconditionsPanel
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
          openDetailsPanel={() => navigateToDetailView("modifier", "small")}
          liveData={liveData}
        />
      )}

      {currentDeviceType === "Button" && (
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
          onTriggerChange={(trigger) => {
            const updatedConfigItem = {
              ...configItem,
              button: trigger as ButtonTrigger,
            }
            onConfigChange(updatedConfigItem)
          }}
        />
      )}
      {currentDeviceType === "Encoder" && (
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
          onTriggerChange={(trigger) => {
            onConfigChange({
              ...configItem,
              encoder: trigger as EncoderTrigger,
            })
          }}
        />
      )}
      {currentDeviceType === "AnalogInput" && (
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
          onTriggerChange={(trigger) => {
            onConfigChange({
              ...configItem,
              analog: trigger as AnalogTrigger,
            })
          }}
        />
      )}
      {currentDeviceType === null && (
        <ActionBindingPanel
          variant="none"
          onActionEdit={() => {}}
          onTriggerChange={() => {}}
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
          <DrawerContent className={drawerVariant[drawerSize]}>
            <DrawerHeader>
              <DrawerTitle className="sr-only">
                {t("Dialog.InputConfigWizard.DrawerTitle")}
              </DrawerTitle>
              <DrawerClose
                onClick={() => closeDetailView(false)}
                className="text-primary flex flex-row items-center gap-2 underline-offset-4 hover:underline"
              >
                <IconArrowBack size={16} />
                {t("Dialog.General.GoBack")}
              </DrawerClose>
            </DrawerHeader>
            <div className="flex flex-1 flex-col overflow-y-auto px-4 pb-4">
              {detailView === "precondition" && (
                <PreconditionsPanel
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
                  event={event}
                  buttonOptions={
                    event.variant === "button"
                      ? currentButtonTrigger
                      : undefined
                  }
                  action={currentAction}
                  onActionChange={handleActionChange}
                />
              )}
            </div>
            <DrawerFooter className="shrink-0 flex-row justify-start border-t">
              {historyControls}
            </DrawerFooter>
          </DrawerContent>
        </Drawer>
      )}
    </div>
  )
}
export default ConfigWizard
