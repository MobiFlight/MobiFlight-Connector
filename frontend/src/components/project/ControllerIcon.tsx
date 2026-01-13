import IconBrandMobiFlightLogo from "@/components/icons/IconBrandMobiFlightLogo"
import { cn } from "@/lib/utils"
import { ControllerBindingStatus } from "@/types/controller"
import {
  IconDeviceGamepad2,
  IconPiano,
  IconQuestionMark,
} from "@tabler/icons-react"
import { HtmlHTMLAttributes } from "react"
import { useTranslation } from "react-i18next"

export type ControllerIconProps = {
  serial: string
  status?: ControllerBindingStatus
}

const ControllerIcons = {
  mobiflight: {
    generic: IconBrandMobiFlightLogo,
    official: {
      mega: "/controller/type/mobiflight-mega.png",
      micro: "/controller/type/mobiflight-micro.png",
      nano: "/controller/type/mobiflight-nano.png",
    },
    miniCockpit: {
      "miniCOCKPIT miniFCU": "/controller/minicockpit/minicockpit-logo.png",
    },
  },
  joystick: {
    generic: IconDeviceGamepad2,
    authentikit: {
      AuthentiKit: "/controller/authentikit/atk-orange-button-logo.png",
    },
    honeycomb: {
      "Alpha Flight Controls": "/controller/honeycomb/alpha-yoke.jpg",
      "Bravo Throttle Quadrant": "/controller/honeycomb/bravo-throttle.jpg",
    },
    octavi: {
      Octavi: "/controller/octavi/octavi-logo-small.png",
    },
    thrustmaster: {
      "T.16000M": "/controller/thrustmaster/t16000m.jpg",
    },
    winwing: {
      "WINWING MCDU-32-CAPTAIN": "/controller/winwing/mcdu.jpg",
    },
  },
  midi: {
    generic: IconPiano,
  },
}

const FindControllerIcon = (controllerType: string, deviceName: string) => {
  const controllerTypeIcons =
    ControllerIcons[controllerType as keyof typeof ControllerIcons]

  if (!controllerTypeIcons) return IconQuestionMark

  const specificControllerIcon =
    Object.values(controllerTypeIcons)
      .flat()
      .find((c) => Object.keys(c).includes(deviceName)) ?? null

  if (specificControllerIcon) {
    return specificControllerIcon[
      deviceName as keyof typeof specificControllerIcon
    ]
  }

  // if we get here, then we didn't find a specific icon for the deviceName
  // let's try a generic one for the type
  return controllerTypeIcons["generic"]
}

const ControllerIcon = ({
  serial,
  status,
  className,
  ...props
}: HtmlHTMLAttributes<HTMLDivElement> & ControllerIconProps) => {
  const { t } = useTranslation()

  const controllerType = serial.includes("SN-")
    ? "mobiflight"
    : serial.includes("JS-")
      ? "joystick"
      : serial.includes("MI-")
        ? "midi"
        : "unknown"

  const usingController = serial != ""
  const deviceName = serial.split("/")[0].trim() || ""
  const iconResult = FindControllerIcon(controllerType, deviceName)
  // Handle component rendering
  const IconComponent = typeof iconResult !== "string" ? iconResult : null

  const variant = {
    Match: "bg-green-600",
    AutoBind: "bg-primary",
    Missing: "bg-background border-3 border-gray-400",
    RequiresManualBind: "bg-red-500",
  } as Record<ControllerBindingStatus, string>

  const titleStatus = t(`Project.BindingStatus.${status}`)

  return usingController ? (
    <div className="relative">
      <div
        data-testid="controller-icon"
        title={`${deviceName} - ${titleStatus}`}
        className={cn(
          `border-card bg-card shadow-foreground/20 flex h-10 w-10 items-center justify-center overflow-hidden rounded-full border-2 shadow-sm dark:shadow-none [&_svg]:h-full [&_svg]:w-full`,
          className,
        )}
        {...props}
      >
        {typeof iconResult === "string" ? (
          <img
            className="h-full w-full object-cover"
            src={iconResult}
            alt={`${controllerType} controller icon`}
          />
        ) : IconComponent ? (
          <IconComponent />
        ) : null}
      </div>
      {status && (
        <div
          className={cn(
            `bg-accent outline-background absolute right-0 bottom-0 h-3 w-3 rounded-full outline-3`,
            status && variant[status],
          )}
        ></div>
      )}
    </div>
  ) : null
}

export default ControllerIcon
