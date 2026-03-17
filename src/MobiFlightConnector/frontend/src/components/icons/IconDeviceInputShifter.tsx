import { cn } from "@/lib/utils"

type IconDeviceInputShifterProps = {
  className?: string
}

const IconDeviceInputShifter = (props: IconDeviceInputShifterProps) => {
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
        d="m5.2924 7.0005a1 1 0 0 1 0.70703 0.29297l4 4a1 1 0 0 1 0.13867 1.2383 1 1 0 0 1-0.13867 0.17578l-4 4a1 1 0 0 1-1.4141 0 1 1 0 0 1 0-1.4141l3.293-3.293-3.293-3.293a1 1 0 0 1 0-1.4141 1 1 0 0 1 0.70703-0.29297z"
        stroke="none"
      />
      <path
        d="m11.292 7.0005a1 1 0 0 1 0.70703 0.29297l4 4a1 1 0 0 1 0 1.4141l-4 4a1 1 0 0 1-1.4141 0 1 1 0 0 1 0-1.4141l3.293-3.293-3.293-3.293a1 1 0 0 1 0-1.4141 1 1 0 0 1 0.70703-0.29297z"
        stroke="none"
      />
      <path
        d="m19 3.0005a1 1 0 0 0-1 1v16a1 1 0 0 0 1 1 1 1 0 0 0 1-1v-16a1 1 0 0 0-1-1z"
        stroke="none"
      />
    </svg>
  )
}

export default IconDeviceInputShifter
