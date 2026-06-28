import { Input } from "@/components/ui/input"
import { Substring } from "@/types/modifier"
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

type SubstringPanelProps = {
  variant: "summary" | "editor"
  modifier: Substring
  onChange: (updated: Substring) => void
  onDelete: () => void
}

const SubstringPanel = ({
  variant,
  modifier,
  onChange,
  onDelete,
}: SubstringPanelProps) => {
  const [open, setOpen] = useState(false)

  return variant === "summary" ? (
    <div>SubstringPanel Summary</div>
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
            <div className="text-md px-2 font-semibold">Substring</div>
            <Button onClick={() => {}} size={"sm"} variant="ghost">
              <IconChevronDown />
            </Button>
          </CollapsibleTrigger>
          <Button onClick={onDelete} size={"sm"} variant="ghost">
            <IconTrash />
          </Button>
        </div>
        <CollapsibleContent className="data-[state=closed]:animate-collapsible-up data-[state=open]:animate-collapsible-down overflow-hidden flex flex-col gap-4 border-t pt-2 pr-12 pl-12 pb-2">
          <div className="text-muted-foreground text-sm">
            Extract a substring from the input value using the specified start and end indices.
          </div>
          <div className="flex flex-row items-center gap-4 pr-16 pb-4">
            <div className="flex flex-col gap-1">
              <Label htmlFor="start">Start index</Label>
              <Input
                className="w-16"
                id="start"
                value={modifier.Start}
                onChange={(e) =>
                  onChange({ ...modifier, Start: parseInt(e.target.value) })
                }
              />
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="end">End index</Label>
              <Input
                className="w-16"
                id="end"
                value={modifier.End}
                onChange={(e) =>
                  onChange({ ...modifier, End: parseInt(e.target.value) })
                }
              />
            </div>
          </div>
        </CollapsibleContent>
      </Collapsible>
    </div>
  )
}
export default SubstringPanel
