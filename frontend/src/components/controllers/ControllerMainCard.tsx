import ControllerCard from './ControllerCard'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { ControllerType } from '@/types'
import { Controller } from '@/types/controller'

const ControllerMainCard = () => {
  const controller: Controller[] = [
    {
      Name: "MobiFlight Mega",
      Vendor: "MobiFlight",
      ProductId: "1234",
      VendorId: "5678",
      Type: "MobiFlight" as ControllerType,
      Connected: true,
      ImageUrl: "/controller/mobiflight/mobiflight-mega.jpg",
      certified: false,
      firmwareUpdate: true,
    },
    {
      Name: "Thrustmaster T.16000M",
      Vendor: "Thrustmaster",
      ProductId: "1234",
      VendorId: "5678",
      Type: "Joystick" as ControllerType,
      Connected: true,
      ImageUrl: "/controller/thrustmaster/t16000m.jpg",
      certified: false,
    },
    {
      Name: "Bravo Throttle",
      Vendor: "Honeycomb Aeronautical",
      ProductId: "8765",
      VendorId: "4321",
      Type: "Joystick" as ControllerType,
      Connected: false,
      ImageUrl: "/controller/honeycomb/bravo-throttle.jpg",
      certified: true,
    },
    {
      Name: "Alpha Yoke",
      Vendor: "Honeycomb Aeronautical",
      ProductId: "1122",
      VendorId: "3344",
      Type: "Joystick" as ControllerType,
      Connected: true,
      ImageUrl: "/controller/honeycomb/alpha-yoke.jpg",
      certified: true,
    },
    {
      Name: "FCU Unit1",
      Vendor: "WinWing",
      ProductId: "5566",
      VendorId: "7788",
      Type: "Joystick" as ControllerType,
      Connected: false,
      ImageUrl: "/controller/winwing/fcu-unit1.jpg",
      certified: true,
    },
    {
      Name: "MCDU",
      Vendor: "WinWing",
      ProductId: "5566",
      VendorId: "7788",
      Type: "Joystick" as ControllerType,
      Connected: false,
      ImageUrl: "/controller/winwing/mcdu.jpg",
      certified: true,
    },
    {
      Name: "X-Touch Mini",
      Vendor: "Behringer",
      ProductId: "5566",
      VendorId: "7788",
      Type: "Midi" as ControllerType,
      Connected: true,
      ImageUrl: "/controller/behringer/x-touch-mini.jpg",
      certified: true,
    },
  ]

  return (
    <Card className="border-shadow-none border-none shadow-none">
        <CardHeader className="flex flex-row items-center justify-between">
          <div className="flex flex-col gap-2">
            <CardTitle>
              <h2>My Controllers</h2>
            </CardTitle>
            <CardDescription>
              Overview of my controllers used in my projects.
            </CardDescription>
          </div>
          <Button>Go to Settings</Button>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col gap-4">
            <div className="flex flex-row gap-2">
              <Button className="h-8 px-3 text-sm" variant={"default"}>
                All
              </Button>
              <Button className="h-8 px-3 text-sm" variant={"outline"}>
                MobiFlight
              </Button>
              <Button className="h-8 px-3 text-sm" variant={"outline"}>
                Joysticks
              </Button>
              <Button className="h-8 px-3 text-sm" variant={"outline"}>
                Midi
              </Button>
            </div>
            <div className="overflow-auto scroll-smooth">
              <div className="flex flex-row gap-6 pb-3">
                {controller.map((ctrl) => (
                  <ControllerCard
                    controller={ctrl}
                    key={ctrl.Name}
                    className="w-72"
                  />
                ))}
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
  )
}

export default ControllerMainCard