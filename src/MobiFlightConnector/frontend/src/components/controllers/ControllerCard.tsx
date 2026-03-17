import { cn } from "@/lib/utils"
import { Controller } from "@/types/controller"
import {
  IconDotsVertical,
  IconPlug,
  IconPlugOff,
  IconRosetteDiscountCheckFilled,
} from "@tabler/icons-react"
import { HtmlHTMLAttributes } from "react"
import { Button } from "../ui/button"

export type ControllerCardProps = HtmlHTMLAttributes<HTMLDivElement> & {
  controller: Controller
}

const ControllerCard = ({
  controller,
  className,
  ...otherProps
}: ControllerCardProps) => {
  return (
    <div
      {...otherProps}
      className={cn(
        className,
        "border-border bg-card space-y-2 rounded-md p-4 shadow-sm transition-all duration-200 ease-in-out hover:shadow-lg",
      )}
    >
      <div className="flex flex-row items-center justify-between">
        <div className="flex flex-row items-center justify-start">
          <h3 className="truncate text-lg font-semibold">{controller.Name}</h3>
          {controller.certified && (
            <span className="text-primary flex flex-row items-center font-semibold">
              <IconRosetteDiscountCheckFilled className="h-6" />
            </span>
          )}
        </div>
        <div role="button" title="Controller Options">
          <IconDotsVertical className="text-muted-foreground h-6" />
        </div>
      </div>
      <div className="3xl:h-48 relative h-32 w-full overflow-hidden rounded-md">
        {controller.firmwareUpdate && (
          <div className="absolute inset-0 flex items-center justify-center">
            <Button className="shadow-md">Update available!</Button>
          </div>
        )}
        <img
          src={
            new URL(
              `${controller.ImageUrl || "default-controller.png"}`,
              import.meta.url,
            ).href
          }
          alt={controller.Name}
          className={cn(
            "h-full w-full rounded-md object-scale-down",
            controller.Connected ? "" : "opacity-60",
          )}
        />
      </div>
      <p className="text-muted-foreground flex flex-col gap-2 text-sm">
        <span className="flex flex-row items-center gap-1">
          {controller.Connected ? (
            <IconPlug className="h-6" />
          ) : (
            <IconPlugOff className="h-6" />
          )}
          {controller.Connected ? "Connected" : "Disconnected"}
        </span>
        {/* <span className="flex flex-row items-center justify-between">
          <div className="flex flex-row items-center gap-1">
            <IconSettings className="text-muted-foreground h-6" />
            Settings
          </div>
          {!controller.Connected && (
            <span
              className="flex flex-row items-center"
              role="button"
              title="Delete Controller"
            >
              <IconTrash className="h-6 hover:text-red-600" />
            </span>
          )}
        </span> */}
      </p>
    </div>
  )
}

export default ControllerCard
