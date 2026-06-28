import { Input } from "@/components/ui/input"
import { Blink } from "@/types/modifier"
import { Switch } from "@/components/ui/switch"
import { Button } from "@/components/ui/button"
import {
  IconChevronDown,
  IconGripVertical,
  IconPlus,
  IconTrash,
} from "@tabler/icons-react"
import { Label } from "@/components/ui/label"

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible"
import { useState } from "react"

type BlinkPanelProps = {
  variant: "summary" | "editor"
  modifier: Blink
  onChange: (updated: Blink) => void
  onDelete: () => void
}

const BlinkPanel = ({
  variant,
  modifier,
  onChange,
  onDelete,
}: BlinkPanelProps) => {
  const [open, setOpen] = useState(false)
  const blinkValues: { on: number; off: number }[] = []

  for (let i = 0; i < modifier.OnOffSequence.length; i += 2) {
    const on = modifier.OnOffSequence[i]
    const off = modifier.OnOffSequence[i + 1]
    blinkValues.push({
      on: on,
      off: off,
    })
  }

  const convertToFlatArray = (updateValues: { on: number; off: number }[]) => {
    const flatArray: number[] = []
    updateValues.forEach(({ on, off }) => {
      flatArray.push(on, off)
    })
    return flatArray
  }

  const addBlink = () => {
    const lastMapping =
      blinkValues.length > 0
        ? blinkValues[blinkValues.length - 1]
        : { on: 0, off: 0 }
    const newMapping = {
      on: lastMapping.on,
      off: lastMapping.off,
    }
    const updatedValues = [...blinkValues, newMapping]
    onChange({ ...modifier, OnOffSequence: convertToFlatArray(updatedValues) })
  }

  const deleteBlink = (index: number) => {
    const updatedValues = blinkValues.filter((_, i) => i !== index)
    onChange({ ...modifier, OnOffSequence: convertToFlatArray(updatedValues) })
  }

  return variant === "summary" ? (
    <div>BlinkPanel Summary</div>
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
            <div className="text-md px-2 font-semibold">Blink</div>
            <Button onClick={() => {}} size={"sm"} variant="ghost">
              <IconChevronDown />
            </Button>
          </CollapsibleTrigger>
          <Button onClick={onDelete} size={"sm"} variant="ghost">
            <IconTrash />
          </Button>
        </div>
        <CollapsibleContent className="border-t pt-2 pr-13 pl-27">
          <div className="flex flex-row gap-2">
            <div className="flex flex-col gap-1 pr-12">
              <Label htmlFor="value">Blink value</Label>
              <Input
                className="w-12"
                id="value"
                value={modifier.BlinkValue}
                onChange={(e) =>
                  onChange({ ...modifier, BlinkValue: e.target.value })
                }
              />
            </div>
            <div className="flex flex-col gap-1">
              <Label htmlFor="repeat">Sequence</Label>
              <div className="flex h-9 flex-row items-center gap-2 px-2">
                <div className="w-12 text-sm font-semibold">#</div>
                <div className="w-20 text-center text-sm font-semibold">On</div>
                <div className="w-20 text-center text-sm font-semibold">
                  Off
                </div>
              </div>
              {blinkValues.map((range, index) => {
                const { on, off } = range
                return (
                  <div
                    className="flex flex-row items-center gap-2 px-2"
                    key={index}
                  >
                    <div className="w-12 text-sm font-semibold">
                      {index + 1}
                    </div>
                    <div className="flex w-20 flex-row items-center gap-1">
                      <Input
                        id="on"
                        value={on ?? ""}
                        onChange={(e) =>
                          onChange({
                            ...modifier,
                            OnOffSequence: convertToFlatArray(
                              blinkValues.map((v, i) =>
                                i === index
                                  ? {
                                      on: parseInt(e.target.value),
                                      off: v.off,
                                    }
                                  : v,
                              ),
                            ),
                          })
                        }
                      />
                    </div>
                    <div className="flex w-20 flex-row items-center gap-1">
                      <Input
                        id="off"
                        value={off ?? ""}
                        onChange={(e) =>
                          onChange({
                            ...modifier,
                            OnOffSequence: convertToFlatArray(
                              blinkValues.map((v, i) =>
                                i === index
                                  ? {
                                      on: v.on,
                                      off: parseInt(e.target.value),
                                    }
                                  : v,
                              ),
                            ),
                          })
                        }
                      />
                    </div>
                    <div className="flex flex-col gap-1">
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => {
                          deleteBlink(index)
                        }}
                      >
                        <IconTrash />
                      </Button>
                    </div>
                  </div>
                )
              })}
              <Button variant="outline" size="sm" onClick={addBlink}>
                <IconPlus />
                Add blink sequence
              </Button>
            </div>
          </div>
        </CollapsibleContent>
      </Collapsible>
    </div>
  )
}
export default BlinkPanel
