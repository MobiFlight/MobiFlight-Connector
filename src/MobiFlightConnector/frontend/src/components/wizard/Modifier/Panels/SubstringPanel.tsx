import { Substring } from "@/types/modifier"
import { Switch } from "@/components/ui/switch"
import { Button } from "@/components/ui/button"
import {
  IconChevronDown,
  IconChevronUp,
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
import { Badge } from "@/components/ui/badge"
import {
  validateNumberInput,
} from "@/lib/hooks/useDraftCommitInput"
import Input from "@/components/Input"

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
    <Badge className="bg-blue-700">Substring</Badge>
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
              <div className="text-md w-32 px-2 text-left font-semibold">
                Substring
              </div>
              <div className="flex flex-row items-center gap-2 text-sm">
                From <Badge variant={"secondary"}>{modifier.Start}</Badge> to{" "}
                <Badge variant={"secondary"}>{modifier.End}</Badge>
              </div>
            </div>
            <div className="hover:bg-accent hover:text-accent-foreground flex h-8 flex-row items-center justify-center rounded-md px-2 [&_svg]:size-4">
              {!open ? <IconChevronDown /> : <IconChevronUp />}
            </div>
          </CollapsibleTrigger>
          <Button onClick={onDelete} size={"sm"} variant="ghost">
            <IconTrash />
            <span className="sr-only">Remove modifier</span>
          </Button>
        </div>
        <CollapsibleContent className="data-[state=closed]:animate-collapsible-up data-[state=open]:animate-collapsible-down flex flex-col gap-4 overflow-hidden border-t pt-2 pr-12 pb-2 pl-12">
          <div className="text-muted-foreground text-sm">
            Extract a substring from the input value using the specified start
            and end indices.
          </div>
          <div className="flex flex-row items-center gap-4 pr-16 pb-4">
            <div className="flex flex-col gap-1">
              <Label htmlFor="start">Start index</Label>
              <Input
                id="start"
                value={modifier.Start}
                className="w-16"
                validateOnCommit={validateNumberInput}
                onChange={(value) => {
                  onChange({ ...modifier, Start: value })
                }}
              />
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="end">End index</Label>
              <Input
                id="end"
                value={modifier.End}
                className="w-16"
                validateOnCommit={validateNumberInput}
                onChange={(value) => {
                  onChange({ ...modifier, End: value })
                }}
              />
            </div>
          </div>
        </CollapsibleContent>
      </Collapsible>
    </div>
  )
}
export default SubstringPanel
