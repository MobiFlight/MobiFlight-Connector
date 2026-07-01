import { Input } from "@/components/ui/input"
import { Comparison, ComparisonOperators } from "@/types/modifier"
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
import ComboBox from "@/components/ComboBox"
import { Badge } from "@/components/ui/badge"

type ComparisonPanelProps = {
  variant: "summary" | "editor"
  modifier: Comparison
  onChange: (updated: Comparison) => void
  onDelete: () => void
}

const ComparisonPanel = ({
  variant,
  modifier,
  onChange,
  onDelete,
}: ComparisonPanelProps) => {
  const [open, setOpen] = useState(false)
  const availableOperators = ComparisonOperators
  const selectedDirection = availableOperators.find(
    (option) => option === modifier.Operand,
  )

  const setSelectedDirection = (item: string | null) => {
    if (item) {
      onChange({
        ...modifier,
        Operand: item as "=" | "!=" | "<" | ">" | "<=" | ">=",
      })
    }
  }

  return variant === "summary" ? (
    <Badge className="bg-amber-600">Comparison</Badge>
  ) : (
    <div className="flex flex-col gap-2 rounded-md border p-1">
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
            <div className="text-md px-2 font-semibold w-32 text-left">Comparison</div>
            <div className="h-8 rounded-md px-2 [&_svg]:size-4 flex flex-row items-center justify-center hover:bg-accent hover:text-accent-foreground">
              { !open ? <IconChevronDown /> : <IconChevronUp /> }
            </div>
          </CollapsibleTrigger>
          <Button onClick={onDelete} size={"sm"} variant="ghost">
            <IconTrash />
            <span className="sr-only">Remove modifier</span>
          </Button>
        </div>
        <CollapsibleContent className="data-[state=closed]:animate-collapsible-up data-[state=open]:animate-collapsible-down flex flex-col gap-4 overflow-hidden border-t pt-2 pr-12 pb-2 pl-12">
          <div className="text-muted-foreground text-sm">
            Compare the current value with another value and adjust the output
            if it matches or not.
          </div>
          <div className="flex flex-row items-center gap-4">
            <div className="flex flex-row gap-1">
              <Label htmlFor="current">If current value</Label>
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="operator">Operator</Label>
              <ComboBox
                id="operator"
                items={availableOperators}
                selected={selectedDirection}
                getLabel={(item) => item}
                getValue={(item) => item}
                isSelected={(item) => item === selectedDirection}
                setSelected={(item) => {
                  setSelectedDirection(item ? item : null)
                }}
                variant="nofilter"
                widthClass="w-20"
              />
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="start">Value</Label>
              <div className="relative flex flex-row items-center">
                <Input
                  id="start"
                  className="text-code pl-8"
                  value={modifier.Value}
                  onChange={(e) =>
                    onChange({ ...modifier, Value: e.target.value })
                  }
                />
                <IconMathFunction className="stroke-muted-foreground bg-accent absolute rounded-l-sm px-1" />
              </div>
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="if">Then</Label>
              <div className="relative flex flex-row items-center">
                <Input
                  id="if"
                  className="text-code pl-8"
                  value={modifier.IfValue}
                  onChange={(e) =>
                    onChange({ ...modifier, IfValue: e.target.value })
                  }
                />
                <IconMathFunction className="stroke-muted-foreground bg-accent absolute rounded-l-sm px-1" />
              </div>
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="else">Else</Label>
              <div className="relative flex flex-row items-center">
                <Input
                  className="text-code pl-8"
                  id="else"
                  value={modifier.ElseValue}
                  onChange={(e) =>
                    onChange({ ...modifier, ElseValue: e.target.value })
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
export default ComparisonPanel
