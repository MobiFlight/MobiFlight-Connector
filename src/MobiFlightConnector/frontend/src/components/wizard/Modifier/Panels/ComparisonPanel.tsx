import { Input } from "@/components/ui/input"
import { Comparison, ComparisonOperators } from "@/types/modifier"
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
import ComboBox from "@/components/ComboBox"

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
  const selectedDirection = availableOperators.find((option) => option === modifier.Operand)

  const setSelectedDirection = (item: string | null) => {
    if (item) {
      onChange({ ...modifier, Operand: item as "=" | "!=" | "<" | ">" | "<=" | ">=" })
    }
  }

  return variant === "summary" ? (
    <div>ComparisonPanel Summary</div>
  ) : (
    <div className="flex flex-col gap-2 rounded-md border p-1">
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
            <div className="text-md px-2 font-semibold">Comparison</div>
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
                widthClass="w-32"
              />
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="start">Value</Label>
              <Input
                id="start"
                value={modifier.Value}
                onChange={(e) =>
                  onChange({ ...modifier, Value: e.target.value })
                }
              />
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="if">Then</Label>
              <Input
                id="if"
                value={modifier.IfValue}
                onChange={(e) =>
                  onChange({ ...modifier, IfValue: e.target.value })
                }
              />
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="else">Else</Label>
              <Input
                id="else"
                value={modifier.ElseValue}
                onChange={(e) =>
                  onChange({ ...modifier, ElseValue: e.target.value })
                }
              />
            </div>
          </div>
        </CollapsibleContent>
      </Collapsible>
    </div>
  )
}
export default ComparisonPanel
