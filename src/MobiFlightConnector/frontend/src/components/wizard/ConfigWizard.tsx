import ActionBindingPanel from "@/components/wizard/components/ActionBindingPanel"
import ConfigReferencePanel from "@/components/wizard/components/ConfigReferencePanel"
import ConfigTrigger from "@/components/wizard/components/ConfigTrigger"
import PreconditionsPanel from "@/components/wizard/components/PreconditionsPanel"
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
} from "@/types/config"
import {
  Drawer,
  DrawerContent,
  DrawerHeader,
  DrawerTitle,
  DrawerClose,
} from "@/components/ui/drawer"
import { IconArrowBack } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"
import ActionEditor from "@/components/wizard/components/ActionEditor"
import ModifiersPanel from "@/components/wizard/Modifier/ModifiersPanel"

export type ConfigWizardProps = {
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

  const initialButtonOptions = configItem.button
    ? (configItem.button as Partial<ButtonHoldOptions> &
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

  return (
    <div className="flex flex-col gap-4">
      <ConfigTrigger
        configItem={configItem}
        setConfigItem={(item: IConfigItem) => {
          // Update the configItem state here
          onConfigChange(item)
        }}
      />
      <div className="flex flex-row gap-2">
        <div className="w-1/2">
          <PreconditionsPanel
            preconditions={configItem.Preconditions ?? []}
            variant="summary"
            openDetailsPanel={() => navigateToDetailView("precondition")}
          />
        </div>
        <div className="w-1/2">
          <ConfigReferencePanel
            configReferences={configItem.ConfigRefs ?? []}
            variant="summary"
            openDetailsPanel={() => navigateToDetailView("configReference")}
          />
        </div>
      </div>
      <ModifiersPanel
        configItem={configItem}
        variant="summary"
        onConfigChange={onConfigChange}
        openDetailsPanel={() => navigateToDetailView("modifier", "small")}
        liveData={liveData}
      />
      {currentDeviceType === "Button" && (
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
      {currentDeviceType === "Encoder" && (
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
      {currentDeviceType === "AnalogInput" && (
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

      {detailView && (
        <Drawer
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
              <DrawerClose className="text-primary flex flex-row items-center gap-2 underline-offset-4 hover:underline">
                <IconArrowBack size={16} />
                {t("Dialog.General.GoBack")}
              </DrawerClose>
            </DrawerHeader>
            <div className="flex flex-1 flex-col px-4 pb-4">
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
            </div>
          </DrawerContent>
        </Drawer>
      )}
    </div>
  )
}
export default ConfigWizard
