import {
  Icon123,
  IconAbc,
  IconBulb,
  IconPower,
  IconQuestionMark,
  IconRefreshDot,
} from "@tabler/icons-react";

export type DeviceIconVariant =
  | "default"
  | "Output"
  | "Display Module"
  | "lcd"
  | "Button"
  | "Encoder"
  | "analogInput";

export type DeviceIconProps = {
  variant?: DeviceIconVariant;
  className?: string;
};

const DeviceIcon = ({
  variant = "default",
  className = "",
}: DeviceIconProps) => {
  let icon = <IconQuestionMark />;
  if (variant === "Output") {
    icon = <IconBulb className="stroke-pink-600" />;
  } else if (variant === "Display Module") {
    icon = <Icon123 className="stroke-pink-600"/>;
  } else if (variant === "lcd") {
    icon = <IconAbc className="stroke-pink-600"/>;
  } else if (variant === "Button") {
    icon = <IconPower className="stroke-teal-600" />;
  } else if (variant === "Encoder") {
    icon = <IconRefreshDot className="stroke-teal-600"/>;
  }
  return <div className={className}>{icon}</div>;
};

export default DeviceIcon;
