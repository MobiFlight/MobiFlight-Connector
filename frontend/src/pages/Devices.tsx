import H2 from "@/components/mobiflight/H2";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
} from "@/components/ui/card";
import { Switch } from "@/components/ui/switch";
import { useDevicesStore } from "@/stores/deviceStateStore";
import { IconDots } from "@tabler/icons-react";
import { NavLink } from "react-router-dom";

export default function DevicesPage() {
  const { devices } = useDevicesStore();
  return (
    <div className="flex flex-col overflow-y-auto gap-4">
      <div className="flex flex-row gap-4 items-center">
        <p className="scroll-m-20 text-3xl tracking-tight first:mt-0 font-bold">
          Devices
        </p>
      </div>
      <div className="flex flex-row flex-wrap w-full gap-4 overflow-y-auto">
        {devices?.map((device) => (
          <Card key={device.Id} className="w-96">
            <NavLink to={`/devices/${device.Type}/${device.Id}`}>
              <CardHeader className="flex flex-row gap-2 items-center">
                {device?.MetaData && (
                  <div>
                    {device?.MetaData["Icon"] && (
                      <img
                        className="w-10 h-10"
                        src={device.MetaData["Icon"]}
                      />
                    )}
                  </div>
                )}
                <div className="flex flex-col mt-0">
                  <div className="text-xl font-semibold">{device.Name}</div>
                  <div className="text-sm">{device.MetaData.Project}</div>
                </div>
              </CardHeader>
              <CardContent>
                {device?.MetaData && (
                  <div className="h-60 flex items-center">
                    {device?.MetaData["Picture"] && (
                      <img
                        className="w-full h-full object-contain"
                        src={device.MetaData["Picture"]}
                      />
                    )}
                  </div>
                )}
                {/* <Collapsible
                open={isOpen[device.Id]}
                onOpenChange={(value: boolean) => {
                  setIsOpen({ deviceId: value });
                }}
              >
                <CollapsibleTrigger>Meta Data</CollapsibleTrigger>
                <CollapsibleContent>
                  {Object.keys(device?.MetaData ?? {}).map((key: string) => (
                    <p>
                      {key} : {device.MetaData[key]}
                    </p>
                  ))}
                </CollapsibleContent>
              </Collapsible> */}
              </CardContent>
            </NavLink>
            <CardFooter className="flex justify-between">
              <Switch>Active</Switch>
              <IconDots className="cursor-pointer"></IconDots>
            </CardFooter>
          </Card>
        ))}
      </div>
    </div>
  );
}
