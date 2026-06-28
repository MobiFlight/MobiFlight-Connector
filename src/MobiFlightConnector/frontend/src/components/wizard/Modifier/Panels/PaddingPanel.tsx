import { Input } from "@/components/ui/input"
import { Padding } from "@/types/modifier"
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
import { useTranslation } from "react-i18next"

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

  const selectedDirection = directionOptions.find((option) => option.value === modifier.Direction)

  const { t } = useTranslation()
  const setSelectedDirection = (item : { value: "Left" | "Right" | "Centered"; label: string } | null) => {
    if (item) {
      onChange({ ...modifier, Direction: item.value })
    }
  }

  return variant === "summary" ? (
    <div>PaddingPanel Summary</div>
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
            <div className="text-md px-2 font-semibold">Padding</div>
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
            <div className="flex flex-col gap-1">
              <Label htmlFor="length">Length</Label>
              <Input
                className="w-12"
                id="length"
                value={modifier.Length}
                onChange={(e) =>
                  onChange({ ...modifier, Length: parseInt(e.target.value) })
                }
              />
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="padChar">Value</Label>
              <Input
                className="w-12"
                id="padChar"
                maxLength={1}
                value={modifier.PadChar}
                onChange={(e) =>
                  onChange({ ...modifier, PadChar: e.target.value })
                }
              />
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="padDirection">Direction</Label>
              <ComboBox
                id="padDirection"
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
