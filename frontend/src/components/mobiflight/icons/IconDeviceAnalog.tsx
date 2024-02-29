import { cn } from "@/lib/utils"

type IconDeviceAnalogProps = {
  className?: string
}

const IconDeviceAnalog = (props: IconDeviceAnalogProps) => {
  const className = cn(
    "stroke-current fill-current ",
    props.className
  )
  
  return (
    <svg
      className={className}
      id="icon_device_analog"
      width="24"
      height="24"
      version="1.1"
      viewBox="0 0 24 24"
    >
      <g transform="translate(247 -655)">
        <g id="icon_in_analog" transform="translate(1.1176e-5 -24)">
          <path
            d="m-235 685c-3.3019 0-6 2.6981-6 6 0 3.3018 2.6981 6 6 6 3.3018 0 6-2.6982 6-6 0-1.2863-0.41034-2.4798-1.1055-3.459-0.0806 0.0689-0.15094 0.13071-0.19726 0.17578l-3.5 3.4102c-0.39587 0.3851-1.029 0.37634-1.4141-0.0195-0.38509-0.39587-0.37633-1.029 0.0195-1.4141l3.5-3.4082c0.0419-0.0408 0.0989-0.10623 0.16016-0.17579-0.98006-0.69704-2.1746-1.1094-3.4629-1.1094z"
            stroke="none"
            strokeLinecap="round" 
            strokeLinejoin="round" 
          />
          <path
            d="m-241.36 697.36a9 9 0 0 1 0-12.728 9 9 0 0 1 12.728 0 9 9 0 0 1 0 12.728"
            fill="none"
            strokeLinecap="round" 
            strokeLinejoin="round" 
          />
        </g>
      </g>
    </svg>
  )
}

export default IconDeviceAnalog
