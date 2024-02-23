import { cn } from "@/lib/utils";
import { DeviceElementType } from "../../../types/config";
import {
  Icon123,
  IconAbc,
  IconBulb,
  IconPower,
  IconQuestionMark,
  IconRefreshDot,
} from "@tabler/icons-react";

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
    icon = <IconAbc className={cn("stroke-pink-600", className) } />;
  } else if (variant === "Stepper") {
    icon = <IconAbc className={cn("stroke-pink-600", className) } />;
  } else if (variant === "Servo") {
    icon = <IconAbc className={cn("stroke-pink-600", className) } />;
  }else if (variant === "Button") {
    icon = <IconPower className={cn("stroke-teal-600", className) }/>;
  } else if (variant === "Encoder") {
    icon = <IconRefreshDot className={cn("stroke-teal-600", className) }/>;
  } else if (variant === "AnalogInput") {
    icon = <IconRefreshDot className={cn("stroke-teal-600", className) }/>;
  } else if (variant === "InputShiftRegister") {
    icon = <IconAbc className={cn("stroke-pink-600", className) } />;
  } else if (variant === "InputMultiplexer") {
    icon = <IconAbc className={cn("stroke-pink-600", className) } />;
  } else if (variant === "CustomDevice") {
    icon = <IconAbc className={cn("stroke-pink-600", className) } />;
  }

  return <div className="inline-block">{icon}</div>;
};

export default DeviceIcon;
