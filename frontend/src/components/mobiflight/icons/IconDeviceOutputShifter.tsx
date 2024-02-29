import { cn } from "@/lib/utils"

type IconDeviceOutputShifterProps = {
  className?: string
}

const IconDeviceOutputShifter = (props: IconDeviceOutputShifterProps) => {
  const className = cn(
    "stroke-current fill-current ",
    props.className
  )
  
  return (
    <svg
      className={className}
      id="SVGRoot"
      width="24"
      height="24"
      version="1.1"
      viewBox="0 0 24 24"
    >
      <path
        d="m15.414 6.9269a1 1 0 0 0-0.70703 0.29297 1 1 0 0 0 0 1.4141l3.293 3.293-3.293 3.293a1 1 0 0 0 0 1.4141 1 1 0 0 0 0.81836 0.28516 1 1 0 0 0 0.5957-0.28516l4-4a1 1 0 0 0 0.05664-0.06055 1 1 0 0 0-0.05664-1.3535l-4-4a1 1 0 0 0-0.70703-0.29297z"
        stroke="none"
      />
      <path
        d="m9.4138 6.9269a1 1 0 0 0-0.70703 0.29297 1 1 0 0 0 0 1.4141l3.293 3.293-3.293 3.293a1 1 0 0 0 0 1.4141 1 1 0 0 0 1.4141 0l4-4a1 1 0 0 0 0.05664-0.06055 1 1 0 0 0-0.05664-1.3535l-4-4a1 1 0 0 0-0.70703-0.29297z"
        stroke="none"
      />
      <path
        d="m5.3535 2.9269a1 1 0 0 0-1 1v16a1 1 0 0 0 1 1 1 1 0 0 0 1-1v-16a1 1 0 0 0-1-1z"
        stroke="none"
      />
    </svg>
  )
}

export default IconDeviceOutputShifter
