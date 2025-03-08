import { Tooltip, TooltipContent, TooltipTrigger } from "./ui/tooltip"


type ToolTipProps = {
  children: React.ReactNode
  content: string | React.ReactNode
}

const ToolTip = ({children, content }: ToolTipProps) => {
  return (
    
      <Tooltip delayDuration={500} aria-role="tooltip" >
        <TooltipTrigger  asChild>
          { children }
        </TooltipTrigger>
        <TooltipContent align="start" side="bottom" className="rounded-none">
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