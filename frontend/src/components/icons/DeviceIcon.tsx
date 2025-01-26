import { cn } from "@/lib/utils";
import { DeviceElementType } from "@/types/deviceElements.d";
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
  variant?: DeviceElementType
  disabled?: boolean
  className?: string
};

const DeviceIcon = (props: DeviceIconProps) => {
  const { variant, disabled, className } = props;

  const shortType = variant?.replace("MobiFlight.", "").replace("OutputConfig.", "").replace("InputConfig.", "") as DeviceElementType

  let icon = <IconQuestionMark className={className}/>
  if (shortType === "Output") {
    icon = <IconBulb className={cn("stroke-pink-600", disabled?"stroke-slate-400":"", className) } />;
  } else if (shortType === "LedModule") {
    icon = <Icon123 className={cn("stroke-pink-600", disabled?"stroke-slate-400":"", className) } />;
  } else if (shortType === "LcdDisplay") {
    icon = <IconAbc className={cn("stroke-pink-600", disabled?"stroke-slate-400":"", className) } />;
  } else if (shortType === "ShiftRegister") {
    icon = <IconDeviceOutputShifter className={cn("stroke-pink-600 fill-pink-600", disabled?"stroke-slate-400 fill-slate-400":"", className) } />;
  } else if (shortType === "Stepper") {
    icon = <IconGauge className={cn("stroke-pink-600", disabled?"stroke-slate-400":"", className) } />;
  } else if (shortType === "Servo") {
    icon = <IconDashboard className={cn("stroke-pink-600", disabled?"stroke-slate-400":"", className) } />;
  }else if (shortType === "Button") {
    icon = <IconPower className={cn("stroke-teal-600", disabled?"stroke-slate-400":"", className) }/>;
  } else if (shortType === "Encoder") {
    icon = <IconRefreshDot className={cn("stroke-teal-600", disabled?"stroke-slate-400":"", className) }/>;
  } else if (shortType === "AnalogInput") {
    icon = <IconDeviceAnalog className={cn("stroke-teal-600 fill-teal-600 stroke-2", disabled?"stroke-slate-400 fill-slate-400":"", className) }/>;
  } else if (shortType === "InputShiftRegister") {
    icon = <IconDeviceInputShifter className={cn("stroke-teal-600 fill-teal-600", disabled?"stroke-slate-400 fill-slate-400":"", className) } />;
  } else if (shortType === "InputMultiplexer") {
    icon = <IconDeviceInputMultiplexer className={cn("stroke-teal-600 fill-teal-600", disabled?"stroke-slate-400 fill-slate-400":"", className) } />;
  } else if (shortType === "CustomDevice") {
    icon = <IconBox className={cn("stroke-pink-600", disabled?"stroke-slate-400":"", className) } />;
  }

  return <div className="inline-block">{icon}</div>;
};

export default DeviceIcon;
