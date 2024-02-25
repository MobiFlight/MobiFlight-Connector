import { cn } from "@/lib/utils";
import { DeviceElementType } from "../../../types/deviceElements.d";
import {
  Icon123,
  IconAbc,
  IconBox,
  IconBulb,
  IconDashboard,
  IconGauge,
  IconPower,
  IconQuestionMark,
  IconRefreshDot,
} from "@tabler/icons-react";
import IconDeviceAnalog from "./IconDeviceAnalog";
import IconDeviceOutputShifter from "./IconDeviceOutputShifter";
import IconDeviceInputShifter from "./IconDeviceInputShifter";
import IconDeviceInputMultiplexer from "./IconDeviceInputMultiplexer";

export type DeviceIconProps = {
  variant?: DeviceElementType;
  className?: string;
};

const DeviceIcon = (props: DeviceIconProps) => {
  const { variant, className } = props;

  let icon = <IconQuestionMark className={className}/>
  if (variant === "Output") {
    icon = <IconBulb className={cn("stroke-pink-600", className) } />;
  } else if (variant === "LedModule") {
    icon = <Icon123 className={cn("stroke-pink-600", className) } />;
  } else if (variant === "LcdDisplay") {
    icon = <IconAbc className={cn("stroke-pink-600", className) } />;
  } else if (variant === "ShiftRegister") {
    icon = <IconDeviceOutputShifter className={cn("stroke-pink-600 fill-pink-600", className) } />;
  } else if (variant === "Stepper") {
    icon = <IconGauge className={cn("stroke-pink-600", className) } />;
  } else if (variant === "Servo") {
    icon = <IconDashboard className={cn("stroke-pink-600", className) } />;
  }else if (variant === "Button") {
    icon = <IconPower className={cn("stroke-teal-600", className) }/>;
  } else if (variant === "Encoder") {
    icon = <IconRefreshDot className={cn("stroke-teal-600", className) }/>;
  } else if (variant === "AnalogInput") {
    icon = <IconDeviceAnalog className={cn("stroke-teal-600 fill-teal-600 stroke-2", className) }/>;
  } else if (variant === "InputShiftRegister") {
    icon = <IconDeviceInputShifter className={cn("stroke-teal-600 fill-teal-600", className) } />;
  } else if (variant === "InputMultiplexer") {
    icon = <IconDeviceInputMultiplexer className={cn("stroke-teal-600 fill-teal-600", className) } />;
  } else if (variant === "CustomDevice") {
    icon = <IconBox className={cn("stroke-pink-600", className) } />;
  }

  return <div className="inline-block">{icon}</div>;
};

export default DeviceIcon;
