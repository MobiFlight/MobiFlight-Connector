import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { IconEdit } from "@tabler/icons-react"

const PreconditionPanel = () => {
  return (
    <Card>
      <CardContent className="flex flex-col gap-2 pt-4">
        <div className="text-lg font-semibold">Preconditions (optional)</div>
        <div className="text-muted-foreground text-sm">
          The preconditions define conditions that must be met before the action
          can be executed.
        </div>
        <Button variant="outline">
          <IconEdit className="" />
          Preconditions
        </Button>
      </CardContent>
    </Card>
  )
}
export default PreconditionPanel
