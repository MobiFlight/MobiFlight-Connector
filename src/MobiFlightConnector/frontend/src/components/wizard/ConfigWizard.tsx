import { Button } from "@/components/ui/button"
import AnalogActionBindingPanel from "@/components/wizard/components/AnalogActionBindingPanel"
import ButtonActionBindingPanel from "@/components/wizard/components/ButtonActionBindingPanel"
import ConfigReferencePanel from "@/components/wizard/components/ConfigReferencePanel"
import ConfigTrigger from "@/components/wizard/components/ConfigTrigger"
import EncoderActionBindingPanel from "@/components/wizard/components/EncoderActionBindingPanel"
import PreconditionPanel from "@/components/wizard/components/PreconditionPanel"
import { publishOnMessageExchange } from "@/lib/hooks/appMessage"
import { IConfigItem } from "@/types"
import { RefObject, useState } from "react"
import { useLocation, useNavigate, useSearchParams } from "react-router"
import {
  Drawer,
  DrawerContent,
  DrawerHeader,
  DrawerTitle,
  DrawerClose,
} from "@/components/ui/drawer"
import { IconArrowBack } from "@tabler/icons-react"

export type ConfigWizardProps = {
  configItem: IConfigItem
  onClose: () => void
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
  onClose,
  drawerContainer,
}: ConfigWizardProps) => {
  const [currentConfigItem, setCurrentConfigItem] = useState(configItem)
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()

  const location = useLocation()
  console.log("Current location:", location)

  const currentDeviceType = determineInputDeviceType(
    currentConfigItem.Device?.Type,
  )
  const [ drawerOpen, setDrawerOpen ] = useState(false)

  const detailView = searchParams.get("detail")
  const navigateToDetailView = (view: string) => {
    setDrawerOpen(true)
    navigate(`?detail=${view}`)
  }

  const closeDetailView = (open: boolean) => {
    console.log(
      "Closing detail view, current search params:",
      searchParams.toString(),
    )
    if (open) return
    setDrawerOpen(false)
    setTimeout(() => navigate(-1), 500)
  }

  const saveChanges = () => {
    const { publish } = publishOnMessageExchange()
    publish({
      key: "CommandUpdateConfigItem",
      payload: {
        item: currentConfigItem,
      },
    })
    onClose() // Close the wizard after saving
  }

  return (
    <div className="flex flex-col gap-4">
      <ConfigTrigger
        configItem={currentConfigItem}
        setConfigItem={(item: IConfigItem) => {
          // Update the configItem state here
          setCurrentConfigItem(item)
        }}
      />
      <div className="flex flex-row gap-2">
        <div className="w-1/2">
          <PreconditionPanel
            preconditions={currentConfigItem.Preconditions ?? []}
            variant="summary"
            openDetailsPanel={() => navigateToDetailView("precondition")}
          />
        </div>
        <div className="w-1/2">
          <ConfigReferencePanel />
        </div>
      </div>
      {currentDeviceType === "Button" && (
        <ButtonActionBindingPanel
          trigger={
            currentConfigItem.button ??
            currentConfigItem.inputMultiplexer ??
            currentConfigItem.inputShiftRegister
          }
          onTriggerChange={(trigger) => {
            setCurrentConfigItem({
              ...currentConfigItem,
              button: trigger,
            })
          }}
        />
      )}
      {currentDeviceType === "Encoder" && (
        <EncoderActionBindingPanel
          trigger={currentConfigItem.encoder}
          onTriggerChange={(trigger) => {
            setCurrentConfigItem({
              ...currentConfigItem,
              encoder: trigger,
            })
          }}
        />
      )}
      {currentDeviceType === "AnalogInput" && (
        <AnalogActionBindingPanel
          trigger={currentConfigItem.analog}
          onTriggerChange={(trigger) => {
            setCurrentConfigItem({
              ...currentConfigItem,
              analog: trigger,
            })
          }}
        />
      )}

      <div className="flex flex-row justify-end gap-2">
        <Button variant="outline">Cancel</Button>
        <Button onClick={saveChanges}>Save</Button>
      </div>

      {detailView && (
        <Drawer
          container={drawerContainer?.current || undefined}
          direction="right"
          open={drawerOpen}
          onClose={() => closeDetailView(false)}
        >
          <DrawerContent className="data-[vaul-drawer-direction=right]:sm:max-w-200 data-[vaul-drawer-direction=right]:w-200">
            <DrawerHeader>
              <DrawerTitle className="sr-only">Preconditions</DrawerTitle>
              <DrawerClose className="flex flex-row">
                <Button variant="link">
                  <IconArrowBack />
                  Go back
                </Button>
              </DrawerClose>
            </DrawerHeader>
            <div className="px-4">
              <PreconditionPanel
                onPreconditionsChange={(preconditions) => {
                  setCurrentConfigItem({
                    ...currentConfigItem,
                    Preconditions: preconditions,
                  })
                }} 
                preconditions={currentConfigItem.Preconditions ?? []}
                variant="details"
                openDetailsPanel={() => {}}
              />
            </div>
          </DrawerContent>
        </Drawer>
      )}
    </div>
  )
}
export default ConfigWizard
