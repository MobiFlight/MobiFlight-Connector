import { Card, CardContent } from "@/components/ui/card"
import { cn } from "@/lib/utils"

export type WizardSectionProps = React.ComponentProps<typeof CardContent>

const WizardSection = ({
  className,
  children,
  ...props
}: WizardSectionProps) => {
  return (
    <Card>
      <CardContent
        className={cn("flex flex-col gap-4 pt-4", className)}
        {...props}
      >
        {children}
      </CardContent>
    </Card>
  )
}
export default WizardSection
