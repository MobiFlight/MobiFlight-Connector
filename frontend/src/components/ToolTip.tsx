import { Tooltip, TooltipContent, TooltipTrigger } from "./ui/tooltip"


type ToolTipProps = {
  children: React.ReactNode
  title: string
}

const ToolTip = ({children, title }: ToolTipProps) => {
  return (
    
      <Tooltip delayDuration={500} aria-role="tooltip">
        <TooltipTrigger  asChild>
          { children }
        </TooltipTrigger>
        <TooltipContent side="bottom" className="max-w-52">
          <div>{ title }</div>
        </TooltipContent>
      </Tooltip>
  )
}

export default ToolTip