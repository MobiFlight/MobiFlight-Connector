import H2 from "@/components/mobiflight/H2";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
} from "@/components/ui/card";
import { Collapsible, CollapsibleContent } from "@/components/ui/collapsible";
import { useDevicesStore } from "@/stores/deviceStateStore";
import { IDictionary } from "@/types/config";
import { CollapsibleTrigger } from "@radix-ui/react-collapsible";
import { useState } from "react";

export default function DevicesPage() {
  const { devices } = useDevicesStore();
  const [isOpen, setIsOpen] = useState<{ [Key: string]: boolean }>({});

  return (
    <div>
      <H2>Devices</H2>
      <div className="flex flex-row flex-wrap w-full gap-4">
        {devices.map((device) => (
          <Card key={device.Id} className="w-96">
            <CardHeader className="flex flex-row gap-2 items-center">
              {device?.MetaData && (
                <div>
                  {device?.MetaData["BoardIcon"] && (
                    <img
                      className="w-8 h-8"
                      src={device.MetaData["BoardIcon"]}
                    />
                  )}
                  {device?.MetaData["Icon"] && (
                    <img className="w-8 h-8" src={device.MetaData["Icon"]} />
                  )}
                </div>
              )}
              <div>{device.Name}</div>
            </CardHeader>
            <CardContent>
              {device?.MetaData && (
                <>
                  <div>
                    {device?.MetaData["BoardPicture"] && (
                      <img src={device.MetaData["BoardPicture"]} />
                    )}
                    {device?.MetaData["Picture"] && (
                      <img src={device.MetaData["Picture"]} />
                    )}
                  </div>
                </>
              )}
              <Collapsible
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
              </Collapsible>
            </CardContent>
            <CardFooter>{device.Type}</CardFooter>
          </Card>
        ))}
      </div>
    </div>
  );
}
