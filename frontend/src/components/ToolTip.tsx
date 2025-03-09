import { Tooltip, TooltipContent, TooltipTrigger } from "./ui/tooltip"
import { cn } from "@/lib/utils"

type ToolTipProps = {
  children: React.ReactNode
  content: string | React.ReactNode
  className?: string
}

const ToolTip = ({children, content, className }: ToolTipProps) => {

  return (
      <Tooltip aria-role="tooltip">
        <TooltipTrigger  asChild>
          { children }
        </TooltipTrigger>
        <TooltipContent align="start" side="bottom" className={cn("rounded-none", className)}>
          {
            typeof(content)=="string" ? 
            <div className="text-xs">{ content }</div>
            :
            content
          }
        </TooltipContent>
      </Tooltip>
  )
}

export default ToolTip