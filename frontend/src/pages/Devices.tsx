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

export default function DevicesPage() {
  const { devices } = useDevicesStore();
  return (
    <div className="flex flex-col overflow-y-auto">
      <H2>Devices</H2>
      <div className="flex flex-row flex-wrap w-full gap-4 overflow-y-auto">
        {devices?.map((device) => (
          <Card key={device.Id} className="w-96 border-4 border-transparent hover:border-gray-200 hover:bg-slate-100 hover:dark:border-primary ease-in-out transition-all hover:dark:bg-gray-700">
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
                <div className="h-60 overflow-hidden flex items-center">
                  {device?.MetaData["Picture"] && (
                    <img className="w-full" src={device.MetaData["Picture"]} />
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
