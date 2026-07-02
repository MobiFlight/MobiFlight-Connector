import { Padding } from "@/types/modifier"
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
import ComboBox from "@/components/ComboBox"
import { useTranslation } from "react-i18next"
import { Badge } from "@/components/ui/badge"
import Input from "@/components/Input"
import { validateNumberInput } from "@/lib/hooks/useDraftCommitInput"

type PaddingPanelProps = {
  variant: "summary" | "editor"
  modifier: Padding
  onChange: (updated: Padding) => void
  onDelete: () => void
}

const PaddingPanel = ({
  variant,
  modifier,
  onChange,
  onDelete,
}: PaddingPanelProps) => {
  const [open, setOpen] = useState(false)

  const directionOptions = [
    { value: "Left", label: "Left" },
    { value: "Right", label: "Right" },
    { value: "Centered", label: "Centered" },
  ] as { value: "Left" | "Right" | "Centered"; label: string }[]

  const selectedDirection = directionOptions.find(
    (option) => option.value === modifier.Direction,
  )

  const { t } = useTranslation()
  const setSelectedDirection = (
    item: { value: "Left" | "Right" | "Centered"; label: string } | null,
  ) => {
    if (item) {
      onChange({ ...modifier, Direction: item.value })
    }
  }

  return variant === "summary" ? (
    <Badge className="bg-teal-600">Padding</Badge>
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
                Padding
              </div>
              <div className="flex flex-row items-center gap-2 text-sm">
                Length <Badge variant={"secondary"}>{modifier.Length}</Badge>
                Value <Badge variant={"secondary"}>{modifier.Character === " " ? "Space" : modifier.Character}</Badge>
                Direction{" "}
                <Badge variant={"secondary"}>{modifier.Direction}</Badge>
              </div>
            </div>
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
            Adjust the padding of the input value using the specified length,
            character, and direction.
          </div>
          <div className="flex flex-row items-center gap-4 pr-16 pb-4">
            <div className="flex flex-col gap-1">
              <Label htmlFor="length">Length</Label>
              <Input
                className="w-12"
                id="length"
                value={modifier.Length}
                validateOnCommit={validateNumberInput}
                onChange={(value) =>
                  onChange({ ...modifier, Length: value })
                }
              />
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="character">Value</Label>
              <Input
                className="w-12"
                id="character"
                maxLength={1}
                value={modifier.Character}
                onChange={(value) =>
                  onChange({ ...modifier, Character: value })
                }
              />
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="direction">Direction</Label>
              <ComboBox
                id="direction"
                items={directionOptions}
                selected={selectedDirection}
                getLabel={(item) => item.label}
                getValue={(item) => item.value}
                isSelected={(item) => item.value === selectedDirection?.value}
                setSelected={(item) => {
                  setSelectedDirection(item ? item : null)
                }}
                searchPlaceholder={t(
                  "Dialog.InputConfigWizard.InputActions.Common.SearchPresets",
                )}
                variant="nofilter"
                widthClass="w-32"
              />
            </div>
          </div>
        </CollapsibleContent>
      </Collapsible>
    </div>
  )
}
export default PaddingPanel
