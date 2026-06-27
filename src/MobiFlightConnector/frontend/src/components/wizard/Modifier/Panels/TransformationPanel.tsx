import { Input } from "@/components/ui/input"
import { Transformation } from "@/types/modifier"
import { Switch } from "@/components/ui/switch"
import { Button } from "@/components/ui/button"
import { IconTrash } from "@tabler/icons-react"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"

type TransformationPanelProps = {
  variant: "summary" | "editor"
  modifier: Transformation
  onChange: (updated: Transformation) => void
  onDelete: () => void
}

const TransformationPanel = ({
  variant,
  modifier,
  onChange,
  onDelete,
}: TransformationPanelProps) => {
  return variant === "summary" ? (
    <div>TransformationPanel Summary</div>
  ) : (
    <div className="flex flex-col gap-2 rounded-md border p-2">
      <div className="flex flex-row items-center gap-4">
        <Switch
          id="active"
          checked={modifier.Active}
          onCheckedChange={(checked) =>
            onChange({ ...modifier, Active: checked })
          }
        />
        <div className="grow">Transformation</div>
        <Button onClick={onDelete} size={"sm"} variant="ghost">
          <IconTrash />
        </Button>
      </div>
      <Separator className="mb-2" />
      <div className="flex flex-row items-center gap-4">
        <div className="flex grow flex-col gap-1">
          <Label htmlFor="expression">Expression</Label>
          <Input
            id="expression"
            value={modifier.Expression}
            onChange={(e) =>
              onChange({ ...modifier, Expression: e.target.value })
            }
          />
        </div>
      </div>
    </div>
  )
}
export default TransformationPanel
