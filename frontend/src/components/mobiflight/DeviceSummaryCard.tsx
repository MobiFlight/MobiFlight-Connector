import { Card, CardContent, CardFooter, CardHeader } from "../ui/card";
import { IconDots } from "@tabler/icons-react";
import { IDeviceItem } from "@/types";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu";
import { publishOnMessageExchange } from "@/lib/hooks";
import { cn } from "@/lib/utils";
import { NavLink } from "react-router-dom";
import ConditionalWrapper from "./ConditionalWrapper";
import { Progress } from "@/components/ui/progress";
import { Switch } from "@/components/ui/switch";

export type DeviceSummaryCardProps = {
  device: IDeviceItem;
  className?: string;
  navLink?: string;
};

const DeviceSummaryCard = (props: DeviceSummaryCardProps) => {
  const { device, className, navLink } = props;
  const { publish } = publishOnMessageExchange();
  const usedPins = device.Pins ? device.Pins?.filter((pin) => pin.Used).length : 0;
  const usedPercentage = device.Pins ? Math.round(usedPins * 100 / device.Pins!.length) : 0
  console.log(usedPins, usedPercentage)

  const uploadConfigRequest = () => {
    console.log("Upload config request");
    publish({
      key: "DeviceUploadMessage",
      payload: device,
    });
  };

  const openFromFileRequest = () => {
    publish({
      key: "DeviceFileOpenRequest",
      payload: {
        IgnoreTypeMismatch: false,
        Device: device,
      },
    });
  };

  const saveToFileRequest = () => {
    publish({
      key: "DeviceFileSaveRequest",
      payload: {
        Device: device,
      },
    });
  };

  const firmwareUpdateRequest = () => {
    publish({
      key: "DeviceFirmwareUpdateRequest",
      payload: device,
    });
  };

  const firmwareResetRequest = () => {
    publish({
      key: "DeviceFirmwareResetRequest",
      payload: device,
    });
  };

  return (
    <Card className={className}>
      <ConditionalWrapper
        condition={navLink === undefined}
        wrapper={(children) => (
          <NavLink to={`/devices/${device.Type}/${device.Id}`}>
            {children}
          </NavLink>
        )}
      >
        <CardHeader className={cn("flex flex-row items-center gap-2")}>
          {device?.MetaData && (
            <div>
              {device?.MetaData["Icon"] && (
                <img className="h-10 w-10" src={device.MetaData["Icon"]} />
              )}
            </div>
          )}
          <div className="mt-0 flex flex-col">
            <div className="text-xl font-semibold">{device.Name}</div>
            <div className="text-sm">{device.Type}</div>
          </div>
        </CardHeader>
        <CardContent className="hidden md:block">
          {device?.MetaData && (
            <div className="flex h-60 items-center overflow-hidden">
              {device?.MetaData["Picture"] && (
                <img
                  className="h-full w-full object-contain"
                  src={device.MetaData["Picture"]}
                />
              )}
            </div>
          )}
        </CardContent>
      </ConditionalWrapper>
      <CardFooter className="flex flex-row justify-between gap-4 group">
        <Switch>Active</Switch>
        {device.Pins && (
          <Progress
            color={cn(
              "bg-gray-200 group-hover/card:bg-green-800", 
              usedPercentage > 60 ? "group-hover/card:bg-green-600" : "",
              usedPercentage > 70 ? "group-hover/card:bg-blue-500" : "",
              usedPercentage > 90 ? "group-hover/card:bg-primary" : "",
              ) 
            }
            className={"w-1/2"}
            value={usedPercentage}
          />
        )}
        <DropdownMenu>
          <DropdownMenuTrigger>
            <IconDots className="cursor-pointer"></IconDots>
          </DropdownMenuTrigger>
          <DropdownMenuContent side="top" align="end">
            <DropdownMenuItem onClick={uploadConfigRequest}>
              Upload config
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuLabel>Backup</DropdownMenuLabel>
            <DropdownMenuItem onClick={openFromFileRequest}>
              Open from file
            </DropdownMenuItem>
            <DropdownMenuItem onClick={saveToFileRequest}>
              Save to file
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuLabel>Firmware</DropdownMenuLabel>
            <DropdownMenuItem onClick={firmwareUpdateRequest}>
              Update
            </DropdownMenuItem>
            <DropdownMenuItem onClick={firmwareResetRequest}>
              Reset
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuLabel>Troubleshooting</DropdownMenuLabel>
            <DropdownMenuItem>Regenerate serial</DropdownMenuItem>
            <DropdownMenuItem>Reload config</DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </CardFooter>
    </Card>
  );
};

export default DeviceSummaryCard;
