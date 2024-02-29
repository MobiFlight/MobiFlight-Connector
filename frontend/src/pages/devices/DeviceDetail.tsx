import { Card, CardHeader, CardContent, CardFooter } from "@/components/ui/card"
import { useDevicesStore } from "@/stores/deviceStateStore"
import { Switch } from "@/components/ui/switch"
import { IconDots, IconInfoCircle } from "@tabler/icons-react"
import { useParams } from "react-router"
import {
  Link,
  Outlet,
  useBlocker,
  useLocation,
  useNavigate,
  useOutletContext,
} from "react-router-dom"
import { MobiFlightDeviceEditPanel } from "@/components/mobiflight/edit/DeviceDetailMobiFlightElements"
import { Button } from "@/components/ui/button"
import { useEffect, useState } from "react"
import { Dialog, DialogContent } from "@/components/ui/dialog"
import { H3 } from "@/components/mobiflight/H2"
import { DeviceSelection } from "@/components/mobiflight/edit/DeviceSelection"
import { IDeviceItem } from "@/types"
import { IDeviceElement } from "@/types/deviceElements"
import { publishOnMessageExchange } from "@/lib/hooks"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog"
import {
  DeviceElementCreateResponse,
  DeviceUploadMessage,
} from "@/types/messages"

export type DeviceDetailContext = {
  device: IDeviceItem
  element: IDeviceElement
  updateDevice: (device: IDeviceItem) => void
}

export function useDeviceDetailPageContext() {
  return useOutletContext<DeviceDetailContext>()
}

const DeviceDetailPage = () => {
  // we extract the base route for the current device
  const baseRoute = useLocation().pathname.split("/elements")[0]

  // we will still be able to navigate to other elements while changes are made
  // but as soon as we want to navigate to a different device or page
  // blocker will prevent that
  const blocker = useBlocker(({ currentLocation, nextLocation }) => {
    return deviceHasChanged && nextLocation.pathname.indexOf(baseRoute) !== 0
  })

  const { publish } = publishOnMessageExchange()
  const navigate = useNavigate()
  const params = useParams()
  const id = params.id
  const elementId = params.elementId
  const { devices } = useDevicesStore()
  const [device, setDevice] = useState(
    devices.find((device) => device.Id === id)
  )
  const [deviceHasChanged, setDeviceHasChanged] = useState(false)
  const [isOpen, setIsOpen] = useState(false)
  const element = device?.Elements!.find((element) => element.Id === elementId)

  const updateDevice = (d: IDeviceItem) => {
    setDevice(d)
    setDeviceHasChanged(true)
  }

  const uploadDeviceConfig = () => {
    publish({ key: "DeviceUpload", payload: device } as DeviceUploadMessage)
  }

  const onElementDelete = (element:IDeviceElement) => {
    const updatedElements = device!.Elements!.filter((e) => e.Id !== element.Id)
    updateDevice({
      ...device!,
      Elements: updatedElements,
    })    
  }

  const onElementAdded = (message: DeviceElementCreateResponse) => {
    // get the updated pins
    // by convention we use PinXXX as the key for any pin
    const usedPins = Object.keys(message.Element.ConfigData)
      .filter((k) => k.indexOf("Pin") > -1)
      .map((key) => message.Element.ConfigData[key])

    const updatePins = device!.Pins!.map((pin) => {
      if (usedPins.indexOf(pin.Pin.toString()) > -1) {
        return { ...pin, Used: true }
      }
      return pin
    })

    updateDevice({
      ...device!,
      Elements: [...device!.Elements!, message.Element],
      Pins: updatePins,
    })
    setIsOpen(false)
    navigate(
      `/devices/${device?.Type}/${device!.Id}/elements/${message.Element.Id}`
    )
  }

  useEffect(() => {
    setDeviceHasChanged(false)
  }, [devices])

  return (
    <div className="flex flex-col overflow-y-auto gap-4 select-none">
      {blocker && (
        <AlertDialog open={blocker.state === "blocked"}>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>You have unsaved changes</AlertDialogTitle>
              <AlertDialogDescription>
                You will lose all unsaved changes if you continue.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel onClick={() => blocker.reset?.()}>
                Cancel
              </AlertDialogCancel>
              <AlertDialogAction onClick={() => blocker.proceed?.()}>
                Continue
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      )}
      <div className="flex flex-row gap-4 items-center">
        <Link
          to="/devices"
          className="scroll-m-20 text-3xl tracking-tight first:mt-0"
        >
          Devices
        </Link>
        <p className="scroll-m-20 text-3xl tracking-tight first:mt-0">&gt;</p>
        <p
          className={`scroll-m-20 text-3xl tracking-tight first:mt-0 ${element ? "" : "font-bold"}`}
        >
          {device?.Name}
        </p>
        {element && (
          <>
            <p className="scroll-m-20 text-3xl tracking-tight first:mt-0">
              &gt;
            </p>
            <p className="scroll-m-20 text-3xl tracking-tight first:mt-0 font-bold">
              {element.Name}
            </p>
          </>
        )}
      </div>
      <div className="flex flex-row gap-4 pb-4 overflow-auto items-start">
        {!device && <div>Device not found</div>}
        {device && (
          <Card className="w-3/12 hidden lg:block">
            <CardHeader className="flex flex-row gap-2 items-center">
              {device?.MetaData && (
                <div>
                  {device?.MetaData["Icon"] && (
                    <img className="w-10 h-10" src={device.MetaData["Icon"]} />
                  )}
                </div>
              )}
              <div className="flex flex-col mt-0">
                <div className="text-xl font-semibold">{device.Name}</div>
                <div className="text-sm">{device.Type}</div>
              </div>
            </CardHeader>
            <CardContent>
              {device?.MetaData && (
                <div className="h-96 overflow-hidden flex items-center">
                  {device?.MetaData["Picture"] && (
                    <img
                      className="w-full h-full object-contain"
                      src={device.MetaData["Picture"]}
                    />
                  )}
                </div>
              )}
            </CardContent>
            <CardFooter className="flex justify-between">
              <Switch>Active</Switch>
              <IconDots className="cursor-pointer"></IconDots>
            </CardFooter>
            {deviceHasChanged && (
              <div className="bg-green-200 dark:bg-green-950 p-4 rounded-md text-green-700 dark:text-green-600 flex flex-row justify-between items-center">
                <IconInfoCircle />
                <p>Device has been updated</p>
                <Button onClick={uploadDeviceConfig}>Upload changes</Button>
              </div>
            )}
          </Card>
        )}
        {device && device.Type === "MobiFlight" && (
          <Card className="flex flex-col w-96 overflow-y-auto select-none bg-transparent shadow-lg hover:border-none border-none hover:bg-transparent  dark:bg-zinc-700/10 dark:hover:bg-zinc-700/20">
            <CardHeader className="flex flex-col mt-0">
              <div className="text-xl font-semibold">Components</div>
              <div className="text-sm">Some cool sub title here</div>
            </CardHeader>
            <MobiFlightDeviceEditPanel device={device} onElementDelete={onElementDelete}/>
            <CardFooter className="p-6 px-4">
              <Dialog open={isOpen} onOpenChange={setIsOpen}>
                <Button onClick={() => setIsOpen(true)}>Add device</Button>
                <DialogContent className="max-w-2xl items-center">
                  <H3>
                    Select your new device (
                    {device!.Pins!.filter((p) => !p.Used).length || 0} pins
                    free)
                  </H3>
                  <DeviceSelection
                    device={device}
                    onDeviceSelected={onElementAdded}
                  />
                </DialogContent>
              </Dialog>
            </CardFooter>
          </Card>
        )}
        {device && device.Type == "Joystick" && (
          <Card className="w-96">
            <CardHeader className="flex flex-row gap-2 items-center text-2xl">
              Settings
            </CardHeader>
            <div>Joystick</div>
          </Card>
        )}
        <div className="grow">
          <Outlet
            context={{
              device: device,
              element: element,
              updateDevice: updateDevice,
            }}
          />
        </div>
        {isOpen && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center">
            <div className="bg-white p-4">Add device</div>
            <Button onClick={() => setIsOpen(false)}>Close</Button>
          </div>
        )}
      </div>
    </div>
  )
}

export default DeviceDetailPage
