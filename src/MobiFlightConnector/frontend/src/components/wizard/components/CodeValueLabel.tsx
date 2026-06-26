import { cn } from "@/lib/utils"

type CodeValueLabelProps = React.HTMLAttributes<HTMLDivElement> & {
  children: React.ReactNode
}

const CodeValueLabel = ({ children, ...props }: CodeValueLabelProps) => {
  return (
    <div
      {...props}
      className={cn(
        "bg-accent min-h-7 truncate rounded px-2 py-1 font-mono text-sm whitespace-pre-wrap",
        props.className,
      )}
    >
      {children}
    </div>
  )
}
export default CodeValueLabel
