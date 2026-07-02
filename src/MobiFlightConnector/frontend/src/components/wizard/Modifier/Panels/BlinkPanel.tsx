import { Blink } from "@/types/modifier"
import { Switch } from "@/components/ui/switch"
import { Button } from "@/components/ui/button"
import {
  IconChevronDown,
  IconChevronUp,
  IconGripVertical,
  IconPlus,
  IconTrash,
} from "@tabler/icons-react"

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible"
import { useState } from "react"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import { Label } from "@/components/ui/label"
import Input from "@/components/Input"
import { validateNumberInput } from "@/lib/hooks/useDraftCommitInput"
import { Trans, useTranslation } from "react-i18next"

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
  const { t } = useTranslation()
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
    <Badge className="bg-amber-400">Blink</Badge>
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
            <div className="flex flex-row items-center gap-2">
              <div className="text-md px-2 font-semibold w-32 text-left">{t("Dialog.Modifiers.Type.Blink.Label")}</div>
              <div className="flex flex-row items-center gap-2 text-sm">
                <Trans 
                  i18nKey="Dialog.Modifiers.Type.Blink.Summary"
                  values={{
                    blinkValue: modifier.BlinkValue,
                    on: blinkValues.length > 0 ? `${blinkValues[0].on}` : "",
                    off: blinkValues.length > 0 ? `${blinkValues[0].off}` : ""
                  }}
                  components={{
                    badge: <Badge variant={"secondary"} />,
                    span: <span className="text-sm font-semibold" />,
                  }}
                />
              </div>
            </div>
            <div className="h-8 rounded-md px-2 [&_svg]:size-4 flex flex-row items-center justify-center hover:bg-accent hover:text-accent-foreground">
              { !open ? <IconChevronDown /> : <IconChevronUp /> }
            </div>
          </CollapsibleTrigger>
          <Button onClick={onDelete} size={"sm"} variant="ghost">
            <IconTrash />
            <span className="sr-only">{t("Dialog.Modifiers.Editor.DeleteModifier")}</span>
          </Button>
        </div>
        <CollapsibleContent className="data-[state=closed]:animate-collapsible-up data-[state=open]:animate-collapsible-down flex flex-col gap-4 overflow-hidden border-t pt-2 pr-12 pb-2 pl-12">
          <div className="text-muted-foreground text-sm">
            {t("Dialog.Modifiers.Type.Blink.Description")}
          </div>
          <div className="flex flex-col gap-1 pr-12">
            <Label htmlFor="alternate" className="text-md font-semibold">{t("Dialog.Modifiers.Type.Blink.AlternateValue")}</Label>
            <Input
              className="w-12"
              id="alternate"
              value={modifier.BlinkValue}
              onChange={(value) =>
                onChange({ ...modifier, BlinkValue: value })
              }
            />
          </div>
          <div>
            <div className="text-md font-semibold">{t("Dialog.Modifiers.Type.Blink.BlinkSequence")}</div>
            <Table className="">
              <TableHeader>
                <TableRow>
                  <TableHead>{t("Dialog.Modifiers.Type.Blink.On")}</TableHead>
                  <TableHead>{t("Dialog.Modifiers.Type.Blink.Off")}</TableHead>
                  <TableHead>
                    <span className="sr-only">{t("Dialog.Modifiers.Type.Blink.Action")}</span>
                  </TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {blinkValues.map((range, index) => {
                  const { on, off } = range
                  return (
                    <TableRow key={index}>
                      <TableCell className="px-2 py-1">
                        <Input
                          id="on"
                          value={on ?? 500}
                          validateOnCommit={validateNumberInput}
                          onChange={(value) =>
                            onChange({
                              ...modifier,
                              OnOffSequence: convertToFlatArray(
                                blinkValues.map((v, i) =>
                                  i === index
                                ? {
                                  on: value,
                                  off: v.off,
                                }
                                : v,
                              ),
                            ),
                          })
                        }
                        />
                      </TableCell>
                      <TableCell className="px-2 py-1">
                        <Input
                          id="off"
                          value={off ?? 500}
                          validateOnCommit={validateNumberInput}
                          onChange={(value) =>
                            onChange({
                              ...modifier,
                              OnOffSequence: convertToFlatArray(
                                blinkValues.map((v, i) =>
                                  i === index
                                    ? {
                                        on: v.on,
                                        off: value,
                                      }
                                    : v,
                                ),
                              ),
                            })
                          }
                        />
                      </TableCell>
                      <TableCell className="px-2 py-1">
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => {
                            deleteBlink(index)
                          }}
                        >
                          <IconTrash />
                          <span className="sr-only">{t("Dialog.Modifiers.Type.Blink.Remove")}</span>
                        </Button>
                      </TableCell>
                    </TableRow>
                  )
                })}
              </TableBody>
            </Table>
          </div>
          <Button variant="outline" size="sm" onClick={addBlink}>
            <IconPlus />
            {t("Dialog.Modifiers.Type.Blink.Add")}
          </Button>
        </CollapsibleContent>
      </Collapsible>
    </div>
  )
}
export default BlinkPanel
