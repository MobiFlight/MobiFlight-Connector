import { Card, CardHeader, CardContent, CardFooter } from "@/components/ui/card"
import { useDevicesStore } from "@/stores/deviceStateStore"
import { Switch } from "@/components/ui/switch"
import { IconDots } from "@tabler/icons-react"
import { useParams } from "react-router"
import { Link, Outlet, useOutletContext } from "react-router-dom"
import { MobiFlightDeviceEditPanel } from "@/components/mobiflight/edit/DeviceDetailMobiFlightElements"
import { Button } from "@/components/ui/button"
import { useEffect, useState } from "react"
import { Dialog, DialogContent } from "@/components/ui/dialog"
import { H3 } from "@/components/mobiflight/H2"
import { DeviceSelection } from "@/components/mobiflight/edit/DeviceSelection"
import { IDeviceItem } from "@/types"
import { IDeviceElement } from "@/types/config"
import { publishOnMessageExchange } from "@/lib/hooks"

export type DeviceDetailContext = {
  device: IDeviceItem
  element: IDeviceElement
  updateDevice: (device: IDeviceItem) => void
}

export function useDeviceDetailPageContext() {
  return useOutletContext<DeviceDetailContext>();
}

const DeviceDetailPage = () => {
  const { publish } = publishOnMessageExchange()
  const params = useParams()
  const id = params.id
  const elementId = params.elementId
  const { devices } = useDevicesStore()
  const [ device, setDevice ] = useState(devices.find((device) => device.Id === id))
  const [isOpen, setIsOpen] = useState(false)
  const element = device?.Elements!.find((element) => element.Id === elementId)

  const updateDevice = (d: IDeviceItem) => {
    setDevice(d)
  }

  
  useEffect(() => {
    console.log(device)  
  }, [device])
  

  return (
    <div className="flex flex-col overflow-y-auto gap-4 select-none">
      <div className="flex flex-row gap-4 items-center">
        <Link
          to="/devices"
          className="scroll-m-20 text-3xl tracking-tight first:mt-0"
        >
          Devices
        </Link>
        <p className="scroll-m-20 text-3xl tracking-tight first:mt-0">&gt;</p>
        <p className={`scroll-m-20 text-3xl tracking-tight first:mt-0 ${element?'':'font-bold'}`}>
          {device?.Name}
        </p>
        {element && (
          <>
            <p className="scroll-m-20 text-3xl tracking-tight first:mt-0">
              &gt;
            </p>
            <p className="scroll-m-20 text-3xl tracking-tight first:mt-0 font-bold">{element.Name}</p>
          </>
        )}
      </div>
      <div className="flex flex-row gap-4 pb-4 overflow-auto">
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
                    <img className="w-full" src={device.MetaData["Picture"]} />
                  )}
                </div>
              )}
            </CardContent>
            <CardFooter className="flex justify-between">
              <Switch>Active</Switch>
              <IconDots className="cursor-pointer"></IconDots>
            </CardFooter>
          </Card>
        )}
        {device && device.Type === "MobiFlight" && (
          <Card className="flex flex-col w-96 overflow-y-auto select-none bg-transparent shadow-none hover:border-none border-none hover:bg-transparent  dark:bg-zinc-700/10 dark:hover:bg-zinc-700/20">
            <CardHeader className="flex flex-col mt-0">
                <div className="text-xl font-semibold">Components</div>
                <div className="text-sm">Some cool sub title here</div>
            </CardHeader>
            <MobiFlightDeviceEditPanel device={device} />
            <CardFooter className="p-6 px-4">
              <Dialog open={isOpen} onOpenChange={setIsOpen}>
                <Button onClick={()=>setIsOpen(true)}>
                  Add device
                </Button>
                <DialogContent className="max-w-2xl items-center">
                  <H3>Select your new device</H3>
                  <DeviceSelection />
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
          <Outlet context={{ device: device, element: element, updateDevice: updateDevice}} />
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
