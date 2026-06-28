import { Input } from "@/components/ui/input"
import { Transformation } from "@/types/modifier"
import { Switch } from "@/components/ui/switch"
import { Button } from "@/components/ui/button"
import {
  IconChevronDown,
  IconGripVertical,
  IconTrash,
} from "@tabler/icons-react"
import { Label } from "@/components/ui/label"

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible"
import { useState } from "react"

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
  const [open, setOpen] = useState(false)

  return variant === "summary" ? (
    <div>TransformationPanel Summary</div>
  ) : (
    <div className="flex flex-col gap-2 rounded-md border px-1 py-0.5">
      <Collapsible
        open={open}
        onOpenChange={setOpen}
        className="flex flex-col gap-2"
      >
        <div className="flex flex-row items-center gap-4">
          <IconGripVertical className="stroke-2" />
          <Switch
            id="active"
            checked={modifier.Active}
            onCheckedChange={(checked) =>
              onChange({ ...modifier, Active: checked })
            }
          />
          <CollapsibleTrigger className="flex grow flex-row items-center justify-between">
            <div className="text-md px-2 font-semibold">Transformation</div>
            <Button onClick={() => {}} size={"sm"} variant="ghost">
              <IconChevronDown />
            </Button>
          </CollapsibleTrigger>
          <Button onClick={onDelete} size={"sm"} variant="ghost">
            <IconTrash />
          </Button>
        </div>
        <CollapsibleContent className="pl-27">
          <div className="flex flex-row items-center gap-4 pr-16 pb-4">
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
        </CollapsibleContent>
      </Collapsible>
    </div>
  )
}
export default TransformationPanel
