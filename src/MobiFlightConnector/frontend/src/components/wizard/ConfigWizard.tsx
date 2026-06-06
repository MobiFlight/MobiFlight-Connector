import { Button } from "@/components/ui/button"
import AnalogActionBindingPanel from "@/components/wizard/components/AnalogActionBindingPanel"
import ButtonActionBindingPanel from "@/components/wizard/components/ButtonActionBindingPanel"
import ConfigReferencePanel from "@/components/wizard/components/ConfigReferencePanel"
import ConfigTrigger from "@/components/wizard/components/ConfigTrigger"
import EncoderActionBindingPanel from "@/components/wizard/components/EncoderActionBindingPanel"
import PreconditionsPanel from "@/components/wizard/components/PreconditionsPanel"
import { IConfigItem } from "@/types"
import { RefObject, useState } from "react"
import { useNavigate, useSearchParams } from "react-router"
import {
  Drawer,
  DrawerContent,
  DrawerHeader,
  DrawerTitle,
  DrawerClose,
} from "@/components/ui/drawer"
import { IconArrowBack } from "@tabler/icons-react"
import { useTranslation } from "react-i18next"

export type ConfigWizardProps = {
  configItem: IConfigItem
  onConfigChange: (configItem: IConfigItem) => void
  drawerContainer?: RefObject<HTMLDivElement | null>
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

const ConfigWizard = ({
  configItem,
  onConfigChange,
  drawerContainer,
}: ConfigWizardProps) => {
  const { t } = useTranslation()
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()

  const currentDeviceType = determineInputDeviceType(
    configItem.Device?.Type,
  )
  const [drawerOpen, setDrawerOpen] = useState(false)

  const detailView = searchParams.get("detail")
  const navigateToDetailView = (view: string) => {
    setDrawerOpen(true)
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
      {currentDeviceType === "Button" && (
        <ButtonActionBindingPanel
          trigger={
            configItem.button ??
            configItem.inputMultiplexer ??
            configItem.inputShiftRegister
          }
          onTriggerChange={(trigger) => {
            onConfigChange({
              ...configItem,
              button: trigger,
            })
          }}
        />
      )}
      {currentDeviceType === "Encoder" && (
        <EncoderActionBindingPanel
          trigger={configItem.encoder}
          onTriggerChange={(trigger) => {
            onConfigChange({
              ...configItem,
              encoder: trigger,
            })
          }}
        />
      )}
      {currentDeviceType === "AnalogInput" && (
        <AnalogActionBindingPanel
          trigger={configItem.analog}
          onTriggerChange={(trigger) => {
            onConfigChange({
              ...configItem,
              analog: trigger,
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
          <DrawerContent className="data-[vaul-drawer-direction=right]:w-200 data-[vaul-drawer-direction=right]:sm:max-w-200">
            <DrawerHeader>
              <DrawerTitle className="sr-only">{t("Dialog.InputConfigWizard.DrawerTitle")}</DrawerTitle>
              <DrawerClose className="flex flex-row">
                <Button variant="link">
                  <IconArrowBack />
                  {t("Dialog.InputConfigWizard.GoBack")}
                </Button>
              </DrawerClose>
            </DrawerHeader>
            <div className="px-4">
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
            </div>
          </DrawerContent>
        </Drawer>
      )}
    </div>
  )
}
export default ConfigWizard
