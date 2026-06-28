import { cn } from "@/lib/utils"

type CodeValueLabelProps = React.HTMLAttributes<HTMLDivElement> & {
  children: React.ReactNode
}

const CodeValueLabel = ({ children, ...props }: CodeValueLabelProps) => {
  return (
    <div
      {...props}
      className={cn(
        "bg-secondary min-h-7 truncate rounded px-2 py-1 pt-1.5 text-code",
        props.className,
      )}
    >
      {children}
    </div>
  )
}
export default CodeValueLabel
