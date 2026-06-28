import { Input } from "@/components/ui/input"
import { Transformation } from "@/types/modifier"
import { Switch } from "@/components/ui/switch"
import { Button } from "@/components/ui/button"
import {
  IconChevronDown,
  IconChevronUp,
  IconGripVertical,
  IconMathFunction,
  IconTrash,
} from "@tabler/icons-react"
import { Label } from "@/components/ui/label"

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible"
import { useState } from "react"
import { Badge } from "@/components/ui/badge"
import CodeValueLabel from "@/components/wizard/components/CodeValueLabel"

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
    <Badge className="bg-indigo-600">Transformation</Badge>
  ) : (
    <div className="flex flex-col gap-2 rounded-md border px-1 py-0.5">
      <Collapsible
        open={open}
        onOpenChange={setOpen}
        className="flex flex-col gap-2"
      >
        <div className="flex flex-row items-center gap-2">
          <IconGripVertical className="stroke-2" />
          <Switch
            id="active"
            checked={modifier.Active}
            onCheckedChange={(checked) =>
              onChange({ ...modifier, Active: checked })
            }
          />
          <CollapsibleTrigger className="flex grow flex-row items-center justify-between">
            <div className="flex flex-row items-center gap-2">
              <div className="text-md px-2 font-semibold">Transformation</div>
              <CodeValueLabel className="text-xs pt-2">{modifier.Expression}</CodeValueLabel>
            </div>
            <div className="h-8 rounded-md px-2 [&_svg]:size-4 flex flex-row items-center justify-center hover:bg-accent hover:text-accent-foreground">
              { !open ? <IconChevronDown /> : <IconChevronUp /> }
            </div>
          </CollapsibleTrigger>
          <Button onClick={onDelete} size={"sm"} variant="ghost">
            <IconTrash />
          </Button>
        </div>
        <CollapsibleContent className="data-[state=closed]:animate-collapsible-up data-[state=open]:animate-collapsible-down flex flex-col gap-4 overflow-hidden border-t pt-2 pr-12 pb-2 pl-12">
          <div className="text-muted-foreground text-sm">
            Apply a transformation to the input value using the specified
            expression.
          </div>
          <div className="flex flex-row items-center gap-2">
            <div className="flex grow flex-col gap-1">
              <Label htmlFor="expression">Expression</Label>
              <div className="relative flex flex-row items-center">
                <Input
                  id="expression"
                  className="text-code pl-8"
                  value={modifier.Expression}
                  onChange={(e) =>
                    onChange({ ...modifier, Expression: e.target.value })
                  }
                />
                <IconMathFunction className="stroke-muted-foreground bg-accent absolute rounded-l-sm px-1" />
              </div>
            </div>
          </div>
        </CollapsibleContent>
      </Collapsible>
    </div>
  )
}
export default TransformationPanel
