import { cn } from "@/lib/utils"

type CodeValueLabelProps = React.HTMLAttributes<HTMLDivElement> & {
  children: React.ReactNode
}

const CodeValueLabel = ({ children, ...props }: CodeValueLabelProps) => {
  return (
    <div
      {...props}
      className={cn(
        "bg-secondary truncate rounded px-2 text-code whitespace-nowrap",
        props.className,
      )}
    >
      {children}
    </div>
  )
}
export default CodeValueLabel