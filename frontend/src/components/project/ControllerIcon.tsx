import IconBrandMobiFlightLogo from "@/components/icons/IconBrandMobiFlightLogo"
import { cn } from "@/lib/utils"
import { ControllerBinding, ControllerBindingStatus } from "@/types/controller"
import {
  IconDeviceGamepad2,
  IconPiano,
  IconQuestionMark,
} from "@tabler/icons-react"
import { HtmlHTMLAttributes } from "react"
import { useTranslation } from "react-i18next"

export type ControllerIconProps = {
  controllerBinding: ControllerBinding
}

const ControllerIcons = {
  mobiflight: {
    generic: <IconBrandMobiFlightLogo />,
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
    generic: <IconDeviceGamepad2 />,
    authentikit: {
      AuthentiKit: "/controller/authentikit/atk-orange-button-logo.png",
    },
    honeycomb: {
      "Alpha Flight Controls": "/controller/honeycomb/alpha-yoke.jpg",
      "Bravo Throttle Quadrant": "/controller/honeycomb/bravo-throttle.jpg",
    },
    octavi: {
      Octavi: "/controller/type/ocatvi-octavi.png",
    },
    saitek: {
      "Saitek Aviator Stick": "/controller/type/saitek-aviator-stick.png",
    },
    thrustmaster: {
      "Thrustmaster T.16000M": "/controller/type/thrustmaster-t16000m.png",
    },
    vkbsim: {
      "S-TECS MODERN THROTTLE MAX":
        "/controller/type/vkbsim-stecs-throttle.png",
      "S-TECS MODERN THROTTLE MAX STEM":
        "/controller/type/vkbsim-stecs-throttle.png",
      "S-TECS MODERN THROTTLE MAX STEM FSM.GA":
        "/controller/type/vkbsim-stecs-throttle.png",
      "S-TECS MODERN THROTTLE MINI":
        "/controller/type/vkbsim-stecs-throttle.png",
    },
    wingflex: {
      "FCU Cube": "/controller/type/wingflex-joystick.png",
    },
    winwing: {
      "WINWING MCDU-32-CAPTAIN": "/controller/winwing/mcdu.jpg",
    },
  },
  midi: {
    generic: <IconPiano />,
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
  console.log(controllerTypeIcons)
  return controllerTypeIcons["generic"]
}

const ControllerIcon = ({
  controllerBinding,
  className,
  ...props
}: HtmlHTMLAttributes<HTMLDivElement> & ControllerIconProps) => {
  const serial =
    controllerBinding.BoundController ||
    controllerBinding.OriginalController ||
    ""
  const status = controllerBinding.Status
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
  const controllerIcon = FindControllerIcon(controllerType, deviceName)

  const variant = {
    Match: "bg-green-600",
    AutoBind: "bg-primary",
    Missing: "bg-gray-300",
    RequiresManualBind: "bg-red-500",
  } as Record<ControllerBindingStatus, string>

  const titleStatus = t(`Project.BindingStatus.${status}`)

  return usingController ? (
    <div className="relative">
      <div
        data-testid="controller-icon"
        title={`${deviceName} - ${titleStatus}`}
        className={cn(
          `border-card bg-card flex h-10 w-10 items-center justify-center overflow-hidden rounded-full outline-3 outline-background [&_svg]:h-full [&_svg]:w-full shadow-md shadow-foreground/50`,
          className,
        )}
        {...props}
      >
        {typeof controllerIcon === "string" ? (
          <img
            className="h-full w-full object-cover"
            src={controllerIcon}
            alt={`${controllerType} controller icon`}
          />
        ) : (
          controllerIcon
        )}
      </div>
      <div className={`rounded-full h-2.5 w-2.5 absolute bottom-0 right-0 bg-accent outline-background outline-3 ${variant[status]}`}></div>
    </div>
  ) : null
}

export default ControllerIcon
