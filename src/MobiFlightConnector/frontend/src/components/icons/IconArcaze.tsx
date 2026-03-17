import { cn } from "@/lib/utils"
import { IconXboxA } from "@tabler/icons-react"
export type IconArcazeProps = {
  className?: string
}

const IconArcaze = (props: IconArcazeProps) => {
  const className = cn("stroke-emerald-500 fill-green-700", props.className)

  return (
    <IconXboxA className={className} />
  )
}
export default IconArcaze