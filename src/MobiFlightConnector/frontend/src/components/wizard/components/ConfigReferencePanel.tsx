import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { IconEdit } from "@tabler/icons-react"

const ConfigReferencePanel = () => {
  return (
    <Card>
      <CardContent className="flex flex-col gap-2 pt-4">
        <div className="text-lg font-semibold">Config References (optional)</div>
        <div className="text-muted-foreground text-sm">
          Config references let you use values from other configurations or variables.
        </div>
        <Button variant="outline">
          <IconEdit className="" />
          Config References
        </Button>
      </CardContent>
    </Card>
  )
}
export default ConfigReferencePanel
